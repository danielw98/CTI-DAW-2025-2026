import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article.service';
import { AuthService } from '../../../core/services/auth.service';
import { Article } from '../../../shared/models/article.model';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrl: './article-list.component.css'
})
export class ArticleListComponent implements OnInit {
  articles: Article[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private articleService: ArticleService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadArticles();
  }

  private loadArticles(): void {
    this.loading = true;
    this.articleService.getAll().subscribe({
      next: articles => {
        this.articles = articles;
        this.loading = false;
      },
      error: () => {
        this.error = 'Nu s-au putut incarca articolele';
        this.loading = false;
      }
    });
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  canModify(article: Article): boolean {
    // TODO Lab 12: returnati true daca user-ul curent este Admin SAU autorul articolului
    // Hint: cititi user-ul curent prin AuthService (currentUser$ sau o metoda noua)
    //       si verificati article.authorId === user.id || user.roles.includes('Admin')
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
    // TODO Lab 12: confirmati cu confirm(), apoi articleService.delete(id).subscribe(...)
    // La succes: reincarcati lista (apelati loadArticles() sau filtrati local)
    // La eroare: setati this.error
  }
}
