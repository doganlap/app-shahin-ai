using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrcMvc.Models.DTOs
{
    // ==================== INPUT DTOs ====================

    /// <summary>
    /// DTO for creating a new chat session
    /// </summary>
    public class CreateChatSessionInputDto
    {
        /// <summary>
        /// Page URL where conversation starts
        /// </summary>
        [StringLength(500)]
        public string? StartPageUrl { get; set; }

        /// <summary>
        /// Referrer URL
        /// </summary>
        [StringLength(500)]
        public string? ReferrerUrl { get; set; }

        /// <summary>
        /// Initial category (optional)
        /// </summary>
        [StringLength(50)]
        public string? Category { get; set; }
    }

    /// <summary>
    /// DTO for sending a chat message
    /// </summary>
    public class SendChatMessageInputDto
    {
        /// <summary>
        /// Session ID (required for anonymous users, optional for authenticated)
        /// </summary>
        [StringLength(100)]
        public string? SessionId { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        [Required(ErrorMessage = "Message is required")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Current page URL for context
        /// </summary>
        [StringLength(500)]
        public string? PageUrl { get; set; }

        /// <summary>
        /// Page section/context (e.g., "pricing", "features")
        /// </summary>
        [StringLength(100)]
        public string? PageContext { get; set; }
    }

    /// <summary>
    /// DTO for escalating to human support
    /// </summary>
    public class EscalateChatInputDto
    {
        /// <summary>
        /// Session ID
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Escalation reason
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for resolving a chat conversation
    /// </summary>
    public class ResolveChatInputDto
    {
        /// <summary>
        /// Session ID
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Satisfaction rating (1-5)
        /// </summary>
        [Range(1, 5)]
        public int? SatisfactionRating { get; set; }

        /// <summary>
        /// User feedback
        /// </summary>
        [StringLength(1000)]
        public string? Feedback { get; set; }
    }

    // ==================== OUTPUT DTOs ====================

    /// <summary>
    /// Output DTO for chat session creation
    /// </summary>
    public class CreateChatSessionOutputDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
        public List<string> SuggestedQuestions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Output DTO for chat message response
    /// </summary>
    public class ChatMessageOutputDto
    {
        public Guid MessageId { get; set; }
        public string Response { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public List<string>? Suggestions { get; set; }
        public bool IsEscalated { get; set; }
        public bool IsAiGenerated { get; set; }
        public double? AiConfidence { get; set; }
    }

    /// <summary>
    /// Output DTO for chat conversation
    /// </summary>
    public class ChatConversationDto
    {
        public Guid Id { get; set; }
        public string? SessionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool IsEscalated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int MessageCount { get; set; }
    }

    /// <summary>
    /// Output DTO for a single chat message
    /// </summary>
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public string SenderType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAiGenerated { get; set; }
    }

    /// <summary>
    /// Output DTO for conversation history
    /// </summary>
    public class ChatConversationHistoryDto
    {
        public ChatConversationDto Conversation { get; set; } = null!;
        public List<ChatMessageDto> Messages { get; set; } = new();
    }

    /// <summary>
    /// Output DTO for escalation response
    /// </summary>
    public class EscalateChatOutputDto
    {
        public bool Success { get; set; }
        public Guid? SupportConversationId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
