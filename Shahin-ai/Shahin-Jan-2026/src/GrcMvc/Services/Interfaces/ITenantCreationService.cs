using System.Security.Claims;

namespace GrcMvc.Services.Interfaces;

public interface ITenantCreationService
{
    Task<TenantCreationResult> CreateTenantWithAdminAsync(
        TenantCreationRequest request, 
        ClaimsPrincipal actor);
}

public sealed class TenantCreationRequest
{
    public string TenantName { get; set; } = "";
    public string OrganizationName { get; set; } = "";
    public string AdminEmail { get; set; } = "";
    public string? AdminPassword { get; set; }
    public bool EmailConfirmed { get; set; } = true;
}

public sealed class TenantCreationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? AdminUserId { get; set; }
}
