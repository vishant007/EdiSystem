import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

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

  constructor(private cartService: CartService, private http: HttpClient) {}

  ngOnInit(): void {
    this.cartItems = this.cartService.getCartItems();

    // Calculate total fees for UI
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

  // Send only individual item details to the backend
  submitPayments() {
    // Prepare data in the required format without total fields
    const paymentData = this.cartItems.map((item) => ({
      containerNumber: item.ContainerNumber,
      totalDemurrageFees: item.TotalDemurrageFees,
      otherPayments: item.OtherPayments,
    }));

    // Send a POST request to the backend
    this.http
      .post('http://localhost:4000/api/Payment/UpdatePayments', paymentData, {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: (response) => {
          console.log('Payment data submitted successfully', response);
          alert('Payment data submitted successfully!');
        },
        error: (error) => {
          console.error('Error submitting payment data', error);
          alert('Error submitting payment data.');
        },
      });
  }
}
