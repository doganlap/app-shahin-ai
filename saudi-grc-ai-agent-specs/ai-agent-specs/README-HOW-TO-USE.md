# ============================================================
# HOW TO USE WITH AI CODING AGENTS IN YOUR IDE
# ============================================================

## QUICK START (Copy-Paste Ready)

### Step 1: Add Specs to Your Project

```bash
# Create specs folder in your project root
mkdir -p .ai-specs

# Copy all spec files
cp 00-PROJECT-SPEC.yaml .ai-specs/
cp 01-ENTITIES.yaml .ai-specs/
cp 02-DATABASE-SCHEMA.sql .ai-specs/
cp 03-API-SPEC.yaml .ai-specs/
cp 04-ABP-CLI-SETUP.sh .ai-specs/
cp 05-TASK-BREAKDOWN.yaml .ai-specs/
```

### Step 2: Create AI Rules File

Create `.cursorrules` (for Cursor) or `.github/copilot-instructions.md` (for Copilot):

```markdown
# Project: Saudi GRC Platform

## Context Files (READ THESE FIRST)
- `.ai-specs/00-PROJECT-SPEC.yaml` - Project overview and tech stack
- `.ai-specs/01-ENTITIES.yaml` - All domain entities with C# code
- `.ai-specs/02-DATABASE-SCHEMA.sql` - Complete PostgreSQL schema
- `.ai-specs/03-API-SPEC.yaml` - OpenAPI specification
- `.ai-specs/05-TASK-BREAKDOWN.yaml` - Implementation tasks

## Tech Stack
- ABP.io Open Source 8.3+
- .NET 8.0, EF Core 8.0
- Angular 17+, PrimeNG
- PostgreSQL 16+
- Redis, RabbitMQ, MinIO

## Coding Standards
- Follow ABP.io conventions
- Use DDD patterns
- All entities bilingual (Arabic/English)
- Multi-tenant with IMultiTenant interface
```

---

## IDE-SPECIFIC INSTRUCTIONS

### 🔵 CURSOR IDE

**Method 1: Chat with Context**
```
1. Open Cursor
2. Press Cmd+L (Mac) or Ctrl+L (Windows) to open chat
3. Type: @.ai-specs/01-ENTITIES.yaml
4. Then ask: "Create the Regulator entity class based on this spec"
```

**Method 2: Composer Mode**
```
1. Press Cmd+I (Mac) or Ctrl+I (Windows)
2. Paste this prompt:

Read @.ai-specs/01-ENTITIES.yaml and create:
1. src/Grc.Domain/Regulators/Regulator.cs
2. src/Grc.Domain/Regulators/IRegulatorRepository.cs
3. src/Grc.Application.Contracts/Regulators/RegulatorDto.cs
4. src/Grc.Application/Regulators/RegulatorAppService.cs

Follow ABP.io conventions and the entity specification exactly.
```

**Method 3: .cursorrules File**
Create `.cursorrules` in project root:
```
You are building a Saudi GRC Compliance Platform.

ALWAYS read these files before generating code:
- .ai-specs/00-PROJECT-SPEC.yaml (project context)
- .ai-specs/01-ENTITIES.yaml (entity definitions)
- .ai-specs/03-API-SPEC.yaml (API contracts)

RULES:
1. Use ABP.io Open Source patterns
2. All strings must be bilingual (LocalizedString)
3. Implement IMultiTenant on tenant-scoped entities
4. Follow the exact property names from specs
5. Use FullAuditedAggregateRoot for aggregates
6. Use FullAuditedEntity for child entities
```

---

### 🟢 GITHUB COPILOT

**Method 1: Copilot Chat**
```
1. Open VS Code with Copilot
2. Press Ctrl+Shift+I to open Copilot Chat
3. Type: @workspace /explain .ai-specs/01-ENTITIES.yaml
4. Then: "Generate the Assessment entity based on this spec"
```

