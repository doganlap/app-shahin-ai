// ============================================
// ABP.io GRC PLATFORM - DOMAIN MODEL SAMPLES
// Saudi Arabia Multi-Tenant Compliance Platform
// ============================================

// ========================================
// 1. DOMAIN ENTITIES (GRC.Domain project)
// ========================================

// --- Framework Aggregate Root ---
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using System.Collections.ObjectModel;

namespace GRC.Domain.Frameworks
{
    /// <summary>
    /// Regulatory framework (e.g., NCA-ECC, SAMA-CSF, PDPL)
    /// This is a shared entity - not tenant-specific
    /// </summary>
    public class Framework : FullAuditedAggregateRoot<Guid>
    {
        public string Code { get; private set; }           // "NCA-ECC"
        public string Version { get; private set; }        // "v2.0"
        public string TitleAr { get; private set; }        // Arabic title
        public string TitleEn { get; private set; }        // English title
        public string RegulatorCode { get; private set; }  // "NCA"
        public string Category { get; private set; }       // "national_law" | "sector_specific"
        public bool IsMandatory { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public int TotalControls { get; private set; }
        public FrameworkStatus Status { get; private set; }
        public string OfficialReference { get; private set; }
        public string DescriptionAr { get; private set; }
        public string DescriptionEn { get; private set; }

        // Navigation
        public virtual ICollection<Control> Controls { get; private set; }

        protected Framework() { }

        public Framework(
            Guid id,
            string code,
            string version,
            string titleAr,
            string titleEn,
            string regulatorCode,
            bool isMandatory = true)
            : base(id)
        {
            Code = code;
            Version = version;
            TitleAr = titleAr;
            TitleEn = titleEn;
            RegulatorCode = regulatorCode;
            IsMandatory = isMandatory;
            Controls = new Collection<Control>();
            Status = FrameworkStatus.Active;
        }
    }

    public enum FrameworkStatus
    {
        Draft,
        Active,
        Deprecated
    }
}

// --- Control Entity ---
namespace GRC.Domain.Frameworks
{
    /// <summary>
    /// Individual compliance control within a framework
    /// Shared entity - actual implementation is tenant-specific
    /// </summary>
    public class Control : FullAuditedEntity<Guid>
    {
        public Guid FrameworkId { get; private set; }
        public string ControlNumber { get; private set; }   // "2.1.1"
        public string Domain { get; private set; }          // "Asset Management"
        public string TitleAr { get; private set; }
        public string TitleEn { get; private set; }
        public string RequirementAr { get; private set; }
        public string RequirementEn { get; private set; }
        public ControlType ControlType { get; private set; }
        public int MaturityLevel { get; private set; }      // 1-5
        public string ImplementationGuidance { get; private set; }
        public string EvidenceRequirements { get; private set; }
        public string MappingISO27001 { get; private set; } // Cross-mapping
        public string MappingNIST { get; private set; }
        public ControlStatus Status { get; private set; }

        // Navigation
        public virtual Framework Framework { get; private set; }

        protected Control() { }

        public Control(
            Guid id,
            Guid frameworkId,
            string controlNumber,
            string domain,
            string titleAr,
            string titleEn,
            string requirementAr,
            string requirementEn,
            ControlType controlType,
            int maturityLevel)
            : base(id)
        {
            FrameworkId = frameworkId;
            ControlNumber = controlNumber;
            Domain = domain;
            TitleAr = titleAr;
            TitleEn = titleEn;
            RequirementAr = requirementAr;
            RequirementEn = requirementEn;
            ControlType = controlType;
            MaturityLevel = maturityLevel;
            Status = ControlStatus.Active;
        }
    }

    public enum ControlType
    {
        Preventive,
        Detective,
        Corrective
    }

    public enum ControlStatus
    {
        Active,
        Deprecated,
        Draft
    }
}

// --- Control Implementation (Tenant-Specific) ---
namespace GRC.Domain.Compliance
{
    /// <summary>
    /// Organization's implementation of a control - MULTI-TENANT
    /// Each tenant has their own implementations
    /// </summary>
    public class ControlImplementation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }  // ABP Multi-Tenancy
        
