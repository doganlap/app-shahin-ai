using System;
using Grc.Enums;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Product quota DTO
/// </summary>
public class ProductQuotaDto
{
    public Guid Id { get; set; }
    public QuotaType QuotaType { get; set; }
    public decimal? Limit { get; set; }
    public string Unit { get; set; }
    public bool IsEnforced { get; set; }
    public bool IsUnlimited { get; set; }
}