**Method 2: Inline with Comments**
```csharp
// File: src/Grc.Domain/Assessments/Assessment.cs
// Copilot: Generate entity from .ai-specs/01-ENTITIES.yaml#Assessment
// Include: TenantId, Name, Type, Status, StartDate, TargetEndDate
// Base class: FullAuditedAggregateRoot<Guid>, IMultiTenant

public class Assessment // Copilot will complete this
```

**Method 3: .github/copilot-instructions.md**
```markdown
# Copilot Instructions for GRC Platform

## Read These Specs First
When generating code, reference:
- `.ai-specs/01-ENTITIES.yaml` for entity structures
- `.ai-specs/03-API-SPEC.yaml` for API contracts

## Patterns to Follow
- ABP.io Domain-Driven Design
- Entity base: `FullAuditedAggregateRoot<Guid>`
- Multi-tenant: implement `IMultiTenant`
- Bilingual: use `LocalizedString` for text
```

---

### 🟣 WINDSURF (CODEIUM)

**Method 1: Cascade with Context**
```
1. Open Windsurf
2. Press Cmd+L for Cascade
3. Drag .ai-specs/01-ENTITIES.yaml into chat
4. Ask: "Create all entity classes from this specification"
```

**Method 2: Flow Commands**
```
@create entities from .ai-specs/01-ENTITIES.yaml
@generate API controllers from .ai-specs/03-API-SPEC.yaml
@execute database setup from .ai-specs/02-DATABASE-SCHEMA.sql
```

---

### 🔴 CLAUDE CODE (CLI)

**Method 1: Direct Reference**
```bash
claude "Read .ai-specs/01-ENTITIES.yaml and create the Regulator entity in src/Grc.Domain/Regulators/Regulator.cs"
```

**Method 2: Multi-file Generation**
```bash
claude "Based on .ai-specs/01-ENTITIES.yaml, create all files for the Assessment module:
- Domain entity
- Repository interface
- DTOs
- Application service
- API controller"
```

**Method 3: Task Execution**
```bash
claude "Execute task T016 from .ai-specs/05-TASK-BREAKDOWN.yaml"
```

---

## PROMPT TEMPLATES (Copy-Paste)

### 🔧 Generate Entity
```
Read .ai-specs/01-ENTITIES.yaml and create the [ENTITY_NAME] entity.

Requirements:
1. Follow the exact property definitions
2. Use ABP.io base classes
3. Include domain methods from the spec
4. Add data annotations for validation
5. Create in src/Grc.Domain/[Module]/[Entity].cs
```

### 🔧 Generate Repository
```
Based on .ai-specs/01-ENTITIES.yaml#[ENTITY_NAME], create:
1. IRepository interface in src/Grc.Domain/[Module]/
2. Repository implementation in src/Grc.EntityFrameworkCore/[Module]/

Include these methods:
- GetByCodeAsync
- GetListWithDetailsAsync
- SearchAsync
```

### 🔧 Generate Application Service
```
Read .ai-specs/03-API-SPEC.yaml#/grc/[endpoint] and create:
1. DTOs in src/Grc.Application.Contracts/
2. IAppService interface
3. AppService implementation

Map all API operations to service methods.
Use AutoMapper for entity-DTO mapping.
```

### 🔧 Generate API Controller
```
Based on .ai-specs/03-API-SPEC.yaml, generate REST controller for [RESOURCE].

Requirements:
1. Use ABP's auto API controller conventions
2. Add [Authorize] with proper permissions
3. Include Swagger annotations
4. Support pagination with PagedResultDto
```

### 🔧 Generate Angular Service
```
Read .ai-specs/03-API-SPEC.yaml and create Angular service for [RESOURCE].

Create in: angular/src/app/core/services/[resource].service.ts

Include:
- All CRUD operations
- Proper TypeScript interfaces
- Observable-based methods
- Error handling
```

### 🔧 Generate Database Migration
```
Based on .ai-specs/02-DATABASE-SCHEMA.sql, create EF Core entity configuration:

1. Read the table definition for [TABLE_NAME]
2. Create EntityTypeConfiguration
3. Include indexes, constraints, relationships
4. Configure column types for PostgreSQL
```

