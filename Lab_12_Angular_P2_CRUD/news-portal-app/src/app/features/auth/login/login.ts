import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);

  loginForm: FormGroup;
  loading = false;
  submitted = false;
  error: string | null = null;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  get f() { return this.loginForm.controls; }

  onSubmit(): void {
    this.submitted = true;
    if (this.loginForm.invalid) return;

    this.loading = true;
    this.error = null;

    this.authService.login(this.f['email'].value, this.f['password'].value).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] ?? '/articles';
        this.router.navigateByUrl(returnUrl);
      },
      error: err => {
        this.error = err.error?.message ?? 'Email sau parola incorecta';
        this.loading = false;
      }
    });
  }
}
