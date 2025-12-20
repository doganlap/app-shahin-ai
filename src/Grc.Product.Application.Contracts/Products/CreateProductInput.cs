using System.ComponentModel.DataAnnotations;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

public class CreateProductInput
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; }
    
    [Required]
    public LocalizedString Name { get; set; }
    
    public LocalizedString Description { get; set; }
    
    [Required]
    public string Category { get; set; }
    
    public int DisplayOrder { get; set; }
    
    [StringLength(500)]
    public string IconUrl { get; set; }
}