        public Guid ControlId { get; private set; }
        public Guid? OwnerUserId { get; private set; }
        public ImplementationStatus Status { get; private set; }
        public DateTime? ImplementationDate { get; private set; }
        public DateTime? TargetDate { get; private set; }
        public string Notes { get; private set; }
        public int ComplianceScore { get; private set; }    // 0-100
        public DateTime? LastAssessedDate { get; private set; }

        // Navigation
        public virtual Control Control { get; private set; }
        public virtual ICollection<Evidence> Evidences { get; private set; }
        public virtual ICollection<ImplementationTask> Tasks { get; private set; }

        protected ControlImplementation() { }

        public ControlImplementation(
            Guid id,
            Guid controlId,
            Guid? tenantId = null)
            : base(id)
        {
            ControlId = controlId;
            TenantId = tenantId;
            Status = ImplementationStatus.NotStarted;
            ComplianceScore = 0;
            Evidences = new Collection<Evidence>();
            Tasks = new Collection<ImplementationTask>();
        }

        public void AssignOwner(Guid userId)
        {
            OwnerUserId = userId;
        }

        public void UpdateStatus(ImplementationStatus newStatus)
        {
            Status = newStatus;
            if (newStatus == ImplementationStatus.Implemented)
            {
                ImplementationDate = DateTime.UtcNow;
            }
        }

        public void SetComplianceScore(int score)
        {
            if (score < 0 || score > 100)
                throw new ArgumentException("Score must be between 0 and 100");
            
            ComplianceScore = score;
            LastAssessedDate = DateTime.UtcNow;
        }
    }

    public enum ImplementationStatus
    {
        NotStarted,
        InProgress,
        Implemented,
        NotApplicable,
        PartiallyImplemented,
        NonCompliant
    }
}

// --- Evidence Entity ---
namespace GRC.Domain.Compliance
{
    /// <summary>
    /// Evidence supporting control implementation
    /// </summary>
    public class Evidence : FullAuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        
        public Guid ControlImplementationId { get; private set; }
        public EvidenceType EvidenceType { get; private set; }
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public long FileSize { get; private set; }
        public string MimeType { get; private set; }
        public string Description { get; private set; }
        public DateTime UploadDate { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public Guid? VerifiedByUserId { get; private set; }
        public DateTime? VerifiedDate { get; private set; }
        public EvidenceStatus Status { get; private set; }

        protected Evidence() { }

        public Evidence(
            Guid id,
            Guid controlImplementationId,
            EvidenceType evidenceType,
            string fileName,
            string filePath)
            : base(id)
        {
            ControlImplementationId = controlImplementationId;
            EvidenceType = evidenceType;
            FileName = fileName;
            FilePath = filePath;
            UploadDate = DateTime.UtcNow;
            Status = EvidenceStatus.Pending;
        }

        public void Verify(Guid verifierUserId)
        {
            VerifiedByUserId = verifierUserId;
            VerifiedDate = DateTime.UtcNow;
            Status = EvidenceStatus.Verified;
        }

        public void Reject(string reason)
        {
            Status = EvidenceStatus.Rejected;
            Description = reason;
        }
    }

    public enum EvidenceType
    {
        Policy,
        Procedure,
        Screenshot,
        AuditLog,
        Certificate,
        Report,
        Configuration,
        TrainingRecord,
        RiskAssessment,
        Other
    }

    public enum EvidenceStatus
    {
        Pending,
        Verified,
        Rejected,
        Expired
    }
}

