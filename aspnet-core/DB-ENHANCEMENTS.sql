-- ============================================================
-- DATABASE ENHANCEMENTS FOR GRC PLATFORM
-- Advanced Indexes, Constraints, and Optimizations
-- ============================================================

-- ============================================================
-- 1. REGULATORS TABLE ENHANCEMENTS
-- ============================================================

-- Unique constraint on Code
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Regulators_Code_Unique" 
    ON "Regulators"("Code") WHERE "IsDeleted" = false;

-- Index on Category for filtering
CREATE INDEX IF NOT EXISTS "IX_Regulators_Category" 
    ON "Regulators"("Category");

-- Full-text search index on Name fields
CREATE INDEX IF NOT EXISTS "IX_Regulators_NameEn_FTS" 
    ON "Regulators" USING gin(to_tsvector('english', "NameEn"));

CREATE INDEX IF NOT EXISTS "IX_Regulators_NameAr_FTS" 
    ON "Regulators" USING gin(to_tsvector('arabic', "NameAr"));

-- Composite index for common queries
CREATE INDEX IF NOT EXISTS "IX_Regulators_Category_IsDeleted" 
    ON "Regulators"("Category", "IsDeleted");

-- Index on Website for quick lookups
CREATE INDEX IF NOT EXISTS "IX_Regulators_Website" 
    ON "Regulators"("Website") WHERE "Website" IS NOT NULL;

-- ============================================================
-- 2. FRAMEWORKS TABLE ENHANCEMENTS
-- ============================================================

-- Unique constraint on Code + Version
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Frameworks_Code_Version_Unique" 
    ON "Frameworks"("Code", "Version") WHERE "IsDeleted" = false;

-- Index on Status for filtering active frameworks
CREATE INDEX IF NOT EXISTS "IX_Frameworks_Status" 
    ON "Frameworks"("Status");

-- Index on IsMandatory for compliance queries
CREATE INDEX IF NOT EXISTS "IX_Frameworks_IsMandatory" 
    ON "Frameworks"("IsMandatory");

-- Full-text search on Title
CREATE INDEX IF NOT EXISTS "IX_Frameworks_TitleEn_FTS" 
    ON "Frameworks" USING gin(to_tsvector('english', "TitleEn"));

CREATE INDEX IF NOT EXISTS "IX_Frameworks_TitleAr_FTS" 
    ON "Frameworks" USING gin(to_tsvector('arabic', "TitleAr"));

-- Composite index for common filtering
CREATE INDEX IF NOT EXISTS "IX_Frameworks_Regulator_Status_Mandatory" 
    ON "Frameworks"("RegulatorId", "Status", "IsMandatory");

-- Index on EffectiveDate for timeline queries
CREATE INDEX IF NOT EXISTS "IX_Frameworks_EffectiveDate" 
    ON "Frameworks"("EffectiveDate");

-- ============================================================
-- 3. CONTROLS TABLE ENHANCEMENTS
-- ============================================================

-- Unique constraint on Framework + ControlNumber
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Controls_Framework_Number_Unique" 
    ON "Controls"("FrameworkId", "ControlNumber") WHERE "IsDeleted" = false;

-- Index on Type for filtering
CREATE INDEX IF NOT EXISTS "IX_Controls_Type" 
    ON "Controls"("Type");

-- Index on Priority for sorting
CREATE INDEX IF NOT EXISTS "IX_Controls_Priority" 
    ON "Controls"("Priority");

-- Index on MaturityLevel
CREATE INDEX IF NOT EXISTS "IX_Controls_MaturityLevel" 
    ON "Controls"("MaturityLevel");

-- Index on Status
CREATE INDEX IF NOT EXISTS "IX_Controls_Status" 
    ON "Controls"("Status");

-- Full-text search on Title and Requirement
CREATE INDEX IF NOT EXISTS "IX_Controls_TitleEn_FTS" 
    ON "Controls" USING gin(to_tsvector('english', "TitleEn"));

