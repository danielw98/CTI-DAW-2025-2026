import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CurrentUser } from '../../shared/models/article.model';

interface AuthResponse {
  token: string;
  expiresIn: number;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password })
      .pipe(tap(res => this.handleAuthResponse(res.token)));
  }

  register(email: string, fullName: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { email, fullName, password })
      .pipe(tap(res => this.handleAuthResponse(res.token)));
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return localStorage.getItem('token') !== null;
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  private handleAuthResponse(token: string): void {
    localStorage.setItem('token', token);
    this.currentUserSubject.next(this.decodeUser(token));
  }

  private loadUserFromToken(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.currentUserSubject.next(this.decodeUser(token));
    }
  }

  private decodeUser(token: string): CurrentUser {
    const payload = this.decodeJwtPayload(token);
    const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const roles = Array.isArray(roleClaim) ? roleClaim : roleClaim ? [roleClaim] : [];
    return {
      id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
      name: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      roles
    };
  }

  private decodeJwtPayload(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64 + '='.repeat((4 - base64.length % 4) % 4);
    return JSON.parse(atob(padded));
  }
}
