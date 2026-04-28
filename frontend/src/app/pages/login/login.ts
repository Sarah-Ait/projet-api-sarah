import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  password = '';
  rememberMe = false;
  errorMessage = '';

  constructor(
    private auth: Auth,
    private router: Router
  ) {}

  onLogin(): void {
    this.errorMessage = '';

    this.auth.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => {
        this.auth.saveToken(response.token);
        this.auth.saveUserRole(response.role);

        console.log('Connexion réussie', response);

        this.router.navigate(['/board']);
      },
      error: (error) => {
        console.error('Erreur login', error);
        this.errorMessage = 'Email ou mot de passe incorrect.';
      }
    });
  }
}