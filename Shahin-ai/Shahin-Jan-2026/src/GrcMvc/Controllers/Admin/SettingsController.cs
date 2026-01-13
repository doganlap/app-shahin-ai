using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GrcMvc.Services.Interfaces;
using GrcMvc.Models.Entities;
using GrcMvc.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GrcMvc.Controllers;

/// <summary>
/// Admin Settings Controller - Manage all system configuration settings through UI.
/// Replaces environment variable management with database-backed settings.
/// </summary>
[Route("Admin/[controller]")]
[Authorize(Roles = "Admin,PlatformAdmin")]
public class SettingsController : Controller
{
    private readonly ISystemSettingsService _settings;
    private readonly GrcDbContext _db;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISystemSettingsService settings,
        GrcDbContext db,
        ILogger<SettingsController> logger)
    {
        _settings = settings;
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Main settings dashboard - shows all categories
    /// </summary>
    [HttpGet("")]
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var allSettings = await _settings.GetAllAsync();
        var categories = allSettings
            .GroupBy(s => s.Category.Split(':')[0])
            .Select(g => new SettingsCategoryViewModel
            {
                Category = g.Key,
                Count = g.Count(),
                EnabledCount = g.Count(s => s.IsEnabled),
                Settings = g.ToList()
            })
            .OrderBy(c => c.Category)
            .ToList();

        return View(categories);
    }

    /// <summary>
    /// View and edit settings by category
    /// </summary>
    [HttpGet("Category/{category}")]
    public async Task<IActionResult> Category(string category)
    {
        var settings = await _settings.GetByCategoryAsync(category);
        ViewBag.Category = category;
        ViewBag.CategoryDisplayName = GetCategoryDisplayName(category);
        return View(settings);
    }

    /// <summary>
    /// AI Services settings (Claude, OpenAI, Azure, Gemini, Copilot)
    /// </summary>
    [HttpGet("AI")]
    public async Task<IActionResult> AI()
    {
        var categories = new[] { 
            SettingCategories.Claude, 
            SettingCategories.OpenAI, 
            SettingCategories.AzureOpenAI,
            SettingCategories.Gemini,
            SettingCategories.LocalLlm,
            SettingCategories.Copilot 
        };
        
        var settings = new List<SystemSetting>();
        foreach (var cat in categories)
        {
            settings.AddRange(await _settings.GetByCategoryAsync(cat));
        }
        
        ViewBag.Title = "AI Services Configuration";
        return View("CategoryGroup", settings.GroupBy(s => s.Category).ToList());
    }

    /// <summary>
    /// Integration settings (Camunda, Kafka, RabbitMQ, Redis, Hangfire)
    /// </summary>
    [HttpGet("Integrations")]
    public async Task<IActionResult> Integrations()
    {
        var categories = new[] { 
            SettingCategories.Camunda, 
            SettingCategories.Kafka, 
            SettingCategories.RabbitMQ,
            SettingCategories.Redis,
            SettingCategories.Hangfire
        };
        
        var settings = new List<SystemSetting>();
        foreach (var cat in categories)
        {
            settings.AddRange(await _settings.GetByCategoryAsync(cat));
        }
        
        ViewBag.Title = "Integration Services Configuration";
        return View("CategoryGroup", settings.GroupBy(s => s.Category).ToList());
    }

    /// <summary>
    /// Security settings
    /// </summary>
    [HttpGet("Security")]
    public async Task<IActionResult> Security()
    {
        var settings = await _settings.GetByCategoryAsync(SettingCategories.Security);
        ViewBag.Title = "Security Settings";
        return View("Category", settings);
    }

    /// <summary>
    /// Email/SMTP settings
    /// </summary>
    [HttpGet("Email")]
    public async Task<IActionResult> Email()
    {
        var settings = await _settings.GetByCategoryAsync(SettingCategories.SMTP);
        ViewBag.Title = "Email Configuration";
        return View("Category", settings);
    }

    /// <summary>
    /// Azure settings (Blob, KeyVault, Graph)
    /// </summary>
    [HttpGet("Azure")]
    public async Task<IActionResult> Azure()
    {
        var categories = new[] { 
            SettingCategories.Azure, 
            SettingCategories.AzureBlob, 
            SettingCategories.AzureKeyVault,
            SettingCategories.MicrosoftGraph
        };
        
        var settings = new List<SystemSetting>();
        foreach (var cat in categories)
        {
            settings.AddRange(await _settings.GetByCategoryAsync(cat));
        }
        
        ViewBag.Title = "Azure Services Configuration";
        return View("CategoryGroup", settings.GroupBy(s => s.Category).ToList());
    }

    /// <summary>
    /// Update a single setting value
    /// </summary>
    [HttpPost("Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromForm] string key, [FromForm] string? value)
    {
        try
        {
            var user = User.Identity?.Name ?? "System";
            await _settings.SetValueAsync(key, value, user);
            TempData["Success"] = $"Setting '{key}' updated successfully.";
            _logger.LogInformation("Setting {Key} updated by {User}", key, user);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to update setting: {ex.Message}";
            _logger.LogError(ex, "Failed to update setting {Key}", key);
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Bulk update multiple settings
    /// </summary>
    [HttpPost("BulkUpdate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkUpdate([FromForm] Dictionary<string, string?> settings, [FromForm] string? returnUrl)
    {
        try
        {
            var user = User.Identity?.Name ?? "System";
            await _settings.BulkUpdateAsync(settings, user);
            TempData["Success"] = $"Successfully updated {settings.Count} settings.";
            _logger.LogInformation("{Count} settings updated by {User}", settings.Count, user);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to update settings: {ex.Message}";
            _logger.LogError(ex, "Failed to bulk update settings");
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Toggle a boolean setting on/off
    /// </summary>
    [HttpPost("Toggle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle([FromForm] string key)
    {
        try
        {
            var currentValue = await _settings.GetValueAsync(key);
            var newValue = currentValue?.Equals("true", StringComparison.OrdinalIgnoreCase) == true ? "false" : "true";
            
            var user = User.Identity?.Name ?? "System";
            await _settings.SetValueAsync(key, newValue, user);
            
            TempData["Success"] = $"Setting '{key}' toggled to {newValue}.";
            _logger.LogInformation("Setting {Key} toggled to {Value} by {User}", key, newValue, user);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to toggle setting: {ex.Message}";
            _logger.LogError(ex, "Failed to toggle setting {Key}", key);
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Initialize default settings from environment variables
    /// </summary>
    [HttpPost("Initialize")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Initialize()
    {
        try
        {
            await _settings.InitializeDefaultsAsync();
            TempData["Success"] = "Default settings initialized successfully.";
            _logger.LogInformation("System settings initialized by {User}", User.Identity?.Name);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to initialize settings: {ex.Message}";
            _logger.LogError(ex, "Failed to initialize settings");
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Export all settings to JSON
    /// </summary>
    [HttpGet("Export")]
    public async Task<IActionResult> Export()
    {
        try
        {
            var json = await _settings.ExportToJsonAsync();
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            return File(bytes, "application/json", $"system-settings-{timestamp}.json");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to export settings: {ex.Message}";
            _logger.LogError(ex, "Failed to export settings");
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Import settings from JSON file
    /// </summary>
    [HttpPost("Import")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a JSON file to import.";
                return RedirectToAction("Index");
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var json = await reader.ReadToEndAsync();
            
            var user = User.Identity?.Name ?? "System";
            await _settings.ImportFromJsonAsync(json, user);
            
            TempData["Success"] = "Settings imported successfully.";
            _logger.LogInformation("Settings imported by {User} from file {FileName}", user, file.FileName);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to import settings: {ex.Message}";
            _logger.LogError(ex, "Failed to import settings");
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// API: Get all settings as JSON
    /// </summary>
    [HttpGet("api/all")]
    public async Task<IActionResult> GetAllApi()
    {
        var settings = await _settings.GetAllAsync();
        // Mask sensitive values
        var result = settings.Select(s => new
        {
            s.Id,
            s.Category,
            s.Key,
            Value = s.IsSensitive ? "********" : s.Value,
            s.DisplayName,
            s.Description,
            s.DataType,
            s.IsEnabled,
            s.IsSensitive,
            s.ModifiedAt,
            s.ModifiedBy
        });
        return Json(result);
    }

    /// <summary>
    /// API: Get setting by key
    /// </summary>
    [HttpGet("api/get/{key}")]
    public async Task<IActionResult> GetApi(string key)
    {
        var setting = await _settings.GetSettingAsync(key);
        if (setting == null)
            return NotFound(new { error = "Setting not found", key });

        return Json(new
        {
            setting.Key,
            Value = setting.IsSensitive ? "********" : setting.Value,
            setting.DisplayName,
            setting.Description,
            setting.IsEnabled
        });
    }

    /// <summary>
    /// API: Update setting
    /// </summary>
    [HttpPost("api/update")]
    public async Task<IActionResult> UpdateApi([FromBody] SettingUpdateRequest request)
    {
        try
        {
            var user = User.Identity?.Name ?? "System";
            await _settings.SetValueAsync(request.Key, request.Value, user);
            return Ok(new { success = true, message = "Setting updated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    private string GetCategoryDisplayName(string category)
    {
        return category switch
        {
            "AI" => "AI Services",
            "Claude" => "Claude AI",
            "OpenAI" => "OpenAI",
            "AzureOpenAI" => "Azure OpenAI",
            "Gemini" => "Google Gemini",
            "LocalLlm" => "Local LLM (Ollama)",
            "Copilot" => "Microsoft Copilot",
            "SMTP" => "Email Configuration",
            "Security" => "Security Settings",
            "Camunda" => "Camunda BPM",
            "Kafka" => "Apache Kafka",
            "RabbitMQ" => "RabbitMQ",
            "Redis" => "Redis Cache",
            "Hangfire" => "Hangfire Jobs",
            "Azure" => "Azure Services",
            "AzureBlob" => "Azure Blob Storage",
            "AzureKeyVault" => "Azure Key Vault",
            "MicrosoftGraph" => "Microsoft Graph API",
            _ => category
        };
    }
}

public class SettingsCategoryViewModel
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public int EnabledCount { get; set; }
    public List<SystemSetting> Settings { get; set; } = new();
}

public class SettingUpdateRequest
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
}
