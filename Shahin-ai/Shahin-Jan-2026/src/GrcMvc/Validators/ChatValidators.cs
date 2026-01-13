using FluentValidation;
using GrcMvc.Models.DTOs;
using GrcMvc.Services.Interfaces;

namespace GrcMvc.Validators
{
    /// <summary>
    /// Validator for SendChatMessageInputDto
    /// Prevents XSS, validates length, checks for malicious content
    /// </summary>
    public class SendChatMessageInputDtoValidator : AbstractValidator<SendChatMessageInputDto>
    {
        private readonly IHtmlSanitizerService _sanitizer;

        public SendChatMessageInputDtoValidator(IHtmlSanitizerService sanitizer)
        {
            _sanitizer = sanitizer;

            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("رسالة مطلوبة | Message is required")
                .MaximumLength(500)
                .WithMessage("الرسالة لا يمكن أن تتجاوز 500 حرف | Message cannot exceed 500 characters")
                .Must(BeValidMessage)
                .WithMessage("الرسالة تحتوي على محتوى غير صالح | Message contains invalid content");

            RuleFor(x => x.SessionId)
                .MaximumLength(100)
                .WithMessage("معرّف الجلسة غير صالح | Invalid session ID");

            RuleFor(x => x.PageUrl)
                .MaximumLength(500)
                .WithMessage("عنوان URL غير صالح | Invalid URL");

            RuleFor(x => x.PageContext)
                .MaximumLength(100)
                .WithMessage("سياق الصفحة غير صالح | Invalid page context");
        }

        private bool BeValidMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            // Check for XSS patterns
            var sanitized = _sanitizer.SanitizeHtml(message);

            // If sanitization drastically changes the message, it likely contained malicious content
            // Allow for minor whitespace differences
            var originalTrimmed = message.Trim();
            var sanitizedTrimmed = sanitized.Trim();

            // Simple heuristic: if sanitized version is significantly shorter, reject
            if (sanitizedTrimmed.Length < originalTrimmed.Length * 0.8)
                return false;

            // Check for excessive special characters (potential script injection)
            var specialCharCount = message.Count(c => "<>{}[]()".Contains(c));
            if (specialCharCount > message.Length * 0.2)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Validator for CreateChatSessionInputDto
    /// </summary>
    public class CreateChatSessionInputDtoValidator : AbstractValidator<CreateChatSessionInputDto>
    {
        public CreateChatSessionInputDtoValidator()
        {
            RuleFor(x => x.StartPageUrl)
                .MaximumLength(500)
                .WithMessage("عنوان URL غير صالح | Invalid URL");

            RuleFor(x => x.ReferrerUrl)
                .MaximumLength(500)
                .WithMessage("عنوان URL للمُحيل غير صالح | Invalid referrer URL");

            RuleFor(x => x.Category)
                .MaximumLength(50)
                .WithMessage("فئة غير صالحة | Invalid category");
        }
    }

    /// <summary>
    /// Validator for EscalateChatInputDto
    /// </summary>
    public class EscalateChatInputDtoValidator : AbstractValidator<EscalateChatInputDto>
    {
        public EscalateChatInputDtoValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("معرّف الجلسة مطلوب | Session ID is required")
                .MaximumLength(100)
                .WithMessage("معرّف الجلسة غير صالح | Invalid session ID");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("سبب التصعيد مطلوب | Escalation reason is required")
                .MaximumLength(500)
                .WithMessage("سبب التصعيد لا يمكن أن يتجاوز 500 حرف | Reason cannot exceed 500 characters");
        }
    }

    /// <summary>
    /// Validator for ResolveChatInputDto
    /// </summary>
    public class ResolveChatInputDtoValidator : AbstractValidator<ResolveChatInputDto>
    {
        public ResolveChatInputDtoValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("معرّف الجلسة مطلوب | Session ID is required")
                .MaximumLength(100)
                .WithMessage("معرّف الجلسة غير صالح | Invalid session ID");

            RuleFor(x => x.SatisfactionRating)
                .InclusiveBetween(1, 5)
                .When(x => x.SatisfactionRating.HasValue)
                .WithMessage("التقييم يجب أن يكون بين 1 و 5 | Rating must be between 1 and 5");

            RuleFor(x => x.Feedback)
                .MaximumLength(1000)
                .WithMessage("التعليقات لا يمكن أن تتجاوز 1000 حرف | Feedback cannot exceed 1000 characters");
        }
    }
}