// --- Assessment Entity ---
namespace GRC.Domain.Assessments
{
    /// <summary>
    /// Compliance assessment for a framework
    /// </summary>
    public class Assessment : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        
        public Guid FrameworkId { get; private set; }
        public string AssessmentName { get; private set; }
        public AssessmentType AssessmentType { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public AssessmentStatus Status { get; private set; }
        public decimal OverallScore { get; private set; }
        public Guid? LeadAssessorId { get; private set; }
        public string Summary { get; private set; }

        // Navigation
        public virtual Framework Framework { get; private set; }
        public virtual ICollection<AssessmentItem> Items { get; private set; }

        protected Assessment() { }

        public Assessment(
            Guid id,
            Guid frameworkId,
            string name,
            AssessmentType type,
            Guid? tenantId = null)
            : base(id)
        {
            FrameworkId = frameworkId;
            AssessmentName = name;
            AssessmentType = type;
            TenantId = tenantId;
            StartDate = DateTime.UtcNow;
            Status = AssessmentStatus.InProgress;
            Items = new Collection<AssessmentItem>();
        }

        public void Complete(decimal overallScore, string summary)
        {
            OverallScore = overallScore;
            Summary = summary;
            EndDate = DateTime.UtcNow;
            Status = AssessmentStatus.Completed;
        }
    }

    public enum AssessmentType
    {
        SelfAssessment,
        InternalAudit,
        ExternalAudit,
        GapAnalysis,
        RegulatoryInspection
    }

    public enum AssessmentStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }
}

// --- Risk Entity ---
namespace GRC.Domain.Risks
{
    /// <summary>
    /// Risk register entry
    /// </summary>
    public class Risk : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        
        public string RiskCode { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public RiskCategory Category { get; private set; }
        public int Likelihood { get; private set; }         // 1-5
        public int Impact { get; private set; }             // 1-5
        public int RiskScore => Likelihood * Impact;         // Calculated
        public RiskLevel RiskLevel => CalculateRiskLevel();
        public RiskTreatment Treatment { get; private set; }
        public string TreatmentPlan { get; private set; }
        public Guid? OwnerUserId { get; private set; }
        public DateTime? ReviewDate { get; private set; }
        public RiskStatus Status { get; private set; }

        // Navigation - link to affected controls
        public virtual ICollection<RiskControlMapping> ControlMappings { get; private set; }

        protected Risk() { }

        public Risk(
            Guid id,
            string riskCode,
            string title,
            string description,
            RiskCategory category,
            Guid? tenantId = null)
            : base(id)
        {
            RiskCode = riskCode;
            Title = title;
            Description = description;
            Category = category;
            TenantId = tenantId;
            Likelihood = 1;
            Impact = 1;
            Treatment = RiskTreatment.Mitigate;
            Status = RiskStatus.Identified;
            ControlMappings = new Collection<RiskControlMapping>();
        }

        public void AssessRisk(int likelihood, int impact)
        {
            if (likelihood < 1 || likelihood > 5)
                throw new ArgumentException("Likelihood must be 1-5");
            if (impact < 1 || impact > 5)
                throw new ArgumentException("Impact must be 1-5");

            Likelihood = likelihood;
            Impact = impact;
            Status = RiskStatus.Assessed;
        }

        private RiskLevel CalculateRiskLevel()
        {
            return RiskScore switch
            {
                >= 20 => RiskLevel.Critical,
                >= 15 => RiskLevel.High,
                >= 10 => RiskLevel.Medium,
                >= 5 => RiskLevel.Low,
                _ => RiskLevel.VeryLow
            };
        }
    }

    public enum RiskCategory
    {
        Cybersecurity,
        DataPrivacy,
        Operational,
        Compliance,
        Financial,
        Reputational,
        Strategic,
        ThirdParty
    }

    public enum RiskTreatment
    {
        Accept,
        Mitigate,
        Transfer,
        Avoid
    }

    public enum RiskStatus
    {
        Identified,
        Assessed,
        InTreatment,
        Monitored,
        Closed
    }

    public enum RiskLevel
    {
        VeryLow,
        Low,
        Medium,
        High,
        Critical
    }
}

