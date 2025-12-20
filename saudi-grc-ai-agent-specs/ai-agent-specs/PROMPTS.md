# ============================================================
# COPY-PASTE PROMPTS FOR AI CODING AGENTS
# ============================================================
# Use these prompts directly in Cursor, Copilot, Claude, etc.
# Just copy, paste, and execute!
# ============================================================

## 🚀 PROJECT SETUP

### Prompt 1: Create ABP Solution
```
Create a new ABP.io open source solution with these specifications:

- Solution name: Grc
- UI: Angular
- Database: PostgreSQL
- Multi-tenancy: Enabled with domain-based resolution

Execute these commands:
1. abp new Grc -t app -u angular -d ef -dbms PostgreSQL
2. Configure domain-based tenant resolution for "{tenant}.grc-platform.sa"
3. Add packages: SignalR, Redis caching, RabbitMQ, MinIO blob storage

Create the module structure for these domain modules:
- FrameworkLibrary
- Assessment
- Evidence
- Risk
- Workflow
- Reporting
```

---

## 📦 ENTITY GENERATION

### Prompt 2: Create Regulator Entity
```
Create a Regulator entity for the GRC platform:

File: src/Grc.Domain/Regulators/Regulator.cs

Properties:
- Code (string, 20, unique) - e.g., "SAMA", "NCA", "CMA"
- Name (LocalizedString) - bilingual name
- Jurisdiction (LocalizedString) - regulatory jurisdiction
- Website (string, 500)
- Category (string, 50) - e.g., "Financial", "Cybersecurity"
- LogoUrl (string, 500)
- ContactEmail (string)
- ContactPhone (string)

Requirements:
1. Extend FullAuditedAggregateRoot<Guid>
2. Use private setters
3. Add constructor with required parameters
4. Add navigation property: ICollection<Framework> Frameworks
5. Add Check.NotNullOrWhiteSpace validation

Also create:
- IRegulatorRepository interface
- RegulatorDto
- CreateUpdateRegulatorDto
```

### Prompt 3: Create Framework Entity
```
Create a Framework entity for the GRC platform:

File: src/Grc.Domain/Frameworks/Framework.cs

Properties:
- RegulatorId (Guid, FK to Regulator)
- Code (string, 30) - e.g., "NCA-ECC", "SAMA-CSF"
- Version (string, 20) - e.g., "v2.0"
- Title (LocalizedString)
- Description (LocalizedString)
- Category (string, 50)
- IsMandatory (bool, default true)
- EffectiveDate (DateTime)
- SunsetDate (DateTime?)
- Status (FrameworkStatus enum)
- OfficialDocumentUrl (string, 500)

Requirements:
1. Extend FullAuditedAggregateRoot<Guid>
2. Add unique constraint on (Code, Version)
3. Add navigation: ICollection<Control> Controls
4. Add computed property: TotalControls => Controls?.Count ?? 0
5. Add method: AddControl() to add child controls

Also create the FrameworkStatus enum:
- Draft = 0
- Active = 1
- Deprecated = 2
- Archived = 3
```

### Prompt 4: Create Control Entity
```
Create a Control entity for the GRC platform:

File: src/Grc.Domain/Frameworks/Control.cs

Properties:
- FrameworkId (Guid, FK)
- ParentControlId (Guid?, self-reference for hierarchy)
- ControlNumber (string, 30) - e.g., "1-1-1", "A.5.1"
- DomainCode (string, 50)
- Title (LocalizedString)
- Requirement (LocalizedString)
- ImplementationGuidance (LocalizedString)
- Type (ControlType: Preventive/Detective/Corrective)
- Category (ControlCategory: Technical/Administrative/Physical)
- MaturityLevel (int, 1-5)
- Priority (Priority: Critical/High/Medium/Low)
- EvidenceTypes (List<string>)
- EstimatedEffortHours (int)
- MappingISO27001 (string, 50)
- MappingNIST (string, 50)
- MappingCOBIT (string, 50)
- Tags (List<string>)
- Status (FrameworkStatus)

Requirements:
1. Extend FullAuditedEntity<Guid>
2. Unique constraint on (FrameworkId, ControlNumber)
3. Add navigation: ICollection<Control> ChildControls
4. Add validation: MaturityLevel between 1-5
```

