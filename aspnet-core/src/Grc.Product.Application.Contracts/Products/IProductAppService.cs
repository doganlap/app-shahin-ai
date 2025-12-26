using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Grc.Product.Application.Contracts.Products;

/// <summary>
/// Application service interface for Product operations
/// </summary>
public interface IProductAppService : IApplicationService
{
    Task<ProductDto> CreateAsync(CreateProductInput input);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductInput input);
    Task<ProductDetailDto> GetAsync(Guid id);
    Task<List<ProductDto>> GetListAsync();
    Task DeleteAsync(Guid id);
}
