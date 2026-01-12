namespace Grc;

/// <summary>
/// Result object for data seeding operations
/// </summary>
public class SeedResult
{
    public int Inserted { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
    public string Message { get; set; }
    public bool Success => Inserted + Skipped == Total;
}
