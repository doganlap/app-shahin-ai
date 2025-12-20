using System;
using Volo.Abp.Application.Dtos;
using Grc.Product.Products;

namespace Grc.Product.Subscriptions;

public class TenantSubscriptionDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }
    public ProductDto Product { get; set; }
    public Guid? PricingPlanId { get; set; }
    public string Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public bool AutoRenew { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}

