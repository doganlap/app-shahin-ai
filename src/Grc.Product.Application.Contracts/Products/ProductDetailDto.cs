using System;
using System.Collections.Generic;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Detailed product DTO
/// </summary>
public class ProductDetailDto : ProductDto
{
    public Dictionary<string, object> Metadata { get; set; }
    public int ActiveSubscriptionsCount { get; set; }
}
