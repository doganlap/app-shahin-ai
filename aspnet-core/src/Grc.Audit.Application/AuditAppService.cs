using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.Audit.Domain.Audits;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Grc.Audit.Application;

/// <summary>
/// Application service for Audit operations
/// </summary>
[Authorize(GrcPermissions.Admin.ViewAuditLog)]
public class AuditAppService : ApplicationService
{
    private readonly IRepository<Audit, Guid> _auditRepository;
    private readonly IRepository<AuditFinding, Guid> _findingRepository;

    public AuditAppService(
        IRepository<Audit, Guid> auditRepository,
        IRepository<AuditFinding, Guid> findingRepository)
    {
        _auditRepository = auditRepository;
        _findingRepository = findingRepository;
    }

    public async Task<Audit> CreateAuditAsync(
        string auditCode,
        ValueObjects.LocalizedString title,
        string auditType,
        DateTime startDate,
        DateTime endDate,
        Guid leadAuditorId)
    {
        var audit = new Audit(
            GuidGenerator.Create(),
            auditCode,
            title,
            auditType,
            startDate,
            endDate,
            leadAuditorId)
        {
            TenantId = CurrentTenant.Id
        };
        
        await _auditRepository.InsertAsync(audit);
        return audit;
    }

    public async Task<AuditFinding> AddFindingAsync(
        Guid auditId,
        string findingCode,
        ValueObjects.LocalizedString title,
        Enums.RiskLevel severity)
    {
        var audit = await _auditRepository.GetAsync(auditId);
        var finding = audit.AddFinding(findingCode, title, severity);
        
        await _auditRepository.UpdateAsync(audit);
        return finding;
    }

    public async Task<List<Audit>> GetListAsync()
    {
        return await _auditRepository.GetListAsync();
    }
}

