import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';

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
   private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);

  //constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap(response => {
          if(isPlatformBrowser(this.platformId)) {
            sessionStorage.setItem(this.tokenKey, response.token);
            sessionStorage.setItem(
              this.expirationKey,
              response.expires
            );
          }



        })
      );
  }

  logout(): void {
      if(!isPlatformBrowser(this.platformId)) {
        return;
      }
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.expirationKey);
  }

  getToken(): string | null {
    if(!isPlatformBrowser(this.platformId)) {
      return null;
    }
    return sessionStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    if(!isPlatformBrowser(this.platformId)) {
      return false;
    }
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
