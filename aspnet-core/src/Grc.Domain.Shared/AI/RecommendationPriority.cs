namespace Grc.AI;

/// <summary>
/// أولوية التوصية - Recommendation Priority
/// </summary>
public enum RecommendationPriority
{
    /// <summary>
    /// حرج - Critical (requires immediate action)
    /// </summary>
    Critical = 1,
    
    /// <summary>
    /// عالي - High (should be addressed soon)
    /// </summary>
    High = 2,
    
    /// <summary>
    /// متوسط - Medium (important but not urgent)
    /// </summary>
    Medium = 3,
    
    /// <summary>
    /// منخفض - Low (nice to have)
    /// </summary>
    Low = 4,
    
    /// <summary>
    /// معلوماتي - Informational (for awareness)
    /// </summary>
    Informational = 5
}
