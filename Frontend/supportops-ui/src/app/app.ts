import { Component, signal,OnInit,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HealthService, HealthResponse } from './core/services/health.service';
import { Header } from './shared/components/header/header';
import { Footer } from './shared/components/footer/footer';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,Header,Footer],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('supportops-ui');
  private healthService = inject(HealthService);

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

}
