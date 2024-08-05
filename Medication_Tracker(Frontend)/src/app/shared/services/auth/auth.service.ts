import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private router:Router , private http:HttpClient) { }
  
  register(userData: any): Observable<any> {
    return this.http.post(`https://localhost:44371/api/users/register`, userData);
  }

  login(email: string, password: string): Observable<any> {
    localStorage.setItem('isLogged','true');
    return this.http.post(`https://localhost:44371/api/Users/login`, { email, password });
  
    localStorage.setItem('isLogged','true');
  }
  logout(){
    this.router.navigate(['/']);
    localStorage.removeItem('isLogged');
  }
  isLoggedIn(){
    return localStorage.getItem('isLogged');
  }
  saveShippingDetails(details: any) {
    return this.http.post(`https://localhost:44371/api/Products/save-shipping-details`, details);
  }
}
