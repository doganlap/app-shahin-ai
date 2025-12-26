using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace Grc.Web.Pages.SecurityLogs;

public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    public List<IdentitySecurityLog> SecurityLogs { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        try
        {
            SecurityLogs = await _dbContext.Set<IdentitySecurityLog>()
                .OrderByDescending(s => s.CreationTime)
                .Take(100)
                .ToListAsync();
        }
        catch
        {
            // If table doesn't exist, show empty list
            SecurityLogs = new List<IdentitySecurityLog>();
        }
    }
}

