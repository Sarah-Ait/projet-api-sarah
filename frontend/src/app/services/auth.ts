import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private readonly apiUrl = 'http://localhost:5065/api/auth';

  constructor(private http: HttpClient) {}

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginRequest);
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  saveUserRole(role: string): void {
    localStorage.setItem('role', role);
  }

  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  getCurrentUserId(): number | null {
    const token = localStorage.getItem('token');

    if (!token) {
      return null;
    }

    const payload = JSON.parse(atob(token.split('.')[1]));
    const userId =
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

    return Number(userId);
  }

  register(registerRequest: RegisterRequest): Observable<void> {
    return this.http.post<void>('http://localhost:5065/api/users', registerRequest);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
  }
}