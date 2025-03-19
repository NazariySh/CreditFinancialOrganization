import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { catchError, switchMap, take, throwError } from 'rxjs';
import { StatusCodes } from '../../shared/models/status-codes';

export const refreshTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === StatusCodes.Status401Unauthorized) {
        return authService.refresh().pipe(
          take(1),
          switchMap((response) => {
            req = req.clone({
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`
              }
            });

            return next(req);
          }),
          catchError((err) => {
            router.navigate(['/auth/login']);
            return throwError(() => err);
          })
        );
      }
      
      return throwError(() => error);
    })
  );
};
