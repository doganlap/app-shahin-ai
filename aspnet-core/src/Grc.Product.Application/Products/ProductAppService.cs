using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Product.Application.Contracts.Products;
using Grc.Product.Products;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Product.Application.Products;

/// <summary>
/// Application service for Product operations
/// </summary>
[Authorize]
public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IProductRepository _productRepository;

    public ProductAppService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductInput input)
    {
        var product = new Product(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            input.Category)
        {
            DisplayOrder = input.DisplayOrder
        };

        if (input.Description != null)
        {
            product.UpdateDescription(input.Description);
        }

        await _productRepository.InsertAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductInput input)
    {
        var product = await _productRepository.GetAsync(id);

        if (input.Name != null)
        {
            // Update name - would need a method in Product entity
        }

        if (input.Description != null)
        {
            product.UpdateDescription(input.Description);
        }

        if (input.IsActive.HasValue)
        {
            if (input.IsActive.Value)
                product.Activate();
            else
                product.Deactivate();
        }

        if (input.DisplayOrder.HasValue)
        {
            product.SetDisplayOrder(input.DisplayOrder.Value);
        }

        if (input.IconUrl != null)
        {
            product.SetIconUrl(input.IconUrl);
        }

        await _productRepository.UpdateAsync(product);
        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDetailDto> GetAsync(Guid id)
    {
        var product = await _productRepository.GetAsync(id);
        var dto = ObjectMapper.Map<Product, ProductDetailDto>(product);
        
        // Load features, quotas, pricing plans
        dto.Features = ObjectMapper.Map<List<ProductFeature>, List<ProductFeatureDto>>(product.Features.ToList());
        dto.Quotas = ObjectMapper.Map<List<ProductQuota>, List<ProductQuotaDto>>(product.Quotas.ToList());
        dto.PricingPlans = ObjectMapper.Map<List<PricingPlan>, List<PricingPlanDto>>(product.PricingPlans.ToList());
        
        return dto;
    }

    public async Task<List<ProductDto>> GetListAsync()
    {
        var products = await _productRepository.GetListAsync(p => p.IsActive);
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(products);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }
}
