using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.ActionItems;
using Grc.Assets;
using Grc.AuditFindings;
using Grc.Enums;
using Grc.Gaps;
using Grc.Organizations;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Grc.Data;

namespace Grc.Domain.Data;

public class GrcLifecycleDataSeeder : ITransientDependency
{
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly IRepository<Asset, Guid> _assetRepository;
    private readonly IRepository<Gap, Guid> _gapRepository;
    private readonly IRepository<ActionItem, Guid> _actionItemRepository;
    private readonly IRepository<Audit, Guid> _auditRepository;
    private readonly IRepository<AuditFinding, Guid> _auditFindingRepository;
    private readonly WorkflowDefinitionSeeder _workflowDefinitionSeeder;

    public GrcLifecycleDataSeeder(
        IRepository<Organization, Guid> organizationRepository,
        IRepository<Asset, Guid> assetRepository,
        IRepository<Gap, Guid> gapRepository,
        IRepository<ActionItem, Guid> actionItemRepository,
        IRepository<Audit, Guid> auditRepository,
        IRepository<AuditFinding, Guid> auditFindingRepository,
        WorkflowDefinitionSeeder workflowDefinitionSeeder)
    {
        _organizationRepository = organizationRepository;
        _assetRepository = assetRepository;
        _gapRepository = gapRepository;
        _actionItemRepository = actionItemRepository;
        _auditRepository = auditRepository;
        _auditFindingRepository = auditFindingRepository;
        _workflowDefinitionSeeder = workflowDefinitionSeeder;
    }

    public async Task<SeedResult> SeedAsync()
    {
        var result = new SeedResult();

        // Seed WorkflowDefinitions first (they are reference data)
        await _workflowDefinitionSeeder.SeedAsync(new Volo.Abp.Data.DataSeedContext());

        // Seed Organizations
        var orgResult = await SeedOrganizationsAsync();
        result.Inserted += orgResult.Inserted;

        // Seed Assets
        var assetResult = await SeedAssetsAsync();
        result.Inserted += assetResult.Inserted;

        // Seed Gaps
        var gapResult = await SeedGapsAsync();
        result.Inserted += gapResult.Inserted;

        // Seed Action Items
        var actionResult = await SeedActionItemsAsync();
        result.Inserted += actionResult.Inserted;

        // Seed Audits and Findings
        var auditResult = await SeedAuditsAsync();
        result.Inserted += auditResult.Inserted;

        return result;
    }

