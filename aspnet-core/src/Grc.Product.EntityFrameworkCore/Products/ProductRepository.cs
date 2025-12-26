using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Grc.Product.Products;

namespace Grc.Product.EntityFrameworkCore.Products;

public class ProductRepository : EfCoreRepository<GrcDbContext, Product, Guid>, IProductRepository
{
    public ProductRepository(IDbContextProvider<GrcDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Product> GetByCodeAsync(string code)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<Product> GetWithDetailsAsync(Guid id)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Include(p => p.Features)
            .Include(p => p.Quotas)
            .Include(p => p.PricingPlans)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetActiveProductsAsync()
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Code)
            .ToListAsync();
    }
}


