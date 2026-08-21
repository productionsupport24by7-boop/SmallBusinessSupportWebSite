import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expires: string;
}

interface JwtPayload {
  sub?: string;
  email?: string;
  role?: string;
  exp?: number;
  iss?: string;
  aud?: string;
  [key: string]: unknown;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5263/api/auth';

  private readonly tokenKey = 'supportops_token';
  private readonly expirationKey = 'supportops_token_expiration';
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);

  //constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        if (isPlatformBrowser(this.platformId)) {
          sessionStorage.setItem(this.tokenKey, response.token);
          sessionStorage.setItem(this.expirationKey, response.expires);
        }
      }),
    );
  }

  logout(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.expirationKey);
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) {
      return null;
    }
    return sessionStorage.getItem(this.tokenKey);
  }

  hasRole(role: string): boolean {
    return this.getRole() === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const currentRole = this.getRole();

    if (!currentRole) {
      return false;
    }

    return roles.includes(currentRole);
  }

  getRole(): string | null {
    const token = this.getToken();

    if (!token) {
      return null;
    }

    try {
      const decoded = jwtDecode<JwtPayload>(token);

      const role =
        decoded.role ??
        (decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] as string) ??
        null;

      console.log('JWT Role:', role);
      return typeof role === 'string' ? role : null;
    } catch (error) {
      console.error('Unable to decode JWT:', error);

      return null;
    }
  }

  isLoggedIn(): boolean {
    const token = this.getToken();

    if (!token) {
      return false;
    }

    try {
      const decoded = jwtDecode<JwtPayload>(token);

      if (!decoded.exp) {
        return false;
      }

      return decoded.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }
}
