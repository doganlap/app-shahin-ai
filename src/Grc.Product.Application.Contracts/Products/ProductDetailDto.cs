using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Products;

public class ProductDetailDto : ProductDto
{
    public List<ProductFeatureDto> Features { get; set; }
    public List<ProductQuotaDto> Quotas { get; set; }
    public List<PricingPlanDto> PricingPlans { get; set; }
}


