using System;
using System.Threading.Tasks;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Theme service implementation for managing user theme preferences
    /// Stores preferences in database with caching for performance
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ThemeService> _logger;
        private readonly string _defaultTheme;
        private readonly string[] _availableThemes = { "light", "dark" };
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public ThemeService(
            UserManager<ApplicationUser> userManager,
            IMemoryCache cache,
            ILogger<ThemeService> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _cache = cache;
            _logger = logger;
            _defaultTheme = configuration.GetValue<string>("Theme:Default") ?? "dark";
        }

        public string DefaultTheme => _defaultTheme;

        public string[] AvailableThemes => _availableThemes;

        public bool IsValidTheme(string theme)
        {
            return Array.Exists(_availableThemes, t => t.Equals(theme, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get the current theme for a user
        /// Uses cache-first strategy with database fallback
        /// </summary>
        public async Task<string> GetUserThemeAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return _defaultTheme;
            }

            var cacheKey = $"user_theme_{userId}";
            
            if (_cache.TryGetValue(cacheKey, out string? cachedTheme) && !string.IsNullOrEmpty(cachedTheme))
            {
                return cachedTheme;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.PreferredTheme))
                {
                    _cache.Set(cacheKey, user.PreferredTheme, _cacheExpiration);
                    return user.PreferredTheme;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve theme preference for user {UserId}", userId);
            }

            return _defaultTheme;
        }

        /// <summary>
        /// Set the theme for a user and persist to database
        /// </summary>
        public async Task SetUserThemeAsync(string userId, string theme)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (!IsValidTheme(theme))
            {
                throw new ArgumentException($"Invalid theme: {theme}. Must be one of: {string.Join(", ", _availableThemes)}", nameof(theme));
            }

            var normalizedTheme = theme.ToLowerInvariant();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.PreferredTheme = normalizedTheme;
                    user.ModifiedDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Update cache
                    var cacheKey = $"user_theme_{userId}";
                    _cache.Set(cacheKey, normalizedTheme, _cacheExpiration);

                    _logger.LogInformation("Theme preference updated for user {UserId}: {Theme}", userId, normalizedTheme);
                }
                else
                {
                    _logger.LogWarning("User {UserId} not found when trying to set theme preference", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save theme preference for user {UserId}", userId);
                throw;
            }
        }
    }
}
