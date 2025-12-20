using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace Grc.Product.Products;

public interface IProductAppService : IApplicationService
{
    Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<ProductDetailDto> GetAsync(Guid id);
    Task<List<ProductFeatureDto>> GetFeaturesAsync(Guid productId);
    Task<List<ProductQuotaDto>> GetQuotasAsync(Guid productId);
    Task<List<PricingPlanDto>> GetPricingPlansAsync(Guid productId);
    Task<ProductDto> CreateAsync(CreateProductInput input);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductInput input);
}


