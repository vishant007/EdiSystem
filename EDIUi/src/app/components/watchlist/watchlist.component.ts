import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { WatchlistService } from '../../services/watchlist.service';
import { CartService } from '../../services/cart.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CookieService } from 'ngx-cookie-service'; // Import CookieService

@Component({
  selector: 'app-watchlist',
  templateUrl: './watchlist.component.html',
  styleUrls: ['./watchlist.component.css'],
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
})
export class WatchlistComponent implements OnInit {
  watchlist: any[] = [];
  cart: any[] = []; // Cart items
  containerDetails: any[] = [];
  filteredContainers: any[] = [];
  errorMessage: string = '';
  searchQuery: string = ''; // For search
  selectedStatus: string = ''; // For filter

  showSearch: boolean = false; // Toggle for search bar
  showFilter: boolean = false; // Toggle for filter dropdown
  showAddModal: boolean = false; // Toggle for Add container modal
  newContainerNumber: string = ''; // Container number for adding
  addedToCart: Set<string> = new Set();

  expandedRows: { [key: number]: boolean } = {}; // Tracks expanded rows

  constructor(
    private authService: AuthService,
    private watchlistService: WatchlistService,
    private cartService: CartService,
    private router: Router,
    private cookieService: CookieService // Inject CookieService
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.getWatchlist(userId);
    } else {
      this.router.navigate(['/login']);
    }

    this.loadCartFromCookies(); // Load cart from cookies
  }

  getWatchlist(userId: string) {
    this.watchlistService.getWatchlist(userId).subscribe(
      (data) => {
        this.watchlist = data;
        this.getContainerDetails();
      },
      (error) => {
        this.errorMessage = 'Failed to fetch watchlist!';
      }
    );
  }

  getContainerDetails() {
    this.containerDetails = []; // Reset container details to avoid duplicate entries
    this.watchlist.forEach((container) => {
      this.watchlistService.getContainerDetails(container.containerNumber).subscribe(
        (data) => {
          console.log(data);
          this.containerDetails.push(data);
          this.filteredContainers = [...this.containerDetails]; // Initialize filtered list
        },
        (error) => {
          this.errorMessage = 'Failed to fetch container details!';
        }
      );
    });
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

      return matchesSearch && matchesStatus;
    });
  }

  addToWatchlist() {
    const userId = this.authService.getUserIdFromToken();
    if (userId && this.newContainerNumber) {
      this.watchlistService.addToWatchlist(userId, this.newContainerNumber).subscribe(
        (response) => {
          console.log(response);
          this.getWatchlist(userId); // Refresh the watchlist immediately
          this.closeAddModal(); // Close the modal
        },
        (error) => {
          this.errorMessage = 'Failed to add container to watchlist!';
        }
      );
    }
  }

  removeFromWatchlist(containerNumber: string) {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.watchlistService.removeFromWatchlist(userId, containerNumber).subscribe(
        (response) => {
          this.watchlist = this.watchlist.filter((container) => container.containerNumber !== containerNumber);
          this.containerDetails = this.containerDetails.filter((container) => container.ContainerNumber !== containerNumber);
          this.filteredContainers = [...this.containerDetails]; // Update filtered containers
          console.log('Container removed from watchlist');
        },
        (error) => {
          this.errorMessage = 'Failed to remove container from watchlist!';
        }
      );
    }
  }

  addToCart(container: any) {
    if (!this.cart.find((c) => c.ContainerNumber === container.ContainerNumber)) {
      this.cart.push(container);
      this.addedToCart.add(container.ContainerNumber); // Mark as added to cart
      this.saveCartToCookies(); // Save cart to cookies
    } else {
      console.log('Container already in cart');
    }
  }

  loadCartFromCookies() {
    const cartData = this.cookieService.get('cart');
    if (cartData) {
      try {
        this.cart = JSON.parse(cartData) || [];
        this.cart.forEach((item) => this.addedToCart.add(item.ContainerNumber));
      } catch (error) {
        console.error('Error parsing cart data from cookies:', error);
      }
    }
  }

  saveCartToCookies() {
    try {
      this.cookieService.set('cart', JSON.stringify(this.cart), { expires: 7, path: '/' });
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
