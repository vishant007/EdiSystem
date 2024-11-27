import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartKey = 'cart'; 
  constructor(private cookieService: CookieService) {}

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


  removeCartItem(containerNumber: string): void {
    const cart = this.getCartItems();
    const updatedCart = cart.filter((item) => item.ContainerNumber !== containerNumber);
    this.saveCartItems(updatedCart);
  }


  clearCart(): void {
    this.cookieService.delete(this.cartKey, '/');
  }


  private saveCartItems(cart: any[]): void {
    try {
      this.cookieService.set(this.cartKey, JSON.stringify(cart), { expires: 7, path: '/' });
    } catch (error) {
      console.error('Error saving cart data to cookies:', error);
    }
  }
}