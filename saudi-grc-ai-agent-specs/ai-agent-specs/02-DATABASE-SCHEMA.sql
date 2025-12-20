-- ============================================================
-- SAUDI GRC PLATFORM - DATABASE SCHEMA
-- AI Agent: Execute this to create PostgreSQL database
-- ============================================================

-- Create schema
CREATE SCHEMA IF NOT EXISTS grc;

-- ============================================================
-- CORE TABLES (Shared across tenants)
-- ============================================================

-- Regulators Table
CREATE TABLE grc.regulators (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(20) NOT NULL UNIQUE,
    name_en VARCHAR(200) NOT NULL,
    name_ar VARCHAR(200) NOT NULL,
    jurisdiction_en VARCHAR(500),
    jurisdiction_ar VARCHAR(500),
    website VARCHAR(500),
    category VARCHAR(50) NOT NULL,
    logo_url VARCHAR(500),
    contact_email VARCHAR(200),
    contact_phone VARCHAR(50),
    contact_address TEXT,
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleter_id UUID,
    deletion_time TIMESTAMPTZ,
    extra_properties JSONB,
    concurrency_stamp VARCHAR(40)
);

CREATE INDEX idx_regulators_code ON grc.regulators(code);
CREATE INDEX idx_regulators_category ON grc.regulators(category);

-- Frameworks Table
CREATE TABLE grc.frameworks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    regulator_id UUID NOT NULL REFERENCES grc.regulators(id),
    code VARCHAR(30) NOT NULL,
    version VARCHAR(20) NOT NULL,
    title_en VARCHAR(300) NOT NULL,
    title_ar VARCHAR(300) NOT NULL,
    description_en TEXT,
    description_ar TEXT,
    category VARCHAR(50) NOT NULL,
    is_mandatory BOOLEAN NOT NULL DEFAULT TRUE,
    effective_date DATE NOT NULL,
    sunset_date DATE,
    status VARCHAR(20) NOT NULL DEFAULT 'Active',
    official_document_url VARCHAR(500),
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleter_id UUID,
    deletion_time TIMESTAMPTZ,
    extra_properties JSONB,
    concurrency_stamp VARCHAR(40),
    UNIQUE(code, version)
);

CREATE INDEX idx_frameworks_regulator ON grc.frameworks(regulator_id);
CREATE INDEX idx_frameworks_code ON grc.frameworks(code);
CREATE INDEX idx_frameworks_status ON grc.frameworks(status);
CREATE INDEX idx_frameworks_category ON grc.frameworks(category);

-- Framework Domains Table
CREATE TABLE grc.framework_domains (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    framework_id UUID NOT NULL REFERENCES grc.frameworks(id) ON DELETE CASCADE,
    code VARCHAR(50) NOT NULL,
    name_en VARCHAR(200) NOT NULL,
    name_ar VARCHAR(200) NOT NULL,
    description_en TEXT,
    description_ar TEXT,
    sort_order INT NOT NULL DEFAULT 0,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(framework_id, code)
);

CREATE INDEX idx_framework_domains_framework ON grc.framework_domains(framework_id);

-- Controls Table
CREATE TABLE grc.controls (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    framework_id UUID NOT NULL REFERENCES grc.frameworks(id) ON DELETE CASCADE,
    parent_control_id UUID REFERENCES grc.controls(id),
    control_number VARCHAR(30) NOT NULL,
    domain_code VARCHAR(50) NOT NULL,
    title_en VARCHAR(500) NOT NULL,
    title_ar VARCHAR(500) NOT NULL,
    requirement_en TEXT NOT NULL,
    requirement_ar TEXT NOT NULL,
    implementation_guidance_en TEXT,
    implementation_guidance_ar TEXT,
    control_type VARCHAR(20) NOT NULL CHECK (control_type IN ('Preventive', 'Detective', 'Corrective')),
    control_category VARCHAR(20) CHECK (control_category IN ('Technical', 'Administrative', 'Physical')),
    maturity_level INT NOT NULL DEFAULT 1 CHECK (maturity_level BETWEEN 1 AND 5),
    priority VARCHAR(20) NOT NULL DEFAULT 'Medium' CHECK (priority IN ('Critical', 'High', 'Medium', 'Low')),
    evidence_types TEXT[],
    estimated_effort_hours INT DEFAULT 0,
    mapping_iso27001 VARCHAR(50),
    mapping_nist VARCHAR(50),
    mapping_cobit VARCHAR(50),
    tags TEXT[],
    status VARCHAR(20) NOT NULL DEFAULT 'Active',
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleter_id UUID,
    deletion_time TIMESTAMPTZ,
    extra_properties JSONB,
    UNIQUE(framework_id, control_number)
);

