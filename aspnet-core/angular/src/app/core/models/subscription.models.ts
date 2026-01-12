import { ProductDto, PricingPlanDto } from './product.models';

export interface TenantSubscriptionDto {
  id: string;
  productId: string;
  product?: ProductDto;
  pricingPlanId?: string;
  pricingPlan?: PricingPlanDto;
  status: string;
  startDate: string;
  endDate?: string;
  trialEndDate?: string;
  autoRenew: boolean;
  isActive: boolean;
  isExpired: boolean;
}

export interface SubscriptionDetailDto extends TenantSubscriptionDto {
  quotaUsages?: QuotaUsageDto[];
  quotaStatuses?: { [key: string]: QuotaStatusDto };
}

export interface QuotaUsageDto {
  id: string;
  quotaType: string;
  currentUsage: number;
  limit?: number;
  resetDate?: string;
  lastUpdated: string;
  percentageUsed: number;
  isExceeded: boolean;
}

export interface QuotaStatusDto {
  currentUsage: number;
  limit?: number;
  percentageUsed: number;
  isExceeded: boolean;
  isUnlimited: boolean;
}

export interface SubscribeInput {
  productId: string;
  pricingPlanId?: string;
  startDate: string;
}

export interface CancelSubscriptionInput {
  reason?: string;
}

export interface UpgradeSubscriptionInput {
  newProductId: string;
  newPricingPlanId?: string;
  effectiveDate: string;
}

export interface QuotaCheckInput {
  quotaType: string;
  requestedAmount: number;
}

export interface QuotaCheckResult {
  allowed: boolean;
  currentUsage: number;
  limit?: number;
  remaining: number;
}

