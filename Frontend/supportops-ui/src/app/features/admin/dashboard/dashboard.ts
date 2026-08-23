import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { ContactService } from '../../../core/services/contact.service';
import { ContactResponse } from '../../../core/models/contact-response';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  readonly service = inject(ContactService);
  readonly cdr = inject(ChangeDetectorRef);

  contacts: ContactResponse[] = [];

  displayedColumns = ['name', 'company', 'service', 'date', 'actions'];

  // private readonly authService = inject(AuthService);

  // constructor() {
  //   console.log(
  //     'Current role:',
  //     this.authService.getRole()
  //   );
  // }

  // constructor(private authService: AuthService) {
  //   console.log('Current role:', this.authService.getRole());
  // }

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadContacts();
  }

  loadContacts() {
    this.service.getAll().subscribe({
      next: (data) => {
        this.contacts = data;
        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error(error);
      },
    });
  }
}
