using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Grc.Assessment.Domain.Data;

public class AssessmentDataSeeder : ITransientDependency
{
    private readonly TeamDataSeeder _teamSeeder;
    private readonly ToolDataSeeder _toolSeeder;
    private readonly IssueDataSeeder _issueSeeder;

    public AssessmentDataSeeder(
        TeamDataSeeder teamSeeder,
        ToolDataSeeder toolSeeder,
        IssueDataSeeder issueSeeder)
    {
        _teamSeeder = teamSeeder;
        _toolSeeder = toolSeeder;
        _issueSeeder = issueSeeder;
    }

    public async Task<AssessmentSeedingSummary> SeedAllAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        var summary = new AssessmentSeedingSummary();

        // Seed Teams
        try
        {
            Console.WriteLine("Starting Team seeding...");
            var teamResult = await _teamSeeder.SeedAsync();
            summary.Teams = teamResult;
            Console.WriteLine($"Teams: {teamResult.Inserted} inserted, {teamResult.Skipped} skipped, {teamResult.Total} total");
        }
        catch (Exception ex)
        {
            summary.Teams = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Team seeding failed: {ex.Message}");
        }

        // Seed Tools
        try
        {
            Console.WriteLine("Starting Tool seeding...");
            var toolResult = await _toolSeeder.SeedAsync();
            summary.Tools = toolResult;
            Console.WriteLine($"Tools: {toolResult.Inserted} inserted, {toolResult.Skipped} skipped, {toolResult.Total} total");
        }
        catch (Exception ex)
        {
            summary.Tools = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Tool seeding failed: {ex.Message}");
        }

        // Seed Issues
        try
        {
            Console.WriteLine("Starting Issue seeding...");
            var issueResult = await _issueSeeder.SeedAsync();
            summary.Issues = issueResult;
            Console.WriteLine($"Issues: {issueResult.Inserted} inserted, {issueResult.Skipped} skipped, {issueResult.Total} total");
        }
        catch (Exception ex)
        {
            summary.Issues = new SeedResult { Inserted = 0, Skipped = 0, Total = 0 };
            Console.WriteLine($"ERROR: Issue seeding failed: {ex.Message}");
        }

        stopwatch.Stop();
        summary.DurationMs = stopwatch.ElapsedMilliseconds;
        summary.Success = true;
        summary.Message = "Assessment data seeding completed";

        Console.WriteLine($"Assessment seeding completed in {summary.DurationMs}ms");

        return summary;
    }
}

public class AssessmentSeedingSummary
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public SeedResult Teams { get; set; } = new SeedResult();
    public SeedResult Tools { get; set; } = new SeedResult();
    public SeedResult Issues { get; set; } = new SeedResult();
    public long DurationMs { get; set; }

    public int TotalInserted => Teams.Inserted + Tools.Inserted + Issues.Inserted;
    public int TotalSkipped => Teams.Skipped + Tools.Skipped + Issues.Skipped;
    public int TotalRecords => Teams.Total + Tools.Total + Issues.Total;
}
