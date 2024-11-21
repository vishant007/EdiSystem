import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl; // Base API URL

  constructor(private http: HttpClient) {}

  // Register method
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/auth/register`, user);
  }

  // Login method
  login(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/auth/authenticate`, user);
  }

  // Save JWT token
  saveToken(token: string): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.setItem('jwt_token', token);
    }
  }

  // Get JWT token
  getToken(): string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem('jwt_token');
    }
    return null;
  }

  // Get userId from JWT token
  getUserIdFromToken(): string | null {
    const token = this.getToken();
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1])); // Decode the JWT token to get the payload
        return payload.nameid; // Extract userId (nameid is used for User Id)
      } catch (error) {
        console.error('Error decoding token', error);
      }
    }
    return null;
  }

  // Remove JWT token
  logout(): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      localStorage.removeItem('jwt_token');
    }
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return this.getToken() !== null;
  }

  // Add the Auth header for Watchlist service
  getAuthHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json', // Matches backend's expected content type
      'Authorization': `Bearer ${this.getToken()}`, // Add the JWT token in the Authorization header
    });
  }
}
