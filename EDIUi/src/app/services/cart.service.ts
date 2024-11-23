import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartItems: any[] = [];

  // Get cart items
  getCartItems() {
    return this.cartItems;
  }

  // Add an item to the cart
  addToCart(item: any) {
    const alreadyInCart = this.cartItems.find(
      (cartItem) => cartItem.ContainerNumber === item.ContainerNumber
    );
    if (!alreadyInCart) {
      this.cartItems.push(item);
    }
  }
}
