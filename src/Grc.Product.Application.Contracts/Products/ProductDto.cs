using System;
using Volo.Abp.Application.Dtos;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

public class ProductDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public string IconUrl { get; set; }
}


