export interface UserProfile {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string | null;
  roles: string[];
}

export interface UpdateUserProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string | null;
}
