import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ContactRequest } from '../models/contact-request';
import { environment } from '../../../environments/environment';
import { ContactResponse } from '../models/contact-response';

@Injectable({
  providedIn: 'root'
})
export class ContactService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/api/contact`;

  create(request: ContactRequest): Observable<any> {

    return this.http.post(this.apiUrl, request);

  }
  getAll(){
    return this.http.get<ContactResponse[]>(this.apiUrl);
  }

}
