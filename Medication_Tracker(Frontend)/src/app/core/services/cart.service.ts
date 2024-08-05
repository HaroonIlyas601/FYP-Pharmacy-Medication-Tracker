import { Injectable } from '@angular/core';
import { Product } from 'src/app/modules/product/model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cart: Product[] = [];
  private products = new BehaviorSubject<Product[]>([]);
  public totalAmount = new BehaviorSubject<number>(0);
  public gstAmount = new BehaviorSubject<number>(0);
  public estimatedTotal = new BehaviorSubject<number>(0);

  constructor() { }

  public get getCart() {
    return this.cart;
  }

  public add(product: Product) {
    this.cart.push(product);
    this.updateQtyAndTotalPrice(product);
    this.products.next(this.cart);
    this.updateTotals();
  }

  public remove(product: Product) {
    this.cart.map((prod: Product, index: number) => {
      if (product.id === prod.id) {
        this.cart.splice(index, 1);
      }
    });
    this.products.next(this.cart);
    this.updateTotals();
  }

  public clearCart() {
    //this.cart = [];
    for (let i = this.cart.length - 1; i >= 0; i--) {
      this.cart.splice(i, 1);
    }
    this.products.next(this.cart);
    this.totalAmount.next(0);
    this.gstAmount.next(0);
    this.estimatedTotal.next(0);
    
  }

  updateQtyAndTotalPrice(item: Product) {
    const index = this.find(item);
    const products = this.getCart;
    let totalQty = products[index].qty = 1;
    totalQty = totalQty;
    let subTotal = products[index].price * totalQty;
    products[index].totalprice = subTotal;
    this.updateTotals();
  }

  find(item: Product): number {
    const products = this.getCart;
    const index = products.findIndex((prod) => {
      return prod.id == item.id;
    })
    return index;
  }

  getTotal(): number {
    const total = this.cart.reduce((r: any, c: any) => r = r + c.totalprice, 0);
    const gstRate = 0.18;
    this.totalAmount.next(total);
    this.gstAmount.next(gstRate * total);
    this.estimatedTotal.next(total + this.gstAmount.value);
    return total;
  }

  addQty(item: Product) {
    const products = this.getCart;
    let index = this.find(item);
    let totalQty = products[index].qty;
    if (totalQty !== 12) {
      totalQty = totalQty && totalQty + 1;
    }
    products[index].qty = totalQty;
    let subTotal = products[index].price * totalQty!;
    products[index].totalprice = subTotal;
    this.updateTotals();
  }

  lessQty(item: Product) {
    const products = this.getCart;
    let index = this.find(item);
    let totalQty = products[index].qty;
    if (totalQty !== 1) {
      totalQty = totalQty && totalQty - 1;
    }
    products[index].qty = totalQty;
    let subTotal = products[index].price * totalQty!;
    products[index].totalprice = subTotal;
    this.updateTotals();
  }

  private updateTotals() {
    const total = this.cart.reduce((r: any, c: any) => r = r + c.totalprice, 0);
    const gstRate = 0.18;
    this.totalAmount.next(total);
    this.gstAmount.next(gstRate * total);
    this.estimatedTotal.next(total + this.gstAmount.value);
  }
}