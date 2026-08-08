import { Component, signal,OnInit,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HealthService, HealthResponse } from './core/services/health.service';
import { Header } from './shared/components/header/header';
import { Footer } from './shared/components/footer/footer';
//import { AuthService }  from  './core/services/auth';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,Header,Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('supportops-ui');
  private healthService = inject(HealthService);
  //private authService = inject(AuthService);




  ngOnInit(): void {

    this.healthService.getHealth().subscribe({

      next: (response) => {
        console.log('API Response');
        console.log(response);
      },

      error: (error) => {
        console.error(error);
      }

    });
  }

  // testLogin(): void {

  //   this.authService.login({
  //     email: 'admin@supportops.com',
  //     password: 'Admin@123'
  //   })
  //   .subscribe({
  //     next: response => {
  //       console.log('LOGIN SUCCESS');
  //       console.log('Token:', response.token);
  //       console.log('Expires:', response.expires);
  //       console.log('Logged in:', this.authService.isLoggedIn());

  //     },

  //     error: error => {
  //       console.error('LOGIN FAILED');
  //       console.error(error);
  //     }
  //   });
  // }
}