    private async Task<SeedResult> SeedOrganizationsAsync()
    {
        var result = new SeedResult();
        var existing = await _organizationRepository.GetCountAsync();
        if (existing > 0) return result;

        var organizations = new List<Organization>();

        // 50 Saudi Organizations
        var orgData = new[]
        {
            ("ARAMCO", "Saudi Aramco", "أرامكو السعودية", "Oil & Gas", OrganizationSize.Enterprise),
            ("STC", "Saudi Telecom Company", "الاتصالات السعودية", "Telecommunications", OrganizationSize.Enterprise),
            ("SABIC", "Saudi Basic Industries", "سابك", "Petrochemicals", OrganizationSize.Enterprise),
            ("SNB", "Saudi National Bank", "البنك الأهلي السعودي", "Banking", OrganizationSize.Enterprise),
            ("RIYADH-BANK", "Riyad Bank", "بنك الرياض", "Banking", OrganizationSize.Large),
            ("ALINMA", "Alinma Bank", "مصرف الإنماء", "Banking", OrganizationSize.Large),
            ("ALRAJHI", "Al Rajhi Bank", "مصرف الراجحي", "Banking", OrganizationSize.Enterprise),
            ("MAADEN", "Saudi Arabian Mining", "معادن", "Mining", OrganizationSize.Large),
            ("SEC", "Saudi Electricity Company", "الشركة السعودية للكهرباء", "Utilities", OrganizationSize.Enterprise),
            ("NEOM", "NEOM", "نيوم", "Development", OrganizationSize.Large),
            ("PIF", "Public Investment Fund", "صندوق الاستثمارات العامة", "Investment", OrganizationSize.Enterprise),
            ("SAUDIA", "Saudi Arabian Airlines", "الخطوط السعودية", "Aviation", OrganizationSize.Enterprise),
            ("ACWA", "ACWA Power", "أكوا باور", "Energy", OrganizationSize.Large),
            ("DALLAH", "Dallah Healthcare", "دله الصحية", "Healthcare", OrganizationSize.Large),
            ("MOUWASAT", "Mouwasat Medical Services", "موسات الطبية", "Healthcare", OrganizationSize.Medium),
            ("JARIR", "Jarir Marketing", "مكتبة جرير", "Retail", OrganizationSize.Large),
            ("EXTRA", "United Electronics (eXtra)", "إكسترا", "Retail", OrganizationSize.Medium),
            ("MOBILY", "Etihad Etisalat (Mobily)", "موبايلي", "Telecommunications", OrganizationSize.Large),
            ("ZAIN", "Zain KSA", "زين السعودية", "Telecommunications", OrganizationSize.Large),
            ("TADAWUL", "Saudi Exchange", "تداول السعودية", "Financial Services", OrganizationSize.Medium),
            ("ARAMEX", "Aramex", "أرامكس", "Logistics", OrganizationSize.Large),
            ("SPIMACO", "Saudi Pharmaceutical Industries", "سبيماكو", "Pharmaceutical", OrganizationSize.Medium),
            ("ALMARAI", "Almarai Company", "المراعي", "Food & Beverage", OrganizationSize.Enterprise),
            ("SAVOLA", "Savola Group", "مجموعة صافولا", "Food & Retail", OrganizationSize.Large),
            ("PANDA", "Panda Retail", "بنده", "Retail", OrganizationSize.Large),
            ("TASNEE", "National Industrialization", "التصنيع الوطنية", "Industrial", OrganizationSize.Large),
            ("YANBU-CEMENT", "Yanbu Cement", "اسمنت ينبع", "Manufacturing", OrganizationSize.Medium),
            ("SIPCHEM", "Saudi International Petrochemical", "سبكيم", "Petrochemicals", OrganizationSize.Medium),
            ("SADARA", "Sadara Chemical", "صدارة للكيماويات", "Petrochemicals", OrganizationSize.Large),
            ("FARABI", "Saudi Arabia Fertilizers", "الفارابي", "Chemicals", OrganizationSize.Medium),
            ("REDTAG", "Red Tag", "ريد تاج", "Retail", OrganizationSize.Medium),
            ("JABAL-OMAR", "Jabal Omar Development", "جبل عمر", "Real Estate", OrganizationSize.Large),
            ("EMAAR-ME", "Emaar, The Economic City", "إعمار المدينة الاقتصادية", "Real Estate", OrganizationSize.Large),
            ("NWC", "National Water Company", "المياه الوطنية", "Utilities", OrganizationSize.Large),
            ("MARAFIQ", "Power and Water Utility (MARAFIQ)", "مرافق", "Utilities", OrganizationSize.Medium),
            ("TAWUNIYA", "The Company for Cooperative Insurance", "التعاونية للتأمين", "Insurance", OrganizationSize.Large),
            ("BUPA-ARABIA", "Bupa Arabia", "بوبا العربية", "Insurance", OrganizationSize.Large),
            ("MEDGULF", "Mediterranean & Gulf Insurance", "ميدغلف", "Insurance", OrganizationSize.Medium),
            ("SAMBA", "Samba Financial Group", "مجموعة سامبا المالية", "Banking", OrganizationSize.Large),
            ("BSF", "Banque Saudi Fransi", "البنك السعودي الفرنسي", "Banking", OrganizationSize.Large),
            ("SABB", "Saudi British Bank", "البنك السعودي البريطاني", "Banking", OrganizationSize.Large),
            ("ANB", "Arab National Bank", "البنك العربي الوطني", "Banking", OrganizationSize.Large),
            ("ALBILAD", "Bank AlBilad", "بنك البلاد", "Banking", OrganizationSize.Medium),
            ("ALJAZIRA", "Bank AlJazira", "بنك الجزيرة", "Banking", OrganizationSize.Medium),
            ("GIB", "Gulf International Bank", "بنك الخليج الدولي", "Banking", OrganizationSize.Medium),
            ("ALFRANSI", "Saudi Fransi Capital", "فرنسي كابيتال", "Financial Services", OrganizationSize.Medium),
            ("RIYAD-CAPITAL", "Riyad Capital", "الرياض المالية", "Financial Services", OrganizationSize.Medium),
            ("NCBC", "NCB Capital", "الأهلي كابيتال", "Financial Services", OrganizationSize.Medium),
            ("JADWA", "Jadwa Investment", "جدوى للاستثمار", "Investment", OrganizationSize.Medium),
            ("SEDCO", "SEDCO Holding", "سدكو القابضة", "Investment", OrganizationSize.Medium)
        };

        foreach (var (code, nameEn, nameAr, industry, size) in orgData)
        {
            var org = new Organization(
                Guid.NewGuid(),
                nameEn,
                code,
                industry,
                size,
                "Saudi Arabia"
            );
            org.SetDetails(nameAr, "Riyadh", "", "", $"info@{code.ToLower()}.com.sa", "", "", 1000);
            organizations.Add(org);
        }

        await _organizationRepository.InsertManyAsync(organizations);
        result.Inserted = organizations.Count;
        return result;
    }

