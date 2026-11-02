import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { TicketService } from '../../../core/services/ticket';
import { AuthService } from '../../../core/services/auth';
import { Ticket } from '../../../core/models/ticket';
import { IupdateTicketRequest } from '../../../core/models/iupdate-ticket-request';

@Component({
  selector: 'app-ticket-details.component',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './ticket-details.component.html',
  styleUrls: ['./ticket-details.component.css'],
})
export class TicketDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly ticketService = inject(TicketService);
  public readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  // ticket: Ticket | null = null;
  // loading = true;
  ticket = signal<Ticket | null>(null);
  loading = signal(true);
  saving = false;
  editing = false;

  errorMessage = '';
  successMessage = '';

  editForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(5000)]],
    priority: ['Medium', Validators.required],
    category: ['Other', Validators.required],
    status: ['Open', Validators.required],
  });

  get canEdit(): boolean {
    const currentTicket = this.ticket();

    if (!currentTicket) return false;

    const role = this.authService.getRole()?.toLowerCase();

    if (role === 'admin' || role === 'supportagent') {
      return true;
    }

    return role === 'customer' && currentTicket.status.toLowerCase() === 'open';
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage = 'Invalid ticket ID.';
      this.loading.set(false);
      return;
    }

    this.loadTicket(id);
  }

  // private loadTicket(id: number): void {
  //   this.loading = true;
  //   this.errorMessage = '';
  //   this.ticket = null;

  //   this.ticketService
  //     .getTicket(id)
  //     .pipe(finalize(() => (this.loading = false)))
  //     .subscribe({
  //       next: (response: any) => {
  //         console.log('Ticket details raw response:', response);

  //         // Handle both direct Ticket and { data: Ticket } responses.
  //         const ticketData = response?.data ?? response;

  //         if (!ticketData || !ticketData.id) {
  //           console.error('Invalid ticket response:', response);
  //           this.errorMessage = 'Ticket details were returned in an unexpected format.';
  //           return;
  //         }

  //         this.ticket = ticketData as Ticket;

  //         console.log('Ticket assigned:', this.ticket);
  //       },
  //       error: (error) => {
  //         console.error('Unable to load ticket:', error);

  //         this.errorMessage =
  //           error.status === 401 || error.status === 403
  //             ? 'You are not authorized to view this ticket.'
  //             : error.status === 404
  //               ? 'Ticket not found.'
  //               : 'Unable to load ticket. Please try again.';
  //       },
  //     });
  // }
  private loadTicket(id: number): void {
    this.loading.set(true);
    this.errorMessage = '';
    this.ticket.set(null);

    this.ticketService
      .getTicket(id)
      .pipe(
        finalize(() => {
          console.log('Ticket loading completed');
          this.loading.set(false);
        }),
      )
      .subscribe({
        next: (response) => {
          console.log('Ticket details received:', response);

          this.ticket.set(response);

          console.log('Ticket signal updated:', this.ticket());
        },
        error: (error) => {
          console.error('Unable to load ticket:', error);

          this.errorMessage =
            error.status === 401 || error.status === 403
              ? 'You are not authorized to view this ticket.'
              : error.status === 404
                ? 'Ticket not found.'
                : 'Unable to load ticket. Please try again.';
        },
      });
  }
  startEdit(): void {
    const currentTicket = this.ticket();

    if (!currentTicket || !this.canEdit) return;

    this.editForm.patchValue({
      title: currentTicket.title,
      description: currentTicket.description,
      priority: currentTicket.priority,
      category: currentTicket.category,
      status: currentTicket.status,
    });

    this.editing = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEdit(): void {
    this.editing = false;
    this.errorMessage = '';
  }

  saveChanges(): void {
    const currentTicket = this.ticket();

    if (!currentTicket || this.editForm.invalid || this.saving) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    const request: IupdateTicketRequest = {
      title: this.editForm.controls.title.value,
      description: this.editForm.controls.description.value,
      priority: this.editForm.controls.priority.value,
      category: this.editForm.controls.category.value,
    };

    // Only Admin and SupportAgent can change status.
    const role = this.authService.getRole()?.toLowerCase();

    if (role === 'admin' || role === 'supportagent') {
      request.status = this.editForm.controls.status.value;
    }

    this.ticketService
      .updateTicket(currentTicket.id, request)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe({
        next: () => {
          this.editing = false;
          this.successMessage = 'Ticket updated successfully.';

          this.loadTicket(currentTicket.id);
        },
        error: (error) => {
          console.error('Unable to update ticket:', error);

          this.errorMessage =
            error.status === 403 || error.status === 404
              ? 'You are not authorized to update this ticket.'
              : error.status === 400
                ? 'Please check the information and try again.'
                : 'Unable to update ticket. Please try again.';
        },
      });
  }

  goBack(): void {
    void this.router.navigate(['/tickets']);
  }
}
