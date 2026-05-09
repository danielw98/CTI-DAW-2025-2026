import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ArticleListComponent } from './features/articles/article-list/article-list.component';
import { ArticleDetailComponent } from './features/articles/article-detail/article-detail.component';
import { ArticleFormComponent } from './features/articles/article-form/article-form.component';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/articles', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'articles', component: ArticleListComponent },
  // 'new' inainte de ':id' - altfel ':id' ar prinde 'new'
  { path: 'articles/new', component: ArticleFormComponent, canActivate: [AuthGuard] },
  { path: 'articles/:id/edit', component: ArticleFormComponent, canActivate: [AuthGuard] },
  { path: 'articles/:id', component: ArticleDetailComponent },
  { path: '**', redirectTo: '/articles' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
