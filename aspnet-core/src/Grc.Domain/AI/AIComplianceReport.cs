using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.AI;

/// <summary>
/// تقرير امتثال مولد بالذكاء الاصطناعي - AI-Generated Compliance Report
/// Comprehensive compliance report with AI-powered insights and recommendations
/// </summary>
public class AIComplianceReport : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// عنوان التقرير - Report title (bilingual)
    /// </summary>
    public LocalizedString Title { get; private set; }
    
    /// <summary>
    /// ملخص تنفيذي - Executive summary
    /// </summary>
    public LocalizedString ExecutiveSummary { get; private set; }
    
    /// <summary>
    /// Assessment this report is based on
    /// </summary>
    public Guid? AssessmentId { get; private set; }
    
    /// <summary>
    /// Framework analyzed
    /// </summary>
    public Guid? FrameworkId { get; private set; }
    
    /// <summary>
    /// Report type
    /// </summary>
    public ComplianceReportType ReportType { get; private set; }
    
    /// <summary>
    /// Report status
    /// </summary>
    public AIAnalysisStatus Status { get; private set; }
    
    /// <summary>
    /// Report period start
    /// </summary>
    public DateTime PeriodStart { get; private set; }
    
    /// <summary>
    /// Report period end
    /// </summary>
    public DateTime PeriodEnd { get; private set; }
    
    /// <summary>
    /// Overall compliance score (0-100)
    /// </summary>
    public decimal OverallComplianceScore { get; private set; }
    
    /// <summary>
    /// AI confidence in this report (0-100)
    /// </summary>
    public decimal ConfidenceScore { get; private set; }
    
    /// <summary>
    /// AI model used
    /// </summary>
    public string AIModel { get; private set; }
    
    /// <summary>
    /// Model version
    /// </summary>
    public string ModelVersion { get; private set; }
    
    /// <summary>
    /// Processing time in seconds
    /// </summary>
    public int ProcessingTimeSeconds { get; private set; }
    
    /// <summary>
    /// Total controls assessed
    /// </summary>
    public int TotalControls { get; private set; }
    
    /// <summary>
    /// Compliant controls
    /// </summary>
    public int CompliantControls { get; private set; }
    
    /// <summary>
    /// Partially compliant controls
    /// </summary>
    public int PartiallyCompliantControls { get; private set; }
    
    /// <summary>
    /// Non-compliant controls
    /// </summary>
    public int NonCompliantControls { get; private set; }
    
    /// <summary>
    /// Not assessed controls
    /// </summary>
    public int NotAssessedControls { get; private set; }
    
    /// <summary>
    /// Critical findings count
    /// </summary>
    public int CriticalFindings { get; private set; }
    
    /// <summary>
    /// High priority findings
    /// </summary>
    public int HighPriorityFindings { get; private set; }
    
    /// <summary>
    /// Medium priority findings
    /// </summary>
    public int MediumPriorityFindings { get; private set; }
    
    /// <summary>
    /// Low priority findings
    /// </summary>
    public int LowPriorityFindings { get; private set; }
    
    /// <summary>
    /// Compliance breakdown by domain
    /// </summary>
    public List<DomainCompliance> DomainBreakdown { get; private set; }
    
    /// <summary>
    /// Key findings
    /// </summary>
    public List<ComplianceFinding> KeyFindings { get; private set; }
    
    /// <summary>
    /// AI-generated recommendations
    /// </summary>
    public List<ComplianceRecommendation> Recommendations { get; private set; }
    
    /// <summary>
    /// Trend analysis (compared to previous period)
    /// </summary>
    public TrendAnalysis? TrendAnalysis { get; set; }
    
    /// <summary>
    /// Risk heat map data
    /// </summary>
    public List<RiskHeatMapItem> RiskHeatMap { get; private set; }
    
    /// <summary>
    /// Report reviewers
    /// </summary>
    public List<Guid> ReviewerIds { get; private set; }
    
    /// <summary>
    /// Review notes
    /// </summary>
    public string? ReviewNotes { get; set; }
    
    /// <summary>
    /// Approved by
    /// </summary>
    public Guid? ApprovedById { get; set; }
    
    /// <summary>
    /// Approval date
    /// </summary>
    public DateTime? ApprovedDate { get; set; }
    
    /// <summary>
    /// PDF file path (after generation)
    /// </summary>
    public string? PdfFilePath { get; set; }
    
    /// <summary>
    /// Report tags
    /// </summary>
    public List<string> Tags { get; private set; }
    
    protected AIComplianceReport()
    {
        DomainBreakdown = new List<DomainCompliance>();
        KeyFindings = new List<ComplianceFinding>();
        Recommendations = new List<ComplianceRecommendation>();
        RiskHeatMap = new List<RiskHeatMapItem>();
        ReviewerIds = new List<Guid>();
        Tags = new List<string>();
    }
    
    public AIComplianceReport(
        Guid id,
        LocalizedString title,
        ComplianceReportType reportType,
        DateTime periodStart,
        DateTime periodEnd,
        Guid? assessmentId = null,
        Guid? frameworkId = null,
        Guid? tenantId = null)
        : base(id)
    {
        Title = Check.NotNull(title, nameof(title));
        ReportType = reportType;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        AssessmentId = assessmentId;
        FrameworkId = frameworkId;
        TenantId = tenantId;
        
        ExecutiveSummary = new(string.Empty, string.Empty);
        Status = AIAnalysisStatus.Pending;
        AIModel = "GPT-4";
        ModelVersion = "1.0";
        
        DomainBreakdown = new List<DomainCompliance>();
        KeyFindings = new List<ComplianceFinding>();
        Recommendations = new List<ComplianceRecommendation>();
        RiskHeatMap = new List<RiskHeatMapItem>();
        ReviewerIds = new List<Guid>();
        Tags = new List<string>();
    }
    
    /// <summary>
    /// بدء توليد التقرير - Start report generation
    /// </summary>
    public void Start(string aiModel, string modelVersion)
    {
        if (Status != AIAnalysisStatus.Pending)
        {
            throw new BusinessException($"Cannot start report generation. Current status: {Status}");
        }
        
        AIModel = Check.NotNullOrWhiteSpace(aiModel, nameof(aiModel));
        ModelVersion = Check.NotNullOrWhiteSpace(modelVersion, nameof(modelVersion));
        Status = AIAnalysisStatus.Running;
    }
    
    /// <summary>
    /// إضافة امتثال المجال - Add domain compliance data
    /// </summary>
    public void AddDomainCompliance(DomainCompliance domain)
    {
        Check.NotNull(domain, nameof(domain));
        DomainBreakdown.Add(domain);
    }
    
    /// <summary>
    /// إضافة نتيجة - Add finding
    /// </summary>
    public void AddFinding(ComplianceFinding finding)
    {
        Check.NotNull(finding, nameof(finding));
        KeyFindings.Add(finding);
        
        // Update counters
        switch (finding.Priority)
        {
            case RecommendationPriority.Critical:
                CriticalFindings++;
                break;
            case RecommendationPriority.High:
                HighPriorityFindings++;
                break;
            case RecommendationPriority.Medium:
                MediumPriorityFindings++;
                break;
            case RecommendationPriority.Low:
                LowPriorityFindings++;
                break;
        }
    }
    
    /// <summary>
    /// إضافة توصية - Add recommendation
    /// </summary>
    public void AddRecommendation(ComplianceRecommendation recommendation)
    {
        Check.NotNull(recommendation, nameof(recommendation));
        Recommendations.Add(recommendation);
    }
    
    /// <summary>
    /// إتمام التقرير - Complete report generation
    /// </summary>
    public void Complete(
        decimal overallComplianceScore,
        decimal confidenceScore,
        int processingTimeSeconds,
        LocalizedString executiveSummary)
    {
        if (Status != AIAnalysisStatus.Running)
        {
            throw new BusinessException($"Cannot complete report. Current status: {Status}");
        }
        
        OverallComplianceScore = Math.Clamp(overallComplianceScore, 0, 100);
        ConfidenceScore = Math.Clamp(confidenceScore, 0, 100);
        ProcessingTimeSeconds = processingTimeSeconds;
        ExecutiveSummary = Check.NotNull(executiveSummary, nameof(executiveSummary));
        
        Status = AIAnalysisStatus.Completed;
        
        CalculateMetrics();
    }
    
    /// <summary>
    /// فشل التوليد - Mark as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = AIAnalysisStatus.Failed;
        ReviewNotes = errorMessage;
    }
    
    /// <summary>
    /// إرسال للمراجعة - Submit for review
    /// </summary>
    public void SubmitForReview()
    {
        if (Status != AIAnalysisStatus.Completed)
        {
            throw new BusinessException("Only completed reports can be submitted for review");
        }
        
        Status = AIAnalysisStatus.UnderReview;
    }
    
    /// <summary>
    /// الموافقة - Approve report
    /// </summary>
    public void Approve(Guid approverId, string? notes = null)
    {
        if (Status != AIAnalysisStatus.UnderReview)
        {
            throw new BusinessException("Only reports under review can be approved");
        }
        
        Status = AIAnalysisStatus.Approved;
        ApprovedById = approverId;
        ApprovedDate = DateTime.UtcNow;
        ReviewNotes = notes;
    }
    
    /// <summary>
    /// حساب المقاييس - Calculate metrics from data
    /// </summary>
    private void CalculateMetrics()
    {
        // Calculate total controls
        TotalControls = CompliantControls + PartiallyCompliantControls + 
                        NonCompliantControls + NotAssessedControls;
        
        // Recalculate from domain breakdown if available
        if (DomainBreakdown.Count > 0)
        {
            TotalControls = 0;
            CompliantControls = 0;
            PartiallyCompliantControls = 0;
            NonCompliantControls = 0;
            
            foreach (var domain in DomainBreakdown)
            {
                TotalControls += domain.TotalControls;
                CompliantControls += domain.CompliantControls;
                PartiallyCompliantControls += domain.PartiallyCompliantControls;
                NonCompliantControls += domain.NonCompliantControls;
            }
            
            NotAssessedControls = TotalControls - CompliantControls - 
                                  PartiallyCompliantControls - NonCompliantControls;
        }
    }
}

