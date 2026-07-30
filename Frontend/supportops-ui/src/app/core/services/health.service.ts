import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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

  getHealth() {
    return this.http.get('http://localhost:5263/api/health');
  }
}
