import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment';
import { AuthService } from './auth.service'; 

@Injectable({
  providedIn: 'root',
})
export class WatchlistService {
  private apiUrl = environment.apiUrl;

  // Inject AuthService into the constructor
  constructor(private http: HttpClient, private authService: AuthService) {}

  // Get the watchlist of the user
  getWatchlist(userId: string): Observable<any> {
    const headers = this.authService.getAuthHeaders(); // Use authService to get headers
    return this.http.get(`${this.apiUrl}/api/watchlist/${userId}`, { headers });
  }
}
