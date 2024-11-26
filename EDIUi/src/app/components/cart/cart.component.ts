import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../services/auth.service';

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

  // New properties for receipt
  showReceipt: boolean = false;
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

  // Remove item from cart
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
  
    this.http
      .post('http://localhost:5284/api/Payment/UpdatePayments', paymentData, {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: (response: any) => {
          console.log('Payment data submitted successfully', response);
  
          const userId = this.authService.getUserIdFromToken();
          if (userId) {
            this.cartService.clearCart();
            this.cookieService.delete('cart_' + userId);
          }
  
          alert('Payment is successful!');
          this.showReceipt = true;
  
          // Navigate to the watchlist page after alert
          this.router.navigate(['/watchlist']);
        },
        error: (error) => {
          console.error('Error submitting payment data', error);
          alert('Error submitting payment data.');
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
