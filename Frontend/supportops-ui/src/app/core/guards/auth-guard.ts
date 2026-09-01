import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();

  console.log('AUTH GUARD TOKEN:', token);

  if (token) {
    return true;
  }
  if (authService.isLoggedIn()) {
    return true;
  }

  //return router.createUrlTree(['/login']);
  return router.createUrlTree(['/login'], {
    queryParams: {
      returnUrl: state.url,
    },
  });
};
