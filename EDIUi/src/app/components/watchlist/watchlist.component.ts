import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { WatchlistService } from '../../services/watchlist.service';
import { CartService } from '../../services/cart.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ChangeDetectorRef } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css'],
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
})
export class WatchlistComponent implements OnInit {
  watchlist: any[] = [];
  cart: any[] = [];
  containerDetails: any[] = [];
  filteredContainers: any[] = [];
  errorMessage: string = '';
  searchQuery: string = '';
  selectedStatus: string = '';
  selectedFeeStatus: string = '';

  showSearch: boolean = false;
  showFilter: boolean = false;
  showAddModal: boolean = false;
  newContainerNumber: string = '';
  addedToCart: Set<string> = new Set();

  expandedRows: { [key: number]: boolean } = {};

  constructor(
    private authService: AuthService,
    private watchlistService: WatchlistService,
    private cartService: CartService,
    private router: Router,
    private cookieService: CookieService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.getWatchlist(userId);
      this.loadCartFromCookies(userId); // Load cart based on userId
    } else {
      this.router.navigate(['/login']);
    }
  }

  toggleRow(index: number): void {
    this.expandedRows[index] = !this.expandedRows[index];
  }

  toggleSearch() {
    this.showSearch = !this.showSearch;
    this.showFilter = false; // Hide filter if search is shown
  }

  toggleFilter() {
    this.showFilter = !this.showFilter;
    this.showSearch = false; // Hide search if filter is shown
  }

  filterContainers() {
    this.filteredContainers = this.containerDetails.filter((container) => {
      const matchesSearch =
        this.searchQuery === '' ||
        container.ContainerNumber.toLowerCase().includes(this.searchQuery.toLowerCase());

      const matchesStatus =
        this.selectedStatus === '' || container.ShipmentStatusCode === this.selectedStatus;

      const matchesFeeStatus =
        this.selectedFeeStatus === '' || container.FeeStatus === this.selectedFeeStatus;

      return matchesSearch && matchesStatus && matchesFeeStatus;
    });
  }
  loading: boolean = false; // For displaying the loader


  getWatchlist(userId: string) {
    const headers = this.authService.getAuthHeaders(); // Get Auth Headers
    this.loading = true; // Start loader
    this.errorMessage = ''; // Reset any error message

    this.watchlistService.getWatchlist(userId).subscribe(
      (data) => {
        this.watchlist = data; // Update watchlist
        this.getContainerDetails(headers); // Fetch container details
        this.loading = false; // Stop loader
      },
      (error) => {
        this.errorMessage = 'Failed to fetch watchlist!';
        this.loading = false; // Stop loader in case of error
      }
    );
  }
  
  
  getContainerDetails(headers: any) {
    this.containerDetails = []; // Clear existing container details
    const observables = this.watchlist.map((container) =>
      this.watchlistService.getContainerDetails(container.containerNumber)
    );
  
    // Use `forkJoin` to wait for all API calls to complete
    forkJoin(observables).subscribe(
      (data) => {
        this.containerDetails = data; // Populate container details
        this.filteredContainers = [...this.containerDetails];
        this.loading = false; // Stop loader
        this.cdr.detectChanges(); // Ensure UI refreshes
      },
      (error) => {
        this.errorMessage = 'Failed to fetch container details!';
        this.loading = false; // Stop loader in case of error
      }
    );
  }
  
  addToWatchlist() {
    const userId = this.authService.getUserIdFromToken();
    if (userId && this.newContainerNumber) {
      this.loading = true; // Show loader during add operation
      this.errorMessage = ''; // Reset any error message
      
      // Set a 10-second timeout to refresh the page
      setTimeout(() => {
        window.location.reload();
      }, 10000);
  
      this.watchlistService.addToWatchlist(userId, this.newContainerNumber).subscribe(
        (response) => {
          this.getWatchlist(userId); // Refresh watchlist after addition
          this.closeAddModal();
        },
        (error) => {
          this.errorMessage = 'Failed to add container to watchlist!';
          this.loading = false; // Stop loader in case of error
        }
      );
    }
  }
  
  removeFromWatchlist(containerNumber: string) {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.loading = true; // Show loader during removal operation
      this.errorMessage = ''; // Reset any error message
      
      // Set a 10-second timeout to refresh the page
      setTimeout(() => {
        window.location.reload();
      }, 10000);
  
      this.watchlistService.removeFromWatchlist(userId, containerNumber).subscribe(
        (response) => {
          this.getWatchlist(userId); // Refresh watchlist after removal
        },
        (error) => {
          this.errorMessage = 'Failed to remove container from watchlist!';
          this.loading = false; // Stop loader in case of error
        }
      );
    }
  }
  
  addToCart(container: any) {
    if (!this.cart.find((c) => c.ContainerNumber === container.ContainerNumber)) {
      this.cart.push(container);
      this.addedToCart.add(container.ContainerNumber);
      const userId = this.authService.getUserIdFromToken();
      if (userId) {
        this.saveCartToCookies(userId); // Save cart to cookies based on userId
      }
    } else {
      console.log('Container already in cart');
    }
  }

  loadCartFromCookies(userId: string) {
    const cartData = this.cookieService.get(`cart_${userId}`);
    if (cartData) {
      try {
        this.cart = JSON.parse(cartData) || [];
        this.cart.forEach((item) => this.addedToCart.add(item.ContainerNumber));
      } catch (error) {
        console.error('Error parsing cart data from cookies:', error);
      }
    }
  }

  saveCartToCookies(userId: string) {
    try {
      this.cookieService.set(`cart_${userId}`, JSON.stringify(this.cart), { expires: 7, path: '/' });
    } catch (error) {
      console.error('Error saving cart data to cookies:', error);
    }
  }

  openAddContainerModal() {
    this.showAddModal = true;
  }

  closeAddModal() {
    this.showAddModal = false;
  }
}
