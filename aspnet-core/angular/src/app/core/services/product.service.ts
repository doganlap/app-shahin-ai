import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductDto, ProductDetailDto, CreateProductInput, UpdateProductInput } from '../models/product.models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = '/api/grc/products';

  constructor(private http: HttpClient) {}

  getList(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(this.apiUrl);
  }

  getById(id: string): Observable<ProductDetailDto> {
    return this.http.get<ProductDetailDto>(`${this.apiUrl}/${id}`);
  }

  create(input: CreateProductInput): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, input);
  }

  update(id: string, input: UpdateProductInput): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.apiUrl}/${id}`, input);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