CREATE INDEX idx_controls_framework ON grc.controls(framework_id);
CREATE INDEX idx_controls_domain ON grc.controls(domain_code);
CREATE INDEX idx_controls_parent ON grc.controls(parent_control_id);
CREATE INDEX idx_controls_type ON grc.controls(control_type);
CREATE INDEX idx_controls_status ON grc.controls(status);
CREATE INDEX idx_controls_number ON grc.controls(control_number);

-- Full-text search index for controls
CREATE INDEX idx_controls_fts ON grc.controls USING gin(
    to_tsvector('english', title_en || ' ' || requirement_en || ' ' || COALESCE(implementation_guidance_en, ''))
);

-- Control Mappings Table (cross-framework)
CREATE TABLE grc.control_mappings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    source_control_id UUID NOT NULL REFERENCES grc.controls(id) ON DELETE CASCADE,
    target_framework_code VARCHAR(30) NOT NULL,
    target_control_number VARCHAR(50) NOT NULL,
    mapping_strength VARCHAR(20) NOT NULL CHECK (mapping_strength IN ('Exact', 'Strong', 'Partial', 'Weak')),
    notes TEXT,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_control_mappings_source ON grc.control_mappings(source_control_id);
CREATE INDEX idx_control_mappings_target ON grc.control_mappings(target_framework_code, target_control_number);

-- Applicability Criteria Table
CREATE TABLE grc.applicability_criteria (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    framework_id UUID NOT NULL REFERENCES grc.frameworks(id) ON DELETE CASCADE,
    criteria_type VARCHAR(50) NOT NULL, -- Sector, EntityType, EmployeeCount, etc.
    criteria_value VARCHAR(200) NOT NULL,
    is_required BOOLEAN NOT NULL DEFAULT TRUE,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_applicability_framework ON grc.applicability_criteria(framework_id);

-- ============================================================
-- TENANT-SCOPED TABLES (Multi-tenant with RLS)
-- ============================================================

-- Tenant Configuration Table
CREATE TABLE grc.tenant_configurations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL UNIQUE,
    organization_name_en VARCHAR(300) NOT NULL,
    organization_name_ar VARCHAR(300) NOT NULL,
    commercial_registration VARCHAR(50),
    vat_number VARCHAR(50),
    industry_sector VARCHAR(50) NOT NULL,
    legal_entity_type VARCHAR(50) NOT NULL,
    employee_count INT,
    annual_revenue DECIMAL(18,2),
    subscription_tier VARCHAR(20) NOT NULL DEFAULT 'Standard',
    database_strategy VARCHAR(20) NOT NULL DEFAULT 'Shared',
    connection_string TEXT,
    operating_licenses TEXT[],
    data_types_processed TEXT[],
    processes_payments BOOLEAN DEFAULT FALSE,
    cloud_environment VARCHAR(50),
    logo_url VARCHAR(500),
    primary_color VARCHAR(10),
    settings JSONB,
    quotas JSONB,
    billing_info JSONB,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification_time TIMESTAMPTZ
);

CREATE INDEX idx_tenant_config_tenant ON grc.tenant_configurations(tenant_id);

-- Assessments Table
CREATE TABLE grc.assessments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    assessment_type VARCHAR(30) NOT NULL CHECK (assessment_type IN ('Initial', 'Annual', 'Continuous', 'Targeted', 'Regulatory')),
    status VARCHAR(30) NOT NULL DEFAULT 'Draft' CHECK (status IN ('Draft', 'Planning', 'InProgress', 'UnderReview', 'Completed', 'Cancelled')),
    start_date DATE NOT NULL,
    target_end_date DATE NOT NULL,
    actual_end_date DATE,
    owner_user_id UUID,
    scope JSONB,
    overall_score DECIMAL(5,2) DEFAULT 0,
    compliance_level VARCHAR(20),
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleter_id UUID,
    deletion_time TIMESTAMPTZ,
    extra_properties JSONB,
    concurrency_stamp VARCHAR(40)
);

