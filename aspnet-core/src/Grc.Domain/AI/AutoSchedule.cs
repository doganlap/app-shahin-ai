using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Grc.ValueObjects;

namespace Grc.AI;

/// <summary>
/// جدولة تلقائية للتقييمات - Automatic Assessment Scheduling
/// AI-powered intelligent scheduling for compliance assessments
/// </summary>
public class AutoSchedule : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    
    /// <summary>
    /// اسم الجدول - Schedule name (bilingual)
    /// </summary>
    public LocalizedString Name { get; private set; }
    
    /// <summary>
    /// الوصف - Description
    /// </summary>
    public LocalizedString Description { get; private set; }
    
    /// <summary>
    /// Framework to assess
    /// </summary>
    public Guid FrameworkId { get; private set; }
    
    /// <summary>
    /// Schedule type (Daily, Weekly, Monthly, Quarterly, Annually, Custom)
    /// </summary>
    public ScheduleFrequency Frequency { get; private set; }
    
    /// <summary>
    /// Custom cron expression (for advanced scheduling)
    /// </summary>
    public string? CronExpression { get; set; }
    
    /// <summary>
    /// Next scheduled run
    /// </summary>
    public DateTime? NextRunDate { get; private set; }
    
    /// <summary>
    /// Last run date
    /// </summary>
    public DateTime? LastRunDate { get; private set; }
    
    /// <summary>
    /// Is schedule enabled
    /// </summary>
    public bool IsEnabled { get; private set; }
    
    /// <summary>
    /// Auto-assign team members
    /// </summary>
    public bool AutoAssignTeam { get; private set; }
    
    /// <summary>
    /// Team template to use for assignment
    /// </summary>
    public Guid? TeamTemplateId { get; set; }
    
    /// <summary>
    /// Auto-create gap analysis after assessment
    /// </summary>
    public bool AutoGenerateGapAnalysis { get; private set; }
    
    /// <summary>
    /// Auto-send notifications
    /// </summary>
    public bool AutoSendNotifications { get; private set; }
    
    /// <summary>
    /// Notification recipients (user IDs)
    /// </summary>
    public List<Guid> NotificationRecipients { get; private set; }
    
    /// <summary>
    /// Auto-generate AI recommendations
    /// </summary>
    public bool AutoGenerateAIRecommendations { get; private set; }
    
    /// <summary>
    /// AI model to use for analysis
    /// </summary>
    public string AIModel { get; private set; } = "GPT-4";
    
    /// <summary>
    /// Assessment template to use
    /// </summary>
    public Guid? AssessmentTemplateId { get; set; }
    
    /// <summary>
    /// SLA days for completion
    /// </summary>
    public int SLADays { get; private set; }
    
    /// <summary>
    /// Tags for categorization
    /// </summary>
    public List<string> Tags { get; private set; }
    
    /// <summary>
    /// Total runs executed
    /// </summary>
    public int TotalRuns { get; private set; }
    
    /// <summary>
    /// Successful runs
    /// </summary>
    public int SuccessfulRuns { get; private set; }
    
    /// <summary>
    /// Failed runs
    /// </summary>
    public int FailedRuns { get; private set; }
    
    /// <summary>
    /// Last error message
    /// </summary>
    public string? LastErrorMessage { get; set; }
    
    /// <summary>
    /// Schedule history
    /// </summary>
    public List<ScheduleRun> RunHistory { get; private set; }
    
    protected AutoSchedule()
    {
        NotificationRecipients = new List<Guid>();
        Tags = new List<string>();
        RunHistory = new List<ScheduleRun>();
    }
    
    public AutoSchedule(
        Guid id,
        LocalizedString name,
        Guid frameworkId,
        ScheduleFrequency frequency,
        Guid? tenantId = null)
        : base(id)
    {
        Name = Check.NotNull(name, nameof(name));
        FrameworkId = frameworkId;
        Frequency = frequency;
        TenantId = tenantId;
        
        Description = new(string.Empty, string.Empty);
        IsEnabled = true;
        AutoAssignTeam = true;
        AutoGenerateGapAnalysis = true;
        AutoSendNotifications = true;
        AutoGenerateAIRecommendations = true;
        SLADays = 30;
        
        NotificationRecipients = new List<Guid>();
        Tags = new List<string>();
        RunHistory = new List<ScheduleRun>();
        
        CalculateNextRunDate();
    }
    
    /// <summary>
    /// تفعيل الجدول - Enable the schedule
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
        CalculateNextRunDate();
    }
    
    /// <summary>
    /// تعطيل الجدول - Disable the schedule
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
    }
    
    /// <summary>
    /// تحديث التكرار - Update frequency
    /// </summary>
    public void UpdateFrequency(ScheduleFrequency frequency, string? cronExpression = null)
    {
        Frequency = frequency;
        
        if (frequency == ScheduleFrequency.Custom)
        {
            CronExpression = Check.NotNullOrWhiteSpace(cronExpression, nameof(cronExpression));
        }
        else
        {
            CronExpression = null;
        }
        
        CalculateNextRunDate();
    }
    
    /// <summary>
    /// إضافة مستلم للإشعارات - Add notification recipient
    /// </summary>
    public void AddNotificationRecipient(Guid userId)
    {
        if (!NotificationRecipients.Contains(userId))
        {
            NotificationRecipients.Add(userId);
        }
    }
    
    /// <summary>
    /// إزالة مستلم - Remove notification recipient
    /// </summary>
    public void RemoveNotificationRecipient(Guid userId)
    {
        NotificationRecipients.Remove(userId);
    }
    
    /// <summary>
    /// تسجيل تشغيل - Record a schedule run
    /// </summary>
    public void RecordRun(Guid assessmentId, bool success, string? errorMessage = null)
    {
        TotalRuns++;
        
        if (success)
        {
            SuccessfulRuns++;
            LastErrorMessage = null;
        }
        else
        {
            FailedRuns++;
            LastErrorMessage = errorMessage;
        }
        
        LastRunDate = DateTime.UtcNow;
        
        RunHistory.Add(new ScheduleRun
        {
            RunDate = LastRunDate.Value,
            AssessmentId = assessmentId,
            Success = success,
            ErrorMessage = errorMessage
        });
        
        // Keep only last 50 runs in history
        if (RunHistory.Count > 50)
        {
            RunHistory.RemoveAt(0);
        }
        
        CalculateNextRunDate();
    }
    
    /// <summary>
    /// حساب تاريخ التشغيل القادم - Calculate next run date
    /// </summary>
    private void CalculateNextRunDate()
    {
        if (!IsEnabled)
        {
            NextRunDate = null;
            return;
        }
        
        var baseDate = LastRunDate ?? DateTime.UtcNow;
        
        NextRunDate = Frequency switch
        {
            ScheduleFrequency.Daily => baseDate.AddDays(1),
            ScheduleFrequency.Weekly => baseDate.AddDays(7),
            ScheduleFrequency.BiWeekly => baseDate.AddDays(14),
            ScheduleFrequency.Monthly => baseDate.AddMonths(1),
            ScheduleFrequency.Quarterly => baseDate.AddMonths(3),
            ScheduleFrequency.SemiAnnually => baseDate.AddMonths(6),
            ScheduleFrequency.Annually => baseDate.AddYears(1),
            ScheduleFrequency.Custom => ParseCronExpression(baseDate),
            _ => throw new ArgumentOutOfRangeException(nameof(Frequency))
        };
    }
    
    private DateTime ParseCronExpression(DateTime fromDate)
    {
        // Simplified cron parsing - in production, use NCronTab or similar
        // For now, default to daily
        return fromDate.AddDays(1);
    }
    
    /// <summary>
    /// هل حان وقت التشغيل - Is it time to run
    /// </summary>
    public bool IsDueToRun()
    {
        return IsEnabled && NextRunDate.HasValue && NextRunDate.Value <= DateTime.UtcNow;
    }
}

