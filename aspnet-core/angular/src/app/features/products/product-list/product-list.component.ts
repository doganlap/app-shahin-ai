import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../core/services/product.service';
import { ProductDto } from '../../../core/models/product.models';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: ProductDto[] = [];
  loading = false;
  currentLanguage: 'en' | 'ar' = 'en';

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getList().subscribe({
      next: (products) => {
        this.products = products.filter(p => p.isActive);
        this.products.sort((a, b) => a.displayOrder - b.displayOrder);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.loading = false;
      }
    });
  }

  getProductName(product: ProductDto): string {
    return this.currentLanguage === 'ar' ? product.name.ar : product.name.en;
  }

  getProductDescription(product: ProductDto): string {
    if (!product.description) return '';
    return this.currentLanguage === 'ar' ? product.description.ar : product.description.en;
  }

  getMonthlyPrice(product: ProductDto): number | null {
    const monthlyPlan = product.pricingPlans?.find(p => p.billingPeriod === 'Monthly');
    return monthlyPlan?.price ?? null;
  }

  getYearlyPrice(product: ProductDto): number | null {
    const yearlyPlan = product.pricingPlans?.find(p => p.billingPeriod === 'Yearly');
    return yearlyPlan?.price ?? null;
  }

  hasFeature(product: ProductDto, featureCode: string): boolean {
    return product.features?.some(f => f.featureCode === featureCode && f.isEnabled) ?? false;
  }

  getQuotaLimit(product: ProductDto, quotaType: string): string {
    const quota = product.quotas?.find(q => q.quotaType === quotaType);
    if (!quota || quota.isUnlimited) return 'Unlimited';
    return `${quota.limit} ${quota.unit || ''}`;
  }
}

