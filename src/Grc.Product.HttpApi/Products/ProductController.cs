using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Grc.Product.Products;

namespace Grc.Product.HttpApi.Products;

[Authorize]
[RemoteService(Name = "Product")]
[Route("api/grc/products")]
public class ProductController : AbpControllerBase, IProductAppService
{
    private readonly IProductAppService _productAppService;

    public ProductController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        return _productAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public virtual Task<ProductDetailDto> GetAsync(Guid id)
    {
        return _productAppService.GetAsync(id);
    }

    [HttpPost]
    [Authorize("Grc.Products.Create")]
    public virtual Task<ProductDto> CreateAsync(CreateProductInput input)
    {
        return _productAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    [Authorize("Grc.Products.Update")]
    public virtual Task<ProductDto> UpdateAsync(Guid id, UpdateProductInput input)
    {
        return _productAppService.UpdateAsync(id, input);
    }

    [HttpGet("{id}/features")]
    public virtual Task<System.Collections.Generic.List<ProductFeatureDto>> GetFeaturesAsync(Guid id)
    {
        return _productAppService.GetFeaturesAsync(id);
    }

    [HttpGet("{id}/quotas")]
    public virtual Task<System.Collections.Generic.List<ProductQuotaDto>> GetQuotasAsync(Guid id)
    {
        return _productAppService.GetQuotasAsync(id);
    }

    [HttpGet("{id}/pricing")]
    public virtual Task<System.Collections.Generic.List<PricingPlanDto>> GetPricingPlansAsync(Guid id)
    {
        return _productAppService.GetPricingPlansAsync(id);
    }
}


