import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { TicketService } from '../../../core/services/ticket.service';
import { AuthService } from '../../../core/services/auth';
import { Ticket } from '../../../core/models/ticket';
import { IupdateTicketRequest } from '../../../core/models/iupdate-ticket-request';
import { TicketActivity } from '../../../core/models/iticket-activity.model';

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
  saving = signal(false);
  editing = signal(false);
  //ticket activities: TicketActivity[] = [];
  activities = signal<TicketActivity[]>([]);
  activityLoading = signal(false);
  activityError = signal('');

  errorMessage = '';
  successMessage = '';

  editForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(5000)]],
    priority: ['Medium', Validators.required],
    category: ['Other', Validators.required],
    status: ['Open', Validators.required],
    resolutionDescription: ['', [Validators.maxLength(4000)]],
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
          this.loadTicketActivities(id);

          console.log('Ticket signal updated:', this.ticket());
        },

        error: (error) => {
          console.error('Unable to update ticket:', error);
          console.error('API validation response:', error.error);

          if (error.status === 400) {
            const apiError = error.error;

            if (apiError?.errors) {
              this.errorMessage = Object.values(apiError.errors).flat().join(' ');
            } else {
              this.errorMessage =
                apiError?.message || apiError?.title || 'Please check the submitted information.';
            }
          } else if (error.status === 403 || error.status === 404) {
            this.errorMessage = 'You are not authorized to update this ticket.';
          } else {
            this.errorMessage = 'Unable to update ticket. Please try again.';
          }
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
      resolutionDescription: currentTicket.resolutionDescription ?? '',
    });
    this.updateResolutionValidator();
    this.editing.set(true);
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.errorMessage = '';
  }

  saveChanges(): void {
    this.updateResolutionValidator();
    const currentTicket = this.ticket();

    if (!currentTicket || this.editForm.invalid || this.saving()) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage = '';

    const request: IupdateTicketRequest = {
      title: this.editForm.controls.title.value,
      description: this.editForm.controls.description.value,
      priority: this.editForm.controls.priority.value,
      category: this.editForm.controls.category.value,
      //status: this.editForm.controls.status.value,
      //resolutionDescription: this.editForm.controls.resolutionDescription.value,
    };

    // Only Admin and SupportAgent can change status.
    const role = this.authService.getRole()?.toLowerCase();

    if (role === 'admin' || role === 'supportagent') {
      request.status = this.editForm.controls.status.value;
      if (request.status === 'Resolved') {
        request.resolutionDescription = this.editForm.controls.resolutionDescription.value.trim();
      }
    }

    this.ticketService
      .updateTicket(currentTicket.id, request)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.editing.set(false);
          this.successMessage = 'Ticket updated successfully.';

          this.loadTicket(currentTicket.id);
        },
        error: (error) => {
          console.error('Unable to update ticket:', error);
          console.error('API validation response:', error.error);

          if (error.status === 400) {
            const apiError = error.error;

            if (apiError?.errors) {
              this.errorMessage = Object.values(apiError.errors).flat().join(' ');
            } else {
              this.errorMessage =
                apiError?.message || apiError?.title || 'Please check the submitted information.';
            }
          } else if (error.status === 403 || error.status === 404) {
            this.errorMessage = 'You are not authorized to update this ticket.';
          } else {
            this.errorMessage = 'Unable to update ticket. Please try again.';
          }
        },
      });
  }

  loadTicketActivities(ticketId: number): void {
    this.activityLoading.set(true);
    this.activityError.set('');

    this.ticketService.getTicketActivities(ticketId).subscribe({
      next: (activities) => {
        this.activities.set(activities);
        this.activityLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading ticket activities:', error);

        this.activityError.set('Unable to load ticket activity history.');

        this.activityLoading.set(false);
      },
    });
  }

  onStatusChange(): void {
    this.updateResolutionValidator();
  }

  private updateResolutionValidator(): void {
    const resolutionControl = this.editForm.controls.resolutionDescription;

    const status = this.editForm.controls.status.value;

    if (status === 'Resolved') {
      resolutionControl.setValidators([Validators.required, Validators.maxLength(4000)]);
    } else {
      resolutionControl.setValidators([Validators.maxLength(4000)]);
    }

    resolutionControl.updateValueAndValidity();
  }

  goBack(): void {
    void this.router.navigate(['/tickets']);
  }
}
