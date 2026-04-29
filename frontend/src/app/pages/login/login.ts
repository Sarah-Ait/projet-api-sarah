import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  email = '';
  password = '';
  rememberMe = false;
  errorMessage = '';

  onLogin(): void {
    this.errorMessage = '';

    this.auth
      .login({
        email: this.email,
        password: this.password,
        rememberMe: this.rememberMe
      })
      .subscribe({
        next: () => this.router.navigate(['/board']),
        error: () => {
          this.errorMessage = 'Email ou mot de passe incorrect.';
        }
      });
  }
}
