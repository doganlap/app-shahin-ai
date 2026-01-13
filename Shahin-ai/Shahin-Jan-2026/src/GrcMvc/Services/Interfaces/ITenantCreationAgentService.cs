using System;
using System.Threading.Tasks;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service for creating tenants with admin users using ABP Framework patterns
    /// </summary>
    public interface ITenantCreationAgentService
    {
        /// <summary>
        /// Creates a new tenant with an admin user in one atomic operation
        /// </summary>
        /// <param name="tenantName">Name of the tenant (organization name)</param>
        /// <param name="adminEmail">Email address for the admin user</param>
        /// <param name="adminPassword">Password for the admin user</param>
        /// <returns>Tenant ID (Guid)</returns>
        Task<Guid> CreateTenantWithAdminAsync(string tenantName, string adminEmail, string adminPassword);
    }
}
