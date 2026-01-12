using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Grc.Web.Pages.Help;

[Authorize]
public class IndexModel : GrcPageModel
{
    public List<HelpCategory> Categories { get; set; } = new();
    public List<FaqItem> FrequentQuestions { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = new List<HelpCategory>
        {
            new() { Icon = "fas fa-rocket", Title = "Getting Started", Description = "Learn the basics of the GRC platform", ArticleCount = 12 },
            new() { Icon = "fas fa-clipboard-check", Title = "Assessments", Description = "How to create and manage assessments", ArticleCount = 18 },
            new() { Icon = "fas fa-exclamation-triangle", Title = "Risk Management", Description = "Managing and treating organizational risks", ArticleCount = 15 },
            new() { Icon = "fas fa-folder-open", Title = "Evidence Management", Description = "Uploading and organizing evidence", ArticleCount = 8 },
            new() { Icon = "fas fa-chart-bar", Title = "Reports & Analytics", Description = "Generating reports and insights", ArticleCount = 10 },
            new() { Icon = "fas fa-cogs", Title = "Administration", Description = "System configuration and user management", ArticleCount = 14 }
        };

        FrequentQuestions = new List<FaqItem>
        {
            new() { Question = "How do I create a new assessment?", Answer = "Navigate to Assessments > Create New, select a framework, and follow the wizard." },
            new() { Question = "How do I upload evidence?", Answer = "Go to Evidence Management, click Upload, and drag your files or browse to select them." },
            new() { Question = "How do I assign controls to team members?", Answer = "Open an assessment, select controls, and use the Bulk Assign feature." },
            new() { Question = "How do I generate a compliance report?", Answer = "Navigate to Reports, select the report type, configure options, and click Generate." },
            new() { Question = "How do I change my password?", Answer = "Go to My Settings > Security and use the Change Password form." }
        };

        await Task.CompletedTask;
    }
}

public class HelpCategory
{
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ArticleCount { get; set; }
}

public class FaqItem
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
