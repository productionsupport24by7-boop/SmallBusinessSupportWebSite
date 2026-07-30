import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
export interface HealthResponse {
  status: string;
  application: string;
  version: string;
  serverTime: string;
}
@Injectable({
  providedIn: 'root'
})
export class HealthService {

  private http = inject(HttpClient);
 private apiUrl = `${environment.apiUrl}/api/health`;
  getHealth() {
    return this.http.get<HealthResponse>(this.apiUrl);
  }
}
