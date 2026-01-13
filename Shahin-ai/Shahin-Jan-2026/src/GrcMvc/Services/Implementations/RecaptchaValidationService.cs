using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GrcMvc.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Service for validating Google reCAPTCHA v3 tokens
    /// </summary>
    public class RecaptchaValidationService : IRecaptchaValidationService, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecaptchaValidationService> _logger;

        private const string GoogleRecaptchaVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

        public RecaptchaValidationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<RecaptchaValidationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Validates a reCAPTCHA token with Google's API
        /// </summary>
        public async Task<bool> ValidateAsync(string token, string? ipAddress = null)
        {
            var result = await ValidateWithScoreAsync(token, ipAddress);
            return result.Success && result.MeetsThreshold;
        }

        /// <summary>
        /// Validates a reCAPTCHA token and returns detailed result with score
        /// </summary>
        public async Task<RecaptchaValidationResult> ValidateWithScoreAsync(string token, string? ipAddress = null)
        {
            // Check if reCAPTCHA is enabled
            var enabled = _configuration.GetValue<bool>("Recaptcha:Enabled", true);
            if (!enabled)
            {
                _logger.LogWarning("Recaptcha: Validation bypassed (disabled in configuration)");
                return new RecaptchaValidationResult
                {
                    Success = true,
                    Score = 1.0,
                    MeetsThreshold = true,
                    ErrorMessage = "reCAPTCHA validation is disabled"
                };
            }

            // Validate token is provided
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Recaptcha: Validation failed - no token provided");
                return new RecaptchaValidationResult
                {
                    Success = false,
                    Score = 0.0,
                    MeetsThreshold = false,
                    ErrorMessage = "No reCAPTCHA token provided"
                };
            }

            // Get secret key from configuration
            var secretKey = _configuration["Recaptcha:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                _logger.LogError("Recaptcha: SecretKey not configured in appsettings.json");
                throw new InvalidOperationException("reCAPTCHA SecretKey is not configured");
            }

            try
            {
                // Call Google's reCAPTCHA verification API
                var httpClient = _httpClientFactory.CreateClient();

                var requestData = new Dictionary<string, string>
                {
                    { "secret", secretKey },
                    { "response", token }
                };

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    requestData.Add("remoteip", ipAddress);
                }

                var response = await httpClient.PostAsync(
                    GoogleRecaptchaVerifyUrl,
                    new FormUrlEncodedContent(requestData)
                );

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Recaptcha: Google API returned status {StatusCode}", response.StatusCode);
                    return new RecaptchaValidationResult
                    {
                        Success = false,
                        Score = 0.0,
                        MeetsThreshold = false,
                        ErrorMessage = $"reCAPTCHA API error: {response.StatusCode}"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var googleResponse = JsonSerializer.Deserialize<GoogleRecaptchaResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (googleResponse == null)
                {
                    _logger.LogError("Recaptcha: Failed to parse Google API response");
                    return new RecaptchaValidationResult
                    {
                        Success = false,
                        Score = 0.0,
                        MeetsThreshold = false,
                        ErrorMessage = "Failed to parse reCAPTCHA response"
                    };
                }

                // Get minimum score threshold from configuration (default 0.5)
                var minimumScore = _configuration.GetValue<double>("Recaptcha:MinimumScore", 0.5);
                var meetsThreshold = googleResponse.Score >= minimumScore;

                var result = new RecaptchaValidationResult
                {
                    Success = googleResponse.Success,
                    Score = googleResponse.Score,
                    Action = googleResponse.Action,
                    ChallengeTimestamp = googleResponse.ChallengeTs,
                    Hostname = googleResponse.Hostname,
                    ErrorCodes = googleResponse.ErrorCodes,
                    MeetsThreshold = meetsThreshold
                };

                if (!googleResponse.Success)
                {
                    result.ErrorMessage = $"reCAPTCHA validation failed: {string.Join(", ", googleResponse.ErrorCodes ?? Array.Empty<string>())}";
                    _logger.LogWarning("Recaptcha: Validation failed - Success=false, ErrorCodes={ErrorCodes}",
                        string.Join(", ", googleResponse.ErrorCodes ?? Array.Empty<string>()));
                }
                else if (!meetsThreshold)
                {
                    result.ErrorMessage = $"reCAPTCHA score {googleResponse.Score:F2} below threshold {minimumScore:F2}";
                    _logger.LogWarning("Recaptcha: Score {Score:F2} below threshold {Threshold:F2} - IP={IP}",
                        googleResponse.Score, minimumScore, ipAddress);
                }
                else
                {
                    _logger.LogInformation("Recaptcha: Validation successful - Score={Score:F2}, IP={IP}",
                        googleResponse.Score, ipAddress);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Recaptcha: HTTP request to Google API failed");
                return new RecaptchaValidationResult
                {
                    Success = false,
                    Score = 0.0,
                    MeetsThreshold = false,
                    ErrorMessage = "Failed to connect to reCAPTCHA service"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recaptcha: Unexpected error during validation");
                return new RecaptchaValidationResult
                {
                    Success = false,
                    Score = 0.0,
                    MeetsThreshold = false,
                    ErrorMessage = "Unexpected error during reCAPTCHA validation"
                };
            }
        }

        /// <summary>
        /// Google reCAPTCHA API response structure
        /// </summary>
        private class GoogleRecaptchaResponse
        {
            public bool Success { get; set; }
            public double Score { get; set; }
            public string? Action { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("challenge_ts")]
            public string? ChallengeTs { get; set; }

            public string? Hostname { get; set; }

            [System.Text.Json.Serialization.JsonPropertyName("error-codes")]
            public string[]? ErrorCodes { get; set; }
        }
    }
}