/// <summary>
/// تكرار الجدولة - Schedule Frequency
/// </summary>
public enum ScheduleFrequency
{
    /// <summary>
    /// يومي - Daily
    /// </summary>
    Daily = 1,
    
    /// <summary>
    /// أسبوعي - Weekly
    /// </summary>
    Weekly = 2,
    
    /// <summary>
    /// نصف شهري - Bi-weekly
    /// </summary>
    BiWeekly = 3,
    
    /// <summary>
    /// شهري - Monthly
    /// </summary>
    Monthly = 4,
    
    /// <summary>
    /// ربع سنوي - Quarterly (every 3 months)
    /// </summary>
    Quarterly = 5,
    
    /// <summary>
    /// نصف سنوي - Semi-annually (every 6 months)
    /// </summary>
    SemiAnnually = 6,
    
    /// <summary>
    /// سنوي - Annually
    /// </summary>
    Annually = 7,
    
    /// <summary>
    /// مخصص - Custom (using cron expression)
    /// </summary>
    Custom = 99
}

/// <summary>
/// سجل تشغيل الجدول - Schedule run record
/// </summary>
public class ScheduleRun
{
    /// <summary>
    /// When the schedule ran
    /// </summary>
    public DateTime RunDate { get; set; }
    
    /// <summary>
    /// Assessment created by this run
    /// </summary>
    public Guid AssessmentId { get; set; }
    
    /// <summary>
    /// Was it successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}
