import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { SnackbarService } from '../services/snackbar.service';

export const adminGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  if (accountService.isAdmin()) {
    return true;
  }

  snackbar.error('Access denied: Admin privileges required.');
  router.navigateByUrl('/');
  return false;
};
