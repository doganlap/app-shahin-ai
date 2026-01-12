using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.Enums;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Grc.Web.Pages.Workflows;

[Authorize(GrcPermissions.Workflows.Default)]
public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public List<WorkflowDefinitionDto> WorkflowDefinitions { get; set; } = new();
    public List<WorkflowInstanceDto> ActiveWorkflows { get; set; } = new();
    public WorkflowStatistics Statistics { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        // Query workflow definitions from database
        var dbWorkflows = await _dbContext.WorkflowDefinitions
            .OrderBy(w => w.Category)
            .ThenBy(w => w.Name)
            .ToListAsync();

        // Map database workflows to DTOs
        WorkflowDefinitions = dbWorkflows.Select(w => new WorkflowDefinitionDto
        {
            Id = w.Id,
            Code = $"WF-{w.Category?.Substring(0, Math.Min(4, w.Category.Length)).ToUpper() ?? "GEN"}",
            Name = w.Name?.En ?? w.Name?.Ar ?? "Unnamed Workflow",
            NameAr = w.Name?.Ar ?? w.Name?.En ?? "سير عمل غير مسمى",
            Description = w.Description?.En ?? "",
            DescriptionAr = w.Description?.Ar ?? "",
            Category = w.Category ?? "General",
            IsActive = w.Status == WorkflowStatus.InProgress,
            Steps = Array.Empty<string>(),
            TriggerType = "Manual",
            Icon = GetIconForCategory(w.Category),
            Color = GetColorForCategory(w.Category)
        }).ToList();

        // If no workflows in database, use defaults
        if (!WorkflowDefinitions.Any())
        {
            WorkflowDefinitions = GetDefaultWorkflowDefinitions();
        }

        // Query active workflow instances from database
        var dbInstances = await _dbContext.WorkflowInstances
            .Where(i => i.Status == WorkflowStatus.InProgress || i.Status == WorkflowStatus.Pending)
            .OrderByDescending(i => i.CreationTime)
            .Take(10)
            .ToListAsync();

        ActiveWorkflows = dbInstances.Select(i => new WorkflowInstanceDto
        {
            Id = i.Id,
            WorkflowCode = i.WorkflowDefinitionId.ToString().Substring(0, 8),
            Name = $"Instance {i.Id.ToString().Substring(0, 8)}",
            CurrentStep = i.CurrentActivityId ?? "In Progress",
            Status = i.Status.ToString(),
            Assignee = "Team",
            StartedAt = i.StartedAt ?? i.CreationTime,
            DueDate = DateTime.UtcNow.AddDays(14)
        }).ToList();

        // Calculate Statistics
        var allInstances = await _dbContext.WorkflowInstances.ToListAsync();
        Statistics = new WorkflowStatistics
        {
            TotalDefinitions = WorkflowDefinitions.Count,
            ActiveDefinitions = WorkflowDefinitions.Count(w => w.IsActive),
            TotalInstances = allInstances.Count,
            InProgress = allInstances.Count(w => w.Status == WorkflowStatus.InProgress),
            PendingReview = allInstances.Count(w => w.Status == WorkflowStatus.Pending),
            Overdue = 0
        };
    }

    private string GetIconForCategory(string? category) => category switch
    {
        "Assessment" => "fas fa-clipboard-check",
        "Control" => "fas fa-shield-alt",
        "Risk" => "fas fa-exclamation-triangle",
        "Evidence" => "fas fa-file-upload",
        "Audit" => "fas fa-clipboard-list",
        "Compliance" => "fas fa-balance-scale",
        "Vendor" => "fas fa-handshake",
        "Policy" => "fas fa-book",
        "Incident" => "fas fa-fire-extinguisher",
        _ => "fas fa-cog"
    };

    private string GetColorForCategory(string? category) => category switch
    {
        "Assessment" => "primary",
        "Control" => "info",
        "Risk" => "danger",
        "Evidence" => "info",
        "Audit" => "primary",
        "Compliance" => "warning",
        "Vendor" => "info",
        "Policy" => "primary",
        "Incident" => "danger",
        _ => "secondary"
    };

    private List<WorkflowDefinitionDto> GetDefaultWorkflowDefinitions() => new()
    {
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Code = "WF-ASSESS-CREATE", Name = "Assessment Creation", NameAr = "إنشاء التقييم", Description = "Workflow for creating new compliance assessments", DescriptionAr = "سير العمل لإنشاء تقييمات الامتثال الجديدة", Category = "Assessment", IsActive = true, Steps = new[] { "Draft", "Review", "Approval", "Active" }, TriggerType = "Manual", Icon = "fas fa-clipboard-check", Color = "primary" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111201"), Code = "WF-CTRL-ASSESS", Name = "Control Assessment", NameAr = "تقييم الضابط", Description = "Workflow for assessing individual controls", DescriptionAr = "سير العمل لتقييم الضوابط الفردية", Category = "Control", IsActive = true, Steps = new[] { "Not Started", "Self Assessment", "Evidence Upload", "Review", "Verification", "Complete" }, TriggerType = "Manual", Icon = "fas fa-shield-alt", Color = "info" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111301"), Code = "WF-RISK-ASSESS", Name = "Risk Assessment", NameAr = "تقييم المخاطر", Description = "Workflow for identifying and assessing risks", DescriptionAr = "سير العمل لتحديد وتقييم المخاطر", Category = "Risk", IsActive = true, Steps = new[] { "Identification", "Analysis", "Evaluation", "Treatment Planning", "Monitoring" }, TriggerType = "Manual", Icon = "fas fa-exclamation-triangle", Color = "danger" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111401"), Code = "WF-EVID-UPLOAD", Name = "Evidence Upload", NameAr = "رفع الأدلة", Description = "Workflow for uploading and reviewing evidence", DescriptionAr = "سير العمل لرفع ومراجعة الأدلة", Category = "Evidence", IsActive = true, Steps = new[] { "Upload", "Validation", "Review", "Approval", "Linked" }, TriggerType = "Manual", Icon = "fas fa-file-upload", Color = "info" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111501"), Code = "WF-AUDIT-PLAN", Name = "Audit Planning", NameAr = "تخطيط التدقيق", Description = "Workflow for planning internal and external audits", DescriptionAr = "سير العمل لتخطيط عمليات التدقيق الداخلية والخارجية", Category = "Audit", IsActive = true, Steps = new[] { "Scope Definition", "Resource Allocation", "Schedule", "Notification", "Ready" }, TriggerType = "Manual", Icon = "fas fa-clipboard-list", Color = "primary" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111601"), Code = "WF-COMPLY-GAP", Name = "Compliance Gap Analysis", NameAr = "تحليل فجوات الامتثال", Description = "Workflow for identifying and addressing compliance gaps", DescriptionAr = "سير العمل لتحديد ومعالجة فجوات الامتثال", Category = "Compliance", IsActive = true, Steps = new[] { "Gap Identification", "Impact Assessment", "Prioritization", "Remediation", "Verification" }, TriggerType = "Manual", Icon = "fas fa-balance-scale", Color = "warning" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111701"), Code = "WF-VENDOR-ASSESS", Name = "Vendor Assessment", NameAr = "تقييم المورد", Description = "Workflow for assessing vendor compliance and security", DescriptionAr = "سير العمل لتقييم امتثال وأمن الموردين", Category = "Vendor", IsActive = true, Steps = new[] { "Questionnaire Sent", "Response Received", "Review", "Risk Rating", "Approved/Rejected" }, TriggerType = "Manual", Icon = "fas fa-handshake", Color = "info" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111801"), Code = "WF-POLICY-REVIEW", Name = "Policy Review Cycle", NameAr = "دورة مراجعة السياسات", Description = "Workflow for periodic policy review and updates", DescriptionAr = "سير العمل للمراجعة الدورية للسياسات وتحديثها", Category = "Policy", IsActive = true, Steps = new[] { "Review Due", "Draft Update", "Stakeholder Review", "Legal Review", "Approval", "Published" }, TriggerType = "Scheduled", Icon = "fas fa-book", Color = "primary" },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111901"), Code = "WF-INCIDENT-RESP", Name = "Incident Response", NameAr = "الاستجابة للحوادث", Description = "Workflow for handling security and compliance incidents", DescriptionAr = "سير العمل للتعامل مع الحوادث الأمنية وحوادث الامتثال", Category = "Incident", IsActive = true, Steps = new[] { "Reported", "Triage", "Investigation", "Containment", "Eradication", "Recovery", "Lessons Learned" }, TriggerType = "Automatic", Icon = "fas fa-fire-extinguisher", Color = "danger" }
    };

    public async Task<IActionResult> OnPostToggleWorkflowAsync(Guid id, bool activate)
    {
        await Task.CompletedTask;
        return new JsonResult(new { success = true, message = activate ? "Workflow activated" : "Workflow deactivated" });
    }

    public async Task<IActionResult> OnPostStartWorkflowAsync(Guid workflowId, string name)
    {
        await Task.CompletedTask;
        return new JsonResult(new { success = true, message = "Workflow started", instanceId = Guid.NewGuid() });
    }
}

public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string[] Steps { get; set; } = Array.Empty<string>();
    public string TriggerType { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public string WorkflowCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Assignee { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime DueDate { get; set; }
}

public class WorkflowStatistics
{
    public int TotalDefinitions { get; set; }
    public int ActiveDefinitions { get; set; }
    public int TotalInstances { get; set; }
    public int InProgress { get; set; }
    public int PendingReview { get; set; }
    public int Overdue { get; set; }
}
