import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule, FormBuilder, FormGroup, Validators,
  AbstractControl, ValidationErrors
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

function passwordsMatch(group: AbstractControl): ValidationErrors | null {
  const pwd = group.get('password')?.value;
  const cpwd = group.get('confirmPassword')?.value;
  return pwd === cpwd ? null : { passwordsMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  registerForm: FormGroup;
  loading = false;
  submitted = false;
  error: string | null = null;

  constructor() {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordsMatch });
  }

  get f() { return this.registerForm.controls; }

  onSubmit(): void {
    this.submitted = true;
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.error = null;

    const { email, fullName, password } = this.registerForm.value;

    this.authService.register(email, fullName, password).subscribe({
      next: () => this.router.navigateByUrl('/articles'),
      error: err => {
        this.error = err.error?.message ?? 'Inregistrare esuata';
        this.loading = false;
      }
    });
  }
}
