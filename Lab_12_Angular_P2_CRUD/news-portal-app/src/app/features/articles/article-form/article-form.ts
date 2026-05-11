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
    // TODO Lab 12 (Ex 2 si Ex 3):
    // 1. Construiti this.form cu FormBuilder (title required+minLength(5), content required+minLength(20), categoryId required)
    // 2. Apelati categoryService.getAll().subscribe(...) si salvati in this.categories cu .set()
    // 3. Citi route.snapshot.paramMap.get('id'); daca exista -> isEditMode.set(true), articleId.set(...), load articol, patchValue
  }

  onSubmit(): void {
    // TODO Lab 12 (Ex 2 si Ex 3):
    // - if (form.invalid) { form.markAllAsTouched(); return; }
    // - if (isEditMode()) articleService.update(...).subscribe(...) else articleService.create(...).subscribe(...)
  }

  cancel(): void {
    this.router.navigate(['/articles']);
  }
}