CREATE INDEX IF NOT EXISTS "IX_Controls_RequirementEn_FTS" 
    ON "Controls" USING gin(to_tsvector('english', "RequirementEn"));

-- GIN index on Tags array for fast searching
CREATE INDEX IF NOT EXISTS "IX_Controls_Tags_GIN" 
    ON "Controls" USING gin("Tags");

-- GIN index on EvidenceTypes array
CREATE INDEX IF NOT EXISTS "IX_Controls_EvidenceTypes_GIN" 
    ON "Controls" USING gin("EvidenceTypes");

-- Composite indexes for common queries
CREATE INDEX IF NOT EXISTS "IX_Controls_Framework_Priority_Status" 
    ON "Controls"("FrameworkId", "Priority", "Status");

CREATE INDEX IF NOT EXISTS "IX_Controls_Type_Category_Priority" 
    ON "Controls"("Type", "Category", "Priority");

-- Index on DomainCode for grouping
CREATE INDEX IF NOT EXISTS "IX_Controls_DomainCode" 
    ON "Controls"("DomainCode");

-- Index on cross-framework mappings
CREATE INDEX IF NOT EXISTS "IX_Controls_MappingISO27001" 
    ON "Controls"("MappingISO27001") WHERE "MappingISO27001" != '';

CREATE INDEX IF NOT EXISTS "IX_Controls_MappingNIST" 
    ON "Controls"("MappingNIST") WHERE "MappingNIST" != '';

-- ============================================================
-- 4. RISKS TABLE ENHANCEMENTS
-- ============================================================

-- Index on Status
CREATE INDEX IF NOT EXISTS "IX_Risks_Status" 
    ON "Risks"("Status");

-- Index on Category
CREATE INDEX IF NOT EXISTS "IX_Risks_Category" 
    ON "Risks"("Category");

-- Full-text search on Title
CREATE INDEX IF NOT EXISTS "IX_Risks_TitleEn_FTS" 
    ON "Risks" USING gin(to_tsvector('english', "TitleEn"));

-- Index on RiskOwnerId for user-specific queries
CREATE INDEX IF NOT EXISTS "IX_Risks_RiskOwnerId" 
    ON "Risks"("RiskOwnerId") WHERE "RiskOwnerId" IS NOT NULL;

-- Composite index for risk assessment queries
CREATE INDEX IF NOT EXISTS "IX_Risks_Status_Category_InherentRiskLevel" 
    ON "Risks"("Status", "Category", "InherentRiskLevel");

-- ============================================================
-- 5. EVIDENCES TABLE ENHANCEMENTS
-- ============================================================

-- Index on FileHash for duplicate detection
CREATE INDEX IF NOT EXISTS "IX_Evidences_FileHash" 
    ON "Evidences"("FileHash") WHERE "FileHash" IS NOT NULL;

-- Index on UploadDate for chronological queries
CREATE INDEX IF NOT EXISTS "IX_Evidences_UploadDate" 
    ON "Evidences"("UploadDate");

-- Full-text search on ExtractedText
CREATE INDEX IF NOT EXISTS "IX_Evidences_ExtractedText_FTS" 
    ON "Evidences" USING gin(to_tsvector('english', "ExtractedText")) 
    WHERE "ExtractedText" IS NOT NULL;

-- ============================================================
-- 6. ABP TABLES ENHANCEMENTS
-- ============================================================

-- Index on AbpUsers Email for faster lookups
CREATE INDEX IF NOT EXISTS "IX_AbpUsers_Email" 
    ON "AbpUsers"("NormalizedEmail") WHERE "IsDeleted" = false;

-- Index on AbpUsers UserName
CREATE INDEX IF NOT EXISTS "IX_AbpUsers_UserName" 
    ON "AbpUsers"("NormalizedUserName") WHERE "IsDeleted" = false;

-- Index on AbpTenants Name
CREATE INDEX IF NOT EXISTS "IX_AbpTenants_Name" 
    ON "AbpTenants"("NormalizedName");

