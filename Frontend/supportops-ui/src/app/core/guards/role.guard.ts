import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router,
  RouterStateSnapshot,
  ActivatedRouteSnapshot,
} from '@angular/router';
import { AuthService } from '../services/auth';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const token = authService.getToken();
    const loggedIn = authService.isLoggedIn();

    console.log('================================');
    console.log('ROLE GUARD');
    console.log('URL:', state.url);
    console.log('TOKEN:', token);
    console.log('LOGGED IN:', loggedIn);
    // console.log('ROLE:', role);
    console.log('ALLOWED:', allowedRoles);
    console.log('================================');

    // TEMPORARY TEST

    if (!token) {
      console.log('No JWT token. Redirecting to login.');
      return router.createUrlTree(['/login']);
    }

    // const allowedRoles = route.data['roles'] as string[];

    // if (role && allowedRoles.includes(role)) {
    //   return true;
    // }

    if (!loggedIn) {
      console.log('❌ isLoggedIn() = FALSE');
      // return router.createUrlTree(['/access-denied']);
      return router.createUrlTree(['/login'], {
        queryParams: {
          returnUrl: state.url,
        },
      });
    }
    // else {

    // return true;
    //}
    const role = authService.getRole();
    if (!role) {
      console.log('❌ getRole() = NULL');
      return router.createUrlTree(['/access-denied']);
    }

    const isAllowed = allowedRoles.some(
      (allowedRole) => allowedRole.toLowerCase() === role.toLowerCase(),
    );

    if (isAllowed) {
      console.log('✅ ACCESS GRANTED');
      return true;
    }

    // if (allowedRoles.includes(role)) {
    //   console.log('✅ ACCESS GRANTED');
    //   return true;
    // }

    console.log('❌ ROLE NOT AUTHORIZED');
    return router.createUrlTree(['/access-denied']);
  };
};
