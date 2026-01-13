using System.Threading.Tasks;
using GrcMvc.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrcMvc.Controllers.Api
{
    /// <summary>
    /// API controller for theme management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IThemeService _themeService;

        public ThemeController(IThemeService themeService)
        {
            _themeService = themeService;
        }

        /// <summary>
        /// Get current user's theme preference
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTheme()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var theme = await _themeService.GetUserThemeAsync(userId);
            
            return Ok(new { 
                theme = theme,
                defaultTheme = _themeService.DefaultTheme,
                availableThemes = _themeService.AvailableThemes
            });
        }

        /// <summary>
        /// Set user's theme preference
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SetTheme([FromBody] SetThemeRequest request)
        {
            if (string.IsNullOrEmpty(request?.Theme))
            {
                return BadRequest(new { error = "Theme is required" });
            }

            if (!_themeService.IsValidTheme(request.Theme))
            {
                return BadRequest(new { 
                    error = $"Invalid theme. Must be one of: {string.Join(", ", _themeService.AvailableThemes)}" 
                });
            }

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            await _themeService.SetUserThemeAsync(userId, request.Theme);
            
            return Ok(new { 
                success = true, 
                theme = request.Theme,
                message = "Theme preference saved"
            });
        }

        /// <summary>
        /// Toggle between light and dark mode
        /// </summary>
        [HttpPost("toggle")]
        [Authorize]
        public async Task<IActionResult> ToggleTheme()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var currentTheme = await _themeService.GetUserThemeAsync(userId);
            var newTheme = currentTheme == "dark" ? "light" : "dark";
            
            await _themeService.SetUserThemeAsync(userId, newTheme);
            
            return Ok(new { 
                success = true, 
                previousTheme = currentTheme,
                theme = newTheme,
                message = $"Theme switched to {newTheme}"
            });
        }
    }

    public class SetThemeRequest
    {
        public string Theme { get; set; } = string.Empty;
    }
}
