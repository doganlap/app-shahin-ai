using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Grc.FrameworkLibrary.Domain.Data;

public class FrameworkLibraryDataSeeder : ITransientDependency
{
    private readonly RegulatorDataSeeder _regulatorSeeder;
    private readonly FrameworkDataSeeder _frameworkSeeder;
    private readonly ControlDataSeeder _controlSeeder;

    public FrameworkLibraryDataSeeder(
        RegulatorDataSeeder regulatorSeeder,
        FrameworkDataSeeder frameworkSeeder,
        ControlDataSeeder controlSeeder)
    {
        _regulatorSeeder = regulatorSeeder;
        _frameworkSeeder = frameworkSeeder;
        _controlSeeder = controlSeeder;
    }

    // Removed [UnitOfWork] to allow each seeder to commit independently
    public async Task<FrameworkLibrarySeedingSummary> SeedAllAsync()
    {
        // #region agent log
        System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "START", location = "FrameworkLibraryDataSeeder.cs:27", message = "SeedAllAsync started", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
        // #endregion
        
        var stopwatch = Stopwatch.StartNew();
        var summary = new FrameworkLibrarySeedingSummary();

        // Step 1: Seed Regulators
        try
        {
            Console.WriteLine("Starting Regulator seeding...");
            var regulatorResult = await _regulatorSeeder.SeedAsync();
            summary.Regulators = regulatorResult;
            Console.WriteLine($"Regulators: {regulatorResult.Inserted} inserted, {regulatorResult.Skipped} skipped, {regulatorResult.Total} total");
            
            // #region agent log
            System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", JsonSerializer.Serialize(new { sessionId = "debug-session", runId = "seed-attempt", hypothesisId = "A", location = "FrameworkLibraryDataSeeder.cs:40", message = "Regulators seeding completed", data = new { inserted = regulatorResult.Inserted, skipped = regulatorResult.Skipped }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            // #endregion
        }
        catch (Exception ex)
        {
            summary.Regulators = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Regulator seeding failed: {ex.Message}");
        }

        // Step 2: Seed Frameworks
        try
        {
            Console.WriteLine("Starting Framework seeding...");
            var frameworkResult = await _frameworkSeeder.SeedAsync();
            summary.Frameworks = frameworkResult;
            Console.WriteLine($"Frameworks: {frameworkResult.Inserted} inserted, {frameworkResult.Skipped} skipped, {frameworkResult.Total} total");
        }
        catch (Exception ex)
        {
            summary.Frameworks = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Framework seeding failed: {ex.Message}");
        }

        // Step 3: Seed Controls
        try
        {
            Console.WriteLine("Starting Control seeding...");
            var controlResult = await _controlSeeder.SeedAsync();
            summary.Controls = controlResult;
            Console.WriteLine($"Controls: {controlResult.Inserted} inserted, {controlResult.Skipped} skipped, {controlResult.Total} total");
        }
        catch (Exception ex)
        {
            summary.Controls = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Control seeding failed: {ex.Message}");
        }

        stopwatch.Stop();
        summary.DurationMs = stopwatch.ElapsedMilliseconds;
        summary.Success = true;
        summary.Message = "Framework Library seeding completed";

        Console.WriteLine($"Total seeding completed in {summary.DurationMs}ms");

        return summary;
    }
}

public class FrameworkLibrarySeedingSummary
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public SeedResult Regulators { get; set; } = new SeedResult();
    public SeedResult Frameworks { get; set; } = new SeedResult();
    public SeedResult Controls { get; set; } = new SeedResult();
    public long DurationMs { get; set; }

    public int TotalInserted => Regulators.Inserted + Frameworks.Inserted + Controls.Inserted;
    public int TotalSkipped => Regulators.Skipped + Frameworks.Skipped + Controls.Skipped;
    public int TotalRecords => Regulators.Total + Frameworks.Total + Controls.Total;
}