-- Index on AbpAuditLogs for performance
CREATE INDEX IF NOT EXISTS "IX_AbpAuditLogs_ExecutionTime" 
    ON "AbpAuditLogs"("ExecutionTime");

CREATE INDEX IF NOT EXISTS "IX_AbpAuditLogs_UserId" 
    ON "AbpAuditLogs"("UserId") WHERE "UserId" IS NOT NULL;

-- ============================================================
-- 7. PERFORMANCE VIEWS
-- ============================================================

-- View: Active Frameworks by Regulator
CREATE OR REPLACE VIEW "vw_ActiveFrameworksByRegulator" AS
SELECT 
    r."Code" as "RegulatorCode",
    r."NameEn" as "RegulatorName",
    f."Code" as "FrameworkCode",
    f."Version",
    f."TitleEn" as "FrameworkTitle",
    f."IsMandatory",
    f."EffectiveDate",
    (SELECT COUNT(*) FROM "Controls" c WHERE c."FrameworkId" = f."Id" AND c."IsDeleted" = false) as "ControlCount"
FROM "Regulators" r
JOIN "Frameworks" f ON f."RegulatorId" = r."Id"
WHERE r."IsDeleted" = false AND f."IsDeleted" = false AND f."Status" = 1;

-- View: Control Summary by Framework
CREATE OR REPLACE VIEW "vw_ControlSummaryByFramework" AS
SELECT 
    f."Code" as "FrameworkCode",
    f."TitleEn" as "FrameworkTitle",
    COUNT(c."Id") as "TotalControls",
    COUNT(c."Id") FILTER (WHERE c."Type" = 1) as "PreventiveControls",
    COUNT(c."Id") FILTER (WHERE c."Type" = 2) as "DetectiveControls",
    COUNT(c."Id") FILTER (WHERE c."Type" = 3) as "CorrectiveControls",
    COUNT(c."Id") FILTER (WHERE c."Priority" = 1) as "CriticalControls",
    AVG(c."EstimatedEffortHours") as "AvgEffortHours"
FROM "Frameworks" f
LEFT JOIN "Controls" c ON c."FrameworkId" = f."Id" AND c."IsDeleted" = false
WHERE f."IsDeleted" = false
GROUP BY f."Id", f."Code", f."TitleEn";

-- View: Regulator Statistics
CREATE OR REPLACE VIEW "vw_RegulatorStatistics" AS
SELECT 
    r."Code",
    r."NameEn" as "Name",
    r."Category",
    (SELECT COUNT(*) FROM "Frameworks" f WHERE f."RegulatorId" = r."Id" AND f."IsDeleted" = false) as "FrameworkCount",
    (SELECT COUNT(*) FROM "Controls" c JOIN "Frameworks" f ON c."FrameworkId" = f."Id" 
     WHERE f."RegulatorId" = r."Id" AND c."IsDeleted" = false AND f."IsDeleted" = false) as "TotalControls"
FROM "Regulators" r
WHERE r."IsDeleted" = false;

-- ============================================================
-- 8. STATISTICS AND MAINTENANCE
-- ============================================================

-- Analyze tables for better query planning
ANALYZE "Regulators";
ANALYZE "Frameworks";
ANALYZE "Controls";
ANALYZE "Risks";
ANALYZE "Evidences";

-- ============================================================
-- 9. DATABASE FUNCTIONS
-- ============================================================

-- Function: Get Framework Hierarchy
CREATE OR REPLACE FUNCTION get_framework_hierarchy(framework_code text)
RETURNS TABLE (
    control_id uuid,
    control_number text,
    title text,
    level integer
) AS $$
BEGIN
    RETURN QUERY
    WITH RECURSIVE control_tree AS (
        -- Base case: top-level controls
        SELECT 
            c."Id",
            c."ControlNumber",
            c."TitleEn",
            1 as level
        FROM "Controls" c
        JOIN "Frameworks" f ON c."FrameworkId" = f."Id"
        WHERE f."Code" = framework_code 
          AND c."ParentControlId" IS NULL
          AND c."IsDeleted" = false
        
        UNION ALL
        
        -- Recursive case: child controls
        SELECT 
            c."Id",
            c."ControlNumber",
            c."TitleEn",
            ct.level + 1
        FROM "Controls" c
        JOIN control_tree ct ON c."ParentControlId" = ct."Id"
        WHERE c."IsDeleted" = false
    )
    SELECT * FROM control_tree ORDER BY control_number;
