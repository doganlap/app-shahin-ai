using System.Threading.Tasks;

namespace Grc.Notification.Application.Channels;

/// <summary>
/// Email notification channel interface
/// </summary>
public interface IEmailChannel
{
    Task SendAsync(string recipientEmail, string subject, string body);
    Task SendTemplateAsync(string recipientEmail, string templateName, Dictionary<string, object> templateData);
}

