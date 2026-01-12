using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Grc.Notification.Application.Channels;

/// <summary>
/// SMS notification channel implementation
/// </summary>
public class SmsChannel : ISmsChannel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsChannel> _logger;

    public SmsChannel(IConfiguration configuration, ILogger<SmsChannel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string recipientPhoneNumber, string message)
    {
        // TODO: Integrate with SMS provider (Twilio, AWS SNS, etc.)
        // For now, log the SMS
        _logger.LogInformation("SMS to {PhoneNumber}: {Message}", recipientPhoneNumber, message);
        
        // Example integration:
        // var smsProvider = _configuration["Sms:Provider"];
        // if (smsProvider == "Twilio")
        // {
        //     await SendViaTwilioAsync(recipientPhoneNumber, message);
        // }
        
        await Task.CompletedTask;
    }
}

