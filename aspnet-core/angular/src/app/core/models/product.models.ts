export interface ProductDto {
  id: string;
  code: string;
  name: LocalizedString;
  description?: LocalizedString;
  category: string;
  isActive: boolean;
  displayOrder: number;
  iconUrl?: string;
  features?: ProductFeatureDto[];
  quotas?: ProductQuotaDto[];
  pricingPlans?: PricingPlanDto[];
}

export interface ProductDetailDto extends ProductDto {
  metadata?: any;
  activeSubscriptionsCount?: number;
}

export interface ProductFeatureDto {
  id: string;
  featureCode: string;
  name: LocalizedString;
  description?: LocalizedString;
  featureType: string;
  value?: string;
  isEnabled: boolean;
}

export interface ProductQuotaDto {
  id: string;
  quotaType: string;
  limit?: number;
  unit?: string;
  isEnforced: boolean;
  isUnlimited: boolean;
}

export interface PricingPlanDto {
  id: string;
  billingPeriod: string;
  price: number;
  currency: string;
  trialDays?: number;
  isActive: boolean;
  stripePriceId?: string;
}

export interface LocalizedString {
  en: string;
  ar: string;
}

export interface CreateProductInput {
  code: string;
  name: LocalizedString;
  description?: LocalizedString;
  category: string;
  displayOrder?: number;
}

export interface UpdateProductInput {
  name?: LocalizedString;
  description?: LocalizedString;
  isActive?: boolean;
  displayOrder?: number;
  iconUrl?: string;
}

