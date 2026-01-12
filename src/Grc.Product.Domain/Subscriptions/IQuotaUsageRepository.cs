using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Grc.Product.Subscriptions;

/// <summary>
/// Repository interface for QuotaUsage entity
/// </summary>
public interface IQuotaUsageRepository : IRepository<QuotaUsage, Guid>
{
    Task<QuotaUsage> GetByTenantAndTypeAsync(Guid tenantId, Enums.QuotaType quotaType);
    Task<QuotaUsage> GetOrCreateAsync(Guid tenantId, Enums.QuotaType quotaType);
}


