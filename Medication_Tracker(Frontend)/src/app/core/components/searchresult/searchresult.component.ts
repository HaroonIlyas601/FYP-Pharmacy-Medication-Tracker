import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { BehaviorSubject, Subscription } from 'rxjs';
import { Product } from 'src/app/modules/product/model';
import { ProductService } from 'src/app/modules/product/services/product.service';
import { FilterService } from 'src/app/modules/product/services/filter.service';

@Component({
  selector: 'app-searchresult',
  templateUrl: './searchresult.component.html',
  styles: []
})
//, OnDestroy
export class SearchresultComponent implements OnInit {
  products: Product[] = [];
  cloneOfProducts: Product[] = [];
  category= '';
  isLoading = false;
  error!: string;
  searchKeyword!: string;
  isFilter = false;
  subsFilterProducts!: Subscription;

  selectedFilter: { rating: BehaviorSubject<number | null>; categoryId: BehaviorSubject<number | null> } = {
    rating: new BehaviorSubject<number | null>(null),
    categoryId: new BehaviorSubject<number | null>(null)
  }
  ratingList: boolean[] = [];
  range: number = 10; // default range in km
  userLocation: { lat: number, lon: number } = { lat: 0, lon: 0 };
  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private filterService: FilterService
  ) {}

  ngOnInit(): void {
    this.getUserLocation();
    this.route.queryParams.subscribe((params: Params) => {
      this.searchKeyword = params['q'];
      this.getResults();
    });
  }
  getResults() {
    this.isLoading = true;
    if (this.userLocation.lat !== 0 && this.userLocation.lon !== 0) {
      this.productService.searchProductsNearby(this.searchKeyword, this.userLocation.lat, this.userLocation.lon, this.range).subscribe(
        (data) => {
          this.isLoading = false;
          this.products = data;
          this.cloneOfProducts = data;
          this.filterService.filterProduct(data);
          console.log( this.products);
        },
        (error) => {
          this.isLoading = false;
          this.error = error.message;
        }
      );
    } 
  }

  handleFilter() {
    this.subsFilterProducts = this.filterService.filteredProducts.subscribe((data) => {
      this.products = data;
    });
  }

  onFilter(value: boolean) {
    this.isFilter = value;
  }

  resetFilter() {
    this.selectedFilter.categoryId.next(null);
    this.selectedFilter.rating.next(null);
  }
  getUserLocation() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((position) => {
        this.userLocation = {
          lat: position.coords.latitude,
          lon: position.coords.longitude
        };
        this.getResults();
      });
    } else {
      console.error("Geolocation is not supported by this browser.");
    }
  }

  // getNearbyPharmacies() {
  //   if (this.userLocation.lat !== 0 && this.userLocation.lon !== 0) {
  //     this.isLoading = true;
  //     this.productService.getNearbyPharmacies(this.userLocation.lat, this.userLocation.lon, this.range).subscribe(
  //       (data: any) => {
  //         this.isLoading = false;
  //         this.products = data;
  //       },
  //       (error) => {
  //         this.isLoading = false;
  //         this.error = error.message;
  //       }
  //     );
  //   }
  // }

  onRangeChange() {
    this.getResults();
  }

  

  // ngOnDestroy(): void {
  //   this.subsFilterProducts.unsubscribe();
  // }
}
