import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';

export const employeeGuard: CanActivateFn = () => {
  const router = inject(Router);
  const accountService = inject(AccountService);
  const snackbar = inject(SnackbarService);

  if (accountService.isEmployee()) {
    return true;
  }

  snackbar.error('Access denied: Employee privileges required.');
  router.navigateByUrl('/');
  return false;
};
