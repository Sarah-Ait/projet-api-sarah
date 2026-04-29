import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  name = '';
  email = '';
  password = '';
  errorMessage = '';
  successMessage = '';

  get hasMinLength(): boolean {
    return this.password.length >= 8;
  }

  get hasUppercase(): boolean {
    return /[A-Z]/.test(this.password);
  }

  get hasLowercase(): boolean {
    return /[a-z]/.test(this.password);
  }

  get hasDigit(): boolean {
    return /\d/.test(this.password);
  }

  get isPasswordValid(): boolean {
    return this.hasMinLength && this.hasUppercase && this.hasLowercase && this.hasDigit;
  }

  onRegister(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.name || !this.email || !this.password) {
      this.errorMessage = 'Tous les champs sont obligatoires.';
      return;
    }

    if (!this.isPasswordValid) {
      this.errorMessage = 'Le mot de passe ne respecte pas les règles de sécurité.';
      return;
    }

    this.auth
      .register({
        name: this.name,
        email: this.email,
        password: this.password
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Compte créé avec succès. Vous pouvez vous connecter.';
          setTimeout(() => this.router.navigate(['/']), 1200);
        },
        error: (error) => {
          this.errorMessage = error?.error?.message ?? 'Impossible de créer le compte.';
        }
      });
  }
}
