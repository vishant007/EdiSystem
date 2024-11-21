import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule]
})
export class LoginComponent implements OnInit {
  email: string = '';  // This is the email field
  password: string = '';  // This is the password field
  errorMessage: string = '';  // This is the error message to be displayed

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  login(): void {
    const user = { email: this.email, password: this.password };  // We create the user object with email and password

    this.authService.login(user).subscribe(
      (response) => {
        if (response.token) {
          this.authService.saveToken(response.token);  // Save the JWT token to localStorage
          const userId = this.authService.getUserIdFromToken();  // Extract userId from token (You can decode JWT token to get userId)
          if (userId) {
            this.router.navigate(['/watchlist']);  // Navigate to the Watchlist page
          }
        }
      },
      (error) => {
        this.errorMessage = 'Invalid credentials. Please try again.';  // Show an error message if login fails
      }
    );
  }
}
