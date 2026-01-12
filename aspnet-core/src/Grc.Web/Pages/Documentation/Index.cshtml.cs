using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.Documentation;

[Authorize]
public class IndexModel : GrcPageModel
{
    public List<DocSection> Sections { get; set; } = new();

    public async Task OnGetAsync()
    {
        Sections = new List<DocSection>
        {
            new DocSection
            {
                Title = "User Guide",
                Icon = "fas fa-book",
                Description = "Complete guide for end users",
                Articles = new List<DocArticle>
                {
                    new() { Title = "Platform Overview", Url = "#overview" },
                    new() { Title = "Dashboard Navigation", Url = "#dashboard" },
                    new() { Title = "Assessment Workflow", Url = "#assessment" },
                    new() { Title = "Evidence Upload", Url = "#evidence" },
                    new() { Title = "Risk Register", Url = "#risk" }
                }
            },
            new DocSection
            {
                Title = "Administrator Guide",
                Icon = "fas fa-user-shield",
                Description = "System administration documentation",
                Articles = new List<DocArticle>
                {
                    new() { Title = "User Management", Url = "#users" },
                    new() { Title = "Role Configuration", Url = "#roles" },
                    new() { Title = "Permission Settings", Url = "#permissions" },
                    new() { Title = "System Settings", Url = "#settings" },
                    new() { Title = "Backup & Restore", Url = "#backup" }
                }
            },
            new DocSection
            {
                Title = "API Reference",
                Icon = "fas fa-code",
                Description = "REST API documentation",
                Articles = new List<DocArticle>
                {
                    new() { Title = "Authentication", Url = "#auth" },
                    new() { Title = "Framework Endpoints", Url = "#framework-api" },
                    new() { Title = "Assessment Endpoints", Url = "#assessment-api" },
                    new() { Title = "Risk Endpoints", Url = "#risk-api" },
                    new() { Title = "Webhooks", Url = "#webhooks" }
                }
            },
            new DocSection
            {
                Title = "Compliance Frameworks",
                Icon = "fas fa-balance-scale",
                Description = "Supported framework documentation",
                Articles = new List<DocArticle>
                {
                    new() { Title = "NCA ECC Overview", Url = "#nca-ecc" },
                    new() { Title = "SAMA CSF Guide", Url = "#sama-csf" },
                    new() { Title = "PDPL Compliance", Url = "#pdpl" },
                    new() { Title = "ISO 27001", Url = "#iso27001" },
                    new() { Title = "NIST CSF", Url = "#nist" }
                }
            }
        };

        await Task.CompletedTask;
    }
}

public class DocSection
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DocArticle> Articles { get; set; } = new();
}

public class DocArticle
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
