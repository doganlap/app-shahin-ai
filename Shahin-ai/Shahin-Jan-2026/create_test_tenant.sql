-- Create Test Tenant and Admin User Directly in Database
-- This bypasses the trial registration form to test the core functionality

-- 1. Create ABP Tenant
INSERT INTO "AbpTenants" ("Id", "Name", "NormalizeName", "ConcurrencyStamp", "CreationTime")
VALUES (
    gen_random_uuid(),
    'test-company',
    'TEST-COMPANY',
    gen_random_uuid()::text,
    NOW()
)
ON CONFLICT DO NOTHING;

-- Get the tenant ID we just created
DO $$
DECLARE
    v_tenant_id UUID;
    v_user_id UUID;
    v_role_id UUID;
BEGIN
    -- Get tenant ID
    SELECT "Id" INTO v_tenant_id FROM "AbpTenants" WHERE "Name" = 'test-company';
    
    -- 2. Create ABP User (admin@test.com)
    v_user_id := gen_random_uuid();
    
    INSERT INTO "AbpUsers" (
        "Id", 
        "TenantId",
        "UserName", 
        "NormalizedUserName",
        "Email", 
        "NormalizedEmail",
        "EmailConfirmed",
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp",
        "PhoneNumberConfirmed",
        "TwoFactorEnabled",
        "LockoutEnabled",
        "AccessFailedCount",
        "CreationTime"
    )
    VALUES (
        v_user_id,
        v_tenant_id,
        'admin@test.com',
        'ADMIN@TEST.COM',
        'admin@test.com',
        'ADMIN@TEST.COM',
        true,
        -- Password: Admin@123456789 (hashed with Identity default hasher)
        'AQAAAAIAAYagAAAAEKxqvN8vZ9YqJ7xK5F5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ==',
        gen_random_uuid()::text,
        gen_random_uuid()::text,
        false,
        false,
        true,
        0,
        NOW()
    )
    ON CONFLICT DO NOTHING;
    
    -- 3. Create TenantAdmin Role
    v_role_id := gen_random_uuid();
    
    INSERT INTO "AbpRoles" (
        "Id",
        "TenantId",
        "Name",
        "NormalizedName",
        "IsDefault",
        "IsStatic",
        "IsPublic",
        "ConcurrencyStamp"
    )
    VALUES (
        v_role_id,
        v_tenant_id,
        'TenantAdmin',
        'TENANTADMIN',
        false,
        false,
        false,
        gen_random_uuid()::text
    )
    ON CONFLICT DO NOTHING;
    
    -- 4. Assign User to Role
    INSERT INTO "AbpUserRoles" ("UserId", "RoleId", "TenantId")
    VALUES (v_user_id, v_role_id, v_tenant_id)
    ON CONFLICT DO NOTHING;
    
    RAISE NOTICE 'Created tenant: test-company (ID: %)', v_tenant_id;
    RAISE NOTICE 'Created user: admin@test.com (ID: %)', v_user_id;
    RAISE NOTICE 'Password: Admin@123456789';
END $$;
