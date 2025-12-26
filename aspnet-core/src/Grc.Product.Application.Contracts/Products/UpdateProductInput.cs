using Grc.ValueObjects;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Input for updating a product
/// </summary>
public class UpdateProductInput
{
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public bool? IsActive { get; set; }
    public int? DisplayOrder { get; set; }
    public string IconUrl { get; set; }
}
