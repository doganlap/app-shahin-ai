using System.Threading.Tasks;
using GrcMvc.Models.DTOs;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Facade service for tenant creation using ABP's ITenantAppService with security enhancements
    /// </summary>
    public interface ITenantCreationFacadeService
    {
        /// <summary>
        /// Creates a new tenant with admin user using ABP's built-in service
        /// Includes security validations: CAPTCHA, fraud detection, rate limiting
        /// </summary>
        /// <param name="request">Tenant creation request with admin details</param>
        /// <returns>Result containing tenant and user information</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when creation fails due to business logic errors</exception>
        /// <exception cref="System.Security.SecurityException">Thrown when security validations fail (CAPTCHA, fraud)</exception>
        Task<TenantCreationFacadeResult> CreateTenantWithAdminAsync(TenantCreationFacadeRequest request);

        /// <summary>
        /// Creates a new tenant with admin user using ABP directly
        /// NO security checks - only registration validation and record creation
        /// Use for internal/admin operations or when security is handled elsewhere
        /// </summary>
        /// <param name="tenantName">Name of the tenant (will be sanitized)</param>
        /// <param name="adminEmail">Admin user email address</param>
        /// <param name="adminPassword">Admin user password</param>
        /// <returns>Result containing tenant and user information</returns>
        /// <exception cref="System.ArgumentException">Thrown when required parameters are missing</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when creation fails due to business logic errors</exception>
        Task<TenantCreationFacadeResult> CreateTenantWithoutSecurityAsync(string tenantName, string adminEmail, string adminPassword);
    }
}
