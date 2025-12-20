using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Product.Subscriptions;

/// <summary>
/// Tenant's subscription to a product
/// </summary>
public class TenantSubscription : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid ProductId { get; private set; }
    public Guid? PricingPlanId { get; private set; }
    public Enums.SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? TrialEndDate { get; private set; }
    public bool AutoRenew { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string CancellationReason { get; private set; }
    public string StripeSubscriptionId { get; private set; }
    
    protected TenantSubscription() { }
    
    public TenantSubscription(Guid id, Guid tenantId, Guid productId, DateTime startDate, Guid? pricingPlanId = null)
        : base(id)
    {
        TenantId = tenantId;
        ProductId = productId;
        PricingPlanId = pricingPlanId;
        StartDate = startDate;
        Status = Enums.SubscriptionStatus.Trial;
        AutoRenew = true;
    }
    
    public void Activate()
    {
        if (Status != Enums.SubscriptionStatus.Trial)
            throw new BusinessException("Only trial subscriptions can be activated");
        
        Status = Enums.SubscriptionStatus.Active;
        // TODO: AddDistributedEvent(new SubscriptionActivatedEto { SubscriptionId = Id, TenantId = TenantId, ProductId = ProductId });
    }
    
    public void Suspend()
    {
        if (Status != Enums.SubscriptionStatus.Active)
            throw new BusinessException("Only active subscriptions can be suspended");
        
        Status = Enums.SubscriptionStatus.Suspended;
    }
    
    public void Cancel(string reason)
    {
        Status = Enums.SubscriptionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        AutoRenew = false;
        
        // TODO: AddDistributedEvent(new SubscriptionCancelledEto { SubscriptionId = Id, TenantId = TenantId, Reason = reason });
    }
    
    public void Expire()
    {
        Status = Enums.SubscriptionStatus.Expired;
        // TODO: AddDistributedEvent(new SubscriptionExpiredEto { SubscriptionId = Id, TenantId = TenantId });
    }
    
    public void Renew(DateTime newEndDate)
    {
        if (Status != Enums.SubscriptionStatus.Active && Status != Enums.SubscriptionStatus.Expired)
            throw new BusinessException("Only active or expired subscriptions can be renewed");
        
        Status = Enums.SubscriptionStatus.Active;
        EndDate = newEndDate;
        CancelledAt = null;
        CancellationReason = null;
        
        // TODO: AddDistributedEvent(new SubscriptionRenewedEto { SubscriptionId = Id, TenantId = TenantId, NewEndDate = newEndDate });
    }
    
    public bool IsActive => Status == Enums.SubscriptionStatus.Active;
    public bool IsExpired => Status == Enums.SubscriptionStatus.Expired || 
                             (EndDate.HasValue && EndDate.Value < DateTime.UtcNow);
    
    public void SetTrialEndDate(DateTime? trialEndDate) => TrialEndDate = trialEndDate;
    public void SetEndDate(DateTime? endDate) => EndDate = endDate;
    public void SetAutoRenew(bool autoRenew) => AutoRenew = autoRenew;
    public void SetStripeSubscriptionId(string stripeSubscriptionId) => StripeSubscriptionId = stripeSubscriptionId;
}


