using System.Text.Json;
using GrcMvc.Data;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GrcMvc.Services.Implementations;

/// <summary>
/// Database-backed system settings service.
/// Caches settings in memory for performance.
/// </summary>
public class SystemSettingsService : ISystemSettingsService
{
    private readonly GrcDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SystemSettingsService> _logger;
    private readonly IConfiguration _configuration;
    private const string CacheKeyPrefix = "SystemSetting:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public SystemSettingsService(
        GrcDbContext db,
        IMemoryCache cache,
        ILogger<SystemSettingsService> logger,
        IConfiguration configuration)
    {
        _db = db;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string?> GetValueAsync(string key, Guid? tenantId = null)
    {
        var cacheKey = $"{CacheKeyPrefix}{key}:{tenantId}";

        if (_cache.TryGetValue(cacheKey, out string? cachedValue))
            return cachedValue;

        var setting = await _db.Set<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == tenantId);

        if (setting == null)
        {
            // Fallback to global setting
            setting = await _db.Set<SystemSetting>()
                .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == null);
        }

        if (setting == null)
        {
            // Fallback to environment variable / appsettings
            var envKey = key.Replace(":", "__").Replace(".", "__");
            var envValue = Environment.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            return _configuration[key];
        }

        var value = setting.IsEnabled ? setting.Value : null;
        _cache.Set(cacheKey, value, CacheDuration);
        return value;
    }

    public async Task<T?> GetValueAsync<T>(string key, Guid? tenantId = null)
    {
        var value = await GetValueAsync(key, tenantId);
        if (string.IsNullOrEmpty(value))
            return default;

        try
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)(value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1");
            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(value);
            if (typeof(T) == typeof(double))
                return (T)(object)double.Parse(value);
            if (typeof(T) == typeof(Guid))
                return (T)(object)Guid.Parse(value);

            return JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return default;
        }
    }

    public async Task<string> GetValueOrDefaultAsync(string key, string defaultValue, Guid? tenantId = null)
    {
        return await GetValueAsync(key, tenantId) ?? defaultValue;
    }

    public async Task SetValueAsync(string key, string? value, string? modifiedBy = null, Guid? tenantId = null)
    {
        var setting = await _db.Set<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == tenantId);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Category = key.Contains(":") ? key.Split(':')[0] : "General",
                DisplayName = key,
                TenantId = tenantId
            };
            _db.Set<SystemSetting>().Add(setting);
        }

        setting.Value = value;
        setting.ModifiedAt = DateTime.UtcNow;
        setting.ModifiedBy = modifiedBy;

        await _db.SaveChangesAsync();

        // Invalidate cache
        var cacheKey = $"{CacheKeyPrefix}{key}:{tenantId}";
        _cache.Remove(cacheKey);

        _logger.LogInformation("System setting updated: {Key} by {User}", key, modifiedBy);
    }

    public async Task<List<SystemSetting>> GetByCategoryAsync(string category, Guid? tenantId = null)
    {
        return await _db.Set<SystemSetting>()
            .Where(s => s.Category.StartsWith(category) && (s.TenantId == tenantId || s.TenantId == null))
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<SystemSetting>> GetAllAsync(Guid? tenantId = null)
    {
        return await _db.Set<SystemSetting>()
            .Where(s => s.TenantId == tenantId || s.TenantId == null)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> IsEnabledAsync(string key, Guid? tenantId = null)
    {
        var value = await GetValueAsync(key, tenantId);
        return !string.IsNullOrEmpty(value) &&
               (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1");
    }

    public async Task BulkUpdateAsync(Dictionary<string, string?> settings, string? modifiedBy = null, Guid? tenantId = null)
    {
        foreach (var kv in settings)
        {
            await SetValueAsync(kv.Key, kv.Value, modifiedBy, tenantId);
        }
    }

    public async Task<SystemSetting?> GetSettingAsync(string key, Guid? tenantId = null)
    {
        return await _db.Set<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && (s.TenantId == tenantId || s.TenantId == null));
    }

    public async Task DeleteAsync(string key, Guid? tenantId = null)
    {
        var setting = await _db.Set<SystemSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && s.TenantId == tenantId);

        if (setting != null)
        {
            _db.Set<SystemSetting>().Remove(setting);
            await _db.SaveChangesAsync();

            var cacheKey = $"{CacheKeyPrefix}{key}:{tenantId}";
            _cache.Remove(cacheKey);
        }
    }

    public async Task<string> ExportToJsonAsync(Guid? tenantId = null)
    {
        var settings = await GetAllAsync(tenantId);
        var export = settings.Select(s => new
        {
            s.Category,
            s.Key,
            s.Value,
            s.DisplayName,
            s.Description,
            s.DataType,
            s.IsEnabled,
            s.IsSensitive
        });
        return JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task ImportFromJsonAsync(string json, string? modifiedBy = null, Guid? tenantId = null)
    {
        var settings = JsonSerializer.Deserialize<List<SystemSettingImport>>(json);
        if (settings == null) return;

        foreach (var s in settings)
        {
            await SetValueAsync(s.Key, s.Value, modifiedBy, tenantId);
        }
    }

    public async Task InitializeDefaultsAsync()
    {
        var existingCount = await _db.Set<SystemSetting>().CountAsync();
        if (existingCount > 0) return; // Already initialized

        var defaults = GetDefaultSettings();
        _db.Set<SystemSetting>().AddRange(defaults);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Initialized {Count} default system settings", defaults.Count);
    }

    private List<SystemSetting> GetDefaultSettings()
    {
        var settings = new List<SystemSetting>();
        var order = 0;

        // AI - Claude
        settings.Add(CreateSetting(SettingCategories.Claude, "Claude:Enabled", "Claude AI Enabled", "true", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Claude, "Claude:ApiKey", "Claude API Key", "", "encrypted", order++, true, true));
        settings.Add(CreateSetting(SettingCategories.Claude, "Claude:Model", "Claude Model", "claude-sonnet-4-20250514", "string", order++));
        settings.Add(CreateSetting(SettingCategories.Claude, "Claude:MaxTokens", "Max Tokens", "4096", "int", order++));

        // AI - OpenAI
        settings.Add(CreateSetting(SettingCategories.OpenAI, "OpenAI:Enabled", "OpenAI Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.OpenAI, "OpenAI:ApiKey", "OpenAI API Key", "", "encrypted", order++, true, true));
        settings.Add(CreateSetting(SettingCategories.OpenAI, "OpenAI:Model", "OpenAI Model", "gpt-4-turbo", "string", order++));

        // AI - Azure OpenAI
        settings.Add(CreateSetting(SettingCategories.AzureOpenAI, "AzureOpenAI:Enabled", "Azure OpenAI Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.AzureOpenAI, "AzureOpenAI:ApiKey", "Azure OpenAI Key", "", "encrypted", order++, true, true));
        settings.Add(CreateSetting(SettingCategories.AzureOpenAI, "AzureOpenAI:Endpoint", "Azure OpenAI Endpoint", "", "string", order++, false, true));

        // AI - Copilot
        settings.Add(CreateSetting(SettingCategories.Copilot, "Copilot:Enabled", "Microsoft Copilot Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Copilot, "Copilot:ClientId", "Copilot Client ID", "", "string", order++, false, true));
        settings.Add(CreateSetting(SettingCategories.Copilot, "Copilot:ClientSecret", "Copilot Client Secret", "", "encrypted", order++, true, true));

        // Email - SMTP
        settings.Add(CreateSetting(SettingCategories.SMTP, "SMTP:FromEmail", "From Email Address", "noreply@shahin-ai.com", "string", order++, false, true));
        settings.Add(CreateSetting(SettingCategories.SMTP, "SMTP:ClientId", "SMTP Client ID", "", "string", order++, false, true));
        settings.Add(CreateSetting(SettingCategories.SMTP, "SMTP:ClientSecret", "SMTP Client Secret", "", "encrypted", order++, true, true));

        // Integration - Camunda
        settings.Add(CreateSetting(SettingCategories.Camunda, "Camunda:Enabled", "Camunda BPM Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Camunda, "Camunda:BaseUrl", "Camunda Base URL", "http://camunda:8080/camunda", "string", order++));
        settings.Add(CreateSetting(SettingCategories.Camunda, "Camunda:Username", "Camunda Username", "admin", "string", order++));
        settings.Add(CreateSetting(SettingCategories.Camunda, "Camunda:Password", "Camunda Password", "", "encrypted", order++, true));

        // Integration - Kafka
        settings.Add(CreateSetting(SettingCategories.Kafka, "Kafka:Enabled", "Kafka Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Kafka, "Kafka:BootstrapServers", "Kafka Bootstrap Servers", "kafka:9092", "string", order++));

        // Integration - RabbitMQ
        settings.Add(CreateSetting(SettingCategories.RabbitMQ, "RabbitMQ:Enabled", "RabbitMQ Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.RabbitMQ, "RabbitMQ:Host", "RabbitMQ Host", "rabbitmq", "string", order++));

        // Integration - Redis
        settings.Add(CreateSetting(SettingCategories.Redis, "Redis:Enabled", "Redis Cache Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Redis, "Redis:ConnectionString", "Redis Connection", "redis:6379", "string", order++));

        // Integration - Hangfire
        settings.Add(CreateSetting(SettingCategories.Hangfire, "Hangfire:Enabled", "Hangfire Jobs Enabled", "false", "bool", order++));

        // Security
        settings.Add(CreateSetting(SettingCategories.Security, "Security:AllowPublicRegistration", "Allow Public Registration", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Security, "Security:Captcha:Enabled", "CAPTCHA Enabled", "false", "bool", order++));
        settings.Add(CreateSetting(SettingCategories.Security, "Security:Captcha:SiteKey", "CAPTCHA Site Key", "", "string", order++));
        settings.Add(CreateSetting(SettingCategories.Security, "Security:Captcha:SecretKey", "CAPTCHA Secret Key", "", "encrypted", order++, true));

        // Azure
        settings.Add(CreateSetting(SettingCategories.Azure, "Azure:TenantId", "Azure Tenant ID", "", "string", order++, false, true));
        settings.Add(CreateSetting(SettingCategories.AzureBlob, "Azure:BlobStorage:ConnectionString", "Blob Storage Connection", "", "encrypted", order++, true));
        settings.Add(CreateSetting(SettingCategories.AzureBlob, "Azure:BlobStorage:ContainerName", "Blob Container Name", "grc-files", "string", order++));

        return settings;
    }

    private SystemSetting CreateSetting(string category, string key, string displayName, string? defaultValue,
        string dataType, int order, bool isSensitive = false, bool isRequired = false)
    {
        return new SystemSetting
        {
            Category = category,
            Key = key,
            DisplayName = displayName,
            DefaultValue = defaultValue,
            Value = defaultValue,
            DataType = dataType,
            DisplayOrder = order,
            IsSensitive = isSensitive,
            IsRequired = isRequired,
            IsEnabled = true
        };
    }

    private class SystemSettingImport
    {
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}
