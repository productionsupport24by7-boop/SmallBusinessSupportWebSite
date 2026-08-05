import { Routes } from '@angular/router';

import { Home } from './features/home/home';
import { Services } from './features/services/services';
import { About } from './features/about/about';
import { Contact } from './features/contact/contact';
import { Dashboard } from './features/admin/dashboard/dashboard';

export const routes: Routes = [
  {
    path: 'admin/dashboard',
    component: Dashboard
    //  path:'admin/dashboard',
    // loadComponent:()=>import('./features/admin/dashboard/dashboard')
    //     .then(m=>m.Dashboard)
  },
  {
    path: '',
    component: Home
  },
  {
    path: 'services',
    component: Services
  },
  {
    path: 'about',
    component: About
  },
  {
    path: 'contact',
    component: Contact
  }
//   {
//     path: 'contact',
//     loadComponent: () =>
//         import('./features/contact/contact')
//             .then(m => m.Contact)
// }
,
  {
    path: '**',
    redirectTo: ''
  }
];
