import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article';
import { CategoryService } from '../../../core/services/category';
import { Category } from '../../../shared/models/article';

@Component({
  selector: 'app-article-form',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './article-form.html',
  styleUrl: './article-form.css'
})
export class ArticleForm implements OnInit {
  protected fb = inject(FormBuilder);
  protected route = inject(ActivatedRoute);
  protected router = inject(Router);
  protected articleService = inject(ArticleService);
  protected categoryService = inject(CategoryService);

  form!: FormGroup;
  categories = signal<Category[]>([]);
  isEditMode = signal(false);
  articleId = signal<number | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.form = this.fb.group({
      title:      ['', [Validators.required, Validators.minLength(5)]],
      content:    ['', [Validators.required, Validators.minLength(20)]],
      categoryId: [null, Validators.required]
    });

    this.categoryService.getAll().subscribe({
      next: cats => this.categories.set(cats),
      error: () => this.error.set('Nu s-au putut incarca categoriile')
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode.set(true);
      this.articleId.set(Number(idParam));

      this.articleService.getById(this.articleId()!).subscribe({
        next: article => {
          this.form.patchValue({
            title: article.title,
            content: article.content,
            categoryId: article.categoryId
          });
        },
        error: () => this.error.set('Articolul nu a fost gasit')
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const dto = this.form.value;
    const next = () => this.router.navigate(['/articles']);
    const error = () => {
      this.error.set('Eroare la salvare');
      this.loading.set(false);
    };

    const id = this.articleId();
    if (this.isEditMode() && id !== null) {
      this.articleService.update(id, dto).subscribe({ next, error });
    } else {
      this.articleService.create(dto).subscribe({ next, error });
    }
  }

  cancel(): void {
    this.router.navigate(['/articles']);
  }
}
