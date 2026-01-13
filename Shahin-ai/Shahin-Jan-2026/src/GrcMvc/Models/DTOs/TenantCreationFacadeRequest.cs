using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Models.DTOs
{
    /// <summary>
    /// Unified request DTO for tenant creation across all entry points (MVC, API, agents)
    /// Used by TenantCreationFacadeService
    /// </summary>
    public class TenantCreationFacadeRequest
    {
        /// <summary>
        /// Name of the tenant (will be used as tenant identifier)
        /// </summary>
        [Required(ErrorMessage = "Tenant name is required")]
        [StringLength(64, MinimumLength = 2, ErrorMessage = "Tenant name must be between 2 and 64 characters")]
        public string TenantName { get; set; } = string.Empty;

        /// <summary>
        /// Email address for the admin user
        /// </summary>
        [Required(ErrorMessage = "Admin email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Password for the admin user
        /// </summary>
        [Required(ErrorMessage = "Admin password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string AdminPassword { get; set; } = string.Empty;

        /// <summary>
        /// Google reCAPTCHA v3 token for bot protection
        /// </summary>
        public string? RecaptchaToken { get; set; }

        /// <summary>
        /// Device fingerprint for fraud detection
        /// </summary>
        public string? DeviceFingerprint { get; set; }

        /// <summary>
        /// IP address of the requester (set by server)
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent of the requester (set by server)
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