### 🔧 Execute Implementation Task
```
Execute task [TASK_ID] from .ai-specs/05-TASK-BREAKDOWN.yaml.

Steps:
1. Read the task specification
2. Check dependencies are complete
3. Create all required files
4. Follow the coding standards
5. Add unit tests if applicable
```

---

## EXAMPLE WORKFLOW

### Creating the Assessment Module (Step-by-Step)

**Step 1: Generate Entities**
```
@Cursor or @Copilot:

Read .ai-specs/01-ENTITIES.yaml and create these files:

1. src/Grc.Domain/Assessments/Assessment.cs
   - Use spec: 01-ENTITIES.yaml#Assessment
   
2. src/Grc.Domain/Assessments/ControlAssessment.cs
   - Use spec: 01-ENTITIES.yaml#ControlAssessment

Follow ABP.io patterns exactly.
```

**Step 2: Generate Repository**
```
Create repository for Assessment:

1. src/Grc.Domain/Assessments/IAssessmentRepository.cs
2. src/Grc.EntityFrameworkCore/Assessments/AssessmentRepository.cs

Methods needed:
- GetWithControlsAsync(Guid id)
- GetListByStatusAsync(AssessmentStatus status)
- GetProgressAsync(Guid id)
```

**Step 3: Generate DTOs**
```
Based on .ai-specs/03-API-SPEC.yaml#AssessmentDto, create:

1. src/Grc.Application.Contracts/Assessments/AssessmentDto.cs
2. src/Grc.Application.Contracts/Assessments/CreateAssessmentInput.cs
3. src/Grc.Application.Contracts/Assessments/AssessmentProgressDto.cs
```

**Step 4: Generate Application Service**
```
Create AssessmentAppService implementing IAssessmentAppService:

File: src/Grc.Application/Assessments/AssessmentAppService.cs

Implement all methods from .ai-specs/03-API-SPEC.yaml:
- CreateAsync
- GetAsync
- GetListAsync
- GenerateAsync (auto-generate from profile)
- GetProgressAsync
- StartAsync
- CompleteAsync
```

**Step 5: Generate Angular Components**
```
Create Angular assessment module:

1. ng generate module features/assessments --routing
2. Create components:
   - assessment-list
   - assessment-detail
   - assessment-wizard
   - control-list

Use PrimeNG components.
Support Arabic RTL.
```

---

## TROUBLESHOOTING

### Agent Not Reading Specs?
```
Be explicit:
"First, read the file .ai-specs/01-ENTITIES.yaml completely.
Then, based on the Assessment entity definition, create..."
```

### Wrong Patterns?
```
Add constraint:
"Use ABP.io patterns ONLY. Do not use generic .NET patterns.
Reference: https://docs.abp.io/en/abp/latest/Domain-Driven-Design"
```

### Missing Multi-Tenancy?
```
Add reminder:
"This entity is tenant-scoped. 
Add: IMultiTenant interface
Add: public Guid? TenantId { get; set; }"
```

### Wrong Base Class?
```
Specify exactly:
"Use FullAuditedAggregateRoot<Guid> for aggregate roots
Use FullAuditedEntity<Guid> for child entities
Do NOT use basic Entity<Guid>"
```

---

## FILE QUICK REFERENCE

| Need | Read This File |
|------|----------------|
| Project overview | `00-PROJECT-SPEC.yaml` |
| Entity structure | `01-ENTITIES.yaml` |
| Database tables | `02-DATABASE-SCHEMA.sql` |
| API endpoints | `03-API-SPEC.yaml` |
| Setup commands | `04-ABP-CLI-SETUP.sh` |
| Task list | `05-TASK-BREAKDOWN.yaml` |

---

## PRO TIPS

1. **Start with Task Breakdown**: Use `05-TASK-BREAKDOWN.yaml` to work systematically

2. **Reference Specific Sections**: Use anchors like `01-ENTITIES.yaml#Assessment`

3. **Chain Commands**: Generate entity → repository → DTO → service → controller

4. **Validate Output**: Always check generated code against spec

5. **Use Code Blocks from Specs**: The `code:` sections in YAML contain ready-to-use C#

6. **Batch Generate**: Ask for multiple related files at once for consistency