CREATE INDEX idx_assessments_tenant ON grc.assessments(tenant_id);
CREATE INDEX idx_assessments_status ON grc.assessments(status);
CREATE INDEX idx_assessments_owner ON grc.assessments(owner_user_id);
CREATE INDEX idx_assessments_dates ON grc.assessments(start_date, target_end_date);

-- Row-Level Security for Assessments
ALTER TABLE grc.assessments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_assessments ON grc.assessments
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Assessment Frameworks (junction)
CREATE TABLE grc.assessment_frameworks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    assessment_id UUID NOT NULL REFERENCES grc.assessments(id) ON DELETE CASCADE,
    framework_id UUID NOT NULL REFERENCES grc.frameworks(id),
    is_mandatory BOOLEAN DEFAULT TRUE,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(assessment_id, framework_id)
);

CREATE INDEX idx_assessment_frameworks_assessment ON grc.assessment_frameworks(assessment_id);
ALTER TABLE grc.assessment_frameworks ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_assessment_frameworks ON grc.assessment_frameworks
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Control Assessments Table
CREATE TABLE grc.control_assessments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    assessment_id UUID NOT NULL REFERENCES grc.assessments(id) ON DELETE CASCADE,
    control_id UUID NOT NULL REFERENCES grc.controls(id),
    assigned_to_user_id UUID,
    assigned_to_department_id UUID,
    status VARCHAR(30) NOT NULL DEFAULT 'NotStarted' CHECK (status IN ('NotStarted', 'InProgress', 'PendingReview', 'Verified', 'Rejected', 'NotApplicable')),
    self_score DECIMAL(5,2) CHECK (self_score IS NULL OR (self_score >= 0 AND self_score <= 100)),
    verified_score DECIMAL(5,2) CHECK (verified_score IS NULL OR (verified_score >= 0 AND verified_score <= 100)),
    verified_by_user_id UUID,
    verification_date TIMESTAMPTZ,
    implementation_notes TEXT,
    rejection_reason TEXT,
    due_date DATE,
    priority VARCHAR(20) NOT NULL DEFAULT 'Medium',
    last_activity_time TIMESTAMPTZ,
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    extra_properties JSONB
);

CREATE INDEX idx_control_assessments_tenant ON grc.control_assessments(tenant_id);
CREATE INDEX idx_control_assessments_assessment ON grc.control_assessments(assessment_id);
CREATE INDEX idx_control_assessments_control ON grc.control_assessments(control_id);
CREATE INDEX idx_control_assessments_assigned ON grc.control_assessments(assigned_to_user_id);
CREATE INDEX idx_control_assessments_status ON grc.control_assessments(status);
CREATE INDEX idx_control_assessments_due ON grc.control_assessments(due_date) WHERE due_date IS NOT NULL;

ALTER TABLE grc.control_assessments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_control_assessments ON grc.control_assessments
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Control Assessment Comments
CREATE TABLE grc.control_assessment_comments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    control_assessment_id UUID NOT NULL REFERENCES grc.control_assessments(id) ON DELETE CASCADE,
    user_id UUID NOT NULL,
    comment TEXT NOT NULL,
    is_internal BOOLEAN DEFAULT FALSE,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_ca_comments_ca ON grc.control_assessment_comments(control_assessment_id);
ALTER TABLE grc.control_assessment_comments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_ca_comments ON grc.control_assessment_comments
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Control Assessment History (Event Log)
CREATE TABLE grc.control_assessment_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    control_assessment_id UUID NOT NULL REFERENCES grc.control_assessments(id) ON DELETE CASCADE,
    action VARCHAR(50) NOT NULL,
    details TEXT,
    user_id UUID,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    old_values JSONB,
    new_values JSONB
);

