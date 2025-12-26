using System.Collections.Generic;

namespace Grc.Assessments;

/// <summary>
/// Result of bulk operation
/// </summary>
public class BulkOperationResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<BulkOperationError> Errors { get; set; }
}

/// <summary>
/// Error from bulk operation
/// </summary>
public class BulkOperationError
{
    public Guid Id { get; set; }
    public string Error { get; set; }
}