// --- IoT Device Entity (Healthcare Module) ---
namespace GRC.Domain.IoT
{
    /// <summary>
    /// IoMT device inventory for healthcare organizations
    /// </summary>
    public class IoTDevice : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        
        public string DeviceId { get; private set; }
        public string DeviceName { get; private set; }
        public IoTDeviceType DeviceType { get; private set; }
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public string SerialNumber { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string NetworkSegment { get; private set; }
        public string IpAddress { get; private set; }
        public string MacAddress { get; private set; }
        public string Location { get; private set; }
        public Guid? OwnerUserId { get; private set; }
        public IoTSecurityStatus SecurityStatus { get; private set; }
        public DateTime? LastSecurityAssessment { get; private set; }
        public DateTime? LastPatchDate { get; private set; }
        public bool IsSegmented { get; private set; }
        public bool HasEncryption { get; private set; }

        protected IoTDevice() { }

        public IoTDevice(
            Guid id,
            string deviceId,
            string deviceName,
            IoTDeviceType deviceType,
            string manufacturer,
            Guid? tenantId = null)
            : base(id)
        {
            DeviceId = deviceId;
            DeviceName = deviceName;
            DeviceType = deviceType;
            Manufacturer = manufacturer;
            TenantId = tenantId;
            SecurityStatus = IoTSecurityStatus.Unknown;
        }

        public void UpdateSecurityStatus(IoTSecurityStatus status)
        {
            SecurityStatus = status;
            LastSecurityAssessment = DateTime.UtcNow;
        }
    }

    public enum IoTDeviceType
    {
        PatientMonitor,
        InfusionPump,
        ImagingDevice,
        LabAnalyzer,
        Ventilator,
        DefibrillatOR,
        Wearable,
        EnvironmentSensor,
        NetworkEquipment,
        Other
    }

    public enum IoTSecurityStatus
    {
        Unknown,
        Compliant,
        PartiallyCompliant,
        NonCompliant,
        RequiresRemediation
    }
}


// ========================================
// 2. APPLICATION SERVICES SAMPLE
// ========================================

namespace GRC.Application.Compliance
{
    using Volo.Abp.Application.Services;
    using Volo.Abp.Domain.Repositories;

    /// <summary>
    /// Application service for compliance management
    /// </summary>
    public class ComplianceAppService : ApplicationService, IComplianceAppService
    {
        private readonly IRepository<ControlImplementation, Guid> _implementationRepository;
        private readonly IRepository<Control, Guid> _controlRepository;
        private readonly IRepository<Framework, Guid> _frameworkRepository;

        public ComplianceAppService(
            IRepository<ControlImplementation, Guid> implementationRepository,
            IRepository<Control, Guid> controlRepository,
            IRepository<Framework, Guid> frameworkRepository)
        {
            _implementationRepository = implementationRepository;
            _controlRepository = controlRepository;
            _frameworkRepository = frameworkRepository;
        }

        /// <summary>
        /// Initialize control implementations for a framework
        /// </summary>
        public async Task InitializeFrameworkAsync(Guid frameworkId)
        {
            var controls = await _controlRepository.GetListAsync(
                c => c.FrameworkId == frameworkId);

            foreach (var control in controls)
            {
                var exists = await _implementationRepository.AnyAsync(
                    i => i.ControlId == control.Id && i.TenantId == CurrentTenant.Id);

                if (!exists)
                {
                    var implementation = new ControlImplementation(
                        GuidGenerator.Create(),
                        control.Id,
                        CurrentTenant.Id);

                    await _implementationRepository.InsertAsync(implementation);
                }
            }
        }

