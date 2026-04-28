import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Board } from './pages/board/board';
import { Admin } from './pages/admin/admin';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'board', component: Board },
  { path: 'admin', component: Admin }
];