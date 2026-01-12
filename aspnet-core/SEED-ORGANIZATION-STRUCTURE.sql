-- ============================================================
-- PROFESSIONAL ORGANIZATION CHART + GRC COMMITTEE STRUCTURE
-- Saudi Organization with GRC Governance
-- ============================================================

-- ============================================================
-- 1. CREATE ROLES (Job Titles + GRC Committee Roles)
-- ============================================================

-- Job Title Roles (Regular Organization)
INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'CEO', 'CEO', false, false, true, 0, '{}',0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'CEO');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'CFO', 'CFO', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'CFO');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'CIO', 'CIO', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'CIO');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'CISO', 'CISO', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'CISO');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Department Manager', 'DEPARTMENT MANAGER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Department Manager');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Team Lead', 'TEAM LEAD', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Team Lead');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Senior Officer', 'SENIOR OFFICER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Senior Officer');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Officer', 'OFFICER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Officer');

-- GRC Committee Roles (Governance Committee)
INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'GRC Committee Chair', 'GRC COMMITTEE CHAIR', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'GRC Committee Chair');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'GRC Committee Member', 'GRC COMMITTEE MEMBER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'GRC Committee Member');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Compliance Manager', 'COMPLIANCE MANAGER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Compliance Manager');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Risk Manager', 'RISK MANAGER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Risk Manager');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Control Owner', 'CONTROL OWNER', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Control Owner');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'Internal Auditor', 'INTERNAL AUDITOR', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'Internal Auditor');

INSERT INTO "AbpRoles" ("Id", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "EntityVersion", "ExtraProperties", "ConcurrencyStamp")
SELECT gen_random_uuid(), 'External Auditor', 'EXTERNAL AUDITOR', false, false, true,0, '{}', gen_random_uuid()::text
WHERE NOT EXISTS (SELECT 1 FROM "AbpRoles" WHERE "Name" = 'External Auditor');

-- ============================================================
-- 2. CREATE ORGANIZATIONAL CHART (Hierarchical Structure)
-- ============================================================

-- Level 1: CEO Office
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), NULL, '00001', 'CEO Office | مكتب الرئيس التنفيذي', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001')
RETURNING "Id";

-- Level 2: Executive Management
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00001', 'Finance Department | الإدارة المالية', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00001');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00002', 'IT Department | قسم تقنية المعلومات', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00003', 'Operations Department | قسم العمليات', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00003');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00004', 'Risk & Compliance Department | إدارة المخاطر والامتثال', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00005', 'Internal Audit Department | إدارة المراجعة الداخلية', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00005');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001'), '00001.00006', 'Human Resources | الموارد البشرية', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00006');

-- Level 3: IT Department Teams
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002'), '00001.00002.00001', 'Cybersecurity Team | فريق الأمن السيبراني', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002.00001');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002'), '00001.00002.00002', 'Infrastructure Team | فريق البنية التحتية', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002.00002');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002'), '00001.00002.00003', 'Application Development | تطوير التطبيقات', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00002.00003');

-- Level 3: Risk & Compliance Teams
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004'), '00001.00004.00001', 'Regulatory Compliance Team | فريق الامتثال التنظيمي', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004.00001');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004'), '00001.00004.00002', 'Risk Management Team | فريق إدارة المخاطر', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004.00002');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004'), '00001.00004.00003', 'Data Protection Team | فريق حماية البيانات', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00001.00004.00003');

-- ============================================================
-- 3. CREATE GRC COMMITTEE (Separate Governance Structure)
-- ============================================================

-- GRC Committee (Independent from regular org chart)
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), NULL, '00002', 'GRC Committee | لجنة الحوكمة والمخاطر والامتثال', 0, '{"Type":"Committee","Purpose":"Governance oversight"}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00002');

-- GRC Sub-committees
INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00002'), '00002.00001', 'Compliance Oversight Committee | لجنة الإشراف على الامتثال', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00002.00001');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00002'), '00002.00002', 'Risk Oversight Committee | لجنة الإشراف على المخاطر', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00002.00002');

INSERT INTO "AbpOrganizationUnits" ("Id", "ParentId", "Code", "DisplayName", "EntityVersion", "ExtraProperties", "ConcurrencyStamp", "CreationTime", "IsDeleted")
SELECT gen_random_uuid(), (SELECT "Id" FROM "AbpOrganizationUnits" WHERE "Code" = '00002'), '00002.00003', 'Information Security Committee | لجنة أمن المعلومات', 0, '{}', gen_random_uuid()::text, NOW(), false
WHERE NOT EXISTS (SELECT 1 FROM "AbpOrganizationUnits" WHERE "Code" = '00002.00003');

-- ============================================================
-- 4. SUMMARY VIEW
-- ============================================================

-- Organization Chart Summary
SELECT 
    'ORGANIZATION CHART' as "Type",
    COUNT(*) as "Total Units"
FROM "AbpOrganizationUnits"
WHERE "Code" LIKE '00001%';

-- GRC Committee Summary
SELECT 
    'GRC COMMITTEE' as "Type",
    COUNT(*) as "Total Units"
FROM "AbpOrganizationUnits"
WHERE "Code" LIKE '00002%';

-- Roles Summary
SELECT 
    'TOTAL ROLES' as "Type",
    COUNT(*) as "Count"
FROM "AbpRoles";

-- Detailed Organization Structure
SELECT 
    CASE 
        WHEN LENGTH("Code") - LENGTH(REPLACE("Code", '.', '')) = 0 THEN '📊 Level 1: Root'
        WHEN LENGTH("Code") - LENGTH(REPLACE("Code", '.', '')) = 1 THEN '  📁 Level 2: Department'
        WHEN LENGTH("Code") - LENGTH(REPLACE("Code", '.', '')) = 2 THEN '    📂 Level 3: Team'
        ELSE '      📄 Level 4+'
    END as "Hierarchy",
    "Code",
    "DisplayName"
FROM "AbpOrganizationUnits"
WHERE "IsDeleted" = false
ORDER BY "Code";

