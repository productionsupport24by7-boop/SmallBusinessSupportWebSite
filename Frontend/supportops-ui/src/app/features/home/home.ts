import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [MatButtonModule, MatCardModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  ngOnInit(): void {
    console.log('ngOnInit called');
  }
  ngviewInit(): void {
    console.log('ngviewInit called');
  }

  ngbeforeViewInit(): void {
    console.log('ngbeforeViewInit called');
  }

  ngbeforeViewChecked(): void {
    console.log('ngbeforeViewChecked called');
  }

  ngafterContentChecked(): void {
    console.log('ngafterContentChecked called');
  }
  ngbeforeContentChecked(): void {
    console.log('ngbeforeContentChecked called');
  }

  constructor() {
    console.log('constructor called');
  }

  ngOnChanges() {
    console.log('ngOnChanges called');
  }

  ngDoCheck() {
    console.log('ngDoCheck called');
  }

  ngAfterContentInit() {
    console.log('ngAfterContentInit called');
  }

  ngAfterContentChecked() {
    console.log('ngAfterContentChecked called');
  }

  ngAfterViewInit() {
    console.log('ngAfterViewInit called');
  }

  ngAfterViewChecked() {
    console.log('ngAfterViewChecked called');
  }

  ngOnDestroy() {
    console.log('ngOnDestroy called');
  }
}
