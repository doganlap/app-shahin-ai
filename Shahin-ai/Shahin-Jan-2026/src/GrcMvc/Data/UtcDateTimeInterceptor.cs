using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GrcMvc.Data
{
    /// <summary>
    /// EF Core interceptor that ensures all DateTime values are converted to UTC before saving.
    /// PostgreSQL timestamptz columns require UTC DateTimes - this prevents "Kind=Local" errors.
    /// </summary>
    public sealed class UtcDateTimeInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ForceUtc(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ForceUtc(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ForceUtc(DbContext? context)
        {
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Properties)
                    {
                        // Handle DateTime
                        if (prop.Metadata.ClrType == typeof(DateTime) && prop.CurrentValue is DateTime dt)
                        {
                            if (dt.Kind == DateTimeKind.Local)
                                prop.CurrentValue = dt.ToUniversalTime();
                            else if (dt.Kind == DateTimeKind.Unspecified)
                                prop.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                        }

                        // Handle DateTime?
                        if (prop.Metadata.ClrType == typeof(DateTime?) && prop.CurrentValue is DateTime ndt)
                        {
                            if (ndt.Kind == DateTimeKind.Local)
                                prop.CurrentValue = ndt.ToUniversalTime();
                            else if (ndt.Kind == DateTimeKind.Unspecified)
                                prop.CurrentValue = DateTime.SpecifyKind(ndt, DateTimeKind.Utc);
                        }
                    }
                }
            }
        }
    }
}
