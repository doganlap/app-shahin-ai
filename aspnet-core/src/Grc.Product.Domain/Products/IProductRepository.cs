using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Grc.Product.Products;

/// <summary>
/// Repository interface for Product aggregate
/// </summary>
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<Product> GetByCodeAsync(string code);
    Task<Product> GetWithDetailsAsync(Guid id);
    Task<List<Product>> GetActiveProductsAsync();
}


