import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.getAccessToken();

  const clonedRequest = req.clone({
    withCredentials: true,
    setHeaders: accessToken ? { Authorization: `Bearer ${accessToken}` } : {}
  });

  return next(clonedRequest);
};
