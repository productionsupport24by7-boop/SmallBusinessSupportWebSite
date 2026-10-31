import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

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

    category: ['General', Validators.required],
  });

  submit(): void {
    if (this.ticketForm.invalid) {
      this.ticketForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const request: CreateTicketRequest = this.ticketForm.getRawValue();

    this.ticketService.createTicket(request).subscribe({
      next: (ticket) => {
        console.log('Ticket created:', ticket);

        this.router.navigate(['/customer/tickets']);
      },

      error: (error) => {
        console.error('Unable to create ticket:', error);

        this.errorMessage = 'Unable to create ticket. Please try again.';

        this.submitting = false;
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/customer/tickets']);
  }
}
