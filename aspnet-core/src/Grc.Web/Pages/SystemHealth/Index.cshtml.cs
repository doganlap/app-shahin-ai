using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grc.EntityFrameworkCore;
using Npgsql;

namespace Grc.Web.Pages.SystemHealth;

public class IndexModel : GrcPageModel
{
    private readonly GrcDbContext _dbContext;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public string DatabaseStatus { get; set; } = "Unknown";
    public long DatabaseResponseTime { get; set; }
    public string ApiStatus { get; set; } = "Healthy";
    public string AppUptime { get; set; } = "Unknown";
    public double MemoryUsageMB { get; set; }
    public double MemoryTotalMB { get; set; }
    public int MemoryPercentage { get; set; }
    public double CpuPercentage { get; set; }

    public IndexModel(GrcDbContext dbContext, Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task OnGetAsync()
    {
        // Check database health
        await CheckDatabaseHealth();
        
        // Check memory usage
        CheckMemoryUsage();
        
        // Calculate uptime
        var currentProcess = Process.GetCurrentProcess();
        var uptime = DateTime.Now - currentProcess.StartTime;
        AppUptime = $"{uptime.Hours}h {uptime.Minutes}m";
        
        // CPU usage (approximate)
        CpuPercentage = Math.Round(currentProcess.TotalProcessorTime.TotalMilliseconds / uptime.TotalMilliseconds * 100, 2);
        if (CpuPercentage > 100) CpuPercentage = Math.Round(CpuPercentage / Environment.ProcessorCount, 2);
        
        ApiStatus = "Healthy";
    }

    private async Task CheckDatabaseHealth()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
            stopwatch.Stop();
            DatabaseStatus = "Healthy";
            DatabaseResponseTime = stopwatch.ElapsedMilliseconds;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            DatabaseStatus = "Unhealthy";
            DatabaseResponseTime = stopwatch.ElapsedMilliseconds;
        }
    }

    private void CheckMemoryUsage()
    {
        var currentProcess = Process.GetCurrentProcess();
        MemoryUsageMB = Math.Round(currentProcess.WorkingSet64 / 1024.0 / 1024.0, 2);
        
        // Estimate total available memory (4GB default)
        MemoryTotalMB = 4096;
        try
        {
            var gcMemoryInfo = GC.GetGCMemoryInfo();
            MemoryTotalMB = Math.Round(gcMemoryInfo.TotalAvailableMemoryBytes / 1024.0 / 1024.0, 2);
        }
        catch { }
        
        MemoryPercentage = (int)Math.Round((MemoryUsageMB / MemoryTotalMB) * 100);
    }
}

