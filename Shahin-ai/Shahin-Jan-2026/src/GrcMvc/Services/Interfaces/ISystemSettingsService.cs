using GrcMvc.Models.Entities;

namespace GrcMvc.Services.Interfaces;

/// <summary>
/// Service for managing system settings from database.
/// Provides runtime configuration management via Admin UI.
/// </summary>
public interface ISystemSettingsService
{
    /// <summary>Get a setting value by key</summary>
    Task<string?> GetValueAsync(string key, Guid? tenantId = null);

    /// <summary>Get a setting value with type conversion</summary>
    Task<T?> GetValueAsync<T>(string key, Guid? tenantId = null);

    /// <summary>Get a setting value or default</summary>
    Task<string> GetValueOrDefaultAsync(string key, string defaultValue, Guid? tenantId = null);

    /// <summary>Set a setting value</summary>
    Task SetValueAsync(string key, string? value, string? modifiedBy = null, Guid? tenantId = null);

    /// <summary>Get all settings by category</summary>
    Task<List<SystemSetting>> GetByCategoryAsync(string category, Guid? tenantId = null);

    /// <summary>Get all settings</summary>
    Task<List<SystemSetting>> GetAllAsync(Guid? tenantId = null);

    /// <summary>Check if a feature/integration is enabled</summary>
    Task<bool> IsEnabledAsync(string key, Guid? tenantId = null);

    /// <summary>Bulk update settings</summary>
    Task BulkUpdateAsync(Dictionary<string, string?> settings, string? modifiedBy = null, Guid? tenantId = null);

    /// <summary>Initialize default settings (seed)</summary>
    Task InitializeDefaultsAsync();

    /// <summary>Get setting with full metadata</summary>
    Task<SystemSetting?> GetSettingAsync(string key, Guid? tenantId = null);

    /// <summary>Delete a setting</summary>
    Task DeleteAsync(string key, Guid? tenantId = null);

    /// <summary>Export all settings to JSON</summary>
    Task<string> ExportToJsonAsync(Guid? tenantId = null);

    /// <summary>Import settings from JSON</summary>
    Task ImportFromJsonAsync(string json, string? modifiedBy = null, Guid? tenantId = null);
}
