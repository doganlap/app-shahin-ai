using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.Assessment.Domain.Tools;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Grc.Assessment.Domain.Data;

public class ToolDataSeeder : ITransientDependency
{
    private readonly IRepository<AssessmentTool, Guid> _toolRepository;

    public ToolDataSeeder(IRepository<AssessmentTool, Guid> toolRepository)
    {
        _toolRepository = toolRepository;
    }

    [UnitOfWork]
    public virtual async Task<SeedResult> SeedAsync()
    {
        var result = new SeedResult();
        var existingTools = (await _toolRepository.GetListAsync()).Select(t => t.Name).ToHashSet();
        var tools = new List<AssessmentTool>();

        var toolData = new List<(string Name, ToolCategory Category, string Vendor, string Description, string Version)>
        {
            ("Nessus Professional", ToolCategory.VulnerabilityScanner, "Tenable", "Industry-leading vulnerability scanner for identifying security weaknesses", "10.6"),
            ("Qualys VMDR", ToolCategory.VulnerabilityScanner, "Qualys", "Cloud-based vulnerability management, detection and response platform", "2024.1"),
            ("ServiceNow GRC", ToolCategory.GRCPlatform, "ServiceNow", "Integrated GRC platform for risk and compliance management", "Vancouver"),
            ("RSA Archer", ToolCategory.GRCPlatform, "RSA Security", "Enterprise GRC platform for managing risk, compliance and audit", "6.14"),
            ("Splunk Enterprise Security", ToolCategory.SIEM, "Splunk", "Security information and event management (SIEM) solution", "7.3"),
            ("OneTrust", ToolCategory.ComplianceManager, "OneTrust", "Privacy and compliance automation platform", "2024"),
            ("Metasploit Pro", ToolCategory.PenetrationTesting, "Rapid7", "Comprehensive penetration testing and exploitation framework", "4.22"),
            ("Burp Suite Professional", ToolCategory.PenetrationTesting, "PortSwigger", "Web application security testing platform", "2024.1"),
            ("LogicManager", ToolCategory.RiskManagement, "LogicManager", "Enterprise risk management and governance platform", "2024"),
            ("Tenable.io", ToolCategory.AssetManagement, "Tenable", "Cloud-based asset discovery and vulnerability management", "2024")
        };

        foreach (var (name, category, vendor, description, version) in toolData)
        {
            if (!existingTools.Contains(name))
            {
                var tool = new AssessmentTool(Guid.NewGuid(), name, category, vendor);
                tool.SetDescription(description);
                tool.SetVersion(version);
                tools.Add(tool);
                result.Inserted++;
            }
            else
            {
                result.Skipped++;
            }
            result.Total++;
        }

        if (tools.Any())
        {
            await _toolRepository.InsertManyAsync(tools);
        }
        
        return result;
    }
}
