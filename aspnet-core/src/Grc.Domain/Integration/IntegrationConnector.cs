using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Grc.Integration;

/// <summary>
/// Integration connector entity for external system integrations
/// </summary>
public class IntegrationConnector : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; private set; }
    public string NameAr { get; private set; }
    public string Description { get; private set; }
    public string DescriptionAr { get; private set; }
    public IntegrationType Type { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public string Icon { get; private set; }
    public string Color { get; private set; }
    public string BaseUrl { get; private set; }
    public string ApiKey { get; private set; }
    public string ClientId { get; private set; }
    public string ClientSecret { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime? TokenExpiresAt { get; private set; }
    public string WebhookUrl { get; private set; }
    public string WebhookSecret { get; private set; }
    public string Configuration { get; private set; }
    public DateTime? LastSyncAt { get; private set; }
    public DateTime? NextSyncAt { get; private set; }
    public int SyncIntervalMinutes { get; private set; }
    public bool AutoSyncEnabled { get; private set; }
    public int SuccessfulSyncs { get; private set; }
    public int FailedSyncs { get; private set; }
    public string LastError { get; private set; }
    public DateTime? LastErrorAt { get; private set; }

    protected IntegrationConnector() { }

    public IntegrationConnector(
        Guid id,
        string name,
        IntegrationType type,
        Guid? tenantId = null)
        : base(id)
    {
        Name = name;
        Type = type;
        Status = IntegrationStatus.NotConfigured;
        SyncIntervalMinutes = 60;
        AutoSyncEnabled = false;
        SuccessfulSyncs = 0;
        FailedSyncs = 0;
        TenantId = tenantId;
    }

    public void SetArabicDetails(string nameAr, string descriptionAr)
    {
        NameAr = nameAr;
        DescriptionAr = descriptionAr;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public void SetAppearance(string icon, string color)
    {
        Icon = icon;
        Color = color;
    }

    public void Configure(string baseUrl, string apiKey = null, string clientId = null, string clientSecret = null)
    {
        BaseUrl = baseUrl;
        ApiKey = apiKey;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Status = IntegrationStatus.Configured;
    }

    public void SetOAuthTokens(string accessToken, string refreshToken, DateTime expiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenExpiresAt = expiresAt;
    }

    public void SetWebhook(string webhookUrl, string webhookSecret)
    {
        WebhookUrl = webhookUrl;
        WebhookSecret = webhookSecret;
    }

    public void SetConfiguration(string configuration)
    {
        Configuration = configuration;
    }

    public void Connect()
    {
        Status = IntegrationStatus.Connected;
    }

    public void Disconnect()
    {
        Status = IntegrationStatus.Disconnected;
    }

    public void SetSyncSchedule(int intervalMinutes, bool autoSync)
    {
        SyncIntervalMinutes = intervalMinutes;
        AutoSyncEnabled = autoSync;
    }

    public void RecordSuccessfulSync()
    {
        LastSyncAt = DateTime.UtcNow;
        NextSyncAt = DateTime.UtcNow.AddMinutes(SyncIntervalMinutes);
        SuccessfulSyncs++;
        LastError = null;
        LastErrorAt = null;
    }

    public void RecordFailedSync(string error)
    {
        FailedSyncs++;
        LastError = error;
        LastErrorAt = DateTime.UtcNow;
        if (FailedSyncs >= 5)
        {
            Status = IntegrationStatus.Error;
        }
    }

    public void ResetErrorCount()
    {
        FailedSyncs = 0;
        LastError = null;
        LastErrorAt = null;
        if (Status == IntegrationStatus.Error)
        {
            Status = IntegrationStatus.Connected;
        }
    }
}

public enum IntegrationType
{
    Identity = 0,
    IssueTracking = 1,
    ITSM = 2,
    DocumentManagement = 3,
    Notifications = 4,
    SIEM = 5,
    Vulnerability = 6,
    CloudProvider = 7,
    Email = 8,
    Calendar = 9,
    AI = 10,
    Workflow = 11,
    Other = 99
}

public enum IntegrationStatus
{
    NotConfigured = 0,
    Configured = 1,
    Connected = 2,
    Disconnected = 3,
    Error = 4,
    Syncing = 5
}
