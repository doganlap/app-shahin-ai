using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessments;
using Grc.Enums;
using Grc.FrameworkLibrary.Domain.Frameworks;
using Grc.FrameworkLibrary.Domain.Regulators;
using Grc.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Grc.Assessment.Application.Assessments;

/// <summary>
/// Service to auto-generate assessments from organization profile
/// </summary>
public class AssessmentTemplateGenerator
{
    private readonly IFrameworkRepository _frameworkRepository;
    private readonly IRegulatorRepository _regulatorRepository;
    private readonly IRepository<GrcTenant, Guid> _tenantRepository;

    public AssessmentTemplateGenerator(
        IFrameworkRepository frameworkRepository,
        IRegulatorRepository regulatorRepository,
        IRepository<GrcTenant, Guid> tenantRepository)
    {
        _frameworkRepository = frameworkRepository;
        _regulatorRepository = regulatorRepository;
        _tenantRepository = tenantRepository;
    }

    /// <summary>
    /// Generate assessment with applicable frameworks based on organization profile
    /// </summary>
    public async Task<Assessment> GenerateAssessmentAsync(
        Guid tenantId,
        string name,
        AssessmentType type,
        DateTime startDate,
        DateTime targetEndDate,
        bool includeOptionalFrameworks = false)
    {
        var tenant = await _tenantRepository.GetAsync(t => t.TenantId == tenantId);
        var applicableFrameworks = await DetermineApplicableFrameworksAsync(tenant, includeOptionalFrameworks);

        var assessment = new Assessment(
            Guid.NewGuid(),
            name,
            type,
            startDate,
            targetEndDate)
        {
            TenantId = tenantId
        };

        // Add frameworks to assessment
        foreach (var framework in applicableFrameworks)
        {
            var assessmentFramework = new AssessmentFramework(
                Guid.NewGuid(),
                assessment.Id,
                framework.Id,
                framework.IsMandatory);
            assessment.Frameworks.Add(assessmentFramework);

            // Add all controls from framework
            var controls = await _frameworkRepository.GetQueryableAsync();
            var frameworkControls = controls.Where(c => c.FrameworkId == framework.Id).ToList();
            
            foreach (var control in frameworkControls)
            {
                assessment.AddControlAssessment(control.Id);
            }
        }

        return assessment;
    }

    /// <summary>
    /// Determine applicable frameworks based on organization profile
    /// </summary>
    private async Task<List<Framework>> DetermineApplicableFrameworksAsync(
        GrcTenant tenant,
        bool includeOptionalFrameworks)
    {
        var applicableFrameworks = new List<Framework>();
        var allFrameworks = await _frameworkRepository.GetActiveFrameworksAsync();

        // 1. Universal frameworks (apply to all organizations)
        var ncaEcc = allFrameworks.FirstOrDefault(f => f.Code == "NCA-ECC");
        if (ncaEcc != null)
        {
            applicableFrameworks.Add(ncaEcc);
        }

        // 2. Sector-based frameworks
        switch (tenant.IndustrySector)
        {
            case IndustrySector.Banking:
            case IndustrySector.Insurance:
            case IndustrySector.CapitalMarkets:
                var samaCsf = allFrameworks.FirstOrDefault(f => f.Code == "SAMA-CSF");
                if (samaCsf != null) applicableFrameworks.Add(samaCsf);
                break;

            case IndustrySector.Healthcare:
            case IndustrySector.Pharmaceutical:
                var mohFramework = allFrameworks.FirstOrDefault(f => 
                    f.Code.StartsWith("MOH-") || f.Code.StartsWith("SFDA-"));
                if (mohFramework != null) applicableFrameworks.Add(mohFramework);
                break;
        }

        // 3. Data processing frameworks
        if (tenant.DataTypesProcessed != null && tenant.DataTypesProcessed.Any())
        {
            var pdpl = allFrameworks.FirstOrDefault(f => f.Code == "PDPL");
            if (pdpl != null) applicableFrameworks.Add(pdpl);
        }

        // 4. Payment processing frameworks
        if (tenant.ProcessesPayments)
        {
            var pciDss = allFrameworks.FirstOrDefault(f => f.Code == "PCI-DSS");
            if (pciDss != null) applicableFrameworks.Add(pciDss);
        }

        // 5. Listed company frameworks
        if (tenant.LegalEntityType == LegalEntityType.Listed)
        {
            var cmaFramework = allFrameworks.FirstOrDefault(f => f.Code.StartsWith("CMA-"));
            if (cmaFramework != null) applicableFrameworks.Add(cmaFramework);
        }

        // 6. Optional frameworks (ISO, NIST, etc.)
        if (includeOptionalFrameworks)
        {
            var iso27001 = allFrameworks.FirstOrDefault(f => f.Code == "ISO27001");
            if (iso27001 != null) applicableFrameworks.Add(iso27001);

            var nistCsf = allFrameworks.FirstOrDefault(f => f.Code == "NIST-CSF");
            if (nistCsf != null) applicableFrameworks.Add(nistCsf);
        }

        // Remove duplicates
        return applicableFrameworks.Distinct().ToList();
    }
}

