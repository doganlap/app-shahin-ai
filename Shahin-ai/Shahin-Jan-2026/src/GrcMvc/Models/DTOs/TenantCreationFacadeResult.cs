using System;
using Volo.Abp.Identity;

namespace GrcMvc.Models.DTOs
{
    /// <summary>
    /// Result of tenant creation operation via TenantCreationFacadeService
    /// </summary>
    public class TenantCreationFacadeResult
    {
        /// <summary>
        /// ID of the created tenant
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Name of the created tenant
        /// </summary>
        public string TenantName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the created admin user
        /// </summary>
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// ID of the created admin user
        /// </summary>
        public Guid AdminUserId { get; set; }

        /// <summary>
        /// The created user entity (for sign-in purposes)
        /// </summary>
        public IdentityUser? User { get; set; }

        /// <summary>
        /// Whether the tenant creation was flagged for review
        /// </summary>
        public bool IsFlaggedForReview { get; set; }

        /// <summary>
        /// Reason for flagging (if flagged)
        /// </summary>
        public string? FlagReason { get; set; }

        /// <summary>
        /// Success message
        /// </summary>
        public string Message { get; set; } = "Tenant created successfully";
    }
}
