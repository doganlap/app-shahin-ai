import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  TenantSubscriptionDto, 
  SubscriptionDetailDto, 
  SubscribeInput, 
  CancelSubscriptionInput, 
  UpgradeSubscriptionInput,
  QuotaCheckInput,
  QuotaCheckResult
} from '../models/subscription.models';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private apiUrl = '/api/grc/subscriptions';

  constructor(private http: HttpClient) {}

  getCurrent(): Observable<SubscriptionDetailDto> {
    return this.http.get<SubscriptionDetailDto>(`${this.apiUrl}/current`);
  }

  getById(id: string): Observable<SubscriptionDetailDto> {
    return this.http.get<SubscriptionDetailDto>(`${this.apiUrl}/${id}`);
  }

  subscribe(input: SubscribeInput): Observable<TenantSubscriptionDto> {
    return this.http.post<TenantSubscriptionDto>(this.apiUrl, input);
  }

  cancel(id: string, input: CancelSubscriptionInput): Observable<TenantSubscriptionDto> {
    return this.http.post<TenantSubscriptionDto>(`${this.apiUrl}/${id}/cancel`, input);
  }

  upgrade(id: string, input: UpgradeSubscriptionInput): Observable<TenantSubscriptionDto> {
    return this.http.post<TenantSubscriptionDto>(`${this.apiUrl}/${id}/upgrade`, input);
  }

  checkQuota(input: QuotaCheckInput): Observable<QuotaCheckResult> {
    return this.http.post<QuotaCheckResult>(`${this.apiUrl}/quota/check`, input);
  }
}

