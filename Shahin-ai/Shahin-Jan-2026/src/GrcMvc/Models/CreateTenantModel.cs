using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Controllers;

public class CreateTenantModel
{
    [Required(ErrorMessage = "Tenant name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tenant name must be 3-50 characters")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Tenant name must be lowercase letters, numbers, and hyphens only")]
    public string TenantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Organization name is required")]
    public string OrganizationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string AdminEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin password is required")]
    [StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters")]
    [DataType(DataType.Password)]
    public string AdminPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm the password")]
    [Compare("AdminPassword", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
