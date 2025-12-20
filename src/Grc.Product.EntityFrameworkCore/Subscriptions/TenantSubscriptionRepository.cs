using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Grc.Product.Subscriptions;

namespace Grc.Product.EntityFrameworkCore.Subscriptions;

public class TenantSubscriptionRepository : EfCoreRepository<GrcDbContext, TenantSubscription, Guid>, ITenantSubscriptionRepository
{
    public TenantSubscriptionRepository(IDbContextProvider<GrcDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<TenantSubscription> GetActiveSubscriptionAsync(Guid tenantId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(s => s.TenantId == tenantId && s.Status == Enums.SubscriptionStatus.Active)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync();
    }

    public async Task<TenantSubscription> GetCurrentSubscriptionAsync(Guid tenantId)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(s => s.TenantId == tenantId)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync();
    }
}