CREATE INDEX idx_ca_history_ca ON grc.control_assessment_history(control_assessment_id);
CREATE INDEX idx_ca_history_timestamp ON grc.control_assessment_history(timestamp);
ALTER TABLE grc.control_assessment_history ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_ca_history ON grc.control_assessment_history
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Evidences Table
CREATE TABLE grc.evidences (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    control_assessment_id UUID REFERENCES grc.control_assessments(id) ON DELETE SET NULL,
    file_name VARCHAR(500) NOT NULL,
    blob_name VARCHAR(1000) NOT NULL,
    container_name VARCHAR(100) NOT NULL,
    file_size BIGINT,
    mime_type VARCHAR(100),
    evidence_type VARCHAR(50) CHECK (evidence_type IN ('Policy', 'Procedure', 'Screenshot', 'Log', 'Report', 'Certificate', 'Configuration', 'Contract', 'Training', 'Other')),
    description TEXT,
    ai_classification JSONB,
    extracted_text TEXT,
    hash_sha256 VARCHAR(64),
    uploaded_by_user_id UUID NOT NULL,
    upload_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    version_number INT NOT NULL DEFAULT 1,
    is_current_version BOOLEAN NOT NULL DEFAULT TRUE,
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    deleter_id UUID,
    deletion_time TIMESTAMPTZ
);

CREATE INDEX idx_evidences_tenant ON grc.evidences(tenant_id);
CREATE INDEX idx_evidences_ca ON grc.evidences(control_assessment_id);
CREATE INDEX idx_evidences_type ON grc.evidences(evidence_type);
CREATE INDEX idx_evidences_upload ON grc.evidences(upload_time);

ALTER TABLE grc.evidences ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_evidences ON grc.evidences
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Risks Table
CREATE TABLE grc.risks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    risk_code VARCHAR(30) NOT NULL,
    title_en VARCHAR(300) NOT NULL,
    title_ar VARCHAR(300) NOT NULL,
    description_en TEXT,
    description_ar TEXT,
    category VARCHAR(50) NOT NULL,
    inherent_probability INT CHECK (inherent_probability BETWEEN 1 AND 5),
    inherent_impact INT CHECK (inherent_impact BETWEEN 1 AND 5),
    inherent_risk_level VARCHAR(20),
    residual_probability INT CHECK (residual_probability BETWEEN 1 AND 5),
    residual_impact INT CHECK (residual_impact BETWEEN 1 AND 5),
    residual_risk_level VARCHAR(20),
    risk_owner_id UUID,
    status VARCHAR(30) NOT NULL DEFAULT 'Identified',
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    extra_properties JSONB,
    concurrency_stamp VARCHAR(40),
    UNIQUE(tenant_id, risk_code)
);

CREATE INDEX idx_risks_tenant ON grc.risks(tenant_id);
CREATE INDEX idx_risks_category ON grc.risks(category);
CREATE INDEX idx_risks_owner ON grc.risks(risk_owner_id);
CREATE INDEX idx_risks_level ON grc.risks(inherent_risk_level);

ALTER TABLE grc.risks ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_risks ON grc.risks
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Risk Treatments Table
CREATE TABLE grc.risk_treatments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    risk_id UUID NOT NULL REFERENCES grc.risks(id) ON DELETE CASCADE,
    treatment_type VARCHAR(30) NOT NULL CHECK (treatment_type IN ('Mitigate', 'Transfer', 'Accept', 'Avoid')),
    description TEXT NOT NULL,
    owner_id UUID,
    target_date DATE,
    status VARCHAR(30) NOT NULL DEFAULT 'Planned',
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification_time TIMESTAMPTZ
);

CREATE INDEX idx_risk_treatments_risk ON grc.risk_treatments(risk_id);
ALTER TABLE grc.risk_treatments ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_risk_treatments ON grc.risk_treatments
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Action Plans Table
CREATE TABLE grc.action_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    source_type VARCHAR(50) NOT NULL, -- Gap, Risk, Audit, Assessment
    source_id UUID,
    owner_id UUID,
    status VARCHAR(30) NOT NULL DEFAULT 'Draft',
    start_date DATE,
    target_end_date DATE,
    actual_end_date DATE,
    progress_percentage INT DEFAULT 0 CHECK (progress_percentage BETWEEN 0 AND 100),
    -- ABP Audit Fields
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    creator_id UUID,
    last_modification_time TIMESTAMPTZ,
    last_modifier_id UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    extra_properties JSONB,
    concurrency_stamp VARCHAR(40)
);

