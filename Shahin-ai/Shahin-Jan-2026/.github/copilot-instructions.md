# Shahin AI GRC Platform - Copilot Instructions

## Quick Start

```bash
# Run with Docker (RECOMMENDED)
docker-compose up -d

# Or run locally (requires PostgreSQL running)
cd src/GrcMvc && dotnet run
# Access: http://localhost:5010 (local) or http://localhost:8888 (Docker)
```

**Active Project**: `src/GrcMvc/` — ASP.NET Core 8.0 MVC + ABP Framework  
**Database**: PostgreSQL 15 (`GrcMvcDb` for app, `GrcAuthDb` for auth)  
**AI**: Claude claude-sonnet-4-5-20250514 via `CLAUDE_API_KEY` environment variable

---

## Architecture Overview

```
                   ┌─────────────────────────────────────────────┐
                   │        Middleware Pipeline                   │
                   │  TenantResolution → Onboarding → Security   │
                   └─────────────────────────────────────────────┘
                                       ▼
Controllers (100+) → Services (Interface/Impl) → IUnitOfWork → GrcDbContext
        ↓                      ↓                                  ↓
   Razor Views          IMapper (DTOs)                    PostgreSQL (ABP)
        ↓                      ↓
   Localization         12 AI Agents (ClaudeAgentService)
```

### Key Directories
| Path | Purpose |
|------|---------|
| `src/GrcMvc/Controllers/` | 100+ MVC controllers, `/Api/` for REST APIs |
| `src/GrcMvc/Services/Interfaces/` | 100+ service contracts |
| `src/GrcMvc/Services/Implementations/` | Service implementations with tenant isolation |
| `src/GrcMvc/Models/Entities/` | 200+ domain entities extending `BaseEntity` |
| `src/GrcMvc/Middleware/` | TenantResolution, Onboarding, Security middlewares |
| `src/GrcMvc/Data/Seeds/` | 20+ seed files (RoleProfiles, Regulators, etc.) |

---

## 🚨 CRITICAL: Multi-Tenancy Rules (ABP Context)

**Every query MUST be tenant-scoped. No exceptions. No backdoors.**

```csharp
// ✅ CORRECT: Always inject ITenantContextService
public class RiskService : IRiskService
{
    private readonly ITenantContextService _tenantContext;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<List<RiskDto>>> GetAllAsync()
    {
        var tenantId = _tenantContext.GetCurrentTenantId();
        if (tenantId == Guid.Empty)
            throw new SecurityException("BLOCKED: No tenant context");
            
        var risks = await _unitOfWork.Risks.GetAllAsync()
            .Where(r => r.TenantId == tenantId);
        return Result.Success(_mapper.Map<List<RiskDto>>(risks));
    }
}

// ❌ WRONG: Never bypass tenant filtering
var allRisks = await _db.Risks.ToListAsync(); // SECURITY VIOLATION
```

### Middleware Pipeline (Order Matters)
1. `TenantResolutionMiddleware` — Resolves tenant from domain/claims
2. `OnboardingRedirectMiddleware` — Forces wizard completion before dashboard
3. `SecurityHeadersMiddleware` — Applies CSP, HSTS headers
4. `GlobalExceptionMiddleware` — Safe error responses

---

## BaseEntity Fields (All Entities Inherit)

```csharp
public abstract class BaseEntity : IGovernedResource
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }           // Multi-tenant isolation (REQUIRED)
    public string? BusinessCode { get; set; }     // Immutable: ACME-CTRL-2026-000143
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }           // Soft delete only
    public byte[]? RowVersion { get; set; }       // Optimistic concurrency
    public string? Owner { get; set; }            // REQUIRE_OWNER policy
    public string? DataClassification { get; set; } // public|internal|confidential|restricted
}
```

---

## Agent Governance (12 Specialized AI Agents)

Unified `ClaudeAgentService` handles all agent types. Enterprise-grade governance applies:

| Agent | Purpose | Governance |
|-------|---------|------------|
| OnboardingAgent | Wizard guidance | Required for first-time setup |
| WorkflowAgent | Process automation | AgentApprovalGate enforced |
| EvidenceAgent | Evidence collection | Tenant-isolated |
| RulesEngineAgent | Policy evaluation | Returns rationale JSON |
| ComplianceAgent | Regulatory analysis | Read-only, no mutations |
| RiskAgent | Risk assessment | Confidence scoring |