END;
$$ LANGUAGE plpgsql;

-- Function: Search Controls by Text
CREATE OR REPLACE FUNCTION search_controls(search_text text, language_code text DEFAULT 'en')
RETURNS TABLE (
    id uuid,
    framework_code text,
    control_number text,
    title text,
    rank real
) AS $$
BEGIN
    IF language_code = 'ar' THEN
        RETURN QUERY
        SELECT 
            c."Id",
            f."Code",
            c."ControlNumber",
            c."TitleAr",
            ts_rank(to_tsvector('arabic', c."TitleAr"), plainto_tsquery('arabic', search_text)) as rank
        FROM "Controls" c
        JOIN "Frameworks" f ON c."FrameworkId" = f."Id"
        WHERE to_tsvector('arabic', c."TitleAr") @@ plainto_tsquery('arabic', search_text)
          AND c."IsDeleted" = false
        ORDER BY rank DESC
        LIMIT 100;
    ELSE
        RETURN QUERY
        SELECT 
            c."Id",
            f."Code",
            c."ControlNumber",
            c."TitleEn",
            ts_rank(to_tsvector('english', c."TitleEn"), plainto_tsquery('english', search_text)) as rank
        FROM "Controls" c
        JOIN "Frameworks" f ON c."FrameworkId" = f."Id"
        WHERE to_tsvector('english', c."TitleEn") @@ plainto_tsquery('english', search_text)
          AND c."IsDeleted" = false
        ORDER BY rank DESC
        LIMIT 100;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- ============================================================
-- 10. PARTITIONING SETUP (For future scalability)
-- ============================================================

-- Comment: Controls table could be partitioned by FrameworkId for large datasets
-- Comment: AuditLogs could be partitioned by ExecutionTime (monthly)
-- Comment: This can be implemented when data volume exceeds 1M records

-- ============================================================
-- 11. MATERIALIZED VIEWS FOR REPORTING
-- ============================================================

-- Materialized View: Framework Compliance Overview
CREATE MATERIALIZED VIEW IF NOT EXISTS "mv_FrameworkComplianceOverview" AS
SELECT 
    f."Id" as "FrameworkId",
    f."Code" as "FrameworkCode",
    f."TitleEn" as "FrameworkTitle",
    r."NameEn" as "RegulatorName",
    f."Category" as "Category",
    f."IsMandatory",
    COUNT(c."Id") as "TotalControls",
    COUNT(c."Id") FILTER (WHERE c."Priority" = 1) as "CriticalControls",
    COUNT(c."Id") FILTER (WHERE c."Priority" = 2) as "HighControls",
    SUM(c."EstimatedEffortHours") as "TotalEstimatedHours"
FROM "Frameworks" f
JOIN "Regulators" r ON f."RegulatorId" = r."Id"
LEFT JOIN "Controls" c ON c."FrameworkId" = f."Id" AND c."IsDeleted" = false
WHERE f."IsDeleted" = false AND r."IsDeleted" = false
GROUP BY f."Id", f."Code", f."TitleEn", r."NameEn", f."Category", f."IsMandatory";

-- Create index on materialized view
CREATE INDEX IF NOT EXISTS "IX_mv_Framework_RegulatorName" 
    ON "mv_FrameworkComplianceOverview"("RegulatorName");

CREATE INDEX IF NOT EXISTS "IX_mv_Framework_IsMandatory" 
    ON "mv_FrameworkComplianceOverview"("IsMandatory");

-- ============================================================
-- 12. CHECK CONSTRAINTS
-- ============================================================

