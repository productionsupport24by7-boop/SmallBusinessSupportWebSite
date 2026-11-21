import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

import { TicketService } from '../../../core/services/ticket';
import { Ticket } from '../../../core/models/ticket';
import { AuthService } from '../../../core/services/auth';

import { SupportAgent } from '../../../core/models/isupport-agent';

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
  supportAgents: SupportAgent[] = [];
  selectedAgentIds: Record<number, number | null> = {};
  agentsLoading = signal(false);
  assigningTicketId = signal<number | null>(null);
  loading = signal(false);
  errorMessage = '';
  assignmentError = '';
  assignmentSuccess = '';

  get hasTickets(): boolean {
    return this.tickets.length > 0;
  }

  get showEmptyState(): boolean {
    return !this.loading() && !this.errorMessage && !this.hasTickets;
  }

  get isAdmin(): boolean {
    return this.authService.getRole()?.toLowerCase() === 'admin';
  }
  get isAdminOrSupportAgent(): boolean {
    const role = this.authService.getRole()?.toLowerCase();
    return role === 'admin' || role === 'supportagent';
  }

  ngOnInit(): void {
    sessionStorage.setItem(this.lastvisitedcomp, 'ticket-list');
    this.loadTickets();
    if (this.isAdmin) {
      this.loadSupportAgents();
    }
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
          this.selectedAgentIds = {};

          for (const ticket of this.tickets) {
            this.selectedAgentIds[ticket.id] = ticket.assignedToUserId ?? null;
          }

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
  loadSupportAgents(): void {
    this.agentsLoading.set(true);

    this.ticketService
      .getSupportAgents()
      .pipe(finalize(() => this.agentsLoading.set(false)))
      .subscribe({
        next: (response) => {
          this.supportAgents = Array.isArray(response) ? response : [];
        },
        error: (error: HttpErrorResponse) => {
          console.error('Unable to load Support Agents:', error);
          this.assignmentError = 'Unable to load Support Agents. Please refresh the page.';
        },
      });
  }

  onAgentSelectionChange(ticketId: number, event: Event): void {
    const select = event.target as HTMLSelectElement;
    const agentId = Number(select.value);

    this.selectedAgentIds[ticketId] = agentId > 0 ? agentId : null;

    this.assignmentError = '';
    this.assignmentSuccess = '';
  }

  isAssignDisabled(ticket: Ticket): boolean {
    const selectedAgentId = this.selectedAgentIds[ticket.id];

    return (
      !selectedAgentId ||
      selectedAgentId === (ticket.assignedToUserId ?? null) ||
      this.assigningTicketId() === ticket.id
    );
  }

  assignTicket(ticket: Ticket): void {
    const agentId = this.selectedAgentIds[ticket.id];

    if (!agentId) {
      this.assignmentError = 'Please select a Support Agent.';
      return;
    }

    this.assignmentError = '';
    this.assignmentSuccess = '';
    this.assigningTicketId.set(ticket.id);

    this.ticketService
      .assignTicket(ticket.id, agentId)
      .pipe(finalize(() => this.assigningTicketId.set(null)))
      .subscribe({
        next: () => {
          ticket.assignedToUserId = agentId;

          const agent = this.supportAgents.find((item) => item.id === agentId);

          this.assignmentSuccess =
            `Ticket ${ticket.ticketNumber} assigned to ` +
            `${agent?.firstName ?? ''} ${agent?.lastName ?? ''}.`;
        },
        error: (error: HttpErrorResponse) => {
          console.error('Ticket assignment failed:', error);

          if (error.status === 401) {
            this.assignmentError = 'Your session has expired. Please log in again.';
          } else if (error.status === 403) {
            this.assignmentError = 'You are not authorized to assign tickets.';
          } else {
            this.assignmentError =
              error.error?.message || 'Unable to assign ticket. Please try again.';
          }
        },
      });
  }

  getAgentName(agentId?: number | null): string {
    if (!agentId) {
      return 'Unassigned';
    }

    const agent = this.supportAgents.find((item) => item.id === agentId);

    return agent ? `${agent.firstName} ${agent.lastName}` : 'Assigned Agent';
  }

  viewDetails(ticket: Ticket): void {
    void this.router.navigate(['/tickets', ticket.id]);
  }

  createTicket(): void {
    void this.router.navigate(['/customer/tickets/create']);
  }
}
