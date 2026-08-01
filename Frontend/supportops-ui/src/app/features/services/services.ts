import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { SERVICES } from './services.data';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './services.html',
  styleUrl: './services.css'
})
export class Services {

  services = SERVICES;//Later, we'll replace: this.serviceService.getServices().subscribe(...);

}
