import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { ContactService } from '../../core/services/contact.service';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule
  ],
  templateUrl: './contact.html',
  styleUrl: './contact.css'
})
export class Contact {

  private fb = inject(FormBuilder);
  private contactService = inject(ContactService);
  private snackBar = inject(MatSnackBar);

  isSubmitting = false;

  form = this.fb.group({
    fullName: ['', Validators.required],
    company: [''],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    service: ['', Validators.required],
    message: ['', [Validators.required, Validators.minLength(10)]]
  });

  submit() {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    this.contactService. create(this.form.getRawValue() as any)
      .subscribe({
        next: () => {

          this.snackBar.open(
            'Thank you! We will contact you shortly.',
            'Close',
            {
              duration: 4000
            });

          this.form.reset();
          this.isSubmitting = false;
        },

        error: () => {

          this.snackBar.open(
            'Unable to submit your request.',
            'Close',
            {
              duration: 4000
            });

          this.isSubmitting = false;
        }
      });
  }

}