CREATE INDEX idx_action_plans_tenant ON grc.action_plans(tenant_id);
CREATE INDEX idx_action_plans_source ON grc.action_plans(source_type, source_id);
CREATE INDEX idx_action_plans_owner ON grc.action_plans(owner_id);
CREATE INDEX idx_action_plans_status ON grc.action_plans(status);

ALTER TABLE grc.action_plans ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_action_plans ON grc.action_plans
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Action Items Table
CREATE TABLE grc.action_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    action_plan_id UUID NOT NULL REFERENCES grc.action_plans(id) ON DELETE CASCADE,
    title VARCHAR(300) NOT NULL,
    description TEXT,
    assigned_to_id UUID,
    priority VARCHAR(20) NOT NULL DEFAULT 'Medium',
    status VARCHAR(30) NOT NULL DEFAULT 'Pending',
    due_date DATE,
    completed_date DATE,
    sort_order INT DEFAULT 0,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification_time TIMESTAMPTZ
);

CREATE INDEX idx_action_items_plan ON grc.action_items(action_plan_id);
CREATE INDEX idx_action_items_assigned ON grc.action_items(assigned_to_id);
ALTER TABLE grc.action_items ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_action_items ON grc.action_items
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Workflows Table
CREATE TABLE grc.workflow_definitions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID,  -- NULL for system-wide workflows
    name VARCHAR(200) NOT NULL,
    description TEXT,
    workflow_type VARCHAR(50) NOT NULL,
    trigger_entity VARCHAR(100) NOT NULL,
    trigger_condition JSONB,
    steps JSONB NOT NULL, -- Array of workflow steps
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification_time TIMESTAMPTZ
);

CREATE INDEX idx_workflow_defs_tenant ON grc.workflow_definitions(tenant_id);
CREATE INDEX idx_workflow_defs_entity ON grc.workflow_definitions(trigger_entity);

-- Workflow Instances Table
CREATE TABLE grc.workflow_instances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    workflow_definition_id UUID NOT NULL REFERENCES grc.workflow_definitions(id),
    entity_type VARCHAR(100) NOT NULL,
    entity_id UUID NOT NULL,
    current_step INT NOT NULL DEFAULT 0,
    status VARCHAR(30) NOT NULL DEFAULT 'Pending',
    started_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at TIMESTAMPTZ,
    context JSONB
);

CREATE INDEX idx_workflow_instances_tenant ON grc.workflow_instances(tenant_id);
CREATE INDEX idx_workflow_instances_entity ON grc.workflow_instances(entity_type, entity_id);
CREATE INDEX idx_workflow_instances_status ON grc.workflow_instances(status);

ALTER TABLE grc.workflow_instances ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_workflow_instances ON grc.workflow_instances
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Workflow Tasks Table
CREATE TABLE grc.workflow_tasks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    workflow_instance_id UUID NOT NULL REFERENCES grc.workflow_instances(id) ON DELETE CASCADE,
    step_number INT NOT NULL,
    step_name VARCHAR(200) NOT NULL,
    assignee_user_id UUID,
    assignee_role VARCHAR(100),
    status VARCHAR(30) NOT NULL DEFAULT 'Pending',
    due_date TIMESTAMPTZ,
    completed_by_id UUID,
    completed_at TIMESTAMPTZ,
    action_taken VARCHAR(50),
    comments TEXT,
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_workflow_tasks_instance ON grc.workflow_tasks(workflow_instance_id);
CREATE INDEX idx_workflow_tasks_assignee ON grc.workflow_tasks(assignee_user_id);
CREATE INDEX idx_workflow_tasks_status ON grc.workflow_tasks(status);

ALTER TABLE grc.workflow_tasks ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_workflow_tasks ON grc.workflow_tasks
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Notifications Table
CREATE TABLE grc.notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    title VARCHAR(300) NOT NULL,
    message TEXT NOT NULL,
    notification_type VARCHAR(50) NOT NULL,
    entity_type VARCHAR(100),
    entity_id UUID,
    is_read BOOLEAN NOT NULL DEFAULT FALSE,
    read_at TIMESTAMPTZ,
    channels_sent TEXT[], -- Email, SMS, Push
    creation_time TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_notifications_tenant_user ON grc.notifications(tenant_id, user_id);
