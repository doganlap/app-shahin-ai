using System;
using System.Collections.Generic;

namespace Grc.Assessments;

/// <summary>
/// History/audit log DTO
/// </summary>
public class HistoryDto
{
    public string Action { get; set; }
    public string Details { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> OldValues { get; set; }
    public Dictionary<string, object> NewValues { get; set; }
}

