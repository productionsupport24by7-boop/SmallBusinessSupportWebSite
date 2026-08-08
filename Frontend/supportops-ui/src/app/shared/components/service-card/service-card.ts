import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-service-card',
  standalone: true,
  imports: [ MatButtonModule, MatCardModule ],
  templateUrl: './service-card.html',
  styleUrl: './service-card.css',
})
export class ServiceCard {}
