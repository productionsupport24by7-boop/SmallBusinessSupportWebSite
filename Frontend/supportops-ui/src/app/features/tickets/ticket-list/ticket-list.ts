import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../../core/services/ticket';
import { Ticket } from '../../../core/models/ticket';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-list.html',
  styleUrl: './ticket-list.css',
})
export class TicketList implements OnInit {
  private readonly ticketService = inject(TicketService);

  tickets: Ticket[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    console.log('ngOnInit called for ticket');
    this.loadTickets();
    console.log(this.tickets);
  }

  loadTickets(): void {
    this.loading = true;
    this.errorMessage = '';

    this.ticketService.getMyTickets().subscribe({
      next: (response) => {
        this.tickets = response;
        this.loading = false;
      },

      error: (error) => {
        console.error('Unable to load tickets:', error);

        this.errorMessage = 'Unable to load your tickets. Please try again.';

        this.loading = false;
      },
    });
  }
}
