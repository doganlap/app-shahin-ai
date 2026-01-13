-- Verify Tenant Creation
-- Run this after creating a tenant to verify it worked

-- 1. List all tenants
SELECT 
    "Id",
    "Name",
    "CreationTime"
FROM "AbpTenants"
ORDER BY "CreationTime" DESC
LIMIT 10;

-- 2. List all users with their tenant
SELECT 
    u."Id",
    u."UserName",
    u."Email",
    u."EmailConfirmed",
    u."TenantId",
    t."Name" as "TenantName"
FROM "AbpUsers" u
LEFT JOIN "AbpTenants" t ON u."TenantId" = t."Id"
ORDER BY u."CreationTime" DESC
LIMIT 10;

-- 3. List user roles
SELECT 
    u."UserName",
    u."Email",
    r."Name" as "RoleName",
    t."Name" as "TenantName"
FROM "AbpUserRoles" ur
JOIN "AbpUsers" u ON ur."UserId" = u."Id"
JOIN "AbpRoles" r ON ur."RoleId" = r."Id"
LEFT JOIN "AbpTenants" t ON u."TenantId" = t."Id"
ORDER BY u."CreationTime" DESC
LIMIT 10;
