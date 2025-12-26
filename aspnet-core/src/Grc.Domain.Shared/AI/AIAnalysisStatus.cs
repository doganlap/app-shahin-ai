namespace Grc.AI;

/// <summary>
/// حالة التحليل الذكي - AI Analysis Status
/// </summary>
public enum AIAnalysisStatus
{
    /// <summary>
    /// قيد الانتظار - Pending (waiting to start)
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// قيد التشغيل - Running (currently analyzing)
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// مكتمل - Completed (analysis finished successfully)
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// فشل - Failed (analysis encountered errors)
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// ملغي - Cancelled (manually cancelled)
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// قيد المراجعة - Under Review (awaiting human validation)
    /// </summary>
    UnderReview = 5,
    
    /// <summary>
    /// معتمد - Approved (validated and approved)
    /// </summary>
    Approved = 6
}
