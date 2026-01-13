using System;
using Volo.Abp.Domain.Entities;

namespace GrcMvc.Models.Entities
{
    /// <summary>
    /// Tracks device fingerprints and metadata for tenant creation for fraud detection
    /// </summary>
    public class TenantCreationFingerprint : Entity<Guid>
    {
        /// <summary>
        /// ID of the tenant that was created
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// Device fingerprint identifier (client-side generated hash)
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// IP address from which the tenant was created
        /// </summary>
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// User agent string of the browser/client
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when tenant was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Whether this creation was flagged as suspicious
        /// </summary>
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Reason for flagging (if flagged)
        /// </summary>
        public string? FlagReason { get; set; }

        /// <summary>
        /// Email address used for tenant admin
        /// </summary>
        public string AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Tenant name that was created
        /// </summary>
        public string TenantName { get; set; } = string.Empty;

        /// <summary>
        /// reCAPTCHA score at time of creation (0.0 to 1.0)
        /// </summary>
        public double? RecaptchaScore { get; set; }
    }
}
