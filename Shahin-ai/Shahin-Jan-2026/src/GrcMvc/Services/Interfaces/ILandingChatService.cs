using System;
using System.Threading;
using System.Threading.Tasks;
using GrcMvc.Common;
using GrcMvc.Models.DTOs;
using GrcMvc.Models.Entities;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service for landing page AI chat functionality
    /// Manages conversations, integrates with Claude AI, and handles escalations
    /// </summary>
    public interface ILandingChatService
    {
        /// <summary>
        /// Create a new chat session (anonymous or authenticated)
        /// </summary>
        Task<Result<CreateChatSessionOutputDto>> CreateSessionAsync(
            CreateChatSessionInputDto input,
            string? ipAddress = null,
            string? userAgent = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Send a message and get AI response
        /// </summary>
        Task<Result<ChatMessageOutputDto>> SendMessageAsync(
            SendChatMessageInputDto input,
            string? userId = null,
            Guid? tenantId = null,
            string? ipAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get conversation history
        /// </summary>
        Task<Result<ChatConversationHistoryDto>> GetConversationHistoryAsync(
            string sessionId,
            string? userId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Escalate conversation to human support
        /// </summary>
        Task<Result<EscalateChatOutputDto>> EscalateToSupportAsync(
            EscalateChatInputDto input,
            string? userId = null,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolve/close a conversation
        /// </summary>
        Task<Result<bool>> ResolveConversationAsync(
            ResolveChatInputDto input,
            string? userId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get active conversation by session ID
        /// </summary>
        Task<Result<ChatConversation>> GetActiveConversationAsync(
            string sessionId,
            string? userId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check rate limit for IP/user
        /// </summary>
        Task<Result<bool>> CheckRateLimitAsync(
            string identifier,
            bool isAuthenticated,
            CancellationToken cancellationToken = default);
    }
}
