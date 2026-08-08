import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expires: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl = 'http://localhost:5263/api/auth';

  private readonly tokenKey = 'supportops_token';
  private readonly expirationKey = 'supportops_token_expiration';

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => {
          sessionStorage.setItem(this.tokenKey, response.token);
          sessionStorage.setItem(
            this.expirationKey,
            response.expires
          );
        })
      );
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.expirationKey);
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();

    if (!token) {
      return false;
    }

    const expiration = sessionStorage.getItem(
      this.expirationKey
    );

    if (!expiration) {
      return false;
    }

    return new Date(expiration).getTime() > Date.now();
  }
}
