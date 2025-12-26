using System;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Product feature DTO
/// </summary>
public class ProductFeatureDto
{
    public Guid Id { get; set; }
    public string FeatureCode { get; set; }
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public FeatureType FeatureType { get; set; }
    public string Value { get; set; }
    public bool IsEnabled { get; set; }
}
