import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from '../../../core/services/article.service';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../shared/models/article.model';

@Component({
  selector: 'app-article-form',
  templateUrl: './article-form.component.html',
  styleUrl: './article-form.component.css'
})
export class ArticleFormComponent implements OnInit {
  form!: FormGroup;
  categories: Category[] = [];
  isEditMode = false;
  articleId: number | null = null;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    // TODO Lab 12: Construiti form-ul cu FormBuilder
    // Campuri: title (required, minLength 5), content (required, minLength 20), categoryId (required)
    // Apelati categoryService.getAll() pentru dropdown-ul de categorii
    // Daca route param :id exista, set isEditMode = true, apelati articleService.getById si patch-uiti form-ul
  }

  onSubmit(): void {
    // TODO Lab 12: Validati form-ul si apelati create() sau update() din ArticleService
    // La succes: navigate spre /articles
    // La eroare: setati this.error
  }

  cancel(): void {
    this.router.navigate(['/articles']);
  }
}
