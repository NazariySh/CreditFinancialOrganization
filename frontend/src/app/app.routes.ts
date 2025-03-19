import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { authGuard } from './core/guards/auth.guard';
import { employeeGuard } from './core/guards/employee.guard';
import { adminGuard } from './core/guards/admin.guard';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { ServerErrorComponent } from './shared/components/server-error/server-error.component';
import { ProfileComponent } from './features/accounts/profile/profile.component';

export const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'profile', component: ProfileComponent },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth-routing').then(r => r.authRoutes)
  },
  {
    path: 'loans',
    loadChildren: () => import('./features/loans/loans-routing').then(r => r.loansRoutes),
    canActivate: [authGuard]
  },
  {
    path: 'employee',
    loadChildren: () => import('./features/employee/employee-routing').then(r => r.employeeRoutes),
    canActivate: [employeeGuard]
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin-routing').then(r => r.adminRoutes),
    canActivate: [adminGuard]
  },
  { 
    path: 'not-found',
    component: NotFoundComponent
  },
  { 
    path: 'server-error',
    component: ServerErrorComponent
  },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
