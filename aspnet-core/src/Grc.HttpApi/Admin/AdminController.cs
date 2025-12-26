using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grc.EntityFrameworkCore;
using Grc.FrameworkLibrary.Domain.Data;
using Grc.Assessment.Domain.Data;
using Grc.Domain.Data;
using Grc.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Uow;

namespace Grc.HttpApi.Admin;

[ApiController]
[Route("api/admin")]

public class AdminController : AbpControllerBase
{
    private readonly FrameworkLibraryDataSeeder _frameworkLibrarySeeder;
    private readonly AssessmentDataSeeder _assessmentSeeder;
    private readonly GrcLifecycleDataSeeder _grcLifecycleSeeder;
    private readonly GrcDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AdminController(
        FrameworkLibraryDataSeeder frameworkLibrarySeeder,
        AssessmentDataSeeder assessmentSeeder,
        GrcLifecycleDataSeeder grcLifecycleSeeder,
        GrcDbContext dbContext,
        IConfiguration configuration)
    {
        _frameworkLibrarySeeder = frameworkLibrarySeeder;
        _assessmentSeeder = assessmentSeeder;
        _grcLifecycleSeeder = grcLifecycleSeeder;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Drop and recreate all FrameworkLibrary tables
    /// </summary>
    [HttpPost("reset-framework-library-schema")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> ResetSchemaAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("Default");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            var dropSql = @"
                DROP TABLE IF EXISTS ""Controls"" CASCADE;
                DROP TABLE IF EXISTS ""FrameworkDomains"" CASCADE;
                DROP TABLE IF EXISTS ""Frameworks"" CASCADE;
                DROP TABLE IF EXISTS ""Regulators"" CASCADE;
            ";
            
            await using (var cmd = new NpgsqlCommand(dropSql, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Create tables using raw SQL from file
            var createTablesSql = System.IO.File.ReadAllText("/tmp/create_all_tables.sql");
            await using (var cmd2 = new NpgsqlCommand(createTablesSql, connection))
            {
                await cmd2.ExecuteNonQueryAsync();
            }
            
            await connection.CloseAsync();
            
            return Ok(new { success = true, message = "All FrameworkLibrary tables dropped and recreated. Ready for seeding." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Check database counts directly
    /// </summary>
    [HttpGet("check-counts")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> CheckCountsAsync()
    {
        try
        {
            // Use raw connection to get actual counts
            var connectionString = _configuration.GetConnectionString("Default");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            long regulatorCount = 0, frameworkCount = 0, controlCount = 0;
            
            await using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Regulators\"", connection))
            {
                var result = await cmd.ExecuteScalarAsync();
                regulatorCount = result != null ? Convert.ToInt64(result) : 0;
            }
            
            await using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Frameworks\"", connection))
            {
                var result = await cmd.ExecuteScalarAsync();
                frameworkCount = result != null ? Convert.ToInt64(result) : 0;
            }
            
            await using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Controls\"", connection))
            {
                var result = await cmd.ExecuteScalarAsync();
                controlCount = result != null ? Convert.ToInt64(result) : 0;
            }
            
            await connection.CloseAsync();
            
            return Ok(new
            {
                regulators = regulatorCount,
                frameworks = frameworkCount,
                controls = controlCount,
                message = "Direct DB counts from Railway"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    /// <summary>
    /// Create Regulators table - call this FIRST before seeding
    /// </summary>
    [HttpPost("create-regulators-table")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> CreateRegulatorsTableAsync()
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS ""Regulators"" (
                    ""Id"" uuid NOT NULL,
                    ""Code"" character varying(20) NOT NULL,
                    ""NameEn"" character varying(256) NOT NULL,
                    ""NameAr"" character varying(256) NOT NULL,
                    ""JurisdictionEn"" character varying(256),
                    ""JurisdictionAr"" character varying(256),
                    ""Website"" text,
                    ""Category"" integer NOT NULL,
                    ""LogoUrl"" text,
                    ""ContactEmail"" character varying(256),
                    ""ContactPhone"" character varying(50),
                    ""ContactAddress"" character varying(512),
                    ""CreationTime"" timestamp with time zone NOT NULL,
                    ""CreatorId"" uuid,
                    ""LastModificationTime"" timestamp with time zone,
                    ""LastModifierId"" uuid,
                    ""IsDeleted"" boolean NOT NULL DEFAULT false,
                    ""DeleterId"" uuid,
                    ""DeletionTime"" timestamp with time zone,
                    ""ConcurrencyStamp"" text,
                    ""ExtraProperties"" text,
                    CONSTRAINT ""PK_Regulators"" PRIMARY KEY (""Id"")
                );
                CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Regulators_Code"" ON ""Regulators"" (""Code"");
            ");
            
            return Ok(new { success = true, message = "Regulators table created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error: {ex.Message}", error = ex.ToString() });
        }
    }

    /// <summary>
    /// Seed Framework Library data (Regulators, Frameworks, Controls)
    /// </summary>
    /// <returns>Seeding summary</returns>
    [HttpPost("seed-framework-library")]
    [AllowAnonymous]
    [UnitOfWork(IsDisabled = true)]  // Disable UoW to execute table creation first
    public async Task<ActionResult<FrameworkLibrarySeedingSummary>> SeedFrameworkLibraryAsync()
    {
        try
        {
            // Use raw connection to create table BEFORE ABP's UoW starts
            var connectionString = _configuration.GetConnectionString("Default");
            
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                
                // Read the SQL from file
                var createTableSql = System.IO.File.ReadAllText("/tmp/create_all_tables.sql");
                
                await using var cmd = new NpgsqlCommand(createTableSql, connection);
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                
                // Table created successfully, continue to seeding
            }
            catch (Exception tableEx)
            {
                // Table creation failed, but might already exist - try seeding anyway
                System.IO.File.AppendAllText("/tmp/grc-debug/debug.log", System.Text.Json.JsonSerializer.Serialize(new { location = "AdminController.cs:145", message = "Table creation warning", error = tableEx.Message, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }) + "\n");
            }
            
            var result = await _frameworkLibrarySeeder.SeedAllAsync();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = $"Seeding failed: {ex.Message}",
                Error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Get seeding status (for future use)
    /// </summary>
    [HttpGet("seed-status")]
    public ActionResult<object> GetSeedStatus()
    {
        return Ok(new
        {
            Message = "Seeding endpoint is available",
            Endpoint = "/api/admin/seed-framework-library",
            Method = "POST"
        });
    }

    /// <summary>
    /// Seed all data (Framework Library + Assessment data: Teams, Tools, Issues)
    /// </summary>
    [HttpPost("seed-all")]
    [AllowAnonymous]
    [UnitOfWork(IsDisabled = true)]
    public async Task<ActionResult<object>> SeedAllDataAsync()
    {
        try
        {
            // Seed Framework Library (Regulators, Frameworks, Controls)
            var frameworkResult = await _frameworkLibrarySeeder.SeedAllAsync();
            
            // Seed Assessment data (Teams, Tools, Issues)
            var assessmentResult = await _assessmentSeeder.SeedAllAsync();

            return Ok(new
            {
                success = true,
                message = "All data seeded successfully",
                frameworkLibrary = new
                {
                    regulators = frameworkResult.Regulators,
                    frameworks = frameworkResult.Frameworks,
                    controls = frameworkResult.Controls,
                    durationMs = frameworkResult.DurationMs
                },
                assessment = new
                {
                    teams = assessmentResult.Teams,
                    tools = assessmentResult.Tools,
                    issues = assessmentResult.Issues,
                    durationMs = assessmentResult.DurationMs
                },
                totalInserted = frameworkResult.TotalInserted + assessmentResult.TotalInserted,
                totalSkipped = frameworkResult.TotalSkipped + assessmentResult.TotalSkipped,
                totalRecords = frameworkResult.TotalRecords + assessmentResult.TotalRecords
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Seeding failed: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }

    /// <summary>
    /// Seed Assessment data only (Teams, Tools, Issues)
    /// </summary>
    [HttpPost("seed-assessment-data")]
    [AllowAnonymous]
    [UnitOfWork(IsDisabled = true)]
    public async Task<ActionResult<AssessmentSeedingSummary>> SeedAssessmentDataAsync()
    {
        try
        {
            var result = await _assessmentSeeder.SeedAllAsync();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Success = false,
                Message = $"Assessment seeding failed: {ex.Message}",
                Error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Create Vendors table and seed 5 vendors
    /// </summary>
    [HttpPost("seed-vendors")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> SeedVendorsAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("Default");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            // Create Vendors table if it doesn't exist
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS ""Vendors"" (
                    ""Id"" uuid NOT NULL DEFAULT gen_random_uuid(),
                    ""VendorName"" character varying(200) NOT NULL,
                    ""NameEn"" character varying(200),
                    ""NameAr"" character varying(200),
                    ""Category"" character varying(100),
                    ""RiskScore"" integer DEFAULT 0,
                    ""Status"" character varying(50) DEFAULT 'Active',
                    ""ContactEmail"" character varying(256),
                    ""ContactPhone"" character varying(50),
                    ""Website"" text,
                    ""Description"" text,
                    ""CreationTime"" timestamp with time zone NOT NULL DEFAULT NOW(),
                    ""CreatorId"" uuid,
                    ""LastModificationTime"" timestamp with time zone,
                    ""LastModifierId"" uuid,
                    ""IsDeleted"" boolean NOT NULL DEFAULT false,
                    ""DeleterId"" uuid,
                    ""DeletionTime"" timestamp with time zone,
                    ""ConcurrencyStamp"" text,
                    ""ExtraProperties"" text,
                    CONSTRAINT ""PK_Vendors"" PRIMARY KEY (""Id"")
                );
                CREATE INDEX IF NOT EXISTS ""IX_Vendors_VendorName"" ON ""Vendors"" (""VendorName"");
            ";
            
            await using (var cmd = new NpgsqlCommand(createTableSql, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Check if vendors already exist
            await using (var checkCmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Vendors\"", connection))
            {
                var existingCount = Convert.ToInt64(await checkCmd.ExecuteScalarAsync());
                if (existingCount >= 5)
                {
                    await connection.CloseAsync();
                    return Ok(new
                    {
                        success = true,
                        message = $"Vendors already exist ({existingCount} vendors found)",
                        inserted = 0,
                        skipped = 5,
                        total = existingCount
                    });
                }
            }
            
            // Seed 5 vendors
            var vendors = new[]
            {
                new { NameEn = "Microsoft Corporation", NameAr = "شركة مايكروسوفت", Category = "Technology", RiskScore = 2, Email = "contact@microsoft.com", Website = "https://microsoft.com" },
                new { NameEn = "Amazon Web Services", NameAr = "خدمات أمازون ويب", Category = "Cloud Services", RiskScore = 3, Email = "aws@amazon.com", Website = "https://aws.amazon.com" },
                new { NameEn = "Oracle Corporation", NameAr = "شركة أوراكل", Category = "Database", RiskScore = 4, Email = "info@oracle.com", Website = "https://oracle.com" },
                new { NameEn = "IBM Security", NameAr = "أمن آي بي إم", Category = "Security", RiskScore = 2, Email = "security@ibm.com", Website = "https://ibm.com/security" },
                new { NameEn = "SAP Middle East", NameAr = "ساب الشرق الأوسط", Category = "ERP", RiskScore = 3, Email = "me@sap.com", Website = "https://sap.com" }
            };
            
            int inserted = 0;
            foreach (var vendor in vendors)
            {
                var insertSql = @"
                    INSERT INTO ""Vendors"" (""Id"", ""VendorName"", ""NameEn"", ""NameAr"", ""Category"", ""RiskScore"", ""Status"", ""ContactEmail"", ""Website"", ""CreationTime"")
                    SELECT gen_random_uuid(), $1, $2, $3, $4, $5, 'Active', $6, $7, NOW()
                    WHERE NOT EXISTS (SELECT 1 FROM ""Vendors"" WHERE ""VendorName"" = $1)
                    RETURNING ""Id"";
                ";
                
                await using (var insertCmd = new NpgsqlCommand(insertSql, connection))
                {
                    insertCmd.Parameters.AddWithValue(vendor.NameEn);
                    insertCmd.Parameters.AddWithValue(vendor.NameEn);
                    insertCmd.Parameters.AddWithValue(vendor.NameAr);
                    insertCmd.Parameters.AddWithValue(vendor.Category);
                    insertCmd.Parameters.AddWithValue(vendor.RiskScore);
                    insertCmd.Parameters.AddWithValue(vendor.Email);
                    insertCmd.Parameters.AddWithValue(vendor.Website);
                    
                    var result = await insertCmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        inserted++;
                    }
                }
            }
            
            await connection.CloseAsync();
            
            return Ok(new
            {
                success = true,
                message = $"Seeded {inserted} vendors successfully",
                inserted = inserted,
                skipped = 5 - inserted,
                total = 5
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Vendor seeding failed: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Get all vendors from database (simple query for existing table)
    /// </summary>
    [HttpGet("vendors")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetVendorsAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("Default");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            var vendors = new List<object>();
            var sql = @"SELECT ""Id"", ""VendorName"", ""NameEn"", ""NameAr"", ""Category"", ""RiskScore"", ""Status"", ""ContactEmail"", ""Website"" 
                       FROM ""Vendors"" 
                       WHERE ""IsDeleted"" = false 
                       ORDER BY ""VendorName""";
            
            await using (var cmd = new NpgsqlCommand(sql, connection))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    vendors.Add(new
                    {
                        id = reader.GetGuid(0),
                        vendorName = reader.IsDBNull(1) ? null : reader.GetString(1),
                        nameEn = reader.IsDBNull(2) ? null : reader.GetString(2),
                        nameAr = reader.IsDBNull(3) ? null : reader.GetString(3),
                        category = reader.IsDBNull(4) ? null : reader.GetString(4),
                        riskScore = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        status = reader.IsDBNull(6) ? "Active" : reader.GetString(6),
                        contactEmail = reader.IsDBNull(7) ? null : reader.GetString(7),
                        website = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
            
            await connection.CloseAsync();
            
            return Ok(new
            {
                totalCount = vendors.Count,
                items = vendors
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Failed to get vendors: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Seed GRC Lifecycle data (Organizations, Assets, Gaps, ActionItems, Audits)
    /// </summary>
    [HttpPost("seed-grc-lifecycle")]
    [AllowAnonymous]
    [UnitOfWork(IsDisabled = true)]
    public async Task<ActionResult<object>> SeedGrcLifecycleDataAsync()
    {
        try
        {
            var result = await _grcLifecycleSeeder.SeedAsync();

            return Ok(new
            {
                success = true,
                message = "GRC Lifecycle data seeded successfully",
                inserted = result.Inserted,
                skipped = result.Skipped,
                total = result.Inserted + result.Skipped
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"GRC Lifecycle seeding failed: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// Seed ALL data - Framework Library + Assessment + GRC Lifecycle
    /// </summary>
    [HttpPost("seed-complete")]
    [AllowAnonymous]
    [UnitOfWork(IsDisabled = true)]
    public async Task<ActionResult<object>> SeedCompleteDataAsync()
    {
        try
        {
            // Seed Framework Library (Regulators, Frameworks, Controls)
            var frameworkResult = await _frameworkLibrarySeeder.SeedAllAsync();

            // Seed Assessment data (Teams, Tools, Issues)
            var assessmentResult = await _assessmentSeeder.SeedAllAsync();

            // Seed GRC Lifecycle (Organizations, Assets, Gaps, ActionItems, Audits)
            var lifecycleResult = await _grcLifecycleSeeder.SeedAsync();

            return Ok(new
            {
                success = true,
                message = "All data seeded successfully",
                frameworkLibrary = new
                {
                    regulators = frameworkResult.Regulators,
                    frameworks = frameworkResult.Frameworks,
                    controls = frameworkResult.Controls,
                    durationMs = frameworkResult.DurationMs
                },
                assessment = new
                {
                    teams = assessmentResult.Teams,
                    tools = assessmentResult.Tools,
                    issues = assessmentResult.Issues,
                    durationMs = assessmentResult.DurationMs
                },
                grcLifecycle = new
                {
                    inserted = lifecycleResult.Inserted,
                    skipped = lifecycleResult.Skipped,
                    total = lifecycleResult.Inserted + lifecycleResult.Skipped
                },
                summary = new
                {
                    totalInserted = frameworkResult.TotalInserted + assessmentResult.TotalInserted + lifecycleResult.Inserted,
                    totalSkipped = frameworkResult.TotalSkipped + assessmentResult.TotalSkipped + lifecycleResult.Skipped,
                    totalRecords = frameworkResult.TotalRecords + assessmentResult.TotalRecords + lifecycleResult.Inserted + lifecycleResult.Skipped
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = $"Complete seeding failed: {ex.Message}",
                stackTrace = ex.StackTrace
            });
        }
    }
}

