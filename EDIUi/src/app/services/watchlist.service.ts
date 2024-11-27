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

  
  constructor(private http: HttpClient, private authService: AuthService) {}

  
  getWatchlist(userId: string): Observable<any> {
    const headers = this.authService.getAuthHeaders(); 
    return this.http.get(`${this.apiUrl}/api/watchlist/${userId}`, { headers });
  }

  addToWatchlist(userId: string, containerNumber: string): Observable<any> {
    const headers = this.authService.getAuthHeaders(); 
    return this.http.post<any>(
      `${this.apiUrl}/api/watchlist/${userId}/add`, 
      JSON.stringify(containerNumber), 
      { headers }
    );
  }
  

  removeFromWatchlist(userId: string, containerNumber: string): Observable<any> {
    const headers = this.authService.getAuthHeaders(); 
    return this.http.post(
      `${this.apiUrl}/api/watchlist/${userId}/remove`,
      JSON.stringify(containerNumber), 
      { headers }
    );
  }
  
   getContainerDetails(containerNumber: string): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get(`${this.apiUrl}/api/watchlist/details/${containerNumber}`, { headers });
  }
}
