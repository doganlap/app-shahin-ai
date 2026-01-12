using System.ComponentModel.DataAnnotations;
using Grc.Enums;
using Grc.ValueObjects;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Input for creating a product
/// </summary>
public class CreateProductInput
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; }
    
    [Required]
    public LocalizedString Name { get; set; }
    
    public LocalizedString Description { get; set; }
    
    [Required]
    public ProductCategory Category { get; set; }
    
    public int DisplayOrder { get; set; }
}
