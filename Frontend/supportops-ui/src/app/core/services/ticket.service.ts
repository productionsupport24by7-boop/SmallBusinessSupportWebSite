import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket } from '../models/ticket';
import { environment } from '../../../environments/environment';
import { CreateTicketRequest } from '../models/icreate-ticket-request';
import { IupdateTicketRequest } from '../models/iupdate-ticket-request';
import { SupportAgent } from '../models/isupport-agent';
import { TicketActivity } from '../models/iticket-activity.model';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiUrl}/api/tickets`;
  private readonly usersApiUrl = `${environment.apiUrl}/api/users`;

  getMyTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${this.apiUrl}/my`);
  }

  getTicket(id: number): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`);
  }
  createTicket(request: CreateTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, request);
  }
  updateTicket(id: number, request: IupdateTicketRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  // Get all active Support Agents. Admin only.
  getSupportAgents(): Observable<SupportAgent[]> {
    return this.http.get<SupportAgent[]>(`${this.usersApiUrl}/support-agents`);
  }

  // Assign or reassign a ticket. Admin only.
  assignTicket(ticketId: number, agentId: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${ticketId}/assign`, { assignedToUserId: agentId });
  }
  getTicketActivities(ticketId: number): Observable<TicketActivity[]> {
    return this.http.get<TicketActivity[]>(`${this.apiUrl}/${ticketId}/activities`);
  }
}
