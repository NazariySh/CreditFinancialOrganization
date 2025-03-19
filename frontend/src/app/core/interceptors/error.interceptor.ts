import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';
import { catchError, throwError } from 'rxjs';
import { ErrorResponse } from '../../shared/models/error-response';
import { StatusCodes } from '../../shared/models/status-codes';
import { ValidationErrorResponse } from '../../shared/models/validation-error-response';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (!err.error || typeof err.error !== 'object') {
        return throwError(() => err);
      }

      const errorResponse = err.error as ErrorResponse;

      switch (err.status) {
        case StatusCodes.Status400BadRequest:
        case StatusCodes.Status403Forbidden:
        case StatusCodes.Status409Conflict:
          snackbar.error(errorResponse.message || 'An error occurred.');
          break;

        case StatusCodes.Status404NotFound:
          router.navigateByUrl('/not-found');
          break;

        case StatusCodes.Status422UnprocessableEntity: {
          const validationErrorResponse = err.error as ValidationErrorResponse;
          if (validationErrorResponse.errors) {
            snackbar.error('Validation failed. Please check your input.');
            return throwError(() => validationErrorResponse);
          }
          break;
        }

        case StatusCodes.Status500InternalServerError: {
          const navigationExtras: NavigationExtras = { state: { error: err.error } };
          router.navigateByUrl('/server-error', navigationExtras);
          break;
        }
      }

      console.log(err.error);

      return throwError(() => err);
    })
  );
};
