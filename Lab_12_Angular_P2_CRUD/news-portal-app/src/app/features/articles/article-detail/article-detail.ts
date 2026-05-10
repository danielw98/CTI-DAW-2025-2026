import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ArticleService } from '../../../core/services/article';
import { Article } from '../../../shared/models/article';

@Component({
  selector: 'app-article-detail',
  imports: [CommonModule, RouterLink],
  templateUrl: './article-detail.html',
  styleUrl: './article-detail.css'
})
export class ArticleDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private articleService = inject(ArticleService);

  article: Article | null = null;
  loading = true;
  notFound = false;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.notFound = true;
      this.loading = false;
      return;
    }

    this.articleService.getById(id).subscribe({
      next: article => {
        this.article = article;
        this.loading = false;
      },
      error: () => {
        this.notFound = true;
        this.loading = false;
      }
    });
  }
}
