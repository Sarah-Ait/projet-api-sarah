import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Board } from './pages/board/board';
import { Admin } from './pages/admin/admin';
import { authGuard } from './core/auth.guard';
import { adminGuard } from './core/admin.guard';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'register', component: Register },
  { path: 'board', component: Board, canActivate: [authGuard] },
  { path: 'admin', component: Admin, canActivate: [adminGuard] }
];