    private async Task<SeedResult> SeedAssetsAsync()
    {
        var result = new SeedResult();
        var existing = await _assetRepository.GetCountAsync();
        if (existing > 0) return result;

        var assets = new List<Asset>();

        var assetData = new[]
        {
            // Hardware Assets (15)
            ("HW-SRV-001", "Primary Database Server", AssetType.Hardware, AssetCategory.Server, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("HW-SRV-002", "Application Server Cluster Node 1", AssetType.Hardware, AssetCategory.Server, AssetCriticality.Critical, AssetClassification.Confidential),
            ("HW-SRV-003", "Application Server Cluster Node 2", AssetType.Hardware, AssetCategory.Server, AssetCriticality.Critical, AssetClassification.Confidential),
            ("HW-SRV-004", "Backup Server", AssetType.Hardware, AssetCategory.Server, AssetCriticality.High, AssetClassification.Confidential),
            ("HW-SRV-005", "Mail Server", AssetType.Hardware, AssetCategory.Server, AssetCriticality.High, AssetClassification.Confidential),
            ("HW-NET-001", "Core Router", AssetType.Network, AssetCategory.NetworkDevice, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("HW-NET-002", "Distribution Switch Layer 1", AssetType.Network, AssetCategory.NetworkDevice, AssetCriticality.Critical, AssetClassification.Confidential),
            ("HW-NET-003", "Distribution Switch Layer 2", AssetType.Network, AssetCategory.NetworkDevice, AssetCriticality.Critical, AssetClassification.Confidential),
            ("HW-SEC-001", "Firewall Primary", AssetType.Network, AssetCategory.SecurityDevice, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("HW-SEC-002", "Firewall Secondary", AssetType.Network, AssetCategory.SecurityDevice, AssetCriticality.Critical, AssetClassification.Restricted),
            ("HW-SEC-003", "Intrusion Detection System", AssetType.Network, AssetCategory.SecurityDevice, AssetCriticality.High, AssetClassification.Confidential),
            ("HW-WS-001", "Executive Workstations", AssetType.Hardware, AssetCategory.Workstation, AssetCriticality.Medium, AssetClassification.Internal),
            ("HW-WS-002", "Developer Workstations", AssetType.Hardware, AssetCategory.Workstation, AssetCriticality.Medium, AssetClassification.Internal),
            ("HW-MOB-001", "Corporate Mobile Devices", AssetType.Hardware, AssetCategory.MobileDevice, AssetCriticality.Medium, AssetClassification.Internal),
            ("HW-MOB-002", "Executive Tablets", AssetType.Hardware, AssetCategory.MobileDevice, AssetCriticality.Medium, AssetClassification.Internal),

            // Software Assets (15)
            ("SW-ERP-001", "Enterprise Resource Planning System", AssetType.Software, AssetCategory.Application, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("SW-CRM-001", "Customer Relationship Management", AssetType.Software, AssetCategory.Application, AssetCriticality.Critical, AssetClassification.Confidential),
            ("SW-HRM-001", "Human Resources Management System", AssetType.Software, AssetCategory.Application, AssetCriticality.High, AssetClassification.Confidential),
            ("SW-FIN-001", "Financial Management System", AssetType.Software, AssetCategory.Application, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("SW-SCM-001", "Supply Chain Management", AssetType.Software, AssetCategory.Application, AssetCriticality.Critical, AssetClassification.Confidential),
            ("SW-BI-001", "Business Intelligence Platform", AssetType.Software, AssetCategory.Application, AssetCriticality.High, AssetClassification.Confidential),
            ("SW-MAIL-001", "Email System", AssetType.Software, AssetCategory.Application, AssetCriticality.Critical, AssetClassification.Confidential),
            ("SW-COLLAB-001", "Collaboration Platform", AssetType.Software, AssetCategory.Application, AssetCriticality.High, AssetClassification.Internal),
            ("SW-SEC-001", "Security Information Management", AssetType.Software, AssetCategory.Application, AssetCriticality.Critical, AssetClassification.Restricted),
            ("SW-IAM-001", "Identity and Access Management", AssetType.Software, AssetCategory.Application, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("SW-OS-001", "Server Operating Systems", AssetType.Software, AssetCategory.OperatingSystem, AssetCriticality.Critical, AssetClassification.Confidential),
            ("SW-OS-002", "Workstation Operating Systems", AssetType.Software, AssetCategory.OperatingSystem, AssetCriticality.Medium, AssetClassification.Internal),
            ("SW-DB-001", "Database Management System", AssetType.Software, AssetCategory.Database, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("SW-AV-001", "Antivirus Solution", AssetType.Software, AssetCategory.Application, AssetCriticality.High, AssetClassification.Internal),
            ("SW-BKP-001", "Backup Software", AssetType.Software, AssetCategory.Application, AssetCriticality.Critical, AssetClassification.Confidential),

            // Data Assets (10)
            ("DATA-CUS-001", "Customer Personal Data", AssetType.Data, AssetCategory.DataStore, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("DATA-FIN-001", "Financial Records", AssetType.Data, AssetCategory.DataStore, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("DATA-EMP-001", "Employee Records", AssetType.Data, AssetCategory.DataStore, AssetCriticality.Critical, AssetClassification.Confidential),
            ("DATA-IP-001", "Intellectual Property", AssetType.Data, AssetCategory.DataStore, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("DATA-CON-001", "Contracts and Agreements", AssetType.Data, AssetCategory.DataStore, AssetCriticality.High, AssetClassification.Confidential),
            ("DATA-LOG-001", "System Logs", AssetType.Data, AssetCategory.DataStore, AssetCriticality.High, AssetClassification.Internal),
            ("DATA-AUD-001", "Audit Trails", AssetType.Data, AssetCategory.DataStore, AssetCriticality.Critical, AssetClassification.Confidential),
            ("DATA-BKP-001", "Backup Data", AssetType.Data, AssetCategory.DataStore, AssetCriticality.Critical, AssetClassification.Confidential),
            ("DATA-REP-001", "Regulatory Reports", AssetType.Data, AssetCategory.DataStore, AssetCriticality.High, AssetClassification.Confidential),
            ("DATA-MKT-001", "Marketing Data", AssetType.Data, AssetCategory.DataStore, AssetCriticality.Medium, AssetClassification.Internal),

            // Cloud Assets (5)
            ("CLD-IAS-001", "Infrastructure as a Service", AssetType.Cloud, AssetCategory.CloudService, AssetCriticality.Critical, AssetClassification.Confidential),
            ("CLD-PAS-001", "Platform as a Service", AssetType.Cloud, AssetCategory.CloudService, AssetCriticality.High, AssetClassification.Confidential),
            ("CLD-SAS-001", "SaaS Applications", AssetType.Cloud, AssetCategory.CloudService, AssetCriticality.High, AssetClassification.Internal),
            ("CLD-STR-001", "Cloud Storage", AssetType.Cloud, AssetCategory.CloudService, AssetCriticality.Critical, AssetClassification.Confidential),
            ("CLD-DR-001", "Disaster Recovery Site", AssetType.Cloud, AssetCategory.CloudService, AssetCriticality.Critical, AssetClassification.Confidential),

            // Facility Assets (5)
            ("FAC-DC-001", "Primary Data Center", AssetType.Facility, AssetCategory.DataCenter, AssetCriticality.MissionCritical, AssetClassification.Restricted),
            ("FAC-DC-002", "Secondary Data Center", AssetType.Facility, AssetCategory.DataCenter, AssetCriticality.Critical, AssetClassification.Confidential),
            ("FAC-HQ-001", "Headquarters Building", AssetType.Facility, AssetCategory.Building, AssetCriticality.High, AssetClassification.Internal),
            ("FAC-BR-001", "Branch Offices", AssetType.Facility, AssetCategory.Building, AssetCriticality.Medium, AssetClassification.Internal),
            ("FAC-DR-002", "DR Facility", AssetType.Facility, AssetCategory.DataCenter, AssetCriticality.Critical, AssetClassification.Confidential)
        };

        foreach (var (code, name, type, category, criticality, classification) in assetData)
        {
            var asset = new Asset(
                Guid.NewGuid(),
                code,
                name,
                type,
                category,
                classification,
                criticality
            );
            asset.SetLocation("Riyadh, Saudi Arabia");
            assets.Add(asset);
        }

        await _assetRepository.InsertManyAsync(assets);
        result.Inserted = assets.Count;
        return result;
    }

    private async Task<SeedResult> SeedGapsAsync()
    {
        var result = new SeedResult();
        var existing = await _gapRepository.GetCountAsync();
        if (existing > 0) return result;

        var gaps = new List<Gap>();
        var random = new Random(42);

        var gapData = new[]
        {
            // Access Control Gaps
            ("GAP-AC-001", "Privileged Access Management", "No centralized PAM solution", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-AC-002", "Multi-Factor Authentication", "MFA not enforced for all systems", GapSeverity.High, GapPriority.High),
            ("GAP-AC-003", "Access Review Process", "Quarterly access reviews not conducted", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-AC-004", "Service Account Management", "Service accounts not properly inventoried", GapSeverity.High, GapPriority.High),
            ("GAP-AC-005", "Vendor Access Controls", "Third-party access not monitored", GapSeverity.High, GapPriority.High),

            // Data Protection Gaps
            ("GAP-DP-001", "Data Classification", "No formal data classification program", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-DP-002", "Data Encryption at Rest", "Not all sensitive data encrypted", GapSeverity.High, GapPriority.High),
            ("GAP-DP-003", "Data Loss Prevention", "DLP solution not implemented", GapSeverity.High, GapPriority.High),
            ("GAP-DP-004", "Data Retention Policy", "Retention periods not defined", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-DP-005", "Data Masking", "Test environments use production data", GapSeverity.High, GapPriority.High),

            // Network Security Gaps
            ("GAP-NS-001", "Network Segmentation", "Flat network architecture", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-NS-002", "Firewall Rule Review", "Annual review not performed", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-NS-003", "Intrusion Detection", "IDS signatures outdated", GapSeverity.High, GapPriority.High),
            ("GAP-NS-004", "Wireless Security", "WPA2 Enterprise not implemented", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-NS-005", "VPN Configuration", "Split tunneling enabled", GapSeverity.Medium, GapPriority.Medium),

            // Incident Response Gaps
            ("GAP-IR-001", "Incident Response Plan", "Plan not tested in 2 years", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-IR-002", "Security Operations Center", "24x7 coverage not available", GapSeverity.High, GapPriority.High),
            ("GAP-IR-003", "Forensic Capabilities", "No trained forensic investigators", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-IR-004", "Communication Plan", "No defined communication procedures", GapSeverity.High, GapPriority.High),
            ("GAP-IR-005", "Incident Classification", "Severity levels not defined", GapSeverity.Medium, GapPriority.Medium),

            // Business Continuity Gaps
            ("GAP-BC-001", "Business Impact Analysis", "BIA not updated in 3 years", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-BC-002", "Disaster Recovery Testing", "DR tests not conducted annually", GapSeverity.High, GapPriority.High),
            ("GAP-BC-003", "Recovery Time Objectives", "RTOs not met in last test", GapSeverity.High, GapPriority.High),
            ("GAP-BC-004", "Backup Verification", "Restore tests not performed", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-BC-005", "Crisis Management", "No crisis management team", GapSeverity.High, GapPriority.High),

            // Security Awareness Gaps
            ("GAP-SA-001", "Security Training Program", "No mandatory security training", GapSeverity.High, GapPriority.High),
            ("GAP-SA-002", "Phishing Awareness", "No phishing simulations conducted", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-SA-003", "Secure Coding Training", "Developers not trained", GapSeverity.High, GapPriority.High),
            ("GAP-SA-004", "Executive Awareness", "No executive briefings", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-SA-005", "New Hire Training", "Security not in onboarding", GapSeverity.Medium, GapPriority.Medium),

            // Vulnerability Management Gaps
            ("GAP-VM-001", "Vulnerability Scanning", "Quarterly scans not comprehensive", GapSeverity.High, GapPriority.High),
            ("GAP-VM-002", "Patch Management", "Critical patches delayed 30+ days", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-VM-003", "Penetration Testing", "Annual pentest not performed", GapSeverity.High, GapPriority.High),
            ("GAP-VM-004", "Application Security", "SAST/DAST not implemented", GapSeverity.High, GapPriority.High),
            ("GAP-VM-005", "Third-Party Assessments", "Vendor security not assessed", GapSeverity.Medium, GapPriority.Medium),

            // Compliance Gaps
            ("GAP-CM-001", "PDPL Compliance", "Privacy notices not updated", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-CM-002", "NCA ECC Alignment", "30% controls not implemented", GapSeverity.Critical, GapPriority.Urgent),
            ("GAP-CM-003", "PCI DSS Requirements", "Scope not properly defined", GapSeverity.High, GapPriority.High),
            ("GAP-CM-004", "ISO 27001 Certification", "Internal audit not conducted", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-CM-005", "Regulatory Reporting", "Reports submitted late", GapSeverity.High, GapPriority.High),

            // Asset Management Gaps
            ("GAP-AM-001", "Asset Inventory", "Inventory 70% complete", GapSeverity.High, GapPriority.High),
            ("GAP-AM-002", "Configuration Management", "Baseline configs not defined", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-AM-003", "Change Management", "Emergency changes not documented", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-AM-004", "License Management", "Software audit findings", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-AM-005", "End-of-Life Systems", "EOL systems in production", GapSeverity.High, GapPriority.High),

            // Physical Security Gaps
            ("GAP-PS-001", "Access Control Systems", "Badge access not reviewed", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-PS-002", "CCTV Coverage", "Blind spots identified", GapSeverity.Medium, GapPriority.Medium),
            ("GAP-PS-003", "Visitor Management", "No escort policy enforced", GapSeverity.Low, GapPriority.Low),
            ("GAP-PS-004", "Clean Desk Policy", "Policy not enforced", GapSeverity.Low, GapPriority.Low),
            ("GAP-PS-005", "Data Center Security", "Dual authentication missing", GapSeverity.High, GapPriority.High)
        };

        foreach (var (code, title, description, severity, priority) in gapData)
        {
            var gap = new Gap(
                Guid.NewGuid(),
                code,
                title,
                "CTRL-REF",
                severity
            );
            gap.SetGapDetails("Current state not compliant", "Full compliance required", description);
            gap.UpdatePriority(priority);
            gap.AssignOwner($"owner@company.com", $"owner@company.com", DateTime.UtcNow.AddDays(random.Next(30, 180)));
            gaps.Add(gap);
        }

        await _gapRepository.InsertManyAsync(gaps);
        result.Inserted = gaps.Count;
        return result;
    }

    private async Task<SeedResult> SeedActionItemsAsync()
    {
        var result = new SeedResult();
        var existing = await _actionItemRepository.GetCountAsync();
        if (existing > 0) return result;

        var actionItems = new List<ActionItem>();
        var random = new Random(42);

        var actionData = new[]
        {
            // Implementation Actions
            ("ACT-IMP-001", "Implement PAM Solution", ActionItemType.Implementation, ActionItemPriority.Critical),
            ("ACT-IMP-002", "Deploy MFA Enterprise-Wide", ActionItemType.Implementation, ActionItemPriority.Critical),
            ("ACT-IMP-003", "Implement DLP Solution", ActionItemType.Implementation, ActionItemPriority.High),
            ("ACT-IMP-004", "Deploy SIEM Platform", ActionItemType.Implementation, ActionItemPriority.Critical),
            ("ACT-IMP-005", "Implement Network Segmentation", ActionItemType.Implementation, ActionItemPriority.Critical),
            ("ACT-IMP-006", "Deploy Endpoint Detection", ActionItemType.Implementation, ActionItemPriority.High),
            ("ACT-IMP-007", "Implement Data Classification Tool", ActionItemType.Implementation, ActionItemPriority.High),
            ("ACT-IMP-008", "Deploy Cloud Security Posture", ActionItemType.Implementation, ActionItemPriority.High),
            ("ACT-IMP-009", "Implement Zero Trust Architecture", ActionItemType.Implementation, ActionItemPriority.Medium),
            ("ACT-IMP-010", "Deploy WAF Solution", ActionItemType.Implementation, ActionItemPriority.High),

            // Remediation Actions
            ("ACT-REM-001", "Remediate Critical Vulnerabilities", ActionItemType.Remediation, ActionItemPriority.Critical),
            ("ACT-REM-002", "Update Firewall Rules", ActionItemType.Remediation, ActionItemPriority.High),
            ("ACT-REM-003", "Patch Legacy Systems", ActionItemType.Remediation, ActionItemPriority.High),
            ("ACT-REM-004", "Fix Configuration Drift", ActionItemType.Remediation, ActionItemPriority.Medium),
            ("ACT-REM-005", "Remediate Audit Findings", ActionItemType.Remediation, ActionItemPriority.High),
            ("ACT-REM-006", "Address Password Policy Issues", ActionItemType.Remediation, ActionItemPriority.Medium),
            ("ACT-REM-007", "Fix SSL/TLS Configurations", ActionItemType.Remediation, ActionItemPriority.High),
            ("ACT-REM-008", "Remediate Database Security", ActionItemType.Remediation, ActionItemPriority.Critical),
            ("ACT-REM-009", "Fix API Security Issues", ActionItemType.Remediation, ActionItemPriority.High),
            ("ACT-REM-010", "Address Cloud Misconfigurations", ActionItemType.Remediation, ActionItemPriority.Critical),

            // Documentation Actions
            ("ACT-DOC-001", "Update Security Policies", ActionItemType.Documentation, ActionItemPriority.High),
            ("ACT-DOC-002", "Document Incident Response Plan", ActionItemType.Documentation, ActionItemPriority.Critical),
            ("ACT-DOC-003", "Create Data Flow Diagrams", ActionItemType.Documentation, ActionItemPriority.Medium),
            ("ACT-DOC-004", "Document DR Procedures", ActionItemType.Documentation, ActionItemPriority.High),
            ("ACT-DOC-005", "Update Network Diagrams", ActionItemType.Documentation, ActionItemPriority.Medium),
            ("ACT-DOC-006", "Create Runbooks", ActionItemType.Documentation, ActionItemPriority.Medium),
            ("ACT-DOC-007", "Document Compliance Controls", ActionItemType.Documentation, ActionItemPriority.High),
            ("ACT-DOC-008", "Update Risk Register", ActionItemType.Documentation, ActionItemPriority.High),
            ("ACT-DOC-009", "Create Security Standards", ActionItemType.Documentation, ActionItemPriority.Medium),
            ("ACT-DOC-010", "Document Change Management", ActionItemType.Documentation, ActionItemPriority.Medium),

            // Training Actions
            ("ACT-TRN-001", "Security Awareness Program", ActionItemType.Training, ActionItemPriority.High),
            ("ACT-TRN-002", "Phishing Simulation Campaign", ActionItemType.Training, ActionItemPriority.Medium),
            ("ACT-TRN-003", "Secure Coding Training", ActionItemType.Training, ActionItemPriority.High),
            ("ACT-TRN-004", "Incident Response Training", ActionItemType.Training, ActionItemPriority.High),
            ("ACT-TRN-005", "GDPR/PDPL Training", ActionItemType.Training, ActionItemPriority.High),
            ("ACT-TRN-006", "Cloud Security Training", ActionItemType.Training, ActionItemPriority.Medium),
            ("ACT-TRN-007", "Executive Security Briefing", ActionItemType.Training, ActionItemPriority.Medium),
            ("ACT-TRN-008", "New Hire Security Orientation", ActionItemType.Training, ActionItemPriority.Medium),
            ("ACT-TRN-009", "SOC Analyst Training", ActionItemType.Training, ActionItemPriority.High),
            ("ACT-TRN-010", "Compliance Training", ActionItemType.Training, ActionItemPriority.Medium),

            // Review Actions
            ("ACT-REV-001", "Quarterly Access Review", ActionItemType.Review, ActionItemPriority.High),
            ("ACT-REV-002", "Annual Policy Review", ActionItemType.Review, ActionItemPriority.Medium),
            ("ACT-REV-003", "Vendor Risk Assessment", ActionItemType.Review, ActionItemPriority.High),
            ("ACT-REV-004", "Firewall Rule Review", ActionItemType.Review, ActionItemPriority.Medium),
            ("ACT-REV-005", "BIA Review and Update", ActionItemType.Review, ActionItemPriority.High),
            ("ACT-REV-006", "Risk Assessment Review", ActionItemType.Review, ActionItemPriority.High),
            ("ACT-REV-007", "Compliance Gap Review", ActionItemType.Review, ActionItemPriority.High),
            ("ACT-REV-008", "Control Effectiveness Review", ActionItemType.Review, ActionItemPriority.Medium),
            ("ACT-REV-009", "Incident Review Meeting", ActionItemType.Review, ActionItemPriority.Medium),
            ("ACT-REV-010", "Audit Finding Review", ActionItemType.Review, ActionItemPriority.High)
        };

        foreach (var (code, title, type, priority) in actionData)
        {
            var actionItem = new ActionItem(
                Guid.NewGuid(),
                code,
                title,
                type,
                priority,
                ActionItemSource.Gap
            );
            actionItem.Assign($"assignee@company.com", $"assignee@company.com", "manager@company.com", DateTime.UtcNow.AddDays(random.Next(14, 120)));
            actionItems.Add(actionItem);
        }

        await _actionItemRepository.InsertManyAsync(actionItems);
        result.Inserted = actionItems.Count;
        return result;
    }

    private async Task<SeedResult> SeedAuditsAsync()
    {
        var result = new SeedResult();
        var existingAudits = await _auditRepository.GetCountAsync();
        if (existingAudits > 0) return result;

        var audits = new List<Audit>();
        var findings = new List<AuditFinding>();
        var random = new Random(42);

        var auditData = new[]
        {
            ("AUD-2024-001", "NCA ECC Annual Assessment", AuditType.Regulatory),
            ("AUD-2024-002", "ISO 27001 Surveillance Audit", AuditType.Certification),
            ("AUD-2024-003", "PCI DSS Assessment", AuditType.Certification),
            ("AUD-2024-004", "SAMA CSF Compliance Review", AuditType.Regulatory),
            ("AUD-2024-005", "Internal Security Audit Q1", AuditType.Internal),
            ("AUD-2024-006", "PDPL Readiness Assessment", AuditType.Regulatory),
            ("AUD-2024-007", "SOC 2 Type II Audit", AuditType.External),
            ("AUD-2024-008", "Business Continuity Audit", AuditType.Internal),
            ("AUD-2024-009", "Vendor Security Assessment", AuditType.Internal),
            ("AUD-2024-010", "Cloud Security Review", AuditType.Internal)
        };

        foreach (var (auditCode, title, type) in auditData)
        {
            var startDate = DateTime.UtcNow.AddDays(-random.Next(30, 180));
            var audit = new Audit(
                Guid.NewGuid(),
                auditCode,
                title,
                type,
                startDate,
                startDate.AddDays(random.Next(14, 45))
            );
            audit.SetAuditor(
                $"Auditor {random.Next(1, 10)}",
                "External Audit Firm",
                $"auditor{random.Next(1, 10)}@auditfirm.com",
                "Lead Auditor"
            );
            audit.Start(startDate.AddDays(2));
            audits.Add(audit);

            // Add 5-8 findings per audit
            var findingCount = random.Next(5, 9);
            for (int i = 1; i <= findingCount; i++)
            {
                var finding = new AuditFinding(
                    Guid.NewGuid(),
                    audit.Id,
                    $"{auditCode}-F{i:D2}",
                    $"Finding {i} for {auditCode}",
                    (FindingSeverity)random.Next(0, 5)
                );
                finding.SetDescription($"Detailed description of finding {i}", null);
                finding.SetRecommendation($"Recommended action for finding {i}", null);
                findings.Add(finding);
            }
        }

        await _auditRepository.InsertManyAsync(audits);
        await _auditFindingRepository.InsertManyAsync(findings);
        result.Inserted = audits.Count + findings.Count;
        return result;
    }
}

public class SeedResult
{
    public int Inserted { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
}
