using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Volo.Abp.AuditLogging;

namespace Grc.Web.Pages.AuditLogs;

public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    public List<AuditLog> AuditLogs { get; set; } = new();

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        AuditLogs = await _dbContext.Set<AuditLog>()
            .OrderByDescending(a => a.ExecutionTime)
            .Take(100)
            .ToListAsync();
    }
}

