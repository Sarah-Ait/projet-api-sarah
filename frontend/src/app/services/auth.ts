import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, finalize, shareReplay, tap } from 'rxjs';
import { API_BASE_URL } from '../core/api.config';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

interface AuthState {
  accessToken: string | null;
  expiresAt: Date | null;
  email: string | null;
  role: string | null;
}

const EMPTY_STATE: AuthState = {
  accessToken: null,
  expiresAt: null,
  email: null,
  role: null
};

@Injectable({ providedIn: 'root' })
export class Auth {
  private readonly http = inject(HttpClient);
  private readonly authUrl = `${API_BASE_URL}/api/auth`;
  private readonly usersUrl = `${API_BASE_URL}/api/users`;

  private readonly state = signal<AuthState>(EMPTY_STATE);
  private refreshInProgress$: Observable<AuthResponse> | null = null;

  readonly accessToken = computed(() => this.state().accessToken);
  readonly email = computed(() => this.state().email);
  readonly role = computed(() => this.state().role);
  readonly isAuthenticated = computed(() => this.state().accessToken !== null);
  readonly isAdmin = computed(() => this.state().role === 'Admin');

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.authUrl}/login`, request, { withCredentials: true })
      .pipe(tap(response => this.applyResponse(response)));
  }

  register(request: RegisterRequest): Observable<void> {
    return this.http.post<void>(this.usersUrl, request);
  }

  refresh(): Observable<AuthResponse> {
    if (this.refreshInProgress$) {
      return this.refreshInProgress$;
    }

    this.refreshInProgress$ = this.http
      .post<AuthResponse>(`${this.authUrl}/refresh`, {}, { withCredentials: true })
      .pipe(
        tap(response => this.applyResponse(response)),
        finalize(() => (this.refreshInProgress$ = null)),
        shareReplay(1)
      );

    return this.refreshInProgress$;
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.authUrl}/logout`, {}, { withCredentials: true })
      .pipe(tap(() => this.clearState()));
  }

  getCurrentUserId(): number | null {
    const token = this.state().accessToken;

    if (!token) {
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const userId =
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      return Number(userId);
    } catch {
      return null;
    }
  }

  clearState(): void {
    this.state.set(EMPTY_STATE);
  }

  private applyResponse(response: AuthResponse): void {
    this.state.set({
      accessToken: response.accessToken,
      expiresAt: new Date(response.expiresAt),
      email: response.email,
      role: response.role
    });
  }
}
