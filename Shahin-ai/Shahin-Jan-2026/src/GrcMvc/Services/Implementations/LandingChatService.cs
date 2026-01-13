using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GrcMvc.Common;
using GrcMvc.Data;
using GrcMvc.Models.DTOs;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Implementation of landing page chat service
    /// Integrates Claude AI with conversation tracking and support escalation
    /// </summary>
    public class LandingChatService : ILandingChatService
    {
        private readonly GrcDbContext _context;
        private readonly IClaudeAgentService _claudeService;
        private readonly ISupportAgentService _supportService;
        private readonly IHtmlSanitizerService _sanitizer;
        private readonly ILogger<LandingChatService> _logger;

        // Rate limiting constants
        private const int ANONYMOUS_RATE_LIMIT = 10; // messages per minute
        private const int AUTHENTICATED_RATE_LIMIT = 30; // messages per minute
        private const int RATE_LIMIT_WINDOW_MINUTES = 1;

        public LandingChatService(
            GrcDbContext context,
            IClaudeAgentService claudeService,
            ISupportAgentService supportService,
            IHtmlSanitizerService sanitizer,
            ILogger<LandingChatService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _claudeService = claudeService ?? throw new ArgumentNullException(nameof(claudeService));
            _supportService = supportService ?? throw new ArgumentNullException(nameof(supportService));
            _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<CreateChatSessionOutputDto>> CreateSessionAsync(
            CreateChatSessionInputDto input,
            string? ipAddress = null,
            string? userAgent = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Generate unique session ID
                var sessionId = Guid.NewGuid().ToString("N");

                var conversation = new ChatConversation
                {
                    SessionId = sessionId,
                    Status = "Active",
                    Category = input.Category ?? "landing_inquiry",
                    StartPageUrl = input.StartPageUrl,
                    ReferrerUrl = input.ReferrerUrl,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ChatConversations.Add(conversation);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created chat session {SessionId} from IP {IpAddress}",
                    sessionId, ipAddress);

                var output = new CreateChatSessionOutputDto
                {
                    SessionId = sessionId,
                    WelcomeMessage = "مرحباً! كيف يمكنني مساعدتك اليوم؟ | Hello! How can I help you today?",
                    SuggestedQuestions = new List<string>
                    {
                        "ما هي شاهين؟ | What is Shahin?",
                        "كيف أبدأ التجربة المجانية؟ | How do I start a free trial?",
                        "ما هي الأسعار؟ | What is the pricing?"
                    },
                    CreatedAt = conversation.CreatedDate
                };

                return Result<CreateChatSessionOutputDto>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat session");
                return Result<CreateChatSessionOutputDto>.Failure(
                    "Failed to create chat session", "CREATION_ERROR");
            }
        }

        public async Task<Result<ChatMessageOutputDto>> SendMessageAsync(
            SendChatMessageInputDto input,
            string? userId = null,
            Guid? tenantId = null,
            string? ipAddress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Sanitize input
                var sanitizedMessage = _sanitizer.SanitizeHtml(input.Message);

                // Check rate limit
                var identifier = userId ?? ipAddress ?? "unknown";
                var rateLimitResult = await CheckRateLimitAsync(identifier, userId != null, cancellationToken);
                if (!rateLimitResult.IsSuccess || !rateLimitResult.Value)
                {
                    _logger.LogWarning("Rate limit exceeded for {Identifier}", identifier);
                    return Result<ChatMessageOutputDto>.Failure(
                        "معدل الطلبات مرتفع جداً. يرجى المحاولة لاحقاً. | Rate limit exceeded. Please try again later.",
                        "RATE_LIMIT_EXCEEDED");
                }

                // Get or create conversation
                ChatConversation conversation;

                if (!string.IsNullOrEmpty(input.SessionId))
                {
                    var convResult = await GetActiveConversationAsync(input.SessionId, userId, cancellationToken);
                    if (!convResult.IsSuccess)
                    {
                        return Result<ChatMessageOutputDto>.Failure(
                            "جلسة غير صالحة أو منتهية | Invalid or expired session", "INVALID_SESSION");
                    }
                    conversation = convResult.Value;
                }
                else if (!string.IsNullOrEmpty(userId))
                {
                    // Authenticated user without session - create new
                    var createResult = await CreateSessionAsync(
                        new CreateChatSessionInputDto
                        {
                            StartPageUrl = input.PageUrl,
                            Category = input.PageContext
                        },
                        ipAddress,
                        null,
                        cancellationToken);

                    if (!createResult.IsSuccess)
                        return Result<ChatMessageOutputDto>.Failure(createResult.Error, createResult.ErrorCode);

                    var sessionId = createResult.Value.SessionId;
                    var convResult = await GetActiveConversationAsync(sessionId, userId, cancellationToken);
                    if (!convResult.IsSuccess)
                        return Result<ChatMessageOutputDto>.Failure("فشل في إنشاء المحادثة | Failed to create conversation", "CREATION_ERROR");

                    conversation = convResult.Value;
                }
                else
                {
                    return Result<ChatMessageOutputDto>.Failure(
                        "معرّف الجلسة مطلوب للمستخدمين المجهولين | Session ID is required for anonymous users", "SESSION_REQUIRED");
                }

                // Save user message
                var userMessage = new Models.Entities.ChatMessage
                {
                    ConversationId = conversation.Id,
                    SenderType = "User",
                    Content = sanitizedMessage,
                    PageUrl = input.PageUrl,
                    PageContext = input.PageContext,
                    IsAiGenerated = false,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ChatMessages.Add(userMessage);
                await _context.SaveChangesAsync(cancellationToken);

                // Get conversation history for context (last 20 messages)
                var conversationHistory = await BuildConversationHistoryAsync(conversation.Id, cancellationToken);

                // Get AI response
                string aiResponse;
                bool isAiGenerated = false;
                double? aiConfidence = null;

                var isAiAvailable = await _claudeService.IsAvailableAsync(cancellationToken);

                if (isAiAvailable)
                {
                    try
                    {
                        var context = BuildAiContext(input.PageUrl, input.PageContext, conversation.Category);
                        var aiResult = await _claudeService.ChatAsync(
                            sanitizedMessage,
                            conversationHistory,
                            context,
                            cancellationToken);

                        if (aiResult.Success)
                        {
                            aiResponse = aiResult.Response;
                            isAiGenerated = true;
                            aiConfidence = 0.85; // Claude typically has high confidence
                        }
                        else
                        {
                            aiResponse = GetFallbackResponse(sanitizedMessage, input.PageContext);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "AI service error, using fallback");
                        aiResponse = GetFallbackResponse(sanitizedMessage, input.PageContext);
                    }
                }
                else
                {
                    aiResponse = GetFallbackResponse(sanitizedMessage, input.PageContext);
                }

                // Save AI response
                var agentMessage = new Models.Entities.ChatMessage
                {
                    ConversationId = conversation.Id,
                    SenderType = "Agent",
                    Content = aiResponse,
                    PageUrl = input.PageUrl,
                    PageContext = input.PageContext,
                    IsAiGenerated = isAiGenerated,
                    AiModel = isAiGenerated ? "claude-sonnet-4-5" : null,
                    AiConfidence = aiConfidence,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ChatMessages.Add(agentMessage);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Chat message sent in session {SessionId}, AI: {IsAI}",
                    conversation.SessionId, isAiGenerated);

                var output = new ChatMessageOutputDto
                {
                    MessageId = agentMessage.Id,
                    Response = aiResponse,
                    SessionId = conversation.SessionId!,
                    Timestamp = agentMessage.CreatedDate,
                    IsAiGenerated = isAiGenerated,
                    AiConfidence = aiConfidence,
                    IsEscalated = conversation.IsEscalated,
                    Suggestions = GenerateSuggestions(input.PageContext)
                };

                return Result<ChatMessageOutputDto>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat message");
                return Result<ChatMessageOutputDto>.Failure(
                    "فشل في إرسال الرسالة | Failed to send message", "MESSAGE_ERROR");
            }
        }

        public async Task<Result<ChatConversationHistoryDto>> GetConversationHistoryAsync(
            string sessionId,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var conversation = await _context.ChatConversations
                    .Include(c => c.Messages.OrderBy(m => m.CreatedDate))
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId && !c.IsDeleted, cancellationToken);

                if (conversation == null)
                    return Result<ChatConversationHistoryDto>.Failure("Conversation not found", "NOT_FOUND");

                // Verify ownership for authenticated users
                if (!string.IsNullOrEmpty(userId) && conversation.UserId != userId)
                    return Result<ChatConversationHistoryDto>.Failure("غير مصرح بالوصول إلى هذه المحادثة | Not authorized to access this conversation", "UNAUTHORIZED");

                var output = new ChatConversationHistoryDto
                {
                    Conversation = new ChatConversationDto
                    {
                        Id = conversation.Id,
                        SessionId = conversation.SessionId,
                        Status = conversation.Status,
                        Category = conversation.Category,
                        IsEscalated = conversation.IsEscalated,
                        CreatedAt = conversation.CreatedDate,
                        ResolvedAt = conversation.ResolvedAt,
                        MessageCount = conversation.Messages.Count
                    },
                    Messages = conversation.Messages.Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        SenderType = m.SenderType,
                        Content = m.Content,
                        CreatedAt = m.CreatedDate,
                        IsAiGenerated = m.IsAiGenerated
                    }).ToList()
                };

                return Result<ChatConversationHistoryDto>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation history for session {SessionId}", sessionId);
                return Result<ChatConversationHistoryDto>.Failure(
                    "فشل في استرجاع سجل المحادثة | Failed to retrieve conversation history", "RETRIEVAL_ERROR");
            }
        }

        public async Task<Result<EscalateChatOutputDto>> EscalateToSupportAsync(
            EscalateChatInputDto input,
            string? userId = null,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var convResult = await GetActiveConversationAsync(input.SessionId, userId, cancellationToken);
                if (!convResult.IsSuccess)
                    return Result<EscalateChatOutputDto>.Failure(convResult.Error, convResult.ErrorCode);

                var conversation = convResult.Value;

                if (conversation.IsEscalated)
                {
                    return Result<EscalateChatOutputDto>.Success(new EscalateChatOutputDto
                    {
                        Success = true,
                        SupportConversationId = conversation.SupportConversationId,
                        Message = "المحادثة مُصعَّدة بالفعل | Conversation already escalated"
                    });
                }

                // Create support conversation using existing ISupportAgentService
                var supportConversation = await _supportService.StartConversationAsync(
                    tenantId,
                    userId,
                    conversation.SessionId,
                    "escalated_from_landing_chat");

                // Transfer chat messages to support conversation
                var chatMessages = await _context.ChatMessages
                    .Where(m => m.ConversationId == conversation.Id)
                    .OrderBy(m => m.CreatedDate)
                    .ToListAsync(cancellationToken);

                foreach (var msg in chatMessages)
                {
                    await _supportService.SendMessageAsync(
                        supportConversation.Id,
                        msg.Content,
                        msg.SenderType,
                        msg.SenderType == "User" ? userId : "AI");
                }

                // Update chat conversation
                conversation.IsEscalated = true;
                conversation.EscalatedAt = DateTime.UtcNow;
                conversation.EscalationReason = input.Reason;
                conversation.SupportConversationId = supportConversation.Id;
                conversation.Status = "Escalated";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Chat session {SessionId} escalated to support conversation {SupportId}",
                    conversation.SessionId, supportConversation.Id);

                var output = new EscalateChatOutputDto
                {
                    Success = true,
                    SupportConversationId = supportConversation.Id,
                    Message = "تم تصعيد المحادثة إلى فريق الدعم | Conversation escalated to support team"
                };

                return Result<EscalateChatOutputDto>.Success(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error escalating chat session {SessionId}", input.SessionId);
                return Result<EscalateChatOutputDto>.Failure(
                    "فشل في تصعيد المحادثة | Failed to escalate conversation", "ESCALATION_ERROR");
            }
        }

        public async Task<Result<bool>> ResolveConversationAsync(
            ResolveChatInputDto input,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var convResult = await GetActiveConversationAsync(input.SessionId, userId, cancellationToken);
                if (!convResult.IsSuccess)
                    return Result<bool>.Failure(convResult.Error, convResult.ErrorCode);

                var conversation = convResult.Value;

                conversation.Status = "Resolved";
                conversation.ResolvedAt = DateTime.UtcNow;
                conversation.SatisfactionRating = input.SatisfactionRating;
                conversation.Feedback = input.Feedback;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Chat session {SessionId} resolved with rating {Rating}",
                    conversation.SessionId, input.SatisfactionRating);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving chat session {SessionId}", input.SessionId);
                return Result<bool>.Failure("فشل في إغلاق المحادثة | Failed to resolve conversation", "RESOLUTION_ERROR");
            }
        }

        public async Task<Result<ChatConversation>> GetActiveConversationAsync(
            string sessionId,
            string? userId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var conversation = await _context.ChatConversations
                    .FirstOrDefaultAsync(c =>
                        c.SessionId == sessionId &&
                        !c.IsDeleted &&
                        c.Status != "Closed" &&
                        c.Status != "Resolved",
                        cancellationToken);

                if (conversation == null)
                    return Result<ChatConversation>.Failure("Active conversation not found", "NOT_FOUND");

                // Verify ownership for authenticated users
                if (!string.IsNullOrEmpty(userId) && conversation.UserId != userId)
                    return Result<ChatConversation>.Failure("غير مصرح بالوصول إلى هذه المحادثة | Not authorized to access this conversation", "UNAUTHORIZED");

                return Result<ChatConversation>.Success(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active conversation {SessionId}", sessionId);
                return Result<ChatConversation>.Failure("فشل في استرجاع المحادثة | Failed to retrieve conversation", "RETRIEVAL_ERROR");
            }
        }

        public async Task<Result<bool>> CheckRateLimitAsync(
            string identifier,
            bool isAuthenticated,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var limit = isAuthenticated ? AUTHENTICATED_RATE_LIMIT : ANONYMOUS_RATE_LIMIT;
                var windowStart = DateTime.UtcNow.AddMinutes(-RATE_LIMIT_WINDOW_MINUTES);

                // Count messages from this identifier in the time window
                var query = _context.ChatMessages
                    .Join(_context.ChatConversations,
                        msg => msg.ConversationId,
                        conv => conv.Id,
                        (msg, conv) => new { Message = msg, Conversation = conv })
                    .Where(x =>
                        x.Message.CreatedDate >= windowStart &&
                        x.Message.SenderType == "User" &&
                        (x.Conversation.UserId == identifier || x.Conversation.IpAddress == identifier));

                var messageCount = await query.CountAsync(cancellationToken);

                if (messageCount >= limit)
                {
                    _logger.LogWarning("Rate limit exceeded for {Identifier}: {Count}/{Limit}",
                        identifier, messageCount, limit);
                    return Result<bool>.Success(false);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rate limit for {Identifier}", identifier);
                // On error, allow the request (fail open for availability)
                return Result<bool>.Success(true);
            }
        }

        // Private helper methods

        private async Task<List<Interfaces.ChatMessage>> BuildConversationHistoryAsync(
            Guid conversationId,
            CancellationToken cancellationToken)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedDate)
                .Take(20) // Last 20 messages for context
                .ToListAsync(cancellationToken);

            return messages.Select(m => new Interfaces.ChatMessage
            {
                Role = m.SenderType == "User" ? "user" : "assistant",
                Content = m.Content
            }).ToList();
        }

        private string BuildAiContext(string? pageUrl, string? pageContext, string? category)
        {
            var context = "أنت مساعد ذكي لمنصة شاهين (Shahin GRC). ";
            context += "هذا زائر في صفحة الهبوط. ";

            if (!string.IsNullOrEmpty(pageContext))
            {
                context += pageContext.ToLower() switch
                {
                    "pricing" => "الزائر يستعرض صفحة الأسعار. ",
                    "features" => "الزائر يستعرض صفحة المميزات. ",
                    "about" => "الزائر يستعرض صفحة حول المنصة. ",
                    "contact" => "الزائر يريد التواصل معنا. ",
                    _ => ""
                };
            }

            context += "أجب بشكل مختصر ومفيد بالعربية والإنجليزية. ";
            context += "ركز على مساعدته في فهم شاهين وتشجيعه على التسجيل للتجربة المجانية.";

            return context;
        }

        private string GetFallbackResponse(string message, string? pageContext)
        {
            var lowerMessage = message.ToLowerInvariant();

            // Arabic keywords
            if (lowerMessage.Contains("ما هي") || lowerMessage.Contains("what is"))
                return "شاهين هي منصة متكاملة لإدارة الحوكمة والمخاطر والامتثال (GRC). " +
                       "تساعدك على تحقيق الامتثال للأنظمة السعودية والدولية بسهولة. | " +
                       "Shahin is a comprehensive GRC platform that helps you achieve compliance easily.";

            if (lowerMessage.Contains("تجربة") || lowerMessage.Contains("trial"))
                return "يمكنك بدء تجربة مجانية لمدة 14 يوماً دون الحاجة لبطاقة ائتمانية. " +
                       "<a href='/trial'>سجل الآن!</a> | " +
                       "You can start a 14-day free trial without a credit card. " +
                       "<a href='/trial'>Sign up now!</a>";

            if (lowerMessage.Contains("سعر") || lowerMessage.Contains("price"))
                return "نقدم خططاً مرنة تناسب احتياجاتك. تواصل معنا للحصول على عرض سعر مخصص. | " +
                       "We offer flexible plans. Contact us for a custom quote.";

            if (lowerMessage.Contains("اتصال") || lowerMessage.Contains("contact"))
                return "يمكنك التواصل معنا عبر: info@shahin-ai.com أو الهاتف: +966 XX XXX XXXX | " +
                       "Contact us: info@shahin-ai.com or phone: +966 XX XXX XXXX";

            // Context-based responses
            if (pageContext == "pricing")
                return "نقدم ثلاث خطط رئيسية: Starter وProfessional وEnterprise. " +
                       "ابدأ بتجربة مجانية لمدة 14 يوماً! | " +
                       "We offer three plans: Starter, Professional, Enterprise. Start your 14-day free trial!";

            // Default
            return "شكراً لتواصلك! فريق المبيعات سيتواصل معك قريباً. " +
                   "أو يمكنك البدء بالتجربة المجانية الآن! | " +
                   "Thank you! Our sales team will contact you soon. Or start your free trial now!";
        }

        private List<string>? GenerateSuggestions(string? pageContext)
        {
            return pageContext?.ToLower() switch
            {
                "pricing" => new List<string>
                {
                    "ما هي مميزات كل خطة؟ | What are the features of each plan?",
                    "هل يوجد خصومات للمؤسسات؟ | Are there enterprise discounts?",
                    "كيف أبدأ التجربة المجانية؟ | How do I start the free trial?"
                },
                "features" => new List<string>
                {
                    "ما هي الأنظمة المدعومة؟ | What compliance frameworks are supported?",
                    "هل يدعم النظام التكامل؟ | Does the system support integrations?",
                    "ما هو دور الذكاء الاصطناعي؟ | What is the role of AI?"
                },
                _ => new List<string>
                {
                    "ما هي شاهين؟ | What is Shahin?",
                    "كيف أبدأ؟ | How do I get started?",
                    "التواصل مع المبيعات | Contact sales"
                }
            };
        }
    }
}
