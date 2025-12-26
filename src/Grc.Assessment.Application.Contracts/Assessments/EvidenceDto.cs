using System;
using Grc.Enums;

namespace Grc.Assessments;

/// <summary>
/// Evidence DTO
/// </summary>
public class EvidenceDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string MimeType { get; set; }
    public EvidenceType? EvidenceType { get; set; }
    public string Description { get; set; }
    public DateTime UploadTime { get; set; }
    public Guid UploadedByUserId { get; set; }
    public int VersionNumber { get; set; }
    public bool IsCurrentVersion { get; set; }
}

