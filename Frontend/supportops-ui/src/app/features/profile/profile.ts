import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

import { UserProfileService } from '../../core/services/user-profile.service';
import { UserProfile, UpdateUserProfileRequest } from '../../core/models/UserProfile';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfileComponent implements OnInit {
  private readonly profileService = inject(UserProfileService);

  profile: UserProfile | null = null;

  firstName = '';
  lastName = '';
  phoneNumber = '';

  isLoading = signal(false);
  isSaving = signal(false);

  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading.set(true);
    this.errorMessage = '';

    this.profileService.getMyProfile().subscribe({
      next: (response) => {
        this.profile = response;

        this.firstName = response.firstName;
        this.lastName = response.lastName;
        this.phoneNumber = response.phoneNumber ?? '';

        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage =
          error.status === 401
            ? 'Your session has expired. Please sign in again.'
            : 'Unable to load your profile. Please try again.';

        this.isLoading.set(false);
      },
    });
  }

  updateProfile(): void {
    this.errorMessage = '';
    this.successMessage = '';

    const request: UpdateUserProfileRequest = {
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      phoneNumber: this.phoneNumber.trim() || null,
    };

    if (!request.firstName || !request.lastName) {
      this.errorMessage = 'First name and last name are required.';
      return;
    }

    this.isSaving.set(true);

    this.profileService.updateMyProfile(request).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage = 'Your profile was updated successfully.';

        if (this.profile) {
          this.profile = {
            ...this.profile,
            firstName: request.firstName,
            lastName: request.lastName,
            phoneNumber: request.phoneNumber,
          };
        }
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);

        this.errorMessage =
          error.error?.message || 'Unable to update your profile. Please try again.';
      },
    });
  }
}
