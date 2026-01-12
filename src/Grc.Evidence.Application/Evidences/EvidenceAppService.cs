using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Grc.Enums;
using Grc.Evidence.Domain.Evidences;
using Grc.Permissions;
using Grc.Product.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace Grc.Evidence.Application.Evidences;

/// <summary>
/// Application service for Evidence operations
/// </summary>
[Authorize(GrcPermissions.Evidence.Default)]
public class EvidenceAppService : ApplicationService, IEvidenceAppService
{
    private readonly IEvidenceRepository _evidenceRepository;
    private readonly IBlobContainer _blobContainer;
    private readonly QuotaEnforcementService _quotaEnforcementService;
    private const string ContainerName = "grc-evidence";

    public EvidenceAppService(
        IEvidenceRepository evidenceRepository,
        IBlobContainerFactory blobContainerFactory,
        QuotaEnforcementService quotaEnforcementService)
    {
        _evidenceRepository = evidenceRepository;
        _blobContainer = blobContainerFactory.Create(ContainerName);
        _quotaEnforcementService = quotaEnforcementService;
    }

    [Authorize(GrcPermissions.Evidence.Upload)]
    public async Task<EvidenceDto> UploadAsync(Guid? controlAssessmentId, IFormFile file, string description = null)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required", nameof(file));
        }

        var currentUserId = CurrentUser.Id ?? throw new UnauthorizedAccessException();
        var tenantId = CurrentTenant.Id;
        
        // Check quota for evidence storage (in MB)
        if (tenantId.HasValue)
        {
            var fileSizeMB = file.Length / (1024.0 * 1024.0);
            var allowed = await _quotaEnforcementService.CheckQuotaAsync(
                tenantId.Value,
                QuotaType.EvidenceStorageMB,
                (decimal)fileSizeMB);
            
            if (!allowed)
            {
                throw new InvalidOperationException("Evidence storage quota exceeded. Please upgrade your subscription or delete old evidence.");
            }
        }

        // Generate unique blob name
        var fileExtension = Path.GetExtension(file.FileName);
        var blobName = $"{tenantId}/{Guid.NewGuid()}{fileExtension}";

        // Upload to MinIO
        using var stream = file.OpenReadStream();
        await _blobContainer.SaveAsync(blobName, stream, true);

        // Calculate file hash
        stream.Position = 0;
        byte[] fileBytes;
        using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }
        
        // Reserve quota after successful upload
        if (tenantId.HasValue)
        {
            var fileSizeMB = file.Length / (1024.0 * 1024.0);
            await _quotaEnforcementService.ReserveQuotaAsync(
                tenantId.Value,
                QuotaType.EvidenceStorageMB,
                (decimal)fileSizeMB);
        }

        // Create evidence entity
        var evidence = new Evidence.Evidence(
            GuidGenerator.Create(),
            currentUserId,
            file.FileName,
            blobName,
            ContainerName,
            file.Length,
            file.ContentType);

        evidence.TenantId = tenantId;
        
        if (controlAssessmentId.HasValue)
        {
            evidence.LinkToControlAssessment(controlAssessmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            evidence.SetDescription(description);
        }

        evidence.ComputeHash(fileBytes);

        await _evidenceRepository.InsertAsync(evidence);

        // TODO: Trigger AI classification in background job
        // TODO: Trigger OCR extraction in background job

        return ObjectMapper.Map<Evidence.Evidence, EvidenceDto>(evidence);
    }

    [Authorize(GrcPermissions.Evidence.View)]
    public async Task<EvidenceDto> GetAsync(Guid id)
    {
        var evidence = await _evidenceRepository.GetAsync(id);
        return ObjectMapper.Map<Evidence.Evidence, EvidenceDto>(evidence);
    }

    [Authorize(GrcPermissions.Evidence.Download)]
    public async Task<byte[]> DownloadAsync(Guid id)
    {
        var evidence = await _evidenceRepository.GetAsync(id);
        
        using var stream = await _blobContainer.GetAsync(evidence.BlobName);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        
        return memoryStream.ToArray();
    }

    [Authorize(GrcPermissions.Evidence.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var evidence = await _evidenceRepository.GetAsync(id);
        
        // Delete from blob storage
        await _blobContainer.DeleteAsync(evidence.BlobName);
        
        // Soft delete entity
        await _evidenceRepository.DeleteAsync(evidence);
    }
}

