using System;
using System.Linq;
using System.Threading.Tasks;
using GrcMvc.Data;
using GrcMvc.Models.DTOs;
using GrcMvc.Models.Entities;
using GrcMvc.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace GrcMvc.Services.Implementations
{
    /// <summary>
    /// Detects fraudulent tenant creation attempts by analyzing device fingerprints and patterns
    /// </summary>
    public class FingerprintFraudDetector : IFingerprintFraudDetector, ITransientDependency
    {
        private readonly GrcDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FingerprintFraudDetector> _logger;

        public FingerprintFraudDetector(
            GrcDbContext dbContext,
            IConfiguration configuration,
            ILogger<FingerprintFraudDetector> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Analyzes tenant creation request for suspicious patterns
        /// </summary>
        public async Task<FraudCheckResult> CheckAsync(TenantCreationFacadeRequest request)
        {
            var result = new FraudCheckResult
            {
                IsSuspicious = false,
                ShouldBlock = false,
                RiskScore = 0.0
            };

            // Check if fraud detection is enabled
            var enabled = _configuration.GetValue<bool>("FraudDetection:Enabled", true);
            if (!enabled)
            {
                _logger.LogInformation("FraudDetector: Fraud detection disabled in configuration");
                return result;
            }

            // Get thresholds from configuration
            var maxTenantsPerIPPerHour = _configuration.GetValue<int>("FraudDetection:MaxTenantsPerIPPerHour", 3);
            var maxTenantsPerDeviceIdPerDay = _configuration.GetValue<int>("FraudDetection:MaxTenantsPerDeviceIdPerDay", 2);
            var minIntervalBetweenCreationsSeconds = _configuration.GetValue<int>("FraudDetection:MinIntervalBetweenCreationsSeconds", 60);
            var blockThreshold = _configuration.GetValue<double>("FraudDetection:BlockThresholdScore", 0.8);

            var now = DateTime.UtcNow;
            var oneHourAgo = now.AddHours(-1);
            var oneDayAgo = now.AddDays(-1);

            // Check 1: IP address abuse (same IP creating multiple tenants)
            if (!string.IsNullOrEmpty(request.IpAddress))
            {
                var ipCreationCount = await _dbContext.TenantCreationFingerprints
                    .Where(f => f.IpAddress == request.IpAddress && f.CreatedAt >= oneHourAgo)
                    .CountAsync();

                if (ipCreationCount >= maxTenantsPerIPPerHour)
                {
                    result.IsSuspicious = true;
                    result.RiskScore += 0.4;
                    result.Reason = $"IP address {request.IpAddress} created {ipCreationCount} tenants in last hour";
                    _logger.LogWarning("FraudDetector: IP abuse detected - IP={IP}, Count={Count}",
                        request.IpAddress, ipCreationCount);
                }
            }

            // Check 2: Device fingerprint abuse (same device creating multiple tenants)
            if (!string.IsNullOrEmpty(request.DeviceFingerprint))
            {
                var deviceCreationCount = await _dbContext.TenantCreationFingerprints
                    .Where(f => f.DeviceId == request.DeviceFingerprint && f.CreatedAt >= oneDayAgo)
                    .CountAsync();

                if (deviceCreationCount >= maxTenantsPerDeviceIdPerDay)
                {
                    result.IsSuspicious = true;
                    result.RiskScore += 0.4;
                    var existingReason = result.Reason ?? "";
                    result.Reason = string.IsNullOrEmpty(existingReason)
                        ? $"Device created {deviceCreationCount} tenants in last 24 hours"
                        : $"{existingReason}; Device created {deviceCreationCount} tenants in last 24 hours";
                    _logger.LogWarning("FraudDetector: Device abuse detected - DeviceId={DeviceId}, Count={Count}",
                        request.DeviceFingerprint, deviceCreationCount);
                }
            }

            // Check 3: Rapid-fire creation (too fast between requests from same IP or device)
            if (!string.IsNullOrEmpty(request.IpAddress) || !string.IsNullOrEmpty(request.DeviceFingerprint))
            {
                var recentCreation = await _dbContext.TenantCreationFingerprints
                    .Where(f => (f.IpAddress == request.IpAddress || f.DeviceId == request.DeviceFingerprint))
                    .OrderByDescending(f => f.CreatedAt)
                    .FirstOrDefaultAsync();

                if (recentCreation != null)
                {
                    var timeSinceLastCreation = (now - recentCreation.CreatedAt).TotalSeconds;
                    if (timeSinceLastCreation < minIntervalBetweenCreationsSeconds)
                    {
                        result.IsSuspicious = true;
                        result.RiskScore += 0.3;
                        var existingReason = result.Reason ?? "";
                        result.Reason = string.IsNullOrEmpty(existingReason)
                            ? $"Creation attempted {timeSinceLastCreation:F0} seconds after previous (minimum {minIntervalBetweenCreationsSeconds}s)"
                            : $"{existingReason}; Rapid-fire creation ({timeSinceLastCreation:F0}s interval)";
                        _logger.LogWarning("FraudDetector: Rapid-fire creation detected - Interval={Interval}s",
                            timeSinceLastCreation);
                    }
                }
            }

            // Check 4: Missing security fields (suspicious if no fingerprint or IP)
            if (string.IsNullOrEmpty(request.DeviceFingerprint) && string.IsNullOrEmpty(request.IpAddress))
            {
                result.IsSuspicious = true;
                result.RiskScore += 0.2;
                var existingReason = result.Reason ?? "";
                result.Reason = string.IsNullOrEmpty(existingReason)
                    ? "Missing device fingerprint and IP address"
                    : $"{existingReason}; Missing security fields";
                _logger.LogWarning("FraudDetector: Missing security fields - TenantName={TenantName}",
                    request.TenantName);
            }

            // Determine if should block based on risk score
            result.ShouldBlock = result.RiskScore >= blockThreshold;

            if (result.IsSuspicious)
            {
                _logger.LogWarning("FraudDetector: Suspicious activity - RiskScore={RiskScore}, ShouldBlock={ShouldBlock}, Reason={Reason}",
                    result.RiskScore, result.ShouldBlock, result.Reason);
            }
            else
            {
                _logger.LogInformation("FraudDetector: No suspicious activity detected - TenantName={TenantName}",
                    request.TenantName);
            }

            return result;
        }
    }
}
