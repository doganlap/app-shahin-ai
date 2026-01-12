using System.Threading.Tasks;

namespace Grc.Notification.Application.Channels;

/// <summary>
/// SMS notification channel interface
/// </summary>
public interface ISmsChannel
{
    Task SendAsync(string recipientPhoneNumber, string message);
}

