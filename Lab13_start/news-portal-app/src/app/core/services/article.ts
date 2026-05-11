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
    return this.http.post<Article>(this.apiUrl, dto);
  }

  update(id: number, dto: UpdateArticleDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
