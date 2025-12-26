using System;
using System.Collections.Generic;
using Grc.Enums;
using Grc.ValueObjects;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Product data transfer object
/// </summary>
public class ProductDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public ProductCategory Category { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public string IconUrl { get; set; }
    public List<ProductFeatureDto> Features { get; set; }
    public List<ProductQuotaDto> Quotas { get; set; }
    public List<PricingPlanDto> PricingPlans { get; set; }
}
