using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;

namespace Grc.Web.Pages.BackgroundJobs;

public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;

    public int CompletedJobs { get; set; }
    public int RunningJobs { get; set; }
    public int QueuedJobs { get; set; }
    public int FailedJobs { get; set; }

    public IndexModel(GrcDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var jobs = await _dbContext.Set<BackgroundJobRecord>().ToListAsync();
            
            CompletedJobs = jobs.Count(j => j.IsAbandoned == false && j.NextTryTime <= System.DateTime.UtcNow);
            RunningJobs = jobs.Count(j => j.IsAbandoned == false && j.NextTryTime > System.DateTime.UtcNow && j.TryCount > 0);
            QueuedJobs = jobs.Count(j => j.IsAbandoned == false && j.TryCount == 0);
            FailedJobs = jobs.Count(j => j.IsAbandoned == true);
        }
        catch
        {
            // If BackgroundJobRecord table doesn't exist, set to 0
            CompletedJobs = 0;
            RunningJobs = 0;
            QueuedJobs = 0;
            FailedJobs = 0;
        }
    }
}

