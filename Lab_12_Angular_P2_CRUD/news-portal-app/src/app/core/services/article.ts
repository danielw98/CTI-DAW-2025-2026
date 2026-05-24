import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Article,
  CreateArticleDto,
  UpdateArticleDto
} from '../../shared/models/article';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ArticleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/articles`;

  getAll(): Observable<Article[]> {
    return this.http.get<Article[]>(this.apiUrl);
  }

  getById(id: number): Observable<Article> {
    return this.http.get<Article>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateArticleDto): Observable<Article> {
    // TODO Lab 12 (Ex 1): return this.http.post<Article>(this.apiUrl, dto);
    throw new Error('TODO Lab 12: implementati create()');
  }

  update(id: number, dto: UpdateArticleDto): Observable<void> {
    // TODO Lab 12 (Ex 1): return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
    throw new Error('TODO Lab 12: implementati update()');
  }

  delete(id: number): Observable<void> {
    // TODO Lab 12 (Ex 1): return this.http.delete<void>(`${this.apiUrl}/${id}`);
    throw new Error('TODO Lab 12: implementati delete()');
  }

  // SCAFFOLD pre-built Lab12: image upload (multipart/form-data).
  // Apelat din ArticleFormComponent.afterSave() daca user-ul a selectat un fisier.
  // Studentul nu modifica aceasta metoda - vine functionala in Lab12_start.
  uploadImage(articleId: number, file: File): Observable<{ imagePath: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imagePath: string }>(
      `${this.apiUrl}/${articleId}/image`,
      formData
    );
  }
}
