using Grc.Domain.Shared;

namespace Grc.Product.Products;

public class UpdateProductInput
{
    public LocalizedString Name { get; set; }
    public LocalizedString Description { get; set; }
    public bool? IsActive { get; set; }
    public int? DisplayOrder { get; set; }
    public string IconUrl { get; set; }
}