**Agent Enforcement Rules:**
- TenantId MUST be verified before every agent command execution
- Only whitelisted agents may invoke onboarding operations
- No cross-tenant writes permitted under any condition
- RulesEngine MUST return human-readable rationale for every branch
- Onboarding missions must complete in order: OrgProfile → TeamSetup → Framework
- No dashboard access until `OnboardingStatus == Completed`

---

## API Response Pattern

```csharp
// SUCCESS
return Ok(ApiResponse<T>.SuccessResponse(data, "Operation completed"));

// ERROR (never expose internals)
return BadRequest(ApiResponse<T>.ErrorResponse("Validation failed"));

// Service Result Pattern
public async Task<Result<RiskDto>> GetByIdAsync(Guid id)
{
    var risk = await _unitOfWork.Risks.GetByIdAsync(id);
    if (risk == null) return Result.NotFound<RiskDto>("Risk", id);
    return Result.Success(_mapper.Map<RiskDto>(risk));
}
```

---

## Adding New Features

### New Entity Checklist
1. Create `Models/Entities/{Entity}.cs` extending `BaseEntity`
2. Add `DbSet<{Entity}>` to `Data/GrcDbContext.cs`
3. Configure in `OnModelCreating()` with TenantId index
4. Create DTOs: `{Entity}Dto`, `Create{Entity}Dto`, `Update{Entity}Dto`
5. Create FluentValidation in `Validators/`
6. Create `Services/Interfaces/I{Entity}Service.cs`
7. Create `Services/Implementations/{Entity}Service.cs` (inject `ITenantContextService`)
8. Register in `Program.cs`: `builder.Services.AddScoped<I{Entity}Service, {Entity}Service>();`
9. Create controller with `[Authorize]`
10. Run: `dotnet ef migrations add Add{Entity} && dotnet ef database update`

### New API Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YourApiController : ControllerBase
{
    private readonly IYourService _service;
    private readonly ITenantContextService _tenantContext;
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        // Tenant context auto-validated by service layer
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess 
            ? Ok(ApiResponse<YourDto>.SuccessResponse(result.Value))
            : NotFound(ApiResponse<YourDto>.ErrorResponse(result.Error));
    }
}
```

---

## Essential Commands

```bash
# Database
cd src/GrcMvc
dotnet ef migrations add YourMigrationName
dotnet ef database update

# Build & Test
dotnet build
dotnet test tests/GrcMvc.Tests

# Docker
docker-compose up -d          # Start all
docker-compose logs -f grcmvc # View logs
docker-compose down           # Stop

# Health check
curl http://localhost:8888/health
```

---

## Localization (Bilingual EN/AR)

```razor
@* In Razor views *@
@L["KeyName"]

@* RTL support automatic for Arabic *@
```

```csharp
// In services
private readonly IStringLocalizer<SharedResource> _localizer;
var message = _localizer["KeyName"];
```

---

## Common Gotchas

1. **Never bypass tenant filtering** — `GrcDbContext` applies global filters automatically
2. **Never hardcode secrets** — Use `.env` or Azure Key Vault
3. **Soft delete only** — Set `IsDeleted = true`, never `DELETE FROM`
4. **BusinessCode is immutable** — Used in audits, never change after assignment
5. **Two databases** — App data in `GrcMvcDb`, auth in `GrcAuthDbContext`
6. **Onboarding required** — Dashboard inaccessible until `OnboardingStatus == Completed`
7. **Agent rationale required** — RulesEngine must return human-readable rationale JSON

---

## Environment Variables (Required)

```bash
# Database
CONNECTION_STRING="Host=172.18.0.2;Database=GrcMvcDb;Username=postgres;Password=xxx"

# Security
JWT_SECRET="minimum-32-character-secret"
ALLOWED_HOSTS="portal.shahin-ai.com;localhost"

# AI (required for agents)
CLAUDE_API_KEY="sk-ant-api03-xxxxx"
CLAUDE_MODEL="claude-sonnet-4-5-20250514"

# Multi-tenancy
Security__AllowPublicRegistration=true  # For audit demos
```

---

**Last Updated**: January 2026 | **Stack**: ASP.NET Core 8.0, ABP Framework, EF Core 8.0.8, PostgreSQL 15
