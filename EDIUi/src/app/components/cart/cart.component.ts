import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../services/auth.service';  // Import the AuthService

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
    private authService: AuthService // Inject AuthService to get user info
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserIdFromToken(); // Get user ID from token
    if (userId) {
      this.loadCartFromCookies(userId); // Load cart items for the specific user
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

  submitPayments() {
    const paymentData = this.cartItems.map((item) => ({
      containerNumber: item.ContainerNumber,
      totalDemurrageFees: item.TotalDemurrageFees,
      otherPayments: item.OtherPayments,
    }));

    this.http
      .post('http://localhost:4000/api/Payment/UpdatePayments', paymentData, {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: (response: any) => {
          console.log('Payment data submitted successfully', response);

          // Empty the cart and clear cookies for the user
          const userId = this.authService.getUserIdFromToken();
          if (userId) {
            this.cartService.clearCart();
            this.cookieService.delete('cart_' + userId); // Delete the user-specific cookie
          }

          // Show success message
          alert('Payment is successful!');

          // Set receipt data and show receipt
          this.receiptData = {
            transactionId: response.TransactionIds.join(','),
            userId: response.UserId,
            totalDemurrageFees: this.totalDemurrageFees,
            otherPayments: this.totalOtherPayments,
            transactionDate: new Date().toISOString(),
          };

          // Toggle the flag to show receipt
          this.showReceipt = true;
        },
        error: (error) => {
          console.error('Error submitting payment data', error);
          alert('Error submitting payment data.');
        },
      });
  }

  // Load cart items from the cookie for the specific user
  loadCartFromCookies(userId: string) {
    const cartData = this.cookieService.get('cart_' + userId); // Use user-specific cookie
    if (cartData) {
      try {
        this.cartItems = JSON.parse(cartData) || [];
      } catch (error) {
        console.error('Error parsing cart data from cookies:', error);
      }
    }
  }

  // Save cart items to the cookie for the specific user
  saveCartToCookies(userId: string) {
    try {
      this.cookieService.set('cart_' + userId, JSON.stringify(this.cartItems), { expires: 7, path: '/' }); // Store in user-specific cookie
    } catch (error) {
      console.error('Error saving cart data to cookies:', error);
    }
  }
}
