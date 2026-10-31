import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketService } from '../../../core/services/ticket';
import { Ticket } from '../../../core/models/ticket';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-ticket-details.component',
  standalone: true,
  imports: [],
  templateUrl: './ticket-details.component.html',
  styleUrls: ['./ticket-details.component.css'],
})
export class TicketDetailsComponent implements OnInit {
  constructor() {
    console.log('TicketDetailsComponent initialized');
  }
  readonly cdr = inject(ChangeDetectorRef);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ticketService = inject(TicketService);
  ticket: Ticket | null = null;
  loading: boolean = true;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    console.log('TicketDetailsComponent initialized with ID: ' + id);
    if (!id || id <= 0) {
      this.errorMessage = 'Invalid ticket ID.';
      return;
    }
    this.loadTicket(id);
  }
  private loadTicket(id: number): void {
    //this.loading = true;
    this.errorMessage = '';
    this.ticketService.getTicket(id).subscribe({
      next: (ticket) => {
        console.log('Ticket details:', ticket);
        this.ticket = ticket;
        this.cdr.detectChanges();
        this.loading = false;
      },
      error: (error) => {
        console.error('Unable to load ticket:', error);
        //this.loading = false;
        if (error.status === 401) {
          this.errorMessage = 'You are not authorized to view this ticket.';
        } else if (error.status === 404) {
          this.errorMessage = 'Ticket not found.';
        } else {
          this.errorMessage = 'Unable to load ticket. Please try again.';
        }
      },
    });
  }
  goBack(): void {
    void this.router.navigate(['/tickets']);
  }
}