### Prompt 5: Create Assessment Entity
```
Create an Assessment entity for the GRC platform:

File: src/Grc.Domain/Assessments/Assessment.cs

Properties:
- TenantId (Guid?) - implement IMultiTenant
- Name (string, 200)
- Description (string, 2000)
- Type (AssessmentType: Initial/Annual/Continuous/Targeted/Regulatory)
- Status (AssessmentStatus: Draft/Planning/InProgress/UnderReview/Completed/Cancelled)
- StartDate (DateTime)
- TargetEndDate (DateTime)
- ActualEndDate (DateTime?)
- OwnerUserId (Guid?)
- Scope (AssessmentScope - JSON object)

Computed Properties:
- TotalControls => ControlAssessments?.Count ?? 0
- CompletedControls => ControlAssessments?.Count(c => c.IsComplete) ?? 0
- CompletionPercentage => calculated percentage
- OverallScore => average of verified scores

Collections:
- ICollection<AssessmentFramework> Frameworks
- ICollection<ControlAssessment> ControlAssessments

Domain Methods:
- Start() - change status to InProgress
- Complete() - mark as completed
- AddControlAssessment(controlId) - add control to assessment

Domain Events:
- AssessmentCreatedEto
- AssessmentStartedEto
- AssessmentCompletedEto

Requirements:
1. Extend FullAuditedAggregateRoot<Guid>, IMultiTenant
2. Use ABP's AddDistributedEvent for domain events
```

### Prompt 6: Create ControlAssessment Entity
```
Create a ControlAssessment entity for the GRC platform:

File: src/Grc.Domain/Assessments/ControlAssessment.cs

Properties:
- TenantId (Guid?)
- AssessmentId (Guid, FK)
- ControlId (Guid, FK to Control)
- AssignedToUserId (Guid?)
- AssignedToDepartmentId (Guid?)
- Status (ControlAssessmentStatus: NotStarted/InProgress/PendingReview/Verified/Rejected/NotApplicable)
- SelfScore (decimal?, 0-100)
- VerifiedScore (decimal?, 0-100)
- VerifiedByUserId (Guid?)
- VerificationDate (DateTime?)
- ImplementationNotes (string, 4000)
- RejectionReason (string, 2000)
- DueDate (DateTime?)
- Priority (Priority)

Computed:
- IsComplete => Status == Verified
- IsOverdue => DueDate < Now && !IsComplete

Collections:
- ICollection<Evidence> Evidences
- ICollection<ControlAssessmentComment> Comments
- ICollection<ControlAssessmentHistory> History

Domain Methods:
- AssignTo(userId, dueDate) - assign to user
- SubmitSelfScore(score, notes) - submit self-assessment
- Verify(verifierId, score) - verify by manager
- Reject(verifierId, reason) - reject with reason

Domain Events:
- ControlAssignedEto
- SelfScoreSubmittedEto
- ControlVerifiedEto
- ControlRejectedEto
```

---

## 🔌 API GENERATION

### Prompt 7: Create Framework AppService
```
Create the FrameworkAppService for the GRC platform:

File: src/Grc.Application/Frameworks/FrameworkAppService.cs

Interface methods to implement:
1. GetListAsync(GetFrameworkListInput) - paginated list with filters
2. GetAsync(Guid id) - single framework with details
3. GetControlsAsync(Guid frameworkId, GetControlListInput) - controls for framework
4. SearchControlsAsync(string query, Guid[] frameworkIds) - full-text search

DTOs needed:
- FrameworkDto (includes RegulatorDto)
- FrameworkListDto (summary)
- ControlDto
- ControlListDto
- GetFrameworkListInput (filters: regulatorId, category, status, isMandatory)
- GetControlListInput (filters: domain, type, maturityLevel)

Requirements:
1. Use [Authorize(GrcPermissions.Frameworks.View)]
2. Use IRepository<Framework, Guid>
3. Use AutoMapper for mapping
4. Support pagination with PagedResultDto
5. Include eager loading for related entities
```

### Prompt 8: Create Assessment AppService
```
Create the AssessmentAppService for the GRC platform:

File: src/Grc.Application/Assessments/AssessmentAppService.cs

Methods:
1. CreateAsync(CreateAssessmentInput) - create new assessment
2. GenerateAsync(GenerateAssessmentInput) - auto-generate from org profile
3. GetAsync(Guid id) - get with details
4. GetListAsync(GetAssessmentListInput) - paginated list
5. GetProgressAsync(Guid id) - progress metrics
6. GetControlsAsync(Guid id, GetControlAssessmentListInput) - control assessments
7. StartAsync(Guid id) - start assessment
8. CompleteAsync(Guid id) - complete assessment
9. GenerateReportAsync(Guid id, ReportFormat) - generate PDF/Excel

GenerateAsync logic:
- Read tenant's organization profile
- Determine applicable frameworks based on:
  - Industry sector (Banking → SAMA, Healthcare → MOH)
  - Universal requirements (NCA-ECC for all)
  - Data processing (PDPL if processes personal data)
  - Payment processing (PCI-DSS if accepts cards)
  - Listed company (CMA if publicly listed)
- Create assessment with all applicable controls

Requirements:
1. Multi-tenant: auto-filter by CurrentTenant.Id
2. Authorization per method
3. Publish domain events
4. Use unit of work pattern
```

