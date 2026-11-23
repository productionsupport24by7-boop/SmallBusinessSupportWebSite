import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { UserProfileService } from '../../core/services/user-profile.service';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css',
})
export class ChangePasswordComponent {
  private readonly profileService = inject(UserProfileService);

  currentPassword = '';
  newPassword = '';
  confirmNewPassword = '';

  isSaving = signal(false);
  errorMessage = '';
  successMessage = '';

  changePassword(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.newPassword.length < 8) {
      this.errorMessage = 'New password must contain at least 8 characters.';
      return;
    }

    if (this.newPassword !== this.confirmNewPassword) {
      this.errorMessage = 'New password and confirmation do not match.';
      return;
    }

    this.isSaving.set(true);

    this.profileService
      .changePassword({
        currentPassword: this.currentPassword,
        newPassword: this.newPassword,
        confirmNewPassword: this.confirmNewPassword,
      })
      .subscribe({
        next: (response) => {
          this.successMessage = response.message;
          this.currentPassword = '';
          this.newPassword = '';
          this.confirmNewPassword = '';
          this.isSaving.set(false);
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage =
            error.error?.errors?.join(' ') ||
            error.error?.message ||
            'Unable to change password. Please try again.';

          this.isSaving.set(false);
        },
      });
  }
}
