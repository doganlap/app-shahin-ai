using System.Threading.Tasks;
using GrcMvc.Models.DTOs;

namespace GrcMvc.Services.Interfaces
{
    /// <summary>
    /// Service for detecting fraudulent tenant creation attempts based on device fingerprints and patterns
    /// </summary>
    public interface IFingerprintFraudDetector
    {
        /// <summary>
        /// Analyzes a tenant creation request for suspicious patterns
        /// </summary>
        /// <param name="request">The tenant creation request to analyze</param>
        /// <returns>Fraud check result indicating if suspicious and whether to block</returns>
        Task<FraudCheckResult> CheckAsync(TenantCreationFacadeRequest request);
    }

    /// <summary>
    /// Result of fraud detection analysis
    /// </summary>
    public class FraudCheckResult
    {
        /// <summary>
        /// Whether the request shows suspicious patterns
        /// </summary>
        public bool IsSuspicious { get; set; }

        /// <summary>
        /// Whether the request should be blocked entirely
        /// </summary>
        public bool ShouldBlock { get; set; }

        /// <summary>
        /// Reason for flagging as suspicious
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Risk score (0.0 = no risk, 1.0 = maximum risk)
        /// </summary>
        public double RiskScore { get; set; }
    }
}