/// <summary>
/// نوع تقرير الامتثال - Compliance Report Type
/// </summary>
public enum ComplianceReportType
{
    /// <summary>
    /// تقييم شامل - Full Assessment Report
    /// </summary>
    FullAssessment = 1,
    
    /// <summary>
    /// تقرير دوري - Periodic Compliance Report
    /// </summary>
    Periodic = 2,
    
    /// <summary>
    /// تقرير الفجوات - Gap Analysis Report
    /// </summary>
    GapAnalysis = 3,
    
    /// <summary>
    /// ملخص تنفيذي - Executive Summary
    /// </summary>
    ExecutiveSummary = 4,
    
    /// <summary>
    /// تقرير المخاطر - Risk Report
    /// </summary>
    RiskReport = 5,
    
    /// <summary>
    /// تقرير التدقيق - Audit Report
    /// </summary>
    AuditReport = 6,
    
    /// <summary>
    /// تقرير مخصص - Custom Report
    /// </summary>
    Custom = 99
}

/// <summary>
/// امتثال المجال - Domain Compliance Breakdown
/// </summary>
public class DomainCompliance
{
    public Guid DomainId { get; set; }
    public LocalizedString DomainName { get; set; } = default!;
    public int TotalControls { get; set; }
    public int CompliantControls { get; set; }
    public int PartiallyCompliantControls { get; set; }
    public int NonCompliantControls { get; set; }
    public decimal CompliancePercentage { get; set; }
}

