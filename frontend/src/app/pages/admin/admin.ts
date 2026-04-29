import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { User } from '../../models/user.model';
import { Auth } from '../../services/auth';
import { UserService } from '../../services/user';

@Component({
  selector: 'app-admin',
  imports: [CommonModule, RouterLink],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class Admin implements OnInit {
  private readonly userService = inject(UserService);
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);

  readonly currentUserId = computed(() => this.auth.getCurrentUserId());

  users: User[] = [];
  errorMessage = '';

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: users => (this.users = users),
      error: () => (this.errorMessage = 'Impossible de charger les utilisateurs.')
    });
  }

  viewUserBoard(userId: number): void {
    this.router.navigate(['/board'], { queryParams: { userId } });
  }

  deleteUser(user: User): void {
    if (!confirm(`Voulez-vous vraiment supprimer ${user.name} ?`)) {
      return;
    }

    this.userService.deleteUser(user.id).subscribe({
      next: () => {
        this.users = this.users.filter(existing => existing.id !== user.id);
        this.errorMessage = '';
      },
      error: () => (this.errorMessage = "Impossible de supprimer l'utilisateur.")
    });
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.auth.clearState();
        this.router.navigate(['/']);
      }
    });
  }

  getInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }
}
