using System;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Products;

public class ProductQuotaDto : FullAuditedEntityDto<Guid>
{
    public string QuotaType { get; set; }
    public decimal? Limit { get; set; }
    public string Unit { get; set; }
    public bool IsEnforced { get; set; }
    public bool IsUnlimited { get; set; }
}


