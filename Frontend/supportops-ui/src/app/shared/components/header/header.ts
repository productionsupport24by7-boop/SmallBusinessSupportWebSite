import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, MatToolbarModule, MatButtonModule],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  //private readonly isLoggedInSignal = signal(this.authService.isLoggedIn());

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigate(['/home']);
  }

  get userRole(): string | null {
    return this.authService.getRole();
  }

  toggleAuth() {
    if (this.isLoggedIn()) {
      this.logout();
    } else {
      void this.router.navigate(['/login']);
    }
  }
}
