import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [ MatButtonModule, MatCardModule ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {}