CREATE INDEX idx_notifications_unread ON grc.notifications(user_id, is_read) WHERE is_read = FALSE;

ALTER TABLE grc.notifications ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_notifications ON grc.notifications
    USING (tenant_id = current_setting('app.tenant_id', true)::UUID);

-- Event Store Table (for Event Sourcing)
CREATE TABLE grc.event_store (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID,
    aggregate_type VARCHAR(100) NOT NULL,
    aggregate_id UUID NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    event_data JSONB NOT NULL,
    metadata JSONB,
    user_id UUID,
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    version INT NOT NULL DEFAULT 1
);

CREATE INDEX idx_event_store_aggregate ON grc.event_store(aggregate_type, aggregate_id);
CREATE INDEX idx_event_store_timestamp ON grc.event_store(timestamp);
CREATE INDEX idx_event_store_type ON grc.event_store(event_type);

-- ============================================================
-- FUNCTIONS & TRIGGERS
-- ============================================================

-- Function to set tenant_id context
CREATE OR REPLACE FUNCTION grc.set_tenant_context(p_tenant_id UUID)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.tenant_id', p_tenant_id::TEXT, TRUE);
END;
$$ LANGUAGE plpgsql;

-- Function to calculate risk level
CREATE OR REPLACE FUNCTION grc.calculate_risk_level(probability INT, impact INT)
RETURNS VARCHAR(20) AS $$
DECLARE
    score INT;
BEGIN
    score := probability * impact;
    RETURN CASE
        WHEN score >= 20 THEN 'Critical'
        WHEN score >= 12 THEN 'High'
        WHEN score >= 6 THEN 'Medium'
        WHEN score >= 2 THEN 'Low'
        ELSE 'VeryLow'
    END;
END;
$$ LANGUAGE plpgsql IMMUTABLE;

-- Trigger to auto-calculate risk level
CREATE OR REPLACE FUNCTION grc.trg_calculate_risk_level()
RETURNS TRIGGER AS $$
BEGIN
    NEW.inherent_risk_level := grc.calculate_risk_level(NEW.inherent_probability, NEW.inherent_impact);
    IF NEW.residual_probability IS NOT NULL AND NEW.residual_impact IS NOT NULL THEN
        NEW.residual_risk_level := grc.calculate_risk_level(NEW.residual_probability, NEW.residual_impact);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_risks_calculate_level
    BEFORE INSERT OR UPDATE ON grc.risks
    FOR EACH ROW
    EXECUTE FUNCTION grc.trg_calculate_risk_level();

-- Function to update assessment progress
CREATE OR REPLACE FUNCTION grc.update_assessment_score(p_assessment_id UUID)
RETURNS VOID AS $$
DECLARE
    v_total INT;
    v_completed INT;
    v_avg_score DECIMAL(5,2);
BEGIN
    SELECT 
        COUNT(*),
        COUNT(*) FILTER (WHERE status = 'Verified'),
        AVG(verified_score) FILTER (WHERE verified_score IS NOT NULL)
    INTO v_total, v_completed, v_avg_score
    FROM grc.control_assessments
    WHERE assessment_id = p_assessment_id AND is_deleted = FALSE;
    
    UPDATE grc.assessments
    SET overall_score = COALESCE(v_avg_score, 0),
        last_modification_time = NOW()
    WHERE id = p_assessment_id;
END;
$$ LANGUAGE plpgsql;

-- ============================================================
-- VIEWS
-- ============================================================

