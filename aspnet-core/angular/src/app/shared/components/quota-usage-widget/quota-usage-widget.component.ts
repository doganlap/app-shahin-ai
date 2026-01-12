import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SubscriptionService } from '../../../core/services/subscription.service';
import { SubscriptionDetailDto, QuotaStatusDto } from '../../../core/models/subscription.models';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-quota-usage-widget',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quota-usage-widget.component.html',
  styleUrls: ['./quota-usage-widget.component.scss']
})
export class QuotaUsageWidgetComponent implements OnInit, OnDestroy {
  @Input() quotaType: string = '';
  @Input() showLabel: boolean = true;
  @Input() refreshInterval: number = 60000; // 1 minute default

  subscription: SubscriptionDetailDto | null = null;
  quotaStatus: QuotaStatusDto | null = null;
  loading = false;
  private refreshSubscription?: Subscription;

  constructor(private subscriptionService: SubscriptionService) {}

  ngOnInit(): void {
    this.loadQuotaStatus();
    
    // Auto-refresh
    if (this.refreshInterval > 0) {
      this.refreshSubscription = interval(this.refreshInterval)
        .pipe(switchMap(() => this.subscriptionService.getCurrent()))
        .subscribe({
          next: (subscription) => {
            this.subscription = subscription;
            this.updateQuotaStatus();
          },
          error: (error) => {
            console.error('Error refreshing quota:', error);
          }
        });
    }
  }

  ngOnDestroy(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
  }

  loadQuotaStatus(): void {
    this.loading = true;
    this.subscriptionService.getCurrent().subscribe({
      next: (subscription) => {
        this.subscription = subscription;
        this.updateQuotaStatus();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading quota status:', error);
        this.loading = false;
      }
    });
  }

  private updateQuotaStatus(): void {
    if (!this.subscription?.quotaStatuses || !this.quotaType) {
      this.quotaStatus = null;
      return;
    }

    this.quotaStatus = this.subscription.quotaStatuses[this.quotaType];
  }

  getPercentage(): number {
    return this.quotaStatus?.percentageUsed ?? 0;
  }

  isExceeded(): boolean {
    return this.quotaStatus?.isExceeded ?? false;
  }

  getUsageText(): string {
    if (!this.quotaStatus) return 'N/A';
    
    if (this.quotaStatus.isUnlimited) {
      return `${this.quotaStatus.currentUsage} / Unlimited`;
    }
    
    if (this.quotaStatus.limit) {
      return `${this.quotaStatus.currentUsage} / ${this.quotaStatus.limit}`;
    }
    
    return `${this.quotaStatus.currentUsage}`;
  }

  getQuotaLabel(): string {
    const labels: { [key: string]: string } = {
      'Assessments': 'Assessments',
      'Users': 'Users',
      'EvidenceStorageMB': 'Storage (MB)'
    };
    return labels[this.quotaType] || this.quotaType;
  }
}

