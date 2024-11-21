import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { WatchlistService } from '../../services/watchlist.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css'],
  standalone:true,
  imports:[RouterModule, CommonModule, FormsModule]
})
export class WatchlistComponent implements OnInit {
  watchlist: any[] = [];
  newContainerNumber: string = '';
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private watchlistService: WatchlistService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.getWatchlist(userId);
    } else {
      this.router.navigate(['/login']); // If not authenticated, redirect to login page
    }
  }

  // Get watchlist of the user
  getWatchlist(userId: string) {
    this.watchlistService.getWatchlist(userId).subscribe(
      (data) => {
        this.watchlist = data;
      },
      (error) => {
        this.errorMessage = 'Failed to fetch watchlist!';
      }
    );
  }
}
