using System;
using Volo.Abp.Application.Dtos;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

public class ProductFeatureDto : FullAuditedEntityDto<Guid>
{
    public string FeatureCode { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public string FeatureType { get; set; }
    public string Value { get; set; }
    public bool IsEnabled { get; set; }
}


