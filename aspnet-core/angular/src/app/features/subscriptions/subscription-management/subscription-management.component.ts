import { Component, OnInit } from '@angular/core';
import { SubscriptionService } from '../../../core/services/subscription.service';
import { ProductService } from '../../../core/services/product.service';
import { SubscriptionDetailDto, CancelSubscriptionInput, UpgradeSubscriptionInput, ProductDto } from '../../../core/models/subscription.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-subscription-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subscription-management.component.html',
  styleUrls: ['./subscription-management.component.scss']
})
export class SubscriptionManagementComponent implements OnInit {
  subscription: SubscriptionDetailDto | null = null;
  availableProducts: ProductDto[] = [];
  loading = false;
  currentLanguage: 'en' | 'ar' = 'en';
  
  // Cancel subscription
  showCancelDialog = false;
  cancelReason = '';
  
  // Upgrade subscription
  showUpgradeDialog = false;
  selectedProductId = '';
  upgradeEffectiveDate = '';

  constructor(
    private subscriptionService: SubscriptionService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadSubscription();
    this.loadAvailableProducts();
  }

  loadSubscription(): void {
    this.loading = true;
    this.subscriptionService.getCurrent().subscribe({
      next: (subscription) => {
        this.subscription = subscription;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading subscription:', error);
        this.loading = false;
      }
    });
  }

  loadAvailableProducts(): void {
    this.productService.getList().subscribe({
      next: (products) => {
        this.availableProducts = products.filter(p => p.isActive && p.id !== this.subscription?.productId);
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });
  }

  openCancelDialog(): void {
    this.showCancelDialog = true;
  }

  cancelSubscription(): void {
    if (!this.subscription) return;

    const input: CancelSubscriptionInput = {
      reason: this.cancelReason
    };

    this.subscriptionService.cancel(this.subscription.id, input).subscribe({
      next: () => {
        this.showCancelDialog = false;
        this.cancelReason = '';
        this.loadSubscription();
      },
      error: (error) => {
        console.error('Error cancelling subscription:', error);
      }
    });
  }

  openUpgradeDialog(): void {
    this.showUpgradeDialog = true;
    this.upgradeEffectiveDate = new Date().toISOString().split('T')[0];
  }

  upgradeSubscription(): void {
    if (!this.subscription || !this.selectedProductId) return;

    const input: UpgradeSubscriptionInput = {
      newProductId: this.selectedProductId,
      effectiveDate: this.upgradeEffectiveDate
    };

    this.subscriptionService.upgrade(this.subscription.id, input).subscribe({
      next: () => {
        this.showUpgradeDialog = false;
        this.selectedProductId = '';
        this.loadSubscription();
      },
      error: (error) => {
        console.error('Error upgrading subscription:', error);
      }
    });
  }

  getQuotaPercentage(quotaType: string): number {
    if (!this.subscription?.quotaStatuses) return 0;
    const status = this.subscription.quotaStatuses[quotaType];
    return status?.percentageUsed ?? 0;
  }

  isQuotaExceeded(quotaType: string): boolean {
    if (!this.subscription?.quotaStatuses) return false;
    const status = this.subscription.quotaStatuses[quotaType];
    return status?.isExceeded ?? false;
  }

  getQuotaTypes(): string[] {
    if (!this.subscription?.quotaStatuses) return [];
    return Object.keys(this.subscription.quotaStatuses);
  }

  getQuotaName(quotaType: string): string {
    const names: { [key: string]: { en: string; ar: string } } = {
      'Assessments': { en: 'Assessments', ar: 'التقييمات' },
      'Users': { en: 'Users', ar: 'المستخدمون' },
      'EvidenceStorageMB': { en: 'Storage', ar: 'التخزين' }
    };
    const name = names[quotaType] || { en: quotaType, ar: quotaType };
    return this.currentLanguage === 'ar' ? name.ar : name.en;
  }

  getQuotaUsage(quotaType: string): string {
    if (!this.subscription?.quotaStatuses) return '';
    const status = this.subscription.quotaStatuses[quotaType];
    if (!status) return '';
    
    if (status.isUnlimited) {
      return `${status.currentUsage} / Unlimited`;
    }
    
    if (status.limit) {
      return `${status.currentUsage} / ${status.limit} (${status.percentageUsed.toFixed(1)}%)`;
    }
    
    return `${status.currentUsage}`;
  }
}

