import { Component, OnInit } from '@angular/core';
import { ContactService } from '../../../core/services/contact.service';
import { ContactResponse } from '../../../core/models/contact-response';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  imports: [MatTableModule],
  styleUrls: ['./dashboard.css'],
})
export class Dashboard implements OnInit {

  contacts: ContactResponse[] = [];

  constructor(private contactService: ContactService) {}

  ngOnInit() {
    this.contactService.getAll().subscribe((data) => {
      this.contacts = data;
    });
  }
}
