import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../../shared/models/article.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = `${environment.apiUrl}/api/categories`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Category[]> {
    // TODO Lab 12: GET la /api/categories
    // Hint: return this.http.get<Category[]>(this.apiUrl);
    throw new Error('TODO Lab 12: implementati getAll()');
  }
}
