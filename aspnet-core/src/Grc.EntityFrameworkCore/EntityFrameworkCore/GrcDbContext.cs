using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

// Namespace aliases for clarity and consistency
using VendorEntity = Grc.Vendor.Domain.Vendors.Vendor;
using Assessments = Grc.Assessments;                    // Assessment aggregate root + ControlAssessment
using AssessmentDomain = Grc.Assessment.Domain;          // Teams, Tools, Issues sub-entities
using Product = Grc.Product;                             // Product & Subscription module

namespace Grc.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class GrcDbContext :
    AbpDbContext<GrcDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    // Application Entities
    public DbSet<Evidence.Evidence> Evidences { get; set; }
    public DbSet<Risk.Domain.Risks.Risk> Risks { get; set; }
    public DbSet<Risk.Domain.Risks.RiskTreatment> RiskTreatments { get; set; }
    public DbSet<FrameworkLibrary.Domain.Frameworks.Framework> Frameworks { get; set; }
    public DbSet<FrameworkLibrary.Domain.Frameworks.Control> Controls { get; set; }
    public DbSet<FrameworkLibrary.Domain.Frameworks.FrameworkDomain> FrameworkDomains { get; set; }
    public DbSet<FrameworkLibrary.Domain.Regulators.Regulator> Regulators { get; set; }
    
    // Assessment Sub-Entities (Teams, Tools, Issues) - via AssessmentDomain alias
    public DbSet<AssessmentDomain.Teams.Team> Teams { get; set; }
    public DbSet<AssessmentDomain.Teams.TeamMember> TeamMembers { get; set; }
    public DbSet<AssessmentDomain.Tools.AssessmentTool> AssessmentTools { get; set; }
    public DbSet<AssessmentDomain.Issues.Issue> Issues { get; set; }
    
    // Onboarding Entities
    public DbSet<Onboarding.UserOnboarding> UserOnboardings { get; set; }
    public DbSet<Onboarding.OnboardingTemplate> OnboardingTemplates { get; set; }
    
    // AI Entities
    public DbSet<AI.AIGapAnalysis> AIGapAnalyses { get; set; }
    public DbSet<AI.AutoSchedule> AutoSchedules { get; set; }
    public DbSet<AI.AIComplianceReport> AIComplianceReports { get; set; }

    // GRC Lifecycle Entities - Organizations
    public DbSet<Organizations.Organization> GrcOrganizations { get; set; }
    public DbSet<Organizations.OrganizationFramework> OrganizationFrameworks { get; set; }

    // GRC Lifecycle Entities - Assets
    public DbSet<Assets.Asset> Assets { get; set; }
    public DbSet<Assets.AssetDependency> AssetDependencies { get; set; }
    public DbSet<Assets.AssetRisk> AssetRisks { get; set; }

    // GRC Lifecycle Entities - Gaps
    public DbSet<Gaps.Gap> Gaps { get; set; }
    public DbSet<Gaps.GapRemediation> GapRemediations { get; set; }

    // GRC Lifecycle Entities - Action Items
    public DbSet<ActionItems.ActionItem> ActionItems { get; set; }
    public DbSet<ActionItems.ActionItemComment> ActionItemComments { get; set; }
    public DbSet<ActionItems.ActionItemAttachment> ActionItemAttachments { get; set; }

    // GRC Lifecycle Entities - Audits
    public DbSet<AuditFindings.Audit> Audits { get; set; }
    public DbSet<AuditFindings.AuditFinding> AuditFindings { get; set; }

    // Workflow Entities
    public DbSet<Workflow.WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<Workflow.WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<Workflow.WorkflowTask> WorkflowTasks { get; set; }

    // Vendor Management
    public DbSet<VendorEntity> Vendors { get; set; }

    // Calendar Events
    public DbSet<Calendar.CalendarEvent> CalendarEvents { get; set; }

    // Notifications
    public DbSet<Notification.Domain.Notification> Notifications { get; set; }

    // Integrations
    public DbSet<Integration.IntegrationConnector> IntegrationConnectors { get; set; }

    // Policy Management
    public DbSet<Policy.Domain.Policies.Policy> Policies { get; set; }
    public DbSet<Policy.Domain.Policies.PolicyVersion> PolicyVersions { get; set; }
    public DbSet<Policy.Domain.Policies.PolicyAttestation> PolicyAttestations { get; set; }

    // Control Assessments
    public DbSet<Assessments.ControlAssessment> ControlAssessments { get; set; }
    public DbSet<Assessments.ControlAssessmentComment> ControlAssessmentComments { get; set; }
    public DbSet<Assessments.ControlAssessmentHistory> ControlAssessmentHistories { get; set; }

    // Assessment Aggregate Root (CRITICAL - was missing!)
    public DbSet<Assessments.Assessment> Assessments { get; set; }
    public DbSet<Assessments.AssessmentFramework> AssessmentFrameworks { get; set; }

    // Action Plans (Remediation)
    public DbSet<ActionPlan.ActionPlan> ActionPlans { get; set; }
    public DbSet<ActionPlan.ActionItem> ActionPlanItems { get; set; }

    // Product & Subscription Management (CRITICAL - was missing!)
    public DbSet<Product.Products.Product> Products { get; set; }
    public DbSet<Product.Products.ProductFeature> ProductFeatures { get; set; }
    public DbSet<Product.Products.ProductQuota> ProductQuotas { get; set; }
    public DbSet<Product.Products.PricingPlan> PricingPlans { get; set; }
    public DbSet<Product.Subscriptions.TenantSubscription> TenantSubscriptions { get; set; }
    public DbSet<Product.Subscriptions.QuotaUsage> QuotaUsages { get; set; }

    #endregion

    public GrcDbContext(DbContextOptions<GrcDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        // Configure Regulator with owned ContactInfo and LocalizedString
        builder.Entity<FrameworkLibrary.Domain.Regulators.Regulator>(b =>
        {
            b.ToTable("Regulators");
            b.ConfigureByConvention(); // Re-enabled to properly configure ABP fields
            
            // Configure nullable string properties
            b.Property(r => r.LogoUrl).IsRequired(false);
            b.Property(r => r.Website).IsRequired(false);
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(r => r.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(256);
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(256);
            });
            
            // Configure Jurisdiction as owned LocalizedString (nullable)
            b.OwnsOne(r => r.Jurisdiction, j =>
            {
                j.Property(ls => ls.En).HasColumnName("JurisdictionEn").HasMaxLength(256).IsRequired(false);
                j.Property(ls => ls.Ar).HasColumnName("JurisdictionAr").HasMaxLength(256).IsRequired(false);
            });
            
            // Configure Contact as owned ContactInfo (nullable)
            b.OwnsOne(r => r.Contact, c =>
            {
                c.Property(ci => ci.Email).HasColumnName("ContactEmail").HasMaxLength(256).IsRequired(false);
                c.Property(ci => ci.Phone).HasColumnName("ContactPhone").HasMaxLength(50).IsRequired(false);
                c.Property(ci => ci.Address).HasColumnName("ContactAddress").HasMaxLength(512).IsRequired(false);
            });
        });

        // Configure Framework with owned LocalizedString
        builder.Entity<FrameworkLibrary.Domain.Frameworks.Framework>(b =>
        {
            b.ToTable("Frameworks");
            b.ConfigureByConvention();
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(f => f.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(256);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(256);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(f => f.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
        });

        // Configure FrameworkDomain with owned LocalizedString
        builder.Entity<FrameworkLibrary.Domain.Frameworks.FrameworkDomain>(b =>
        {
            b.ToTable("FrameworkDomains");
            b.ConfigureByConvention();
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(fd => fd.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(256);
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(256);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(fd => fd.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
        });

        // Configure Control with owned LocalizedString
        builder.Entity<FrameworkLibrary.Domain.Frameworks.Control>(b =>
        {
            b.ToTable("Controls");
            b.ConfigureByConvention();
            
            // Fix the self-referential relationship to use ParentControlId instead of shadow ControlId
            b.HasMany(c => c.ChildControls)
             .WithOne()
             .HasForeignKey(c => c.ParentControlId)
             .IsRequired(false);
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(c => c.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(512);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(512);
            });
            
            // Configure Requirement as owned LocalizedString
            b.OwnsOne(c => c.Requirement, r =>
            {
                r.Property(ls => ls.En).HasColumnName("RequirementEn").HasMaxLength(2000);
                r.Property(ls => ls.Ar).HasColumnName("RequirementAr").HasMaxLength(2000);
            });
            
            // Configure ImplementationGuidance as owned LocalizedString (nullable)
            b.OwnsOne(c => c.ImplementationGuidance, g =>
            {
                g.Property(ls => ls.En).HasColumnName("GuidanceEn").HasMaxLength(2000);
                g.Property(ls => ls.Ar).HasColumnName("GuidanceAr").HasMaxLength(2000);
            });
        });

        // Configure Evidence
        builder.Entity<Evidence.Evidence>(b =>
        {
            b.ToTable("Evidences");
            b.ConfigureByConvention();
        });

        // Configure Risk
        builder.Entity<Risk.Domain.Risks.Risk>(b =>
        {
            b.ToTable("Risks");
            b.ConfigureByConvention();
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(r => r.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(256);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(256);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(r => r.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
        });

        // Configure RiskTreatment
        builder.Entity<Risk.Domain.Risks.RiskTreatment>(b =>
        {
            b.ToTable("RiskTreatments");
            b.ConfigureByConvention();
            
            // Ignore the TreatmentData dictionary - store as JSON string instead
            b.Ignore(rt => rt.TreatmentData);
        });

        // Configure Team - Assessment teams for group-based assessments
        builder.Entity<AssessmentDomain.Teams.Team>(b =>
        {
            b.ToTable("Teams");
            b.ConfigureByConvention();
            
            // Property constraints
            b.Property(t => t.Name)
             .IsRequired()
             .HasMaxLength(200);
            
            b.Property(t => t.Description)
             .HasMaxLength(1024);
            
            b.Property(t => t.Type).IsRequired();
            b.Property(t => t.IsActive).IsRequired();
            
            // Relationships - Team owns its members
            b.HasMany(t => t.Members)
             .WithOne()
             .HasForeignKey(tm => tm.TeamId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
            
            // Multi-tenant index for efficient filtering
            b.HasIndex(t => t.TenantId);
            b.HasIndex(t => t.IsActive);
            b.HasIndex(t => new { t.TenantId, t.Type });
        });

        // Configure TeamMember - Team membership with role and lead status
        builder.Entity<AssessmentDomain.Teams.TeamMember>(b =>
        {
            b.ToTable("TeamMembers");
            b.ConfigureByConvention();
            
            // Property constraints
            b.Property(tm => tm.Role)
             .HasMaxLength(100);
            
            b.Property(tm => tm.TeamId).IsRequired();
            b.Property(tm => tm.UserId).IsRequired();
            b.Property(tm => tm.IsActive).IsRequired();
            
            // Indexes for queries
            b.HasIndex(tm => tm.TenantId);
            b.HasIndex(tm => tm.TeamId);
            b.HasIndex(tm => tm.UserId);
            b.HasIndex(tm => tm.IsLead);
            
            // Prevent duplicate user-team assignments
            b.HasIndex(tm => new { tm.TeamId, tm.UserId }).IsUnique();
        });

        // Configure AssessmentTool
        builder.Entity<AssessmentDomain.Tools.AssessmentTool>(b =>
        {
            b.ToTable("AssessmentTools");
            b.ConfigureByConvention();
        });

        // Configure Issue
        builder.Entity<AssessmentDomain.Issues.Issue>(b =>
        {
            b.ToTable("Issues");
            b.ConfigureByConvention();
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(i => i.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(256);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(256);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(i => i.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
        });

        // Configure UserOnboarding
        builder.Entity<Onboarding.UserOnboarding>(b =>
        {
            b.ToTable("UserOnboardings");
            b.ConfigureByConvention();
            
            // Configure complex properties as JSON
            b.Property(uo => uo.CompletedSteps)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<Onboarding.OnboardingStep>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Onboarding.OnboardingStep>()
             );
            
            b.Property(uo => uo.AssignedRoles)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
             );
            
            b.Property(uo => uo.AssignedOrganizationUnits)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>()
             );
            
            b.Property(uo => uo.EnabledFeatures)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, bool>()
             );
            
            b.Property(uo => uo.UserPreferences)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
             );
            
            // Indexes
            b.HasIndex(uo => uo.UserId);
            b.HasIndex(uo => uo.Status);
            b.HasIndex(uo => new { uo.TenantId, uo.Status });
        });

        // Configure OnboardingTemplate
        builder.Entity<Onboarding.OnboardingTemplate>(b =>
        {
            b.ToTable("OnboardingTemplates");
            b.ConfigureByConvention();
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(ot => ot.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(256);
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(256);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(ot => ot.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
            
            // Configure complex properties as JSON
            b.Property(ot => ot.RequiredSteps)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<Onboarding.OnboardingStep>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Onboarding.OnboardingStep>()
             );
            
            b.Property(ot => ot.DefaultRoles)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
             );
            
            b.Property(ot => ot.DefaultFeatures)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                 v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, bool>()
             );
            
            // Indexes
            b.HasIndex(ot => ot.TargetRole);
            b.HasIndex(ot => ot.IsActive);
        });

        // Configure AIGapAnalysis
        builder.Entity<AI.AIGapAnalysis>(b =>
        {
            b.ToTable("AIGapAnalyses");
            b.ConfigureByConvention();
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(a => a.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(500);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(500);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(a => a.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
            
            // Configure Gaps as JSONB
            b.Property(a => a.Gaps)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.GapDetail>>(v, (JsonSerializerOptions?)null) ?? new List<AI.GapDetail>()
             );
            
            // Configure Recommendations as JSONB
            b.Property(a => a.Recommendations)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.AIRecommendation>>(v, (JsonSerializerOptions?)null) ?? new List<AI.AIRecommendation>()
             );
            
            // Indexes
            b.HasIndex(a => a.TenantId);
            b.HasIndex(a => a.AssessmentId);
            b.HasIndex(a => a.FrameworkId);
            b.HasIndex(a => a.Status);
            b.HasIndex(a => a.AnalysisType);
        });

        // Configure AutoSchedule
        builder.Entity<AI.AutoSchedule>(b =>
        {
            b.ToTable("AutoSchedules");
            b.ConfigureByConvention();
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(s => s.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(200);
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(200);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(s => s.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(1000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });
            
            // Configure NotificationRecipients as JSONB
            b.Property(s => s.NotificationRecipients)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()
             );
            
            // Configure Tags as JSONB
            b.Property(s => s.Tags)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
             );
            
            // Configure RunHistory as JSONB
            b.Property(s => s.RunHistory)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.ScheduleRun>>(v, (JsonSerializerOptions?)null) ?? new List<AI.ScheduleRun>()
             );
            
            // Indexes
            b.HasIndex(s => s.TenantId);
            b.HasIndex(s => s.FrameworkId);
            b.HasIndex(s => s.IsEnabled);
            b.HasIndex(s => s.NextRunDate);
            b.HasIndex(s => new { s.TenantId, s.IsEnabled, s.NextRunDate });
        });

        // Configure AIComplianceReport
        builder.Entity<AI.AIComplianceReport>(b =>
        {
            b.ToTable("AIComplianceReports");
            b.ConfigureByConvention();
            
            // Configure Title as owned LocalizedString
            b.OwnsOne(r => r.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(500);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(500);
            });
            
            // Configure ExecutiveSummary as owned LocalizedString
            b.OwnsOne(r => r.ExecutiveSummary, e =>
            {
                e.Property(ls => ls.En).HasColumnName("ExecutiveSummaryEn").HasColumnType("text");
                e.Property(ls => ls.Ar).HasColumnName("ExecutiveSummaryAr").HasColumnType("text");
            });
            
            // Configure DomainBreakdown as JSONB
            b.Property(r => r.DomainBreakdown)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.DomainCompliance>>(v, (JsonSerializerOptions?)null) ?? new List<AI.DomainCompliance>()
             );
            
            // Configure KeyFindings as JSONB
            b.Property(r => r.KeyFindings)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.ComplianceFinding>>(v, (JsonSerializerOptions?)null) ?? new List<AI.ComplianceFinding>()
             );
            
            // Configure Recommendations as JSONB
            b.Property(r => r.Recommendations)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.ComplianceRecommendation>>(v, (JsonSerializerOptions?)null) ?? new List<AI.ComplianceRecommendation>()
             );
            
            // Configure TrendAnalysis as JSONB (nullable)
            b.Property(r => r.TrendAnalysis)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => v == null ? null : JsonSerializer.Deserialize<AI.TrendAnalysis>(v, (JsonSerializerOptions?)null)
             );
            
            // Configure RiskHeatMap as JSONB
            b.Property(r => r.RiskHeatMap)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<AI.RiskHeatMapItem>>(v, (JsonSerializerOptions?)null) ?? new List<AI.RiskHeatMapItem>()
             );
            
            // Configure ReviewerIds as JSONB
            b.Property(r => r.ReviewerIds)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()
             );
            
            // Configure Tags as JSONB
            b.Property(r => r.Tags)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
             );
            
            // Indexes
            b.HasIndex(r => r.TenantId);
            b.HasIndex(r => r.AssessmentId);
            b.HasIndex(r => r.FrameworkId);
            b.HasIndex(r => r.Status);
            b.HasIndex(r => r.ReportType);
            b.HasIndex(r => new { r.TenantId, r.Status, r.ReportType });
        });
        
        // Configure simple value objects as keyless entities (prevent EF auto-discovery)
        // Only configure the simple ones that don't have complex relationships
        builder.Entity<Grc.AI.ScheduleRun>().HasNoKey();

        // Configure GRC Lifecycle Entities

        // Configure Organization
        builder.Entity<Organizations.Organization>(b =>
        {
            b.ToTable("Organizations");
            b.ConfigureByConvention();
            b.HasMany(o => o.Units).WithOne().HasForeignKey(u => u.OrganizationId);
            b.HasMany(o => o.Frameworks).WithOne().HasForeignKey(f => f.OrganizationId);
        });


        // Configure OrganizationFramework
        builder.Entity<Organizations.OrganizationFramework>(b =>
        {
            b.ToTable("OrganizationFrameworks");
            b.ConfigureByConvention();
        });

        // Configure Asset
        builder.Entity<Assets.Asset>(b =>
        {
            b.ToTable("Assets");
            b.ConfigureByConvention();
            b.HasMany(a => a.Dependencies).WithOne().HasForeignKey(d => d.AssetId);
            b.HasMany(a => a.Risks).WithOne().HasForeignKey(r => r.AssetId);
        });

        // Configure AssetDependency
        builder.Entity<Assets.AssetDependency>(b =>
        {
            b.ToTable("AssetDependencies");
            b.ConfigureByConvention();
        });

        // Configure AssetRisk
        builder.Entity<Assets.AssetRisk>(b =>
        {
            b.ToTable("AssetRisks");
            b.ConfigureByConvention();
        });

        // Configure Gap
        builder.Entity<Gaps.Gap>(b =>
        {
            b.ToTable("Gaps");
            b.ConfigureByConvention();
            b.HasMany(g => g.RemediationSteps).WithOne().HasForeignKey(r => r.GapId);
        });

        // Configure GapRemediation
        builder.Entity<Gaps.GapRemediation>(b =>
        {
            b.ToTable("GapRemediations");
            b.ConfigureByConvention();
        });

        // Configure ActionItem
        builder.Entity<ActionItems.ActionItem>(b =>
        {
            b.ToTable("ActionItems");
            b.ConfigureByConvention();
            b.HasMany(a => a.Comments).WithOne().HasForeignKey(c => c.ActionItemId);
            b.HasMany(a => a.Attachments).WithOne().HasForeignKey(att => att.ActionItemId);
        });

        // Configure ActionItemComment
        builder.Entity<ActionItems.ActionItemComment>(b =>
        {
            b.ToTable("ActionItemComments");
            b.ConfigureByConvention();
        });

        // Configure ActionItemAttachment
        builder.Entity<ActionItems.ActionItemAttachment>(b =>
        {
            b.ToTable("ActionItemAttachments");
            b.ConfigureByConvention();
        });

        // Configure Audit
        builder.Entity<AuditFindings.Audit>(b =>
        {
            b.ToTable("Audits");
            b.ConfigureByConvention();
            b.HasMany(a => a.Findings).WithOne().HasForeignKey(f => f.AuditId);
        });

        // Configure AuditFinding
        builder.Entity<AuditFindings.AuditFinding>(b =>
        {
            b.ToTable("AuditFindings");
            b.ConfigureByConvention();
        });

        // Configure WorkflowDefinition
        builder.Entity<Workflow.WorkflowDefinition>(b =>
        {
            b.ToTable("WorkflowDefinitions");
            b.ConfigureByConvention();
            b.HasMany(d => d.Instances).WithOne().HasForeignKey(i => i.WorkflowDefinitionId);

            // Configure LocalizedString properties
            b.OwnsOne(d => d.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(200).IsRequired();
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(200).IsRequired();
            });

            b.OwnsOne(d => d.Description, desc =>
            {
                desc.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                desc.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });

            // Configure Variables as JSONB
            b.Property(d => d.Variables)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );

            // Configure other properties
            b.Property(d => d.Category).HasMaxLength(100);
            b.HasIndex(d => d.Category);
            b.HasIndex(d => d.Status);
        });

        // Configure WorkflowInstance
        builder.Entity<Workflow.WorkflowInstance>(b =>
        {
            b.ToTable("WorkflowInstances");
            b.ConfigureByConvention();
            b.HasMany(w => w.Tasks).WithOne().HasForeignKey(t => t.WorkflowInstanceId);

            // Configure Variables as JSONB
            b.Property(w => w.Variables)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );
        });

        // Configure WorkflowTask
        builder.Entity<Workflow.WorkflowTask>(b =>
        {
            b.ToTable("WorkflowTasks");
            b.ConfigureByConvention();

            // Configure TaskData as JSONB
            b.Property(t => t.TaskData)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );
        });

        // Configure Vendor
        builder.Entity<VendorEntity>(b =>
        {
            b.ToTable("Vendors");
            b.ConfigureByConvention();

            // Configure Name as owned LocalizedString
            b.OwnsOne(v => v.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(256);
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(256);
            });

            // Configure Contact as owned ContactInfo (nullable)
            b.OwnsOne(v => v.Contact, c =>
            {
                c.Property(ci => ci.Email).HasColumnName("ContactEmail").HasMaxLength(256).IsRequired(false);
                c.Property(ci => ci.Phone).HasColumnName("ContactPhone").HasMaxLength(50).IsRequired(false);
                c.Property(ci => ci.Address).HasColumnName("ContactAddress").HasMaxLength(512).IsRequired(false);
            });

            // Indexes
            b.HasIndex(v => v.VendorName);
            b.HasIndex(v => v.Status);
            b.HasIndex(v => v.RiskScore);
        });

        // Configure CalendarEvent
        builder.Entity<Calendar.CalendarEvent>(b =>
        {
            b.ToTable("CalendarEvents");
            b.ConfigureByConvention();

            // Indexes
            b.HasIndex(e => e.StartDate);
            b.HasIndex(e => e.DueDate);
            b.HasIndex(e => e.Type);
            b.HasIndex(e => e.Status);
            b.HasIndex(e => e.AssignedToEmail);
        });

        // Configure Notification
        builder.Entity<Notification.Domain.Notification>(b =>
        {
            b.ToTable("Notifications");
            b.ConfigureByConvention();

            // Indexes
            b.HasIndex(n => n.RecipientUserId);
            b.HasIndex(n => n.IsRead);
            b.HasIndex(n => n.Type);
            b.HasIndex(n => new { n.RecipientUserId, n.IsRead });
        });

        // Configure IntegrationConnector
        builder.Entity<Integration.IntegrationConnector>(b =>
        {
            b.ToTable("IntegrationConnectors");
            b.ConfigureByConvention();

            // Sensitive data - not queried
            b.Property(i => i.ApiKey).HasMaxLength(512);
            b.Property(i => i.ClientSecret).HasMaxLength(512);
            b.Property(i => i.AccessToken).HasMaxLength(2000);
            b.Property(i => i.RefreshToken).HasMaxLength(2000);
            b.Property(i => i.WebhookSecret).HasMaxLength(256);

            // Indexes
            b.HasIndex(i => i.Name);
            b.HasIndex(i => i.Type);
            b.HasIndex(i => i.Status);
        });

        // Configure Policy
        builder.Entity<Policy.Domain.Policies.Policy>(b =>
        {
            b.ToTable("Policies");
            b.ConfigureByConvention();

            b.Property(p => p.PolicyCode).HasMaxLength(50).IsRequired();
            b.Property(p => p.Category).HasMaxLength(100);

            // Configure Title as owned LocalizedString
            b.OwnsOne(p => p.Title, t =>
            {
                t.Property(ls => ls.En).HasColumnName("TitleEn").HasMaxLength(256);
                t.Property(ls => ls.Ar).HasColumnName("TitleAr").HasMaxLength(256);
            });

            // Configure Description as owned LocalizedString
            b.OwnsOne(p => p.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });

            // Relationships
            b.HasMany(p => p.Versions)
             .WithOne()
             .HasForeignKey(v => v.PolicyId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.Attestations)
             .WithOne()
             .HasForeignKey(a => a.PolicyId)
             .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(p => p.PolicyCode).IsUnique();
            b.HasIndex(p => p.IsActive);
            b.HasIndex(p => p.OwnerUserId);
        });

        // Configure PolicyVersion
        builder.Entity<Policy.Domain.Policies.PolicyVersion>(b =>
        {
            b.ToTable("PolicyVersions");
            b.ConfigureByConvention();

            b.Property(v => v.VersionNumber).HasMaxLength(20).IsRequired();
            b.Property(v => v.Content).HasColumnType("text");
            b.Property(v => v.ChangeSummary).HasMaxLength(1000);

            // Indexes
            b.HasIndex(v => v.PolicyId);
            b.HasIndex(v => v.IsCurrentVersion);
        });

        // Configure PolicyAttestation
        builder.Entity<Policy.Domain.Policies.PolicyAttestation>(b =>
        {
            b.ToTable("PolicyAttestations");
            b.ConfigureByConvention();

            b.Property(a => a.IpAddress).HasMaxLength(50);

            // Indexes
            b.HasIndex(a => a.PolicyId);
            b.HasIndex(a => a.UserId);
            b.HasIndex(a => new { a.PolicyId, a.UserId });
        });

        // Configure Assessment Aggregate Root - the main assessment entity
        builder.Entity<Assessments.Assessment>(b =>
        {
            b.ToTable("Assessments");
            b.ConfigureByConvention();
            
            // Property constraints
            b.Property(a => a.Name).IsRequired().HasMaxLength(200);
            b.Property(a => a.Description).HasMaxLength(2000);
            b.Property(a => a.StartDate).IsRequired();
            b.Property(a => a.TargetEndDate).IsRequired();
            
            // Configure Scope as JSONB for flexible metadata storage
            b.Property(a => a.Scope)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );
            
            // Relationships - Assessment owns its frameworks (cascade delete)
            b.HasMany(a => a.Frameworks)
             .WithOne()
             .HasForeignKey(af => af.AssessmentId)
             .OnDelete(DeleteBehavior.Cascade);
            
            // Relationships - Assessment owns its control assessments (cascade delete)
            b.HasMany(a => a.ControlAssessments)
             .WithOne()
             .HasForeignKey(ca => ca.AssessmentId)
             .OnDelete(DeleteBehavior.Cascade);
            
            // Multi-tenant index (critical for tenant isolation)
            b.HasIndex(a => a.TenantId);
            
            // Query optimization indexes
            b.HasIndex(a => a.Status);
            b.HasIndex(a => a.Type);
            b.HasIndex(a => a.OwnerUserId);
            b.HasIndex(a => a.StartDate);
            b.HasIndex(a => a.TargetEndDate);
            
            // Composite indexes for common query patterns
            b.HasIndex(a => new { a.TenantId, a.Status });
            b.HasIndex(a => new { a.TenantId, a.Type });
            b.HasIndex(a => new { a.TenantId, a.OwnerUserId });
        });

        // Configure AssessmentFramework (junction table linking Assessments to Frameworks)
        // Uses Id as PK (Entity<Guid>) with unique constraint on (AssessmentId, FrameworkId)
        builder.Entity<Assessments.AssessmentFramework>(b =>
        {
            b.ToTable("AssessmentFrameworks");
            b.ConfigureByConvention();
            
            // Required foreign keys
            b.Property(af => af.AssessmentId).IsRequired();
            b.Property(af => af.FrameworkId).IsRequired();
            b.Property(af => af.IsMandatory).IsRequired();
            b.Property(af => af.CreationTime).IsRequired();
            
            // Relationship to Assessment - cascade delete when assessment is deleted
            b.HasOne<Assessments.Assessment>()
             .WithMany(a => a.Frameworks)
             .HasForeignKey(af => af.AssessmentId)
             .OnDelete(DeleteBehavior.Cascade);
            
            // Relationship to Framework - restrict delete (can't delete framework if linked)
            b.HasOne<FrameworkLibrary.Domain.Frameworks.Framework>()
             .WithMany()
             .HasForeignKey(af => af.FrameworkId)
             .OnDelete(DeleteBehavior.Restrict);
            
            // Multi-tenant index (required for IMultiTenant entities)
            b.HasIndex(af => af.TenantId);
            
            // Composite unique index (an assessment can only link to a framework once per tenant)
            b.HasIndex(af => new { af.TenantId, af.AssessmentId, af.FrameworkId }).IsUnique();
            
            // Query optimization indexes
            b.HasIndex(af => af.AssessmentId);
            b.HasIndex(af => af.FrameworkId);
        });

        // Configure ControlAssessment - Individual control assessments within an Assessment
        builder.Entity<Assessments.ControlAssessment>(b =>
        {
            b.ToTable("ControlAssessments");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(c => c.ImplementationNotes).HasMaxLength(4000);
            b.Property(c => c.RejectionReason).HasMaxLength(1000);
            b.Property(c => c.AssessmentId).IsRequired();
            b.Property(c => c.ControlId).IsRequired();

            // Relationships - ControlAssessment owns its comments and history
            b.HasMany(c => c.Comments)
             .WithOne()
             .HasForeignKey(c => c.ControlAssessmentId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(c => c.History)
             .WithOne()
             .HasForeignKey(h => h.ControlAssessmentId)
             .OnDelete(DeleteBehavior.Cascade);

            // Multi-tenant index (critical for tenant isolation)
            b.HasIndex(c => c.TenantId);
            
            // Query optimization indexes
            b.HasIndex(c => c.AssessmentId);
            b.HasIndex(c => c.ControlId);
            b.HasIndex(c => c.Status);
            b.HasIndex(c => c.AssignedToUserId);
            b.HasIndex(c => c.DueDate);
            
            // Composite indexes for common query patterns
            b.HasIndex(c => new { c.TenantId, c.AssessmentId });
            b.HasIndex(c => new { c.TenantId, c.Status });
            b.HasIndex(c => new { c.AssessmentId, c.ControlId }).IsUnique();
        });

        // Configure ControlAssessmentComment - Comments on control assessments
        builder.Entity<Assessments.ControlAssessmentComment>(b =>
        {
            b.ToTable("ControlAssessmentComments");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(c => c.Comment).IsRequired().HasMaxLength(4000);
            b.Property(c => c.ControlAssessmentId).IsRequired();
            b.Property(c => c.UserId).IsRequired();
            b.Property(c => c.CreationTime).IsRequired();

            // Multi-tenant index (required for IMultiTenant entities)
            b.HasIndex(c => c.TenantId);
            
            // Query indexes
            b.HasIndex(c => c.ControlAssessmentId);
            b.HasIndex(c => c.UserId);
            b.HasIndex(c => c.CreationTime);
        });

        // Configure ControlAssessmentHistory - Audit trail for control assessments
        builder.Entity<Assessments.ControlAssessmentHistory>(b =>
        {
            b.ToTable("ControlAssessmentHistories");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(h => h.Action).IsRequired().HasMaxLength(100);
            b.Property(h => h.Details).HasMaxLength(2000);
            b.Property(h => h.ControlAssessmentId).IsRequired();
            b.Property(h => h.Timestamp).IsRequired();

            // Configure OldValues/NewValues as JSONB for flexible change tracking
            b.Property(h => h.OldValues)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );

            b.Property(h => h.NewValues)
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
             );

            // Multi-tenant index (required for IMultiTenant entities)
            b.HasIndex(h => h.TenantId);
            
            // Query indexes
            b.HasIndex(h => h.ControlAssessmentId);
            b.HasIndex(h => h.Timestamp);
            b.HasIndex(h => h.Action);
        });

        // Configure ActionPlan
        builder.Entity<ActionPlan.ActionPlan>(b =>
        {
            b.ToTable("ActionPlans");
            b.ConfigureByConvention();

            b.Property(a => a.Name).HasMaxLength(256).IsRequired();
            b.Property(a => a.Description).HasMaxLength(2000);

            // Relationships
            b.HasMany(a => a.ActionItems)
             .WithOne()
             .HasForeignKey(i => i.ActionPlanId)
             .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(a => a.Status);
            b.HasIndex(a => a.OwnerUserId);
            b.HasIndex(a => a.AssessmentId);
            b.HasIndex(a => a.RiskId);
        });

        // Configure ActionPlanItem (ActionItem from ActionPlan.Domain)
        builder.Entity<ActionPlan.ActionItem>(b =>
        {
            b.ToTable("ActionPlanItems");
            b.ConfigureByConvention();

            b.Property(i => i.Title).HasMaxLength(256).IsRequired();
            b.Property(i => i.Description).HasMaxLength(2000);
            b.Property(i => i.CompletionNotes).HasMaxLength(2000);

            // Indexes
            b.HasIndex(i => i.ActionPlanId);
            b.HasIndex(i => i.Status);
            b.HasIndex(i => i.AssignedToUserId);
            b.HasIndex(i => i.DueDate);
        });

        // =====================================================
        // PRODUCT & SUBSCRIPTION MODULE CONFIGURATION
        // =====================================================
        
        // Configure Product - Subscription product catalog entry (shared data, no IMultiTenant)
        builder.Entity<Product.Products.Product>(b =>
        {
            b.ToTable("Products");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(p => p.Code).IsRequired().HasMaxLength(50);
            b.Property(p => p.IconUrl).HasMaxLength(500);
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(p => p.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(200).IsRequired();
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(200).IsRequired();
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(p => p.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(2000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(2000);
            });
            
            // Configure Metadata as JSONB
            b.Property(p => p.Metadata)
             .HasColumnType("jsonb");

            // Relationships - Product owns its features, quotas, and pricing plans
            b.HasMany(p => p.Features)
             .WithOne()
             .HasForeignKey(f => f.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.Quotas)
             .WithOne()
             .HasForeignKey(q => q.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.PricingPlans)
             .WithOne()
             .HasForeignKey(pp => pp.ProductId)
             .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            b.HasIndex(p => p.Code).IsUnique();
            b.HasIndex(p => p.Category);
            b.HasIndex(p => p.IsActive);
            b.HasIndex(p => p.DisplayOrder);
        });

        // Configure ProductFeature - Features included in a product
        builder.Entity<Product.Products.ProductFeature>(b =>
        {
            b.ToTable("ProductFeatures");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(f => f.ProductId).IsRequired();
            b.Property(f => f.FeatureCode).IsRequired().HasMaxLength(100);
            b.Property(f => f.Value).HasMaxLength(500);
            
            // Configure Name as owned LocalizedString
            b.OwnsOne(f => f.Name, n =>
            {
                n.Property(ls => ls.En).HasColumnName("NameEn").HasMaxLength(200).IsRequired();
                n.Property(ls => ls.Ar).HasColumnName("NameAr").HasMaxLength(200);
            });
            
            // Configure Description as owned LocalizedString
            b.OwnsOne(f => f.Description, d =>
            {
                d.Property(ls => ls.En).HasColumnName("DescriptionEn").HasMaxLength(1000);
                d.Property(ls => ls.Ar).HasColumnName("DescriptionAr").HasMaxLength(1000);
            });

            // Indexes
            b.HasIndex(f => f.ProductId);
            b.HasIndex(f => new { f.ProductId, f.FeatureCode }).IsUnique();
        });

        // Configure ProductQuota - Quota limits for a product
        builder.Entity<Product.Products.ProductQuota>(b =>
        {
            b.ToTable("ProductQuotas");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(q => q.ProductId).IsRequired();
            b.Property(q => q.Unit).HasMaxLength(50);

            // Indexes
            b.HasIndex(q => q.ProductId);
            b.HasIndex(q => new { q.ProductId, q.QuotaType }).IsUnique();
        });

        // Configure PricingPlan - Pricing plans for products
        builder.Entity<Product.Products.PricingPlan>(b =>
        {
            b.ToTable("PricingPlans");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(pp => pp.ProductId).IsRequired();
            b.Property(pp => pp.Currency).IsRequired().HasMaxLength(3);
            b.Property(pp => pp.StripePriceId).HasMaxLength(100);
            b.Property(pp => pp.Price).HasPrecision(18, 2);

            // Indexes
            b.HasIndex(pp => pp.ProductId);
            b.HasIndex(pp => pp.IsActive);
            b.HasIndex(pp => pp.StripePriceId);
        });

        // Configure TenantSubscription - Tenant's subscription to a product
        builder.Entity<Product.Subscriptions.TenantSubscription>(b =>
        {
            b.ToTable("TenantSubscriptions");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(ts => ts.ProductId).IsRequired();
            b.Property(ts => ts.StartDate).IsRequired();
            b.Property(ts => ts.CancellationReason).HasMaxLength(1000);
            b.Property(ts => ts.StripeSubscriptionId).HasMaxLength(100);

            // Relationship to Product
            b.HasOne<Product.Products.Product>()
             .WithMany()
             .HasForeignKey(ts => ts.ProductId)
             .OnDelete(DeleteBehavior.Restrict);

            // Relationship to PricingPlan (optional)
            b.HasOne<Product.Products.PricingPlan>()
             .WithMany()
             .HasForeignKey(ts => ts.PricingPlanId)
             .OnDelete(DeleteBehavior.Restrict);

            // Multi-tenant index (required for IMultiTenant entities)
            b.HasIndex(ts => ts.TenantId);
            
            // Query indexes
            b.HasIndex(ts => ts.Status);
            b.HasIndex(ts => ts.ProductId);
            b.HasIndex(ts => ts.StartDate);
            b.HasIndex(ts => ts.EndDate);
            b.HasIndex(ts => ts.StripeSubscriptionId);
            
            // Composite indexes
            b.HasIndex(ts => new { ts.TenantId, ts.Status });
            b.HasIndex(ts => new { ts.TenantId, ts.ProductId });
        });

        // Configure QuotaUsage - Track quota usage per tenant
        builder.Entity<Product.Subscriptions.QuotaUsage>(b =>
        {
            b.ToTable("QuotaUsages");
            b.ConfigureByConvention();

            // Property constraints
            b.Property(qu => qu.CurrentUsage).HasPrecision(18, 2);
            b.Property(qu => qu.LastUpdated).IsRequired();

            // Multi-tenant index (required for IMultiTenant entities)
            b.HasIndex(qu => qu.TenantId);
            
            // Query indexes
            b.HasIndex(qu => qu.QuotaType);
            b.HasIndex(qu => qu.ResetDate);
            
            // Unique constraint - one usage record per tenant per quota type
            b.HasIndex(qu => new { qu.TenantId, qu.QuotaType }).IsUnique();
        });
    }
}