### Prompt 9: Create ControlAssessment AppService
```
Create the ControlAssessmentAppService for the GRC platform:

File: src/Grc.Application/Assessments/ControlAssessmentAppService.cs

Methods:
1. GetAsync(Guid id) - get with control details, evidence, comments
2. AssignAsync(Guid id, AssignControlInput) - assign to user
3. BulkAssignAsync(BulkAssignInput) - bulk assign multiple controls
4. SubmitScoreAsync(Guid id, SubmitScoreInput) - submit self-score
5. VerifyAsync(Guid id, VerifyControlInput) - manager verification
6. RejectAsync(Guid id, RejectControlInput) - reject with reason
7. UploadEvidenceAsync(Guid id, UploadEvidenceInput) - upload evidence file

Permissions:
- ViewOwn: can view own assignments
- ViewDepartment: can view department's assignments
- ViewAll: can view all
- UpdateOwn: can update own assignments
- UploadEvidence: can upload evidence
- Verify: can verify/reject controls

Requirements:
1. Check assignment ownership for UpdateOwn
2. Publish events on status changes
3. Update parent assessment progress
4. Notify via SignalR on changes
```

---

## 🎨 ANGULAR GENERATION

### Prompt 10: Create Angular Services
```
Create Angular services for the GRC platform:

Path: angular/src/app/core/services/

1. framework.service.ts
   - getRegulators(): Observable<PagedResultDto<RegulatorDto>>
   - getFrameworks(input): Observable<PagedResultDto<FrameworkDto>>
   - getFramework(id): Observable<FrameworkDto>
   - getControls(frameworkId, input): Observable<PagedResultDto<ControlDto>>
   - searchControls(query): Observable<ControlDto[]>

2. assessment.service.ts
   - create(input): Observable<AssessmentDto>
   - generate(input): Observable<AssessmentDto>
   - getList(input): Observable<PagedResultDto<AssessmentDto>>
   - get(id): Observable<AssessmentDetailDto>
   - getProgress(id): Observable<AssessmentProgressDto>
   - start(id): Observable<AssessmentDto>
   - complete(id): Observable<AssessmentDto>
   - generateReport(id, format): Observable<Blob>

3. control-assessment.service.ts
   - get(id): Observable<ControlAssessmentDetailDto>
   - assign(id, input): Observable<ControlAssessmentDto>
   - bulkAssign(input): Observable<BulkOperationResult>
   - submitScore(id, input): Observable<ControlAssessmentDto>
   - verify(id, input): Observable<ControlAssessmentDto>
   - reject(id, input): Observable<ControlAssessmentDto>
   - uploadEvidence(id, file, input): Observable<EvidenceDto>

4. dashboard.service.ts
   - getOverview(): Observable<DashboardOverviewDto>
   - getMyControls(): Observable<MyControlDto[]>
   - getFrameworkProgress(assessmentId?): Observable<FrameworkProgressDto[]>

Requirements:
1. Use HttpClient with ABP's API prefix
2. Use proper TypeScript interfaces for DTOs
3. Handle errors with catchError
4. Support file upload with FormData
```

### Prompt 11: Create Dashboard Component
```
Create Angular dashboard component for the GRC platform:

Path: angular/src/app/features/dashboard/

Components:
1. dashboard.component.ts - main container
2. overview-cards.component.ts - summary cards
3. framework-progress.component.ts - progress by framework chart
4. my-controls-table.component.ts - assigned controls table
5. timeline-chart.component.ts - compliance timeline

Features:
- Display total assessments, controls, completion %
- Pie chart for control status distribution
- Bar chart for framework progress
- Table of user's assigned controls with filters
- Line chart for compliance score over time

Use:
- PrimeNG components (p-card, p-table, p-chart)
- NgRx for state management
- SignalR for real-time updates
- Support Arabic RTL layout

Create interfaces:
- DashboardOverviewDto
- FrameworkProgressDto
- MyControlDto
```

---

## 🗄️ DATABASE

### Prompt 12: Create EF Core Configurations
```
Create EF Core entity configurations for PostgreSQL:

Path: src/Grc.EntityFrameworkCore/EntityTypeConfigurations/

1. RegulatorConfiguration.cs
2. FrameworkConfiguration.cs
3. ControlConfiguration.cs
4. AssessmentConfiguration.cs
5. ControlAssessmentConfiguration.cs
6. EvidenceConfiguration.cs

Requirements:
1. Use ABP's ConfigureByConvention()
2. Set table names with "grc_" prefix
3. Configure indexes on frequently queried columns
4. Configure JSON columns for complex types (PostgreSQL jsonb)
5. Configure soft delete filter
6. Set proper column types for PostgreSQL
7. Configure cascade delete rules

Example:
```csharp
public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.ToTable("grc_assessments");
        builder.ConfigureByConvention();
        
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.TenantId, x.Status });
        
        builder.Property(x => x.Scope)
            .HasColumnType("jsonb");
    }
}
```
```

