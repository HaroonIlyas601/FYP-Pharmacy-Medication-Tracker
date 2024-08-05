import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup,FormControl, Validators } from '@angular/forms';
import { CartService } from 'src/app/core/services/cart.service';
import { ShippingForm } from './model/ShippingForm.model';
import { AuthService } from 'src/app/shared/services/auth/auth.service';
import { ProductService } from '../../services/product.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styles: [
  ]
})
export class CheckoutComponent implements OnInit{
  gstAmount!:number;
  grandTotal!:number;
  shippingForm!:FormGroup;
  showPayment: boolean = false;
  selectedPaymentMethod: string = '';
  constructor(private cartService:CartService, private formBulider:FormBuilder, private productService:ProductService){
    this.shippingForm=this.formBulider.group({
      firstName:new FormControl('', [Validators.required, Validators.minLength(3),Validators.maxLength(15)]),
      lastName:new FormControl('', [Validators.minLength(3),Validators.maxLength(15)]),
      email:new FormControl('',[Validators.required,Validators.email]),
      mobile:new FormControl('',[Validators.required,Validators.minLength(10)]),
      address:new FormControl('',[Validators.required]),
      city:new FormControl('',[Validators.required]),
      state:new FormControl('',[Validators.required]),
      country:new FormControl('Pakistan',[Validators.required]),
      postalCode:new FormControl('',[Validators.required]),
      paymentMethod: new FormControl('', [Validators.required]),
      visaCardNumber: new FormControl(''),
      jazzCashNumber: new FormControl('')
    })
  }
  ngOnInit(): void {
    this.getTotal();
  }
  getTotal(){
    this.cartService.gstAmount.subscribe(data=>this.gstAmount=parseInt(data.toFixed(2)));
    this.cartService.estimatedTotal.subscribe(data=>this.grandTotal=parseInt(data.toFixed(2)));
  }

  get firstName(){
    return this.shippingForm.get('firstName');
  }
  get lastName(){
    return this.shippingForm.get('lastName');
  }
  get email(){
    return this.shippingForm.get('email');
  }
  get mobile(){
    return this.shippingForm.get('mobile');
  }
  get address(){
    return this.shippingForm.get('address');
  }
  get state(){
    return this.shippingForm.get('state');
  }
  get city(){
    return this.shippingForm.get('city');
  }
  get country(){
    return this.shippingForm.get('country');
  }
  get postalCode(){
    return this.shippingForm.get('postalCode');
  }
  get paymentMethod() { 
    return this.shippingForm.get('paymentMethod');
   }
  get visaCardNumber() {
     return this.shippingForm.get('visaCardNumber');
     }
  get jazzCashNumber() { 
    return this.shippingForm.get('jazzCashNumber');
   }
  // onSave(){
  //   alert(JSON.stringify(this.shippingForm.value));
  //   this.shippingForm.reset();
  // }
  onSave() {
    if (this.shippingForm.valid) {
      this.productService.saveShippingDetails(this.shippingForm.value).subscribe(
        response => {
          alert('Shipping details saved successfully!');
          this.shippingForm.reset();
          this.showPayment = true;
        },
        error => {
          console.error('Error saving shipping details', error);
        }
      );
    }
  }
 onPaymentMethodChange(event: any) {
    this.selectedPaymentMethod = event.target.value;
    const visaCardNumberControl = this.shippingForm.get('visaCardNumber') ?? new FormControl('');
    const jazzCashNumberControl = this.shippingForm.get('jazzCashNumber') ?? new FormControl('');

    if (this.selectedPaymentMethod === 'visa') {
      visaCardNumberControl.setValidators([Validators.required]);
      jazzCashNumberControl.clearValidators();
    } else if (this.selectedPaymentMethod === 'jazzcash') {
      jazzCashNumberControl.setValidators([Validators.required]);
      visaCardNumberControl.clearValidators();
    } else {
      visaCardNumberControl.clearValidators();
      jazzCashNumberControl.clearValidators();
    }

    visaCardNumberControl.updateValueAndValidity();
    jazzCashNumberControl.updateValueAndValidity();
  }
  
  onPlaceOrder() {
    if (this.shippingForm.valid) {
      const orderDetails = {
        totalAmount: this.grandTotal,
        orderItems: this.cartService.getCart.map(item => ({
          productId: item.id,
          quantity: item.qty
        }))
      };

      const orderRequest = {
        shippingDetail: this.shippingForm.value,
        order: orderDetails
      };

      this.productService.placeOrder(orderRequest).subscribe(
        (orderResponse: any) => {
          alert('Order placed successfully!');
          this.cartService.clearCart();
          this.shippingForm.reset();
        },
        (error: any) => {
          console.error('Error placing order', error);
        }
      );
    }
  }
  
}
