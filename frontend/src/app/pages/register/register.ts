import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  name = '';
  email = '';
  password = '';
  errorMessage = '';
  successMessage = '';

  constructor(
    private auth: Auth,
    private router: Router
  ) {}

  onRegister(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.name || !this.email || !this.password) {
      this.errorMessage = 'Tous les champs sont obligatoires.';
      return;
    }

    this.auth.register({
      name: this.name,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.successMessage = 'Compte créé avec succès. Vous pouvez vous connecter.';

        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1000);
      },
      error: (error) => {
        console.error('Erreur création compte', error);
        this.errorMessage = 'Impossible de créer le compte.';
      }
    });
  }
}