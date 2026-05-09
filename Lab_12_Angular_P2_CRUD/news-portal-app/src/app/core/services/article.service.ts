import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Article,
  CreateArticleDto,
  UpdateArticleDto
} from '../../shared/models/article.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ArticleService {
  private apiUrl = `${environment.apiUrl}/api/articles`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Article[]> {
    return this.http.get<Article[]>(this.apiUrl);
  }

  getById(id: number): Observable<Article> {
    return this.http.get<Article>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateArticleDto): Observable<Article> {
    // TODO Lab 12: POST la /api/articles
    // Hint: return this.http.post<Article>(this.apiUrl, dto);
    throw new Error('TODO Lab 12: implementati create()');
  }

  update(id: number, dto: UpdateArticleDto): Observable<void> {
    // TODO Lab 12: PUT la /api/articles/:id
    // Hint: return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
    throw new Error('TODO Lab 12: implementati update()');
  }

  delete(id: number): Observable<void> {
    // TODO Lab 12: DELETE la /api/articles/:id
    // Hint: return this.http.delete<void>(`${this.apiUrl}/${id}`);
    throw new Error('TODO Lab 12: implementati delete()');
  }
}
