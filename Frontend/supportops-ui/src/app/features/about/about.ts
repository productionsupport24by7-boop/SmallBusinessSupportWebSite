import { Component } from '@angular/core';
import { inject } from '@angular/core';
import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [],
  templateUrl: './about.html',
  styleUrl: './about.css',
})
export class About {

  // private readonly authService = inject(AuthService);

  // constructor() {
  //   console.log(
  //     'Current role:',
  //     this.authService.getRole()
  //   );
  // }
}
