import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

import { TicketService } from '../../../core/services/ticket';
import { Ticket } from '../../../core/models/ticket';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-list.html',
  styleUrl: './ticket-list.css',
})
export class TicketList implements OnInit {
  private readonly ticketService = inject(TicketService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly lastvisitedcomp = 'home';

  tickets: Ticket[] = [];
  loading = signal(false);
  errorMessage = '';

  get hasTickets(): boolean {
    return this.tickets.length > 0;
  }

  get showEmptyState(): boolean {
    return !this.loading() && !this.errorMessage && !this.hasTickets;
  }

  get isAdminOrSupportAgent(): boolean {
    const role = this.authService.getRole()?.toLowerCase();
    return role === 'admin' || role === 'supportagent';
  }

  ngOnInit(): void {
    sessionStorage.setItem(this.lastvisitedcomp, 'ticket-list');
    this.loadTickets();
  }

  loadTickets(): void {
    this.loading.set(true);
    this.errorMessage = '';

    console.log('Loading tickets started...');

    this.ticketService
      .getMyTickets()
      .pipe(
        finalize(() => {
          console.log('Ticket request completed. Resetting loading...');
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (response) => {
          console.log('Tickets API response:', response);

          this.tickets = Array.isArray(response) ? response : [];

          console.log('Tickets loaded:', this.tickets.length);
        },
        error: (error: HttpErrorResponse) => {
          console.error('Ticket API error:', error);

          this.tickets = [];

          if (error.status === 401) {
            this.errorMessage = 'Your session has expired. Please log in again.';
          } else if (error.status === 403) {
            this.errorMessage = 'You are not authorized to view these tickets.';
          } else if (error.status === 0) {
            this.errorMessage = 'Unable to connect to the support service.';
          } else {
            this.errorMessage = 'Unable to load your tickets. Please try again.';
          }
        },
      });
  }
  viewDetails(ticket: Ticket): void {
    this.router.navigate(['/tickets', ticket.id]);
  }

  createTicket(): void {
    this.router.navigate(['/customer/tickets/create']);
  }
}
