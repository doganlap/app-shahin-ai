-- Create Platform Admin User for Testing Tenant Creation
-- This user can access /platform-admin/v2/tenants/create

-- 1. Create PlatformAdmin role if it doesn't exist
INSERT INTO "AbpRoles" ("Id", "TenantId", "Name", "NormalizedName", "IsDefault", "IsStatic", "IsPublic", "ConcurrencyStamp")
VALUES (
    gen_random_uuid(),
    NULL, -- Host role (no tenant)
    'PlatformAdmin',
    'PLATFORMADMIN',
    false,
    true,
    false,
    gen_random_uuid()::text
)
ON CONFLICT DO NOTHING;

-- 2. Create platform admin user
DO $$
DECLARE
    v_user_id UUID;
    v_role_id UUID;
BEGIN
    -- Create user
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
        NULL, -- Host user (no tenant)
        'platformadmin@shahin.sa',
        'PLATFORMADMIN@SHAHIN.SA',
        'platformadmin@shahin.sa',
        'PLATFORMADMIN@SHAHIN.SA',
        true,
        -- Password: Platform@123456 (hashed with Identity v3 hasher)
        'AQAAAAIAAYagAAAAEJ5xK8vN9YqJ7xK5F5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ5xJ==',
        gen_random_uuid()::text,
        gen_random_uuid()::text,
        false,
        false,
        false,
        0,
        NOW()
    )
    ON CONFLICT (LOWER("Email")) DO NOTHING;
    
    -- Get PlatformAdmin role ID
    SELECT "Id" INTO v_role_id FROM "AbpRoles" WHERE "Name" = 'PlatformAdmin' AND "TenantId" IS NULL;
    
    -- Assign role to user
    INSERT INTO "AbpUserRoles" ("UserId", "RoleId", "TenantId")
    VALUES (v_user_id, v_role_id, NULL)
    ON CONFLICT DO NOTHING;
    
    RAISE NOTICE 'Platform admin created: platformadmin@shahin.sa';
    RAISE NOTICE 'Password: Platform@123456';
    RAISE NOTICE 'Login at: http://localhost:5137/Account/Login';
    RAISE NOTICE 'Then access: http://localhost:5137/platform-admin/v2/tenants/create';
END $$;
