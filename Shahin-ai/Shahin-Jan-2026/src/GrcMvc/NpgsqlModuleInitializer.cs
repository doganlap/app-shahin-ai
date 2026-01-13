using System.Runtime.CompilerServices;

namespace GrcMvc;

/// <summary>
/// Module initializer that runs before ANY other code in the assembly.
/// This is critical for setting Npgsql behavior switches before the library loads.
/// </summary>
internal static class NpgsqlModuleInitializer
{
    /// <summary>
    /// Enables legacy timestamp behavior to handle DateTime with Kind=Local.
    /// This MUST run before any Npgsql types are accessed.
    /// </summary>
    [ModuleInitializer]
    internal static void Initialize()
    {
        // Prevents: "Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone'"
        // This switch tells Npgsql to accept Local/Unspecified DateTimes and convert them
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}
