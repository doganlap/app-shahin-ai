using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrcMvc.Models.Entities
{
    /// <summary>
    /// Chat conversation entity for landing page AI assistant
    /// Tracks conversations for both anonymous and authenticated users
    /// </summary>
    public class ChatConversation : BaseEntity
    {
        /// <summary>
        /// Session identifier for anonymous users (required for anonymous)
        /// Authenticated users use TenantId + UserId
        /// </summary>
        [StringLength(100)]
        public string? SessionId { get; set; }

        /// <summary>
        /// User ID for authenticated users
        /// </summary>
        [StringLength(100)]
        public string? UserId { get; set; }

        /// <summary>
        /// Conversation status: Active, Resolved, Escalated, Closed
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Category: landing_inquiry, support, technical, sales
        /// </summary>
        [StringLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// Page URL where conversation started
        /// </summary>
        [StringLength(500)]
        public string? StartPageUrl { get; set; }

        /// <summary>
        /// Referrer URL
        /// </summary>
        [StringLength(500)]
        public string? ReferrerUrl { get; set; }

        /// <summary>
        /// User's IP address (for rate limiting)
        /// </summary>
        [StringLength(45)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent string
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// Whether conversation was escalated to human support
        /// </summary>
        public bool IsEscalated { get; set; } = false;

        /// <summary>
        /// Escalation timestamp
        /// </summary>
        public DateTime? EscalatedAt { get; set; }

        /// <summary>
        /// Escalation reason
        /// </summary>
        [StringLength(500)]
        public string? EscalationReason { get; set; }

        /// <summary>
        /// Related support conversation ID (after escalation)
        /// </summary>
        public Guid? SupportConversationId { get; set; }

        /// <summary>
        /// Foreign key to SupportConversation
        /// </summary>
        [ForeignKey("SupportConversationId")]
        public virtual SupportConversation? SupportConversation { get; set; }

        /// <summary>
        /// Conversation resolved timestamp
        /// </summary>
        public DateTime? ResolvedAt { get; set; }

        /// <summary>
        /// User satisfaction rating (1-5)
        /// </summary>
        public int? SatisfactionRating { get; set; }

        /// <summary>
        /// User feedback
        /// </summary>
        [StringLength(1000)]
        public string? Feedback { get; set; }

        /// <summary>
        /// Collection of messages in this conversation
        /// </summary>
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <summary>
        /// Additional metadata as JSON
        /// </summary>
        public string? MetadataJson { get; set; }
    }
}