        /// <summary>
        /// Get compliance dashboard data
        /// </summary>
        public async Task<ComplianceDashboardDto> GetDashboardAsync(Guid frameworkId)
        {
            var implementations = await _implementationRepository.GetListAsync(
                i => i.Control.FrameworkId == frameworkId);

            return new ComplianceDashboardDto
            {
                TotalControls = implementations.Count,
                Implemented = implementations.Count(i => i.Status == ImplementationStatus.Implemented),
                InProgress = implementations.Count(i => i.Status == ImplementationStatus.InProgress),
                NotStarted = implementations.Count(i => i.Status == ImplementationStatus.NotStarted),
                NonCompliant = implementations.Count(i => i.Status == ImplementationStatus.NonCompliant),
                AverageScore = implementations.Average(i => i.ComplianceScore),
                ByDomain = implementations
                    .GroupBy(i => i.Control.Domain)
                    .Select(g => new DomainComplianceDto
                    {
                        Domain = g.Key,
                        Score = g.Average(i => i.ComplianceScore),
                        ControlCount = g.Count()
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Update control implementation status
        /// </summary>
        public async Task UpdateControlStatusAsync(Guid implementationId, UpdateControlStatusDto input)
        {
            var implementation = await _implementationRepository.GetAsync(implementationId);
            
            implementation.UpdateStatus(input.Status);
            
            if (input.Notes != null)
            {
                implementation.Notes = input.Notes;
            }

            await _implementationRepository.UpdateAsync(implementation);
        }

        /// <summary>
        /// Upload evidence for a control
        /// </summary>
        public async Task<EvidenceDto> UploadEvidenceAsync(Guid implementationId, UploadEvidenceDto input)
        {
            var implementation = await _implementationRepository.GetAsync(implementationId);
            
            // Store file (Azure Blob, S3, etc.)
            var filePath = await StoreFileAsync(input.File);

            var evidence = new Evidence(
                GuidGenerator.Create(),
                implementationId,
                input.EvidenceType,
                input.File.FileName,
                filePath);

            evidence.Description = input.Description;
            evidence.ValidUntil = input.ValidUntil;

            implementation.Evidences.Add(evidence);
            await _implementationRepository.UpdateAsync(implementation);

            return ObjectMapper.Map<Evidence, EvidenceDto>(evidence);
        }

        /// <summary>
        /// Generate compliance report for regulator submission
        /// </summary>
        public async Task<ComplianceReportDto> GenerateReportAsync(Guid frameworkId)
        {
            var framework = await _frameworkRepository.GetAsync(frameworkId);
            var implementations = await _implementationRepository.GetListAsync(
                i => i.Control.FrameworkId == frameworkId);

            var report = new ComplianceReportDto
            {
                FrameworkCode = framework.Code,
                FrameworkVersion = framework.Version,
                OrganizationName = CurrentTenant.Name,
                GeneratedDate = DateTime.UtcNow,
                OverallComplianceScore = implementations.Average(i => i.ComplianceScore),
                TotalControls = implementations.Count,
                ImplementedControls = implementations.Count(i => i.Status == ImplementationStatus.Implemented),
                ControlDetails = implementations.Select(i => new ControlDetailDto
                {
                    ControlNumber = i.Control.ControlNumber,
                    Title = i.Control.TitleEn,
                    Status = i.Status.ToString(),
                    Score = i.ComplianceScore,
                    EvidenceCount = i.Evidences.Count(e => e.Status == EvidenceStatus.Verified),
                    LastAssessed = i.LastAssessedDate
                }).ToList()
            };

            return report;
        }

        private Task<string> StoreFileAsync(IFormFile file)
        {
            // Implementation for Azure Blob Storage or AWS S3
            throw new NotImplementedException();
        }
    }
}


// ========================================
// 3. REGULATOR INTEGRATION SERVICE
// ========================================

namespace GRC.Application.Integration
{
    /// <summary>
    /// Service for submitting reports to Saudi regulators
    /// </summary>
    public class RegulatorIntegrationService : IRegulatorIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IAuditingManager _auditingManager;

        public RegulatorIntegrationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IAuditingManager auditingManager)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _auditingManager = auditingManager;
        }

        /// <summary>
        /// Submit compliance report to NCA RASID portal
        /// </summary>
        public async Task<SubmissionResult> SubmitToNCAAsync(ComplianceReportDto report)
        {
            var client = _httpClientFactory.CreateClient("NCA");
            
            // Get authentication token
            var token = await GetNCATokenAsync();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);

            // Transform to NCA format
            var ncaPayload = TransformToNCAFormat(report);

            var response = await client.PostAsJsonAsync(
                _configuration["NCA:SubmissionEndpoint"],
                ncaPayload);

            var result = new SubmissionResult
            {
                RegulatorCode = "NCA",
                SubmissionDate = DateTime.UtcNow,
                Success = response.IsSuccessStatusCode,
                ResponseMessage = await response.Content.ReadAsStringAsync()
            };

            // Audit log the submission
            await LogSubmissionAsync(result);

            return result;
        }

