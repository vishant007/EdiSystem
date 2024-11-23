import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

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

  constructor(private cartService: CartService) {}

  ngOnInit(): void {
    this.cartItems = this.cartService.getCartItems();

    // Calculate total fees
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
}
