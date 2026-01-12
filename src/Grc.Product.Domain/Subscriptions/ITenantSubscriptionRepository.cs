using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Grc.Product.Subscriptions;

/// <summary>
/// Repository interface for TenantSubscription aggregate
/// </summary>
public interface ITenantSubscriptionRepository : IRepository<TenantSubscription, Guid>
{
    Task<TenantSubscription> GetActiveSubscriptionAsync(Guid tenantId);
    Task<TenantSubscription> GetCurrentSubscriptionAsync(Guid tenantId);
}


