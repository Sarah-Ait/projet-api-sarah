import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { Auth } from '../services/auth';

const AUTH_PATHS = ['/api/auth/login', '/api/auth/refresh', '/api/auth/logout'];

function isAuthRequest(request: HttpRequest<unknown>): boolean {
  return AUTH_PATHS.some(path => request.url.includes(path));
}

function attachToken(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return request.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(Auth);
  const router = inject(Router);

  const token = auth.accessToken();
  const initialRequest = token && !isAuthRequest(request) ? attachToken(request, token) : request;

  return next(initialRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      const shouldRefresh = error.status === 401 && !isAuthRequest(request);

      if (!shouldRefresh) {
        return throwError(() => error);
      }

      return auth.refresh().pipe(
        switchMap(() => {
          const newToken = auth.accessToken();

          if (!newToken) {
            return throwError(() => error);
          }

          return next(attachToken(request, newToken));
        }),
        catchError(refreshError => {
          auth.clearState();
          router.navigate(['/']);
          return throwError(() => refreshError);
        })
      );
    })
  );
};
