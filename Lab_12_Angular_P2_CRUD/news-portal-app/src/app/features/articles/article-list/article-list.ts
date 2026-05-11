import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article';
import { AuthService } from '../../../core/services/auth';
import { Article, CurrentUser } from '../../../shared/models/article';

@Component({
  selector: 'app-article-list',
  imports: [CommonModule],
  templateUrl: './article-list.html',
  styleUrl: './article-list.css'
})
export class ArticleList implements OnInit {
  private articleService = inject(ArticleService);
  private authService = inject(AuthService);
  private router = inject(Router);

  articles = signal<Article[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  currentUser = signal<CurrentUser | null>(null);

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(u => this.currentUser.set(u));
    this.loadArticles();
  }

  private loadArticles(): void {
    this.loading.set(true);
    this.articleService.getAll().subscribe({
      next: articles => {
        this.articles.set(articles);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Nu s-au putut incarca articolele');
        this.loading.set(false);
      }
    });
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  canModify(article: Article): boolean {
    // TODO Lab 12 (Ex 4): return true daca currentUser e Admin sau autorul articolului
    return false;
  }

  viewArticle(id: number): void {
    this.router.navigate(['/articles', id]);
  }

  createArticle(): void {
    this.router.navigate(['/articles/new']);
  }

  editArticle(id: number, event: Event): void {
    event.stopPropagation();
    this.router.navigate(['/articles', id, 'edit']);
  }

  deleteArticle(id: number, event: Event): void {
    event.stopPropagation();
    // TODO Lab 12 (Ex 4): confirm() + articleService.delete(id).subscribe + filter local via this.articles.update(...)
  }
}
