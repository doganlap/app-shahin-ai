using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Grc.EntityFrameworkCore;

/// <summary>
/// Dynamic connection string resolver for per-tenant databases
/// </summary>
public class GrcConnectionStringResolver : IConnectionStringResolver, ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantStore _tenantStore;

    public GrcConnectionStringResolver(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        ITenantStore tenantStore)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _tenantStore = tenantStore;
    }

    public virtual async Task<string> ResolveAsync(string connectionStringName = null)
    {
        connectionStringName ??= "Default";

        // If no tenant, return default connection string
        if (!_currentTenant.Id.HasValue)
        {
            return _configuration.GetConnectionString(connectionStringName);
        }

        var tenant = await _tenantStore.FindAsync(_currentTenant.Id.Value);
        if (tenant == null)
        {
            return _configuration.GetConnectionString(connectionStringName);
        }

        // Check if tenant has custom connection string
        var tenantConnectionString = tenant.FindConnectionString(connectionStringName);
        if (!string.IsNullOrEmpty(tenantConnectionString))
        {
            return tenantConnectionString;
        }

        // For separate database strategy, construct connection string with tenant-specific database name
        // This would require additional tenant configuration entity to determine strategy
        // For now, return default connection string
        return _configuration.GetConnectionString(connectionStringName);
    }

    [Obsolete("Use ResolveAsync method.")]
    public virtual string Resolve(string connectionStringName = null)
    {
        return ResolveAsync(connectionStringName).GetAwaiter().GetResult();
    }
}

