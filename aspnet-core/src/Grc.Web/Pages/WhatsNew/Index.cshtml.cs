using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.WhatsNew;

[Authorize]
public class IndexModel : GrcPageModel
{
    public List<ReleaseNote> Releases { get; set; } = new();

    public async Task OnGetAsync()
    {
        Releases = new List<ReleaseNote>
        {
            new ReleaseNote
            {
                Version = "2.5.0",
                ReleaseDate = DateTime.Now.AddDays(-7),
                Title = "AI-Powered Compliance Assistant",
                Description = "Introducing our new AI engine for intelligent compliance recommendations.",
                Features = new[]
                {
                    "AI-based control recommendations",
                    "Automated gap analysis",
                    "Smart document classification",
                    "Predictive risk assessment"
                },
                Type = "major"
            },
            new ReleaseNote
            {
                Version = "2.4.2",
                ReleaseDate = DateTime.Now.AddDays(-21),
                Title = "Performance & Security Updates",
                Description = "Critical security patches and performance improvements.",
                Features = new[]
                {
                    "50% faster report generation",
                    "Enhanced session security",
                    "Improved data export functionality",
                    "Bug fixes for assessment workflow"
                },
                Type = "minor"
            },
            new ReleaseNote
            {
                Version = "2.4.0",
                ReleaseDate = DateTime.Now.AddMonths(-1),
                Title = "Workflow Automation",
                Description = "Automate your compliance workflows with our new workflow engine.",
                Features = new[]
                {
                    "BPMN-based workflow designer",
                    "Automated task assignments",
                    "Email notifications",
                    "SLA tracking"
                },
                Type = "major"
            },
            new ReleaseNote
            {
                Version = "2.3.0",
                ReleaseDate = DateTime.Now.AddMonths(-2),
                Title = "Enhanced Reporting",
                Description = "New reporting capabilities for better compliance insights.",
                Features = new[]
                {
                    "Executive dashboard",
                    "Custom report builder",
                    "Scheduled reports",
                    "Export to PDF/Excel"
                },
                Type = "major"
            }
        };

        await Task.CompletedTask;
    }
}

public class ReleaseNote
{
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Features { get; set; } = Array.Empty<string>();
    public string Type { get; set; } = "minor"; // major, minor, patch
}
