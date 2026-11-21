import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { AuthService, RegisterRequest } from '../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';

  isLoading = signal(false);
  errorMessage = '';
  successMessage = '';

  register(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (
      !this.firstName.trim() ||
      !this.lastName.trim() ||
      !this.email.trim() ||
      !this.password ||
      !this.confirmPassword
    ) {
      this.errorMessage = 'Please complete all required fields.';
      return;
    }

    if (this.password.length < 8) {
      this.errorMessage = 'Password must contain at least 8 characters.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Password and Confirm Password do not match.';
      return;
    }

    const request: RegisterRequest = {
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      email: this.email.trim(),
      password: this.password,
      confirmPassword: this.confirmPassword,
    };

    this.isLoading.set(true);

    this.authService.register(request).subscribe({
      next: (response) => {
        this.isLoading.set(false);

        this.successMessage =
          response.message || 'Registration successful. Your account has been created.';

        // Clear form after successful registration.
        this.firstName = '';
        this.lastName = '';
        this.email = '';
        this.password = '';
        this.confirmPassword = '';
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.errorMessage = this.getRegistrationError(error);

        console.error('Registration failed:', error);
      },
    });
  }

  private getRegistrationError(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Unable to connect to the registration service.';
    }

    if (error.status === 400) {
      const response = error.error;

      // Handle ASP.NET Identity errors returned as an array.
      if (Array.isArray(response?.errors)) {
        const duplicateError = response.errors.find(
          (item: any) => item.code === 'DuplicateEmail' || item.code === 'DuplicateUserName',
        );

        if (duplicateError) {
          return 'This email address is already registered. Please sign in or use another email address.';
        }

        const messages = response.errors
          .map((item: any) => item.description || item.message)
          .filter((message: string) => !!message);

        if (messages.length > 0) {
          return messages.join(' ');
        }
      }

      // Handle ASP.NET validation errors returned as an object.
      if (response?.errors && typeof response.errors === 'object') {
        const messages = Object.values(response.errors)
          .flatMap((value) => (Array.isArray(value) ? value : [value]))
          .filter((value) => typeof value === 'string');

        if (messages.length > 0) {
          return messages.join(' ');
        }
      }

      return (
        response?.message || response?.title || 'Registration failed. Please check your details.'
      );
    }

    if (error.status === 409) {
      return 'This email address is already registered. Please sign in or use another email address.';
    }

    return 'Unable to create your account. Please try again later.';
  }
}