        /// <summary>
        /// Submit e-invoice to ZATCA FATOORA platform
        /// </summary>
        public async Task<SubmissionResult> SubmitToZATCAAsync(EInvoiceDto invoice)
        {
            var client = _httpClientFactory.CreateClient("ZATCA");
            
            // ZATCA uses certificate-based authentication
            // Certificate should be loaded from Key Vault
            var certificate = await LoadZATCACertificateAsync();
            
            // Sign the invoice
            var signedInvoice = await SignInvoiceAsync(invoice, certificate);

            var response = await client.PostAsJsonAsync(
                _configuration["ZATCA:ClearanceEndpoint"],
                signedInvoice);

            var result = new SubmissionResult
            {
                RegulatorCode = "ZATCA",
                SubmissionDate = DateTime.UtcNow,
                Success = response.IsSuccessStatusCode,
                ResponseMessage = await response.Content.ReadAsStringAsync()
            };

            return result;
        }

        /// <summary>
        /// Submit cybersecurity report to SAMA
        /// </summary>
        public async Task<SubmissionResult> SubmitToSAMAAsync(ComplianceReportDto report)
        {
            // SAMA integration implementation
            throw new NotImplementedException();
        }

        private Task<string> GetNCATokenAsync()
        {
            // OAuth2 token retrieval for NCA
            throw new NotImplementedException();
        }

        private object TransformToNCAFormat(ComplianceReportDto report)
        {
            // Transform internal format to NCA-required format
            throw new NotImplementedException();
        }

        private Task<X509Certificate2> LoadZATCACertificateAsync()
        {
            // Load certificate from Azure Key Vault
            throw new NotImplementedException();
        }

        private Task<object> SignInvoiceAsync(EInvoiceDto invoice, X509Certificate2 certificate)
        {
            // XML digital signature for ZATCA
            throw new NotImplementedException();
        }

        private Task LogSubmissionAsync(SubmissionResult result)
        {
            // Audit logging
            throw new NotImplementedException();
        }
    }
}


// ========================================
// 4. MULTI-TENANCY CONFIGURATION
// ========================================

namespace GRC.EntityFrameworkCore
{
    public class GRCDbContext : AbpDbContext<GRCDbContext>
    {
        // Framework entities (shared)
        public DbSet<Framework> Frameworks { get; set; }
        public DbSet<Control> Controls { get; set; }
        public DbSet<Regulator> Regulators { get; set; }

        // Tenant-specific entities
        public DbSet<ControlImplementation> ControlImplementations { get; set; }
        public DbSet<Evidence> Evidences { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<IoTDevice> IoTDevices { get; set; }

        public GRCDbContext(DbContextOptions<GRCDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure multi-tenant entities
            builder.Entity<ControlImplementation>(b =>
            {
                b.ToTable("ControlImplementations");
                b.HasOne(x => x.Control)
                    .WithMany()
                    .HasForeignKey(x => x.ControlId);
                
                // Multi-tenancy filter applied automatically by ABP
            });

            builder.Entity<Evidence>(b =>
            {
                b.ToTable("Evidences");
            });

            builder.Entity<Assessment>(b =>
            {
                b.ToTable("Assessments");
            });

            builder.Entity<Risk>(b =>
            {
                b.ToTable("Risks");
            });

            builder.Entity<IoTDevice>(b =>
            {
                b.ToTable("IoTDevices");
            });

            // Framework and Control are shared (not multi-tenant)
            builder.Entity<Framework>(b =>
            {
                b.ToTable("Frameworks");
                b.HasIndex(x => x.Code);
            });

            builder.Entity<Control>(b =>
            {
                b.ToTable("Controls");
                b.HasIndex(x => new { x.FrameworkId, x.ControlNumber });
            });
        }
    }
}


// ========================================
// 5. ABP MODULE CONFIGURATION
// ========================================

namespace GRC.Domain
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(AbpMultiTenancyModule)
    )]
    public class GRCDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpMultiTenancyOptions>(options =>
            {
                // Enable multi-tenancy
                options.IsEnabled = true;
            });
        }
    }
}

