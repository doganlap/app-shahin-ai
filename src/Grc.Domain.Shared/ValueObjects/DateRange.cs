namespace Grc.ValueObjects;

/// <summary>
/// Date range value object
/// </summary>
public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public DateRange() { }
    
    public DateRange(DateTime startDate, DateTime? endDate = null)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
    
    /// <summary>
    /// Checks if a date falls within the range
    /// </summary>
    public bool Contains(DateTime date)
    {
        if (date < StartDate)
            return false;
        
        if (EndDate.HasValue && date > EndDate.Value)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// Gets the duration in days
    /// </summary>
    public int? DurationInDays
    {
        get
        {
            if (!EndDate.HasValue)
                return null;
            
            return (EndDate.Value - StartDate).Days;
        }
    }
}

