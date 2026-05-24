import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article';
import { CategoryService } from '../../../core/services/category';
import { TagService } from '../../../core/services/tag';
import { Category, Tag } from '../../../shared/models/article';
import { environment } from '../../../../environments/environment';

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
  protected tagService = inject(TagService);

  // SCAFFOLD pre-built Lab12: expune URL-ul backend in template pt <img [src]="backendUrl + imagePath"
  readonly backendUrl = environment.apiUrl;

  form!: FormGroup;
  categories = signal<Category[]>([]);
  allTags = signal<Tag[]>([]);
  isEditMode = signal(false);
  articleId = signal<number | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  // SCAFFOLD pre-built Lab12: imagine
  selectedFile: File | null = null;
  existingImagePath = signal<string | null>(null);

  ngOnInit(): void {
    // SCAFFOLD pre-built Lab12: load lista de tag-uri pt selector-ul din formular
    this.tagService.getAll().subscribe({
      next: tags => this.allTags.set(tags),
      error: () => { /* selector-ul ramane gol - nu blocheaza salvarea */ }
    });

    // TODO Lab 12 (Ex 2 si Ex 3):
    // 1. Construiti this.form cu FormBuilder
    //    - title:      ['', [Validators.required, Validators.minLength(5)]]
    //    - content:    ['', [Validators.required, Validators.minLength(20)]]
    //    - categoryId: [null, Validators.required]
    //    - tagIds:     [[] as number[]]    <- multi-select pre-built (scaffold)
    // 2. Apelati categoryService.getAll().subscribe(...) si salvati in this.categories
    // 3. Citi route.snapshot.paramMap.get('id'); daca exista -> isEditMode.set(true), articleId.set(...)
    //    Pentru edit, apelati articleService.getById(...).subscribe(...) si folositi:
    //      this.form.patchValue({
    //        title: article.title,
    //        content: article.content,
    //        categoryId: article.categoryId,
    //        tagIds: article.tags.map(t => t.id)        // <- mapare scaffold pt tag selector
    //      });
    //      this.existingImagePath.set(article.imagePath ?? null);
  }

  onSubmit(): void {
    // TODO Lab 12 (Ex 2 si Ex 3):
    // - if (form.invalid) { form.markAllAsTouched(); return; }
    // - this.loading.set(true);
    // - if (isEditMode())
    //     articleService.update(articleId()!, form.value).subscribe({
    //       next: () => this.afterSave(this.articleId()!),       // <- afterSave = scaffold
    //       error: () => { this.loading.set(false); this.error.set('Eroare la salvare'); }
    //     });
    //   else
    //     articleService.create(form.value).subscribe({
    //       next: (article) => this.afterSave(article.id),       // <- afterSave = scaffold
    //       error: () => { this.loading.set(false); this.error.set('Eroare la salvare'); }
    //     });
  }

  cancel(): void {
    this.router.navigate(['/articles']);
  }

  // ============================================================================
  // SCAFFOLD pre-built Lab12 - studentul nu modifica zona de mai jos
  // Imaginile sunt incarcate separat (POST /api/articles/{id}/image) dupa
  // ce articolul a fost salvat (POST/PUT). Asta tine logica de upload
  // in afara exercitiilor 2 si 3 (care raman pe contractul JSON pur).
  // ============================================================================

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files && input.files.length > 0 ? input.files[0] : null;
  }

  onTagToggle(tagId: number, checked: boolean): void {
    const control = this.form?.get('tagIds');
    if (!control) return;
    const current: number[] = control.value ?? [];
    if (checked && !current.includes(tagId)) {
      control.setValue([...current, tagId]);
    } else if (!checked) {
      control.setValue(current.filter(id => id !== tagId));
    }
  }

  /**
   * Apelat din onSubmit() in callback-ul de succes al create/update.
   * - daca user-ul a selectat un fisier  -> upload, apoi navigate
   * - altfel                              -> navigate direct
   */
  protected afterSave(articleId: number): void {
    if (this.selectedFile) {
      this.articleService.uploadImage(articleId, this.selectedFile).subscribe({
        next: () => {
          this.loading.set(false);
          this.router.navigate(['/articles', articleId]);
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Articol salvat, dar upload-ul imaginii a esuat');
        }
      });
    } else {
      this.loading.set(false);
      this.router.navigate(['/articles', articleId]);
    }
  }
}
