import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { ArticleList } from './features/articles/article-list/article-list';
import { ArticleDetail } from './features/articles/article-detail/article-detail';
import { ArticleForm } from './features/articles/article-form/article-form';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/articles', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'articles', component: ArticleList },
  { path: 'articles/new', component: ArticleForm, canActivate: [authGuard] },
  { path: 'articles/:id/edit', component: ArticleForm, canActivate: [authGuard] },
  { path: 'articles/:id', component: ArticleDetail },
  { path: '**', redirectTo: '/articles' }
];
