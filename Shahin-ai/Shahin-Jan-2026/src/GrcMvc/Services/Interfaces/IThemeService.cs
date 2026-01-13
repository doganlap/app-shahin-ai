using System.Threading.Tasks;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service for managing user theme preferences
    /// Supports light/dark mode switching with persistence
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Get the current theme for a user
        /// </summary>
        /// <param name="userId">The user's ID (null for anonymous)</param>
        /// <returns>Theme name: "light" or "dark"</returns>
        Task<string> GetUserThemeAsync(string? userId);

        /// <summary>
        /// Set the theme for a user
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <param name="theme">Theme name: "light" or "dark"</param>
        Task SetUserThemeAsync(string userId, string theme);

        /// <summary>
        /// Get the default theme for the application
        /// </summary>
        string DefaultTheme { get; }

        /// <summary>
        /// Get available themes
        /// </summary>
        string[] AvailableThemes { get; }

        /// <summary>
        /// Check if a theme is valid
        /// </summary>
        bool IsValidTheme(string theme);
    }
}
