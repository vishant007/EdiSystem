import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartKey = 'cart'; // Key to store cart items in cookies

  constructor(private cookieService: CookieService) {}

  // Get cart items from cookies
  getCartItems(): any[] {
    const cartData = this.cookieService.get(this.cartKey);
    if (cartData) {
      try {
        return JSON.parse(cartData);
      } catch (error) {
        console.error('Error parsing cart data from cookies:', error);
        return [];
      }
    }
    return [];
  }

  // Add an item to the cart
  addToCart(item: any): void {
    const cart = this.getCartItems();
    const alreadyInCart = cart.find(
      (cartItem) => cartItem.ContainerNumber === item.ContainerNumber
    );
    if (!alreadyInCart) {
      cart.push(item);
      this.saveCartItems(cart);
    }
  }

  // Remove an item from the cart
  removeCartItem(containerNumber: string): void {
    const cart = this.getCartItems();
    const updatedCart = cart.filter((item) => item.ContainerNumber !== containerNumber);
    this.saveCartItems(updatedCart);
  }

  // Clear all cart items
  clearCart(): void {
    this.cookieService.delete(this.cartKey, '/');
  }

  // Save cart items to cookies
  private saveCartItems(cart: any[]): void {
    try {
      this.cookieService.set(this.cartKey, JSON.stringify(cart), { expires: 7, path: '/' });
    } catch (error) {
      console.error('Error saving cart data to cookies:', error);
    }
  }
}