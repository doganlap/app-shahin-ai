using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;
using GrcMvc.Services.Interfaces;
using MsHealthCheckResult = Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult;

namespace GrcMvc.HealthChecks
{
    /// <summary>
    /// Health check for tenant-specific databases
    /// Checks connectivity and migration status for each tenant
    /// </summary>
    public class TenantDatabaseHealthCheck : IHealthCheck
    {
        private readonly ITenantDatabaseResolver _databaseResolver;
        private readonly ITenantContextService _tenantContext;
        private readonly ILogger<TenantDatabaseHealthCheck> _logger;

        public TenantDatabaseHealthCheck(
            ITenantDatabaseResolver databaseResolver,
            ITenantContextService tenantContext,
            ILogger<TenantDatabaseHealthCheck> logger)
        {
            _databaseResolver = databaseResolver;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task<MsHealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // #region agent log
            try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "D", location = "TenantDatabaseHealthCheck.cs:29", message = "CheckHealthAsync entry", data = new { databaseResolverExists = _databaseResolver != null, tenantContextExists = _tenantContext != null, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
            // #endregion
            try
            {
                var tenantId = _tenantContext.GetCurrentTenantId();
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "D", location = "TenantDatabaseHealthCheck.cs:35", message = "Tenant ID retrieved", data = new { tenantId = tenantId.ToString(), isEmpty = tenantId == Guid.Empty, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion

                if (tenantId == Guid.Empty)
                {
                    // For unauthenticated health checks (monitoring systems), return Healthy
                    // Tenant-specific checks will only run for authenticated requests
                    return MsHealthCheckResult.Healthy(
                        "Tenant health check skipped - no tenant context (unauthenticated request)",
                        data: new Dictionary<string, object>
                        {
                            ["tenantId"] = "none",
                            ["reason"] = "Health check executed without tenant context",
                            ["note"] = "This is normal for system health monitoring"
                        });
                }

                // Check if database exists
                var dbExists = await _databaseResolver.DatabaseExistsAsync(tenantId, cancellationToken);
                // #region agent log
                try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "D", location = "TenantDatabaseHealthCheck.cs:52", message = "Database existence check", data = new { tenantId = tenantId.ToString(), dbExists = dbExists, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                // #endregion
                if (!dbExists)
                {
                    // #region agent log
                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "D", location = "TenantDatabaseHealthCheck.cs:55", message = "Database not found - unhealthy", data = new { tenantId = tenantId.ToString(), timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    return MsHealthCheckResult.Unhealthy(
                        $"Database for tenant {tenantId} does not exist",
                        data: new Dictionary<string, object>
                        {
                            ["tenantId"] = tenantId.ToString(),
                            ["databaseName"] = _databaseResolver.GetDatabaseName(tenantId),
                            ["reason"] = "Database not provisioned"
                        });
                }

                // Check database connectivity
                var connectionString = _databaseResolver.GetConnectionString(tenantId);
                await using var connection = new NpgsqlConnection(connectionString);
                
                try
                {
                    await connection.OpenAsync(cancellationToken);
                    
                    // Check if we can query
                    await using var command = new NpgsqlCommand("SELECT 1", connection);
                    await command.ExecuteScalarAsync(cancellationToken);

                    // Get database size
                    await using var sizeCommand = new NpgsqlCommand(
                        "SELECT pg_size_pretty(pg_database_size(current_database()))", connection);
                    var dbSize = await sizeCommand.ExecuteScalarAsync(cancellationToken) as string;

                    // #region agent log
                    try { await System.IO.File.AppendAllTextAsync("/home/Shahin-ai/.cursor/debug.log", System.Text.Json.JsonSerializer.Serialize(new { sessionId = "audit-session", runId = "run1", hypothesisId = "D", location = "TenantDatabaseHealthCheck.cs:82", message = "Database healthy", data = new { tenantId = tenantId.ToString(), databaseName = _databaseResolver.GetDatabaseName(tenantId), databaseSize = dbSize ?? "unknown", timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n"); } catch { }
                    // #endregion
                    return MsHealthCheckResult.Healthy(
                        $"Tenant database is accessible",
                        data: new Dictionary<string, object>
                        {
                            ["tenantId"] = tenantId.ToString(),
                            ["databaseName"] = _databaseResolver.GetDatabaseName(tenantId),
                            ["databaseSize"] = dbSize ?? "unknown",
                            ["status"] = "healthy"
                        });
                }
                catch (Exception ex)
                {
                    return MsHealthCheckResult.Unhealthy(
                        $"Cannot connect to tenant database: {ex.Message}",
                        ex,
                        data: new Dictionary<string, object>
                        {
                            ["tenantId"] = tenantId.ToString(),
                            ["databaseName"] = _databaseResolver.GetDatabaseName(tenantId),
                            ["error"] = ex.Message
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking tenant database health");
                return MsHealthCheckResult.Unhealthy(
                    "Error checking tenant database health",
                    ex);
            }
        }
    }
}
