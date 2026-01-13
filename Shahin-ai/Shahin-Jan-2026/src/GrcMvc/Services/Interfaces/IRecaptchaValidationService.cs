using System.Threading.Tasks;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service for validating Google reCAPTCHA v3 tokens
    /// </summary>
    public interface IRecaptchaValidationService
    {
        /// <summary>
        /// Validates a reCAPTCHA token with Google's API
        /// </summary>
        /// <param name="token">The reCAPTCHA token from the client</param>
        /// <param name="ipAddress">The IP address of the user (optional)</param>
        /// <returns>True if validation succeeds, false otherwise</returns>
        Task<bool> ValidateAsync(string token, string? ipAddress = null);

        /// <summary>
        /// Validates a reCAPTCHA token and returns detailed result with score
        /// </summary>
        /// <param name="token">The reCAPTCHA token from the client</param>
        /// <param name="ipAddress">The IP address of the user (optional)</param>
        /// <returns>Detailed validation result including score</returns>
        Task<RecaptchaValidationResult> ValidateWithScoreAsync(string token, string? ipAddress = null);
    }

    /// <summary>
    /// Detailed result of reCAPTCHA validation
    /// </summary>
    public class RecaptchaValidationResult
    {
        /// <summary>
        /// Whether the validation succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The score from Google (0.0 to 1.0, higher is more likely human)
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The action name from the reCAPTCHA widget
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Timestamp of the challenge
        /// </summary>
        public string? ChallengeTimestamp { get; set; }

        /// <summary>
        /// Hostname of the site where the reCAPTCHA was solved
        /// </summary>
        public string? Hostname { get; set; }

        /// <summary>
        /// Error codes if validation failed
        /// </summary>
        public string[]? ErrorCodes { get; set; }

        /// <summary>
        /// Whether the score meets the minimum threshold
        /// </summary>
        public bool MeetsThreshold { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
