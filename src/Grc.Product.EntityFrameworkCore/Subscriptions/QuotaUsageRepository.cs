using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Grc.Product.Subscriptions;

namespace Grc.Product.EntityFrameworkCore.Subscriptions;

public class QuotaUsageRepository : EfCoreRepository<GrcDbContext, QuotaUsage, Guid>, IQuotaUsageRepository
{
    private readonly IGuidGenerator _guidGenerator;

    public QuotaUsageRepository(
        IDbContextProvider<GrcDbContext> dbContextProvider,
        IGuidGenerator guidGenerator)
        : base(dbContextProvider)
    {
        _guidGenerator = guidGenerator;
    }

    public async Task<QuotaUsage> GetByTenantAndTypeAsync(Guid tenantId, Enums.QuotaType quotaType)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(q => q.TenantId == tenantId && q.QuotaType == quotaType);
    }

    public async Task<QuotaUsage> GetOrCreateAsync(Guid tenantId, Enums.QuotaType quotaType)
    {
        var dbSet = await GetDbSetAsync();
        var existing = await dbSet
            .FirstOrDefaultAsync(q => q.TenantId == tenantId && q.QuotaType == quotaType);

        if (existing != null)
            return existing;

        var newUsage = new QuotaUsage(
            _guidGenerator.Create(),
            tenantId,
            quotaType
        );

        await dbSet.AddAsync(newUsage);
        return newUsage;
    }
}