-- Ensure RegulatorCategory is valid (1-50)
ALTER TABLE "Regulators" DROP CONSTRAINT IF EXISTS "CK_Regulators_Category";
ALTER TABLE "Regulators" ADD CONSTRAINT "CK_Regulators_Category" 
    CHECK ("Category" BETWEEN 1 AND 50);

-- Ensure Framework Status is valid (0-3)
ALTER TABLE "Frameworks" DROP CONSTRAINT IF EXISTS "CK_Frameworks_Status";
ALTER TABLE "Frameworks" ADD CONSTRAINT "CK_Frameworks_Status" 
    CHECK ("Status" BETWEEN 0 AND 3);

-- Ensure Control Type is valid (1-3)
ALTER TABLE "Controls" DROP CONSTRAINT IF EXISTS "CK_Controls_Type";
ALTER TABLE "Controls" ADD CONSTRAINT "CK_Controls_Type" 
    CHECK ("Type" BETWEEN 1 AND 3);

-- Ensure Priority is valid (1-4)
ALTER TABLE "Controls" DROP CONSTRAINT IF EXISTS "CK_Controls_Priority";
ALTER TABLE "Controls" ADD CONSTRAINT "CK_Controls_Priority" 
    CHECK ("Priority" BETWEEN 1 AND 4);

-- Ensure MaturityLevel is valid (1-5)
ALTER TABLE "Controls" DROP CONSTRAINT IF EXISTS "CK_Controls_MaturityLevel";
ALTER TABLE "Controls" ADD CONSTRAINT "CK_Controls_MaturityLevel" 
    CHECK ("MaturityLevel" BETWEEN 1 AND 5);

-- Ensure EstimatedEffortHours is positive
ALTER TABLE "Controls" DROP CONSTRAINT IF EXISTS "CK_Controls_EffortHours";
ALTER TABLE "Controls" ADD CONSTRAINT "CK_Controls_EffortHours" 
    CHECK ("EstimatedEffortHours" >= 0);

-- ============================================================
-- 13. TRIGGERS FOR AUDIT TRAIL
-- ============================================================

-- Function: Update LastModificationTime automatically
CREATE OR REPLACE FUNCTION update_modification_time()
RETURNS TRIGGER AS $$
BEGIN
    NEW."LastModificationTime" = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Triggers on main tables
DROP TRIGGER IF EXISTS "trg_Regulators_UpdateTime" ON "Regulators";
CREATE TRIGGER "trg_Regulators_UpdateTime"
    BEFORE UPDATE ON "Regulators"
    FOR EACH ROW
    EXECUTE FUNCTION update_modification_time();

DROP TRIGGER IF EXISTS "trg_Frameworks_UpdateTime" ON "Frameworks";
CREATE TRIGGER "trg_Frameworks_UpdateTime"
    BEFORE UPDATE ON "Frameworks"
    FOR EACH ROW
    EXECUTE FUNCTION update_modification_time();

DROP TRIGGER IF EXISTS "trg_Controls_UpdateTime" ON "Controls";
CREATE TRIGGER "trg_Controls_UpdateTime"
    BEFORE UPDATE ON "Controls"
    FOR EACH ROW
    EXECUTE FUNCTION update_modification_time();

-- ============================================================
-- 14. VACUUM AND MAINTENANCE
-- ============================================================

-- Vacuum analyze all tables
VACUUM ANALYZE "Regulators";
VACUUM ANALYZE "Frameworks";
VACUUM ANALYZE "Controls";
VACUUM ANALYZE "Risks";
VACUUM ANALYZE "Evidences";

-- ============================================================
-- 15. SUMMARY
-- ============================================================

SELECT 
    'Database Enhancements Applied' as "Status",
    (SELECT COUNT(*) FROM pg_indexes WHERE schemaname = 'public') as "Total Indexes",
    (SELECT COUNT(*) FROM pg_constraint WHERE connamespace = 'public'::regnamespace) as "Total Constraints",
    (SELECT COUNT(*) FROM pg_views WHERE schemaname = 'public') as "Total Views",
    (SELECT COUNT(*) FROM pg_matviews WHERE schemaname = 'public') as "Materialized Views";

