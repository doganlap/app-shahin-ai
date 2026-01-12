using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Web.Pages.Support;

[Authorize]
public class IndexModel : GrcPageModel
{
    public List<SupportTicket> RecentTickets { get; set; } = new();
    public SupportContact ContactInfo { get; set; } = null!;

    public async Task OnGetAsync()
    {
        ContactInfo = new SupportContact
        {
            Email = "support@shahin-ai.com",
            Phone = "+966 11 XXX XXXX",
            Hours = "Sunday - Thursday, 8:00 AM - 6:00 PM (AST)"
        };

        RecentTickets = new List<SupportTicket>
        {
            new() { Id = "TKT-001", Subject = "Unable to upload large files", Status = "Open", CreatedAt = DateTime.Now.AddDays(-1), Priority = "High" },
            new() { Id = "TKT-002", Subject = "Report generation timeout", Status = "In Progress", CreatedAt = DateTime.Now.AddDays(-3), Priority = "Medium" },
            new() { Id = "TKT-003", Subject = "User permission issue", Status = "Resolved", CreatedAt = DateTime.Now.AddDays(-7), Priority = "Low" }
        };

        await Task.CompletedTask;
    }

    public async Task<IActionResult> OnPostSubmitTicketAsync(string subject, string description, string priority)
    {
        await Task.CompletedTask;
        return new JsonResult(new { success = true, ticketId = "TKT-" + new Random().Next(100, 999) });
    }
}

public class SupportTicket
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Priority { get; set; } = string.Empty;
}

public class SupportContact
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Hours { get; set; } = string.Empty;
}
