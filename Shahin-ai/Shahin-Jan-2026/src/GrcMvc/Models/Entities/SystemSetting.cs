using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrcMvc.Models.Entities;

/// <summary>
/// System-wide configuration settings stored in database.
/// Managed via Admin Dashboard - replaces environment variables for runtime config.
/// </summary>
[Table("system_settings")]
public class SystemSetting
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Category for grouping settings (AI, Email, Security, Integration, etc.)</summary>
    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    /// <summary>Unique key for the setting (e.g., "Claude:ApiKey", "SMTP:Host")</summary>
    [Required, MaxLength(200)]
    public string Key { get; set; } = string.Empty;

    /// <summary>Setting value (encrypted for sensitive keys)</summary>
    [MaxLength(4000)]
    public string? Value { get; set; }

    /// <summary>Display name for UI</summary>
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Description for admin users</summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>Data type: string, int, bool, json, encrypted</summary>
    [MaxLength(50)]
    public string DataType { get; set; } = "string";

    /// <summary>Default value if not set</summary>
    [MaxLength(1000)]
    public string? DefaultValue { get; set; }

    /// <summary>Is this a sensitive value (password, API key)?</summary>
    public bool IsSensitive { get; set; }

    /// <summary>Is this setting currently enabled/active?</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>Is this a required setting?</summary>
    public bool IsRequired { get; set; }

    /// <summary>Order for display in UI</summary>
    public int DisplayOrder { get; set; }

    /// <summary>Validation regex pattern (optional)</summary>
    [MaxLength(500)]
    public string? ValidationPattern { get; set; }

    /// <summary>Tenant ID (null = global setting)</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Last modified timestamp</summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Last modified by user</summary>
    [MaxLength(200)]
    public string? ModifiedBy { get; set; }

    /// <summary>Created timestamp</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Setting categories for organization</summary>
public static class SettingCategories
{
    public const string AI = "AI";
    public const string Claude = "AI.Claude";
    public const string OpenAI = "AI.OpenAI";
    public const string AzureOpenAI = "AI.AzureOpenAI";
    public const string Gemini = "AI.Gemini";
    public const string LocalLlm = "AI.LocalLlm";
    public const string Copilot = "AI.Copilot";

    public const string Email = "Email";
    public const string SMTP = "Email.SMTP";
    public const string MicrosoftGraph = "Email.MicrosoftGraph";

    public const string Security = "Security";
    public const string JWT = "Security.JWT";
    public const string Captcha = "Security.Captcha";
    public const string PasswordPolicy = "Security.PasswordPolicy";

    public const string Integration = "Integration";
    public const string Camunda = "Integration.Camunda";
    public const string Kafka = "Integration.Kafka";
    public const string RabbitMQ = "Integration.RabbitMQ";
    public const string Redis = "Integration.Redis";
    public const string Hangfire = "Integration.Hangfire";

    public const string Azure = "Azure";
    public const string AzureBlob = "Azure.BlobStorage";
    public const string AzureKeyVault = "Azure.KeyVault";

    public const string Application = "Application";
    public const string Logging = "Logging";
    public const string RateLimiting = "RateLimiting";
}
