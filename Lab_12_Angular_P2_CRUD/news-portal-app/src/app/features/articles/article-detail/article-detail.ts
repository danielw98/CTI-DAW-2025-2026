import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ArticleService } from '../../../core/services/article';
import { Article } from '../../../shared/models/article';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-article-detail',
  imports: [CommonModule, RouterLink],
  templateUrl: './article-detail.html',
  styleUrl: './article-detail.css'
})
export class ArticleDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private articleService = inject(ArticleService);

  // SCAFFOLD pre-built Lab12: backendUrl pt afisarea imaginii din wwwroot
  readonly backendUrl = environment.apiUrl;

  article = signal<Article | null>(null);
  loading = signal(true);
  notFound = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.notFound.set(true);
      this.loading.set(false);
      return;
    }

    this.articleService.getById(id).subscribe({
      next: article => {
        this.article.set(article);
        this.loading.set(false);
      },
      error: () => {
        this.notFound.set(true);
        this.loading.set(false);
      }
    });
  }
}
