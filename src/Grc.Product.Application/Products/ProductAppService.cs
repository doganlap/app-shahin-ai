using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Grc.Product.Products;
using Grc.Domain.Shared;

namespace Grc.Product.Products;

[Authorize]
public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IProductRepository _productRepository;

    public ProductAppService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _productRepository.GetQueryableAsync();
        var products = queryable
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Code);

        var totalCount = products.Count();
        var items = products
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        return new PagedResultDto<ProductDto>(
            totalCount,
            ObjectMapper.Map<List<Product>, List<ProductDto>>(items)
        );
    }

    public async Task<ProductDetailDto> GetAsync(Guid id)
    {
        var product = await _productRepository.GetWithDetailsAsync(id);
        return ObjectMapper.Map<Product, ProductDetailDto>(product);
    }

    public async Task<List<ProductFeatureDto>> GetFeaturesAsync(Guid productId)
    {
        var product = await _productRepository.GetWithDetailsAsync(productId);
        return ObjectMapper.Map<List<ProductFeature>, List<ProductFeatureDto>>(product.Features.ToList());
    }

    public async Task<List<ProductQuotaDto>> GetQuotasAsync(Guid productId)
    {
        var product = await _productRepository.GetWithDetailsAsync(productId);
        return ObjectMapper.Map<List<ProductQuota>, List<ProductQuotaDto>>(product.Quotas.ToList());
    }

    public async Task<List<PricingPlanDto>> GetPricingPlansAsync(Guid productId)
    {
        var product = await _productRepository.GetWithDetailsAsync(productId);
        return ObjectMapper.Map<List<PricingPlan>, List<PricingPlanDto>>(product.PricingPlans.Where(p => p.IsActive).ToList());
    }

    [Authorize("Grc.Products.Create")]
    public async Task<ProductDto> CreateAsync(CreateProductInput input)
    {
        var product = new Product(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            Enum.Parse<Enums.ProductCategory>(input.Category)
        );

        if (input.Description != null)
            product.UpdateDescription(input.Description);

        product.SetDisplayOrder(input.DisplayOrder);
        if (!string.IsNullOrEmpty(input.IconUrl))
            product.SetIconUrl(input.IconUrl);

        await _productRepository.InsertAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    [Authorize("Grc.Products.Update")]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductInput input)
    {
        var product = await _productRepository.GetAsync(id);

        // Note: Name is immutable for Product (would need a SetName method if allowed)
        // Product name changes typically require creating a new product version

        if (input.Description != null)
            product.UpdateDescription(input.Description);

        if (input.IsActive.HasValue)
        {
            if (input.IsActive.Value)
                product.Activate();
            else
                product.Deactivate();
        }

        if (input.DisplayOrder.HasValue)
            product.SetDisplayOrder(input.DisplayOrder.Value);

        if (input.IconUrl != null)
            product.SetIconUrl(input.IconUrl);

        await _productRepository.UpdateAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}