---

## 🔄 WORKFLOW

### Prompt 13: Create Workflow Engine
```
Create a workflow engine for the GRC platform:

Entities:
1. WorkflowDefinition - defines workflow steps
2. WorkflowInstance - running workflow instance
3. WorkflowTask - individual task in workflow

Example workflow: Evidence Approval
1. Control Owner uploads evidence
2. Task created for Department Head review
3. If approved → Task for Compliance Manager
4. If rejected → Back to Control Owner
5. Final approval → Control marked as Verified

Implementation:
1. WorkflowDefinition stores steps as JSON
2. WorkflowEngine service processes instances
3. WorkflowTask assigned to users/roles
4. SLA tracking with due dates
5. Escalation on timeout

Create:
- src/Grc.Workflow.Domain/WorkflowDefinition.cs
- src/Grc.Workflow.Domain/WorkflowInstance.cs
- src/Grc.Workflow.Domain/WorkflowTask.cs
- src/Grc.Workflow.Application/WorkflowEngine.cs
- Support for approval/rejection actions
- Notification on task creation
```

---

## 🤖 AI FEATURES

### Prompt 14: Create AI Compliance Engine
```
Create an AI engine for the GRC platform using ML.NET:

File: src/Grc.AI.Application/AiComplianceEngine.cs

Features:
1. Document Classification
   - Input: uploaded file content
   - Output: suggested EvidenceType + confidence score
   - Train on labeled evidence documents

2. Control Recommendations
   - Input: control ID, organization context
   - Output: implementation suggestions based on similar orgs
   - Use embeddings to find similar controls

3. Gap Prediction
   - Input: organization profile
   - Output: predicted compliance gaps
   - Based on historical assessment data

4. Text Extraction (OCR)
   - Input: PDF/image file
   - Output: extracted text
   - Use Tesseract for Arabic + English

Implementation:
1. Use ML.NET for classification models
2. Store embeddings in PostgreSQL with pgvector
3. Background job for model training
4. Caching for frequent predictions

Create DTOs:
- DocumentClassificationResult
- ControlRecommendation
- GapPrediction
```

---

## 📊 DATA IMPORT

### Prompt 15: Import Framework Data
```
Create a data seeder to import all GRC framework data:

File: src/Grc.Domain/Data/GrcDataSeeder.cs

Data sources (CSV files in project):
- regulators_complete.csv (33 regulators)
- frameworks_complete.csv (50+ frameworks)
- controls_*.csv (3500+ controls)

Implementation:
1. Read CSV files from embedded resources
2. Parse with CsvHelper library
3. Map to domain entities
4. Bulk insert with EF Core
5. Handle bilingual content (Arabic/English)
6. Create cross-framework mappings

Frameworks to import:
- NCA-ECC (114 controls)
- SAMA-CSF (97 controls)
- PDPL (45 controls)
- ISO 27001 (93 controls)
- NIST CSF (108 controls)
- PCI-DSS (124 controls)
- And 40+ more...

Run as:
1. Part of database migration
2. Separate CLI command: dotnet run --seed-data
```

---

## 🔐 SECURITY

### Prompt 16: Setup Permissions
```
Create the permission system for GRC platform:

File: src/Grc.Application.Contracts/Permissions/GrcPermissions.cs

Permission groups:
1. Frameworks - View, Create, Edit, Delete, Import
2. Assessments - View, Create, Edit, Delete, AssignControls, VerifyControls
3. ControlAssessments - ViewOwn, ViewDepartment, ViewAll, UpdateOwn, UploadEvidence, SubmitForReview
4. Evidence - View, Upload, Delete, Download
5. Risks - View, Create, Edit, Delete, Assess, Treat
6. Reports - View, Generate, Export
7. Workflows - View, Manage, Execute
8. Admin - ManageUsers, ManageSettings, ViewAuditLog

Role definitions:
1. SystemAdmin - all permissions
2. TenantAdmin - all Grc.* permissions
3. ComplianceManager - Assessments.*, Reports.*, ControlAssessments.ViewAll
4. DepartmentHead - ControlAssessments.ViewDepartment, assign within dept
5. ControlOwner - ControlAssessments.ViewOwn, UpdateOwn, UploadEvidence
6. RiskManager - Risks.*
7. InternalAuditor - *.View, Audits.*
8. ExternalAuditor - Assessments.View, Evidence.View (scoped)
9. Executive - Reports.View, Dashboards.View
10. Viewer - *.View (scoped)

Create:
- GrcPermissions.cs (constants)
- GrcPermissionDefinitionProvider.cs (register with ABP)
```

---

# END OF PROMPTS