namespace GRC.Application
{
    [DependsOn(
        typeof(GRCDomainModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpBackgroundJobsModule)
    )]
    public class GRCApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // Configure AutoMapper
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<GRCApplicationModule>();
            });

            // Configure background jobs for compliance monitoring
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = true;
            });
        }
    }
}


// ========================================
// 6. PERMISSION DEFINITIONS
// ========================================

namespace GRC.Application.Authorization
{
    public static class GRCPermissions
    {
        public const string GroupName = "GRC";

        public static class Frameworks
        {
            public const string Default = GroupName + ".Frameworks";
            public const string View = Default + ".View";
            public const string Manage = Default + ".Manage";
        }

        public static class Compliance
        {
            public const string Default = GroupName + ".Compliance";
            public const string View = Default + ".View";
            public const string Edit = Default + ".Edit";
            public const string UploadEvidence = Default + ".UploadEvidence";
            public const string VerifyEvidence = Default + ".VerifyEvidence";
            public const string GenerateReport = Default + ".GenerateReport";
            public const string SubmitToRegulator = Default + ".SubmitToRegulator";
        }

        public static class Assessments
        {
            public const string Default = GroupName + ".Assessments";
            public const string Create = Default + ".Create";
            public const string Conduct = Default + ".Conduct";
            public const string Complete = Default + ".Complete";
        }

        public static class Risks
        {
            public const string Default = GroupName + ".Risks";
            public const string View = Default + ".View";
            public const string Create = Default + ".Create";
            public const string Assess = Default + ".Assess";
            public const string Treat = Default + ".Treat";
        }

        public static class IoT
        {
            public const string Default = GroupName + ".IoT";
            public const string ManageDevices = Default + ".ManageDevices";
            public const string SecurityAssessment = Default + ".SecurityAssessment";
        }

        public static class External
        {
            public const string Consultant = GroupName + ".External.Consultant";
            public const string Auditor = GroupName + ".External.Auditor";
            public const string RegulatorAccess = GroupName + ".External.Regulator";
        }
    }

    public class GRCPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var grcGroup = context.AddGroup(GRCPermissions.GroupName, 
                L("Permission:GRC"));

            // Framework permissions
            var frameworkPermission = grcGroup.AddPermission(
                GRCPermissions.Frameworks.Default, L("Permission:Frameworks"));
            frameworkPermission.AddChild(GRCPermissions.Frameworks.View);
            frameworkPermission.AddChild(GRCPermissions.Frameworks.Manage);

            // Compliance permissions
            var compliancePermission = grcGroup.AddPermission(
                GRCPermissions.Compliance.Default, L("Permission:Compliance"));
            compliancePermission.AddChild(GRCPermissions.Compliance.View);
            compliancePermission.AddChild(GRCPermissions.Compliance.Edit);
            compliancePermission.AddChild(GRCPermissions.Compliance.UploadEvidence);
            compliancePermission.AddChild(GRCPermissions.Compliance.VerifyEvidence);
            compliancePermission.AddChild(GRCPermissions.Compliance.GenerateReport);
            compliancePermission.AddChild(GRCPermissions.Compliance.SubmitToRegulator);

            // Additional permissions...
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<GRCResource>(name);
        }
    }
}
