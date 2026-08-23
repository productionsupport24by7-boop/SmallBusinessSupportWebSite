import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const roleGuard = (
  allowedRoles: string[]
): CanActivateFn => {

  return (route, state) => {

    const authService = inject(AuthService);
    const router = inject(Router);

    const token = authService.getToken();
    const loggedIn = authService.isLoggedIn();
    const role = authService.getRole();

    console.log('================================');
    console.log('ROLE GUARD');
    console.log('URL:', state.url);
    console.log('TOKEN:', token);
    console.log('LOGGED IN:', loggedIn);
    console.log('ROLE:', role);
    console.log('ALLOWED:', allowedRoles);
    console.log('================================');

    // TEMPORARY TEST
    if (!loggedIn) {
      console.log('❌ isLoggedIn() = FALSE');
      return router.createUrlTree(['/access-denied']);
    }

    if (!role) {
      console.log('❌ getRole() = NULL');
      return router.createUrlTree(['/access-denied']);
    }

    if (allowedRoles.includes(role)) {
      console.log('✅ ACCESS GRANTED');
      return true;
    }

    console.log('❌ ROLE NOT AUTHORIZED');
    return router.createUrlTree(['/access-denied']);
  };
};
