import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article.service';
import { Article } from '../../../shared/models/article.model';

@Component({
  selector: 'app-article-detail',
  templateUrl: './article-detail.component.html',
  styleUrl: './article-detail.component.css'
})
export class ArticleDetailComponent implements OnInit {
  article: Article | null = null;
  loading = true;
  notFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.articleService.getById(id).subscribe({
      next: article => {
        this.article = article;
        this.loading = false;
      },
      error: err => {
        this.notFound = err.status === 404;
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/articles']);
  }
}
