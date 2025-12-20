using System;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Products;

public class PricingPlanDto : FullAuditedEntityDto<Guid>
{
    public string BillingPeriod { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public int? TrialDays { get; set; }
    public bool IsActive { get; set; }
}


