import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';

import { TicketService } from '../../../core/services/ticket';
import { CreateTicketRequest } from '../../../core/models/icreate-ticket-request';

@Component({
  selector: 'app-create-ticket',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-ticket.html',
  styleUrl: './create-ticket.css',
})
export class CreateTicketComponent {
  private readonly fb = inject(FormBuilder);
  private readonly ticketService = inject(TicketService);
  private readonly router = inject(Router);

  submitting = false;
  errorMessage = '';

  ticketForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(5000)]],
    priority: ['Medium', Validators.required],
    category: ['Other', Validators.required],
  });

  submit(): void {
    if (this.ticketForm.invalid || this.submitting) {
      this.ticketForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const request: CreateTicketRequest = this.ticketForm.getRawValue();

    this.ticketService
      .createTicket(request)
      .pipe(finalize(() => (this.submitting = false)))
      .subscribe({
        next: (ticket) => {
          this.router.navigate(['/tickets', ticket.id]);
        },
        error: (error) => {
          console.error('Unable to create ticket:', error);

          if (error.status === 400) {
            this.errorMessage = 'Please check the ticket information and try again.';
          } else if (error.status === 401) {
            this.errorMessage = 'Your session has expired. Please log in again.';
          } else if (error.status === 403) {
            this.errorMessage = 'You are not authorized to create a ticket.';
          } else if (error.status === 0) {
            this.errorMessage = 'Unable to connect to the support service.';
          } else {
            this.errorMessage = 'Unable to create ticket. Please try again.';
          }
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/tickets']);
  }
}