-- Dashboard View: Assessment Summary
CREATE OR REPLACE VIEW grc.v_assessment_summary AS
SELECT 
    a.id,
    a.tenant_id,
    a.name,
    a.status,
    a.overall_score,
    a.start_date,
    a.target_end_date,
    COUNT(ca.id) AS total_controls,
    COUNT(ca.id) FILTER (WHERE ca.status = 'Verified') AS completed_controls,
    COUNT(ca.id) FILTER (WHERE ca.status = 'InProgress') AS in_progress_controls,
    COUNT(ca.id) FILTER (WHERE ca.status = 'NotStarted') AS not_started_controls,
    COUNT(ca.id) FILTER (WHERE ca.due_date < CURRENT_DATE AND ca.status NOT IN ('Verified', 'NotApplicable')) AS overdue_controls,
    ROUND(100.0 * COUNT(ca.id) FILTER (WHERE ca.status = 'Verified') / NULLIF(COUNT(ca.id), 0), 2) AS completion_percentage
FROM grc.assessments a
LEFT JOIN grc.control_assessments ca ON ca.assessment_id = a.id AND ca.is_deleted = FALSE
WHERE a.is_deleted = FALSE
GROUP BY a.id;

-- Dashboard View: Framework Progress
CREATE OR REPLACE VIEW grc.v_framework_progress AS
SELECT 
    a.tenant_id,
    a.id AS assessment_id,
    f.id AS framework_id,
    f.code AS framework_code,
    f.title_en AS framework_name,
    COUNT(ca.id) AS total_controls,
    COUNT(ca.id) FILTER (WHERE ca.status = 'Verified') AS verified_controls,
    AVG(ca.verified_score) FILTER (WHERE ca.verified_score IS NOT NULL) AS avg_score,
    ROUND(100.0 * COUNT(ca.id) FILTER (WHERE ca.status = 'Verified') / NULLIF(COUNT(ca.id), 0), 2) AS completion_pct
FROM grc.assessments a
JOIN grc.assessment_frameworks af ON af.assessment_id = a.id
JOIN grc.frameworks f ON f.id = af.framework_id
LEFT JOIN grc.controls c ON c.framework_id = f.id AND c.is_deleted = FALSE
LEFT JOIN grc.control_assessments ca ON ca.assessment_id = a.id AND ca.control_id = c.id AND ca.is_deleted = FALSE
WHERE a.is_deleted = FALSE
GROUP BY a.tenant_id, a.id, f.id, f.code, f.title_en;

-- ============================================================
-- INITIAL SEED DATA (Regulators)
-- ============================================================

INSERT INTO grc.regulators (code, name_en, name_ar, category, website) VALUES
('NCA', 'National Cybersecurity Authority', 'الهيئة الوطنية للأمن السيبراني', 'Cybersecurity', 'https://nca.gov.sa'),
('SAMA', 'Saudi Central Bank', 'البنك المركزي السعودي', 'Financial', 'https://sama.gov.sa'),
('CMA', 'Capital Market Authority', 'هيئة السوق المالية', 'Financial', 'https://cma.org.sa'),
('SDAIA', 'Saudi Data & AI Authority', 'الهيئة السعودية للبيانات والذكاء الاصطناعي', 'Data', 'https://sdaia.gov.sa'),
('ZATCA', 'Zakat, Tax and Customs Authority', 'هيئة الزكاة والضريبة والجمارك', 'Tax', 'https://zatca.gov.sa'),
('MOH', 'Ministry of Health', 'وزارة الصحة', 'Healthcare', 'https://moh.gov.sa'),
('SFDA', 'Saudi Food and Drug Authority', 'الهيئة العامة للغذاء والدواء', 'Healthcare', 'https://sfda.gov.sa'),
('CST', 'Communications, Space & Technology Commission', 'هيئة الاتصالات والفضاء والتقنية', 'Telecommunications', 'https://cst.gov.sa'),
('DGA', 'Digital Government Authority', 'هيئة الحكومة الرقمية', 'Digital', 'https://dga.gov.sa'),
('MOCI', 'Ministry of Commerce', 'وزارة التجارة', 'Commerce', 'https://mc.gov.sa'),
('ISO', 'International Organization for Standardization', 'المنظمة الدولية للمعايير', 'International', 'https://iso.org'),
('NIST', 'National Institute of Standards and Technology', 'المعهد الوطني للمعايير والتقنية', 'International', 'https://nist.gov'),
('PCI', 'Payment Card Industry Security Standards Council', 'مجلس معايير أمن صناعة بطاقات الدفع', 'International', 'https://pcisecuritystandards.org');

-- ============================================================
-- END OF SCHEMA
-- ============================================================
