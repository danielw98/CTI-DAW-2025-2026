import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../../shared/models/article';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/api/categories`;

  getAll(): Observable<Category[]> {
    // TODO Lab 12 (Ex 1): GET /api/categories
    throw new Error('TODO Lab 12: implementati getAll()');
  }
}
