using System;
using System.Security.Cryptography;
using System.Text.Json;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.Enums;

namespace Grc.Evidence;

/// <summary>
/// Supporting evidence document
/// </summary>
public class Evidence : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid? ControlAssessmentId { get; private set; }
    public string FileName { get; private set; }
    public string BlobName { get; private set; }
    public string ContainerName { get; private set; }
    public long FileSize { get; private set; }
    public string MimeType { get; private set; }
    public EvidenceType? EvidenceType { get; private set; }
    public string Description { get; private set; }
    public JsonDocument AiClassification { get; private set; }
    public string ExtractedText { get; private set; }
    public string HashSha256 { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public DateTime UploadTime { get; private set; }
    public int VersionNumber { get; private set; }
    public bool IsCurrentVersion { get; private set; }
    
    protected Evidence() { }
    
    public Evidence(Guid id, Guid uploadedBy, string fileName, string blobName, 
                   string containerName, long fileSize, string mimeType)
        : base(id)
    {
        UploadedByUserId = uploadedBy;
        FileName = fileName;
        BlobName = blobName;
        ContainerName = containerName;
        FileSize = fileSize;
        MimeType = mimeType;
        UploadTime = DateTime.UtcNow;
        VersionNumber = 1;
        IsCurrentVersion = true;
    }
    
    public void LinkToControlAssessment(Guid controlAssessmentId)
    {
        ControlAssessmentId = controlAssessmentId;
    }
    
    public void SetAiClassification(JsonDocument classification)
    {
        AiClassification = classification;
    }
    
    public void SetExtractedText(string text)
    {
        ExtractedText = text;
    }
    
    public void SetEvidenceType(EvidenceType evidenceType)
    {
        EvidenceType = evidenceType;
    }
    
    public void SetDescription(string description)
    {
        Description = description;
    }
    
    public void ComputeHash(byte[] fileBytes)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(fileBytes);
        HashSha256 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
    
    public void IncrementVersion()
    {
        VersionNumber++;
        IsCurrentVersion = true;
    }
    
    public void MarkAsOldVersion()
    {
        IsCurrentVersion = false;
    }
}

