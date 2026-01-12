using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Emailing;

namespace Grc.Notification.Application.Channels;

/// <summary>
/// Email notification channel implementation
/// </summary>
public class EmailChannel : IEmailChannel
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailChannel> _logger;

    public EmailChannel(IEmailSender emailSender, ILogger<EmailChannel> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendAsync(string recipientEmail, string subject, string body)
    {
        try
        {
            await _emailSender.SendAsync(recipientEmail, subject, body);
            _logger.LogInformation("Email sent to {Recipient}", recipientEmail);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Recipient}", recipientEmail);
            throw;
        }
    }

    public async Task SendTemplateAsync(string recipientEmail, string templateName, Dictionary<string, object> templateData)
    {
        // TODO: Load email template and render with data
        // This would use a templating engine (Razor, Handlebars, etc.)
        var subject = $"GRC Platform: {templateName}";
        var body = $"Template: {templateName}\n\nData: {System.Text.Json.JsonSerializer.Serialize(templateData)}";
        
        await SendAsync(recipientEmail, subject, body);
    }
}

