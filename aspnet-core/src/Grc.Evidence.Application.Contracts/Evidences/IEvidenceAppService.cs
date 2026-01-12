using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;

namespace Grc.Evidence;

/// <summary>
/// Application service interface for Evidence operations
/// </summary>
public interface IEvidenceAppService : IApplicationService
{
    Task<EvidenceDto> UploadAsync(Guid? controlAssessmentId, IFormFile file, string description = null);
    Task<EvidenceDto> GetAsync(Guid id);
    Task<byte[]> DownloadAsync(Guid id);
    Task DeleteAsync(Guid id);
}

