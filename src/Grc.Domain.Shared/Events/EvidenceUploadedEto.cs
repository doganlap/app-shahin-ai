using System;
using Volo.Abp.EventBus;

namespace Grc.Events;

/// <summary>
/// Event published when evidence is uploaded
/// </summary>
[EventName("Grc.Evidence.Uploaded")]
public class EvidenceUploadedEto
{
    public Guid EvidenceId { get; set; }
    public Guid? ControlAssessmentId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime UploadTime { get; set; }
}

