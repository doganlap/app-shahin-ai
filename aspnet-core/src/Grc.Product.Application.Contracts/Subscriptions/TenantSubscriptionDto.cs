using System;
using Grc.Enums;
using Grc.Product.Application.Contracts.Products;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Application.Contracts.Subscriptions;

/// <summary>
/// Tenant subscription DTO
/// </summary>
public class TenantSubscriptionDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }
    public ProductDto Product { get; set; }
    public Guid? PricingPlanId { get; set; }
    public PricingPlanDto PricingPlan { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}
