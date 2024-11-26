import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environment'; 

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  standalone: true,
  imports: [RouterModule, CommonModule],
  styleUrls: ['./cart.component.css'],
})
export class CartComponent implements OnInit {
  cartItems: any[] = [];
  totalDemurrageFees: number = 0;
  totalOtherPayments: number = 0;

  // New properties for receipt and loader
  showReceipt: boolean = false;
  showLoader: boolean = false; // Loader visibility
  receiptData: any = null;

  constructor(
    private cartService: CartService,
    private http: HttpClient,
    private router: Router,
    private cookieService: CookieService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.loadCartFromCookies(userId);
    } else {
      this.router.navigate(['/login']);
    }
    this.calculateTotals();
  }

  calculateTotals() {
    this.totalDemurrageFees = this.cartItems.reduce(
      (total, item) => total + (item.TotalDemurrageFees || 0),
      0
    );
    this.totalOtherPayments = this.cartItems.reduce(
      (total, item) => total + (item.OtherPayments || 0),
      0
    );
  }

  removeItem(containerNumber: string) {
    this.cartService.removeCartItem(containerNumber);
    this.cartItems = this.cartItems.filter(
      (item) => item.ContainerNumber !== containerNumber
    );
    const userId = this.authService.getUserIdFromToken();
    if (userId) {
      this.saveCartToCookies(userId);
    }
    this.calculateTotals();
  }

  submitPayments() {
    const paymentData = this.cartItems.map((item) => ({
      containerNumber: item.ContainerNumber,
      totalDemurrageFees: item.TotalDemurrageFees,
      otherPayments: item.OtherPayments,
    }));
  
    const paymentEndpoint = `${environment.paymentapi}/api/Payment/UpdatePayments`;
  
    this.showLoader = true; // Show the loader
  
    this.http.post(paymentEndpoint, paymentData, {
    }).subscribe({
      next: (blob) => {
  
        alert('Payment is successful!');
        this.router.navigate(['/watchlist']);
      },
      error: (error) => {
        console.error('Error submitting payment data', error);
        alert('Error submitting payment data.');
      },
      complete: () => {
        this.showLoader = false; // Hide the loader after completion
      },
    });
  }
  

  

  loadCartFromCookies(userId: string) {
    const cartData = this.cookieService.get('cart_' + userId);
    if (cartData) {
      try {
        this.cartItems = JSON.parse(cartData) || [];
      } catch (error) {
        console.error('Error parsing cart data from cookies:', error);
      }
    }
  }

  saveCartToCookies(userId: string) {
    try {
      this.cookieService.set('cart_' + userId, JSON.stringify(this.cartItems), {
        expires: 7,
        path: '/',
      });
    } catch (error) {
      console.error('Error saving cart data to cookies:', error);
    }
  }
}

