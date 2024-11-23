import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { WatchlistService } from '../../services/watchlist.service';
import { CartService } from '../../services/cart.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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

  expandedRows: { [key: number]: boolean } = {}; // Tracks expanded rows

  constructor(
    private authService: AuthService,
    private watchlistService: WatchlistService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.getWatchlist(userId);
    } else {
      this.router.navigate(['/login']);
    }
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

  // Toggle the search bar
  toggleSearch() {
    this.showSearch = !this.showSearch;
    this.showFilter = false; // Hide filter if search is shown
  }

  // Toggle the filter dropdown
  toggleFilter() {
    this.showFilter = !this.showFilter;
    this.showSearch = false; // Hide search if filter is shown
  }

  // Filter containers based on search query and status
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

  // Add the container to the watchlist
  addToWatchlist() {
    const userId = this.authService.getUserIdFromToken();
    if (userId && this.newContainerNumber) {
      this.watchlistService.addToWatchlist(userId, this.newContainerNumber).subscribe(
        (response) => {
          console.log(response);
          this.getWatchlist(userId); // Refresh the watchlist
          this.closeAddModal(); // Close the modal
        },
        (error) => {
          this.errorMessage = 'Failed to add container to watchlist!';
        }
      );
    }
  }

  // Add a container to the cart
  addToCart(container: any) {
    this.cartService.addToCart(container);
  }

  // Open the Add Container modal
  openAddContainerModal() {
    this.showAddModal = true;
  }

  // Close the Add Container modal
  closeAddModal() {
    this.showAddModal = false;
  }
}
