using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Product.Application.Contracts.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Grc.Product.HttpApi.Products;

/// <summary>
/// REST API controller for products
/// </summary>
[ApiController]
[Route("api/grc/products")]
[Authorize]
public class ProductController : AbpControllerBase
{
    private readonly IProductAppService _productAppService;

    public ProductController(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetListAsync()
    {
        var products = await _productAppService.GetListAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDetailDto>> GetAsync(Guid id)
    {
        var product = await _productAppService.GetAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Create a new product (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize("Grc.Admin")]
    public async Task<ActionResult<ProductDto>> CreateAsync([FromBody] CreateProductInput input)
    {
        var product = await _productAppService.CreateAsync(input);
        return CreatedAtAction(nameof(GetAsync), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update a product (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize("Grc.Admin")]
    public async Task<ActionResult<ProductDto>> UpdateAsync(Guid id, [FromBody] UpdateProductInput input)
    {
        var product = await _productAppService.UpdateAsync(id, input);
        return Ok(product);
    }

    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize("Grc.Admin")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _productAppService.DeleteAsync(id);
        return NoContent();
    }
}