/// <summary>
/// نتيجة امتثال - Compliance Finding
/// </summary>
public class ComplianceFinding
{
    public Guid? ControlId { get; set; }
    public string ControlNumber { get; set; } = default!;
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Description { get; set; } = default!;
    public RecommendationPriority Priority { get; set; }
    public LocalizedString Impact { get; set; } = default!;
    public LocalizedString Recommendation { get; set; } = default!;
    public decimal Confidence { get; set; }
}

/// <summary>
/// توصية امتثال - Compliance Recommendation
/// </summary>
public class ComplianceRecommendation
{
    public LocalizedString Title { get; set; } = default!;
    public LocalizedString Recommendation { get; set; } = default!;
    public RecommendationPriority Priority { get; set; }
    public string Category { get; set; } = default!;
    public List<Guid> RelatedControlIds { get; set; } = new();
    public LocalizedString ExpectedOutcome { get; set; } = default!;
    public int EstimatedDays { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal Confidence { get; set; }
}

/// <summary>
/// تحليل الاتجاه - Trend Analysis
/// </summary>
public class TrendAnalysis
{
    public decimal PreviousComplianceScore { get; set; }
    public decimal CurrentComplianceScore { get; set; }
    public decimal ChangePercentage => CurrentComplianceScore - PreviousComplianceScore;
    public TrendDirection Direction => ChangePercentage > 0 ? TrendDirection.Improving : 
                                        ChangePercentage < 0 ? TrendDirection.Declining : 
                                        TrendDirection.Stable;
    public LocalizedString Analysis { get; set; } = default!;
}

/// <summary>
/// اتجاه الاتجاه - Trend Direction
/// </summary>
public enum TrendDirection
{
    /// <summary>
    /// تحسن - Improving
    /// </summary>
    Improving = 1,
    
    /// <summary>
    /// مستقر - Stable
    /// </summary>
    Stable = 2,
    
    /// <summary>
    /// متدهور - Declining
    /// </summary>
    Declining = 3
}

/// <summary>
/// عنصر خريطة المخاطر - Risk Heat Map Item
/// </summary>
public class RiskHeatMapItem
{
    public Guid? ControlId { get; set; }
    public string ControlNumber { get; set; } = default!;
    public LocalizedString ControlName { get; set; } = default!;
    public int Likelihood { get; set; } // 1-5
    public int Impact { get; set; } // 1-5
    public int RiskScore => Likelihood * Impact; // 1-25
    public string RiskLevel => RiskScore switch
    {
        >= 20 => "Critical",
        >= 12 => "High",
        >= 6 => "Medium",
        _ => "Low"
    };
}
