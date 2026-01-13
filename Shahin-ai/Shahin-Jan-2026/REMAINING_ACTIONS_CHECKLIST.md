# Remaining Actions & Fixes Checklist

**Date:** 2026-01-22  
**Status:** ⚠️ **PRODUCTION BLOCKERS REMAIN**

---

## 🔴 CRITICAL BLOCKERS (Must Fix Before Production)

### 1. grc-frontend Build Failures ❌
**Status:** ❌ **BLOCKING DEPLOYMENT**

**Affected Pages:**
- `/(dashboard)/dashboard/analytics/page` - Runtime error
- `/(marketing)/about/page` - Runtime error  
- `/(marketing)/contact/page` - Runtime error
- `/(marketing)/pricing/page` - Runtime error
- `/page` (home page) - Runtime error

**Error Type:** Runtime errors during static page generation

**Action Required:**
1. Investigate runtime errors in each page
2. Check for:
   - Missing environment variables accessed at build time
   - Client-side only code running during SSR
   - Missing error boundaries
   - Invalid imports or dependencies
3. Fix errors and verify build succeeds:
   ```bash
   cd grc-frontend
   npm run build
   ```

**Priority:** 🔴 **CRITICAL** - Blocks production deployment

---

### 2. Remove Secrets from Git History ❌
**Status:** ⚠️ **SECURITY RISK**

**Files Exposed:**
- `.env.backup` - Contains real DB password
- `.env.production.secure` - Contains Claude API key
- `.env.grcmvc.production` - Contains Azure credentials
- `.env.grcmvc.secure` - Contains secrets

**Action Required:**
```bash
# 1. Remove from git tracking (keep local files)
git rm --cached .env.backup .env.production.secure .env.grcmvc.production .env.grcmvc.secure

# 2. Commit the removal
git commit -m "Remove exposed secrets from version control"

# 3. Rotate ALL exposed credentials:
#    - Database passwords
#    - Claude API keys
#    - Azure tenant/client secrets
#    - JWT secrets

# 4. Verify .gitignore is working
echo "TEST_SECRET=123" > .env.test
git status  # Should show .env.test as ignored
rm .env.test
```

**Priority:** 🔴 **CRITICAL** - Security vulnerability

---

## 🟡 HIGH PRIORITY (Should Fix Before Production)

### 3. Hardcoded Configuration Values ⚠️
**File:** `src/GrcMvc/appsettings.json`

**Issues:**
- Line 3: `"BaseUrl": "https://app.shahin-ai.com"` - Hardcoded
- Line 4: `"LandingUrl": "https://shahin-ai.com"` - Hardcoded
- Line 25: `"Secret": "DevSecretKeyForTestingOnly..."` - Dev default

**Action Required:**
1. Create `appsettings.Production.json`:
   ```json
   {
     "App": {
       "BaseUrl": "${APP_BASE_URL}",
       "LandingUrl": "${APP_LANDING_URL}"
     },
     "JwtSettings": {
       "Secret": "${JWT_SECRET}"
     }
   }
   ```
2. Or use environment variable overrides in `Program.cs`
3. Remove hardcoded production URLs from base `appsettings.json`

**Priority:** 🟡 **HIGH** - Configuration management

---

### 4. Missing Production Environment Variables ⚠️
**Project:** grc-app

**Missing Variables:**
- `NEXT_PUBLIC_API_URL` - Currently uses fallback
- `NEXT_PUBLIC_PORTAL_URL` - Currently uses fallback

**Action Required:**
1. Create `.env.production` in `grc-app/`:
   ```bash
   NEXT_PUBLIC_API_URL=https://portal.shahin-ai.com
   NEXT_PUBLIC_PORTAL_URL=https://portal.shahin-ai.com
   NEXT_PUBLIC_SUPERSET_URL=https://superset.shahin-ai.com
   NEXT_PUBLIC_GRAFANA_URL=https://grafana.shahin-ai.com
   NEXT_PUBLIC_METABASE_URL=https://metabase.shahin-ai.com
   ```
2. Update deployment scripts to load these variables
3. Verify no localhost fallbacks in production

**Priority:** 🟡 **HIGH** - Production configuration

---

### 5. API Key Validation at Startup ⚠️
**File:** `src/GrcMvc/Program.cs`

**Issue:** Claude API key not validated if `ClaudeAgents:Enabled=true`

**Action Required:**
Add validation in `Program.cs`:
```csharp
if (builder.Configuration["ClaudeAgents:Enabled"] == "true") {
    var apiKey = builder.Configuration["ClaudeAgents:ApiKey"];
    if (string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException(
            "ClaudeAgents:ApiKey is required when ClaudeAgents:Enabled=true");
    }
}
```

**Priority:** 🟡 **HIGH** - Prevents runtime failures

---

## 🟢 MEDIUM PRIORITY (Can Fix After Initial Deployment)

### 6. Database Connection Retry Logic ⚠️
**File:** `src/GrcMvc/Program.cs:1063-1066`

**Issue:** Single connection attempt, fails if DB slow to start

**Action Required:**
1. Install Polly package: `dotnet add package Polly`
2. Implement exponential backoff retry:
   ```csharp
   var retryPolicy = Policy
       .Handle<Exception>()
       .WaitAndRetryAsync(
           retryCount: 5,
           sleepDurationProvider: retryAttempt => 
               TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
           onRetry: (exception, timeSpan, retryCount, context) => {
               logger.LogWarning("Database connection retry {RetryCount}", retryCount);
           });
   ```

**Priority:** 🟢 **MEDIUM** - Improves reliability

---

### 7. Rate Limiting Too Permissive ⚠️
**File:** `src/GrcMvc/Program.cs:408`

**Issue:** Global rate limit 500 req/min - too high

**Action Required:**
Reduce to 100-200 requests per minute per IP:
```csharp
options.GlobalPermitLimit = 100;
options.Window = TimeSpan.FromMinutes(1);
```

**Priority:** 🟢 **MEDIUM** - Security hardening

---

### 8. Logging Configuration ⚠️
**File:** `src/GrcMvc/appsettings.json:140-162`

**Issues:**
- Log retention only 30 days (may not meet compliance)
- No centralized logging to external service
- Logs to `/app/logs/` - not persistent in containers

**Action Required:**
1. Configure Serilog to external service (Seq, Elasticsearch, Application Insights)
2. Increase retention to 90+ days for compliance
3. Use volume mounts for log persistence

**Priority:** 🟢 **MEDIUM** - Compliance and monitoring

---

### 9. Auto-Migration Risks ⚠️
**File:** `src/GrcMvc/Program.cs:1340`

**Issue:** Migrations run on every startup, no backup/rollback

**Action Required:**
1. Disable auto-migration in production
2. Run migrations separately in CI/CD:
   ```bash
   dotnet ef database update --project src/GrcMvc
   ```
3. Add backup before migration
4. Add rollback mechanism

**Priority:** 🟢 **MEDIUM** - Database safety

---

## 📋 TESTING & QUALITY (Recommended)

### 10. No Frontend Tests ❌
**Projects:** grc-frontend, grc-app

**Issue:** Zero test files, no test frameworks

**Action Required:**
1. Install Jest + React Testing Library:
   ```bash
   cd grc-frontend
   npm install --save-dev jest @testing-library/react @testing-library/jest-dom @types/jest
   ```
2. Create test files for critical components
3. Add test scripts to `package.json`
4. Target: 50% code coverage minimum

**Priority:** 🟢 **MEDIUM** - Quality assurance

---

### 11. No Pre-commit Hooks ⚠️
**Issue:** No validation before commits

**Action Required:**
1. Install Husky:
   ```bash
   npm install husky lint-staged --save-dev
   npx husky install
   ```
2. Add pre-commit hook for linting
3. Add pre-push hook for tests

**Priority:** 🟢 **MEDIUM** - Code quality

---

## 📊 SUMMARY

### Critical Blockers (Must Fix):
1. ❌ grc-frontend build failures (5 pages)
2. ❌ Remove secrets from git history + rotate credentials

### High Priority (Should Fix):
3. ⚠️ Hardcoded configuration values
4. ⚠️ Missing production environment variables
5. ⚠️ API key validation at startup

### Medium Priority (Can Fix Later):
6. ⚠️ Database connection retry logic
7. ⚠️ Rate limiting too permissive
8. ⚠️ Logging configuration
9. ⚠️ Auto-migration risks
10. ⚠️ No frontend tests
11. ⚠️ No pre-commit hooks

---

## 🎯 IMMEDIATE ACTION PLAN

### Today (Critical):
1. **Fix grc-frontend build errors** - Investigate and fix runtime errors
2. **Remove secrets from git** - Run git commands above
3. **Rotate credentials** - Change all exposed passwords/keys

### This Week (High Priority):
4. **Create appsettings.Production.json** - Move hardcoded values
5. **Set production env vars** - Create `.env.production` files
6. **Add API key validation** - Prevent startup failures

### Next Week (Medium Priority):
7. **Add database retry logic** - Improve reliability
8. **Reduce rate limits** - Security hardening
9. **Configure centralized logging** - Compliance
10. **Disable auto-migrations** - Database safety

---

## ✅ VERIFICATION CHECKLIST

Before production deployment, verify:

- [ ] All builds succeed: `npm run build` in grc-frontend and grc-app
- [ ] No secrets in git: `git log --all --source -- '.env*'` returns nothing
- [ ] Environment variables set: Check `.env.production` files exist
- [ ] Health checks work: `curl http://localhost:8888/health/ready`
- [ ] No localhost fallbacks: Check logs for "localhost" warnings
- [ ] API key validation: App fails fast if missing
- [ ] Security headers: CSP doesn't allow `'unsafe-eval'`
- [ ] CORS configured: Only allowed domains
- [ ] Database migrations: Run manually, not on startup
- [ ] Logging configured: Logs go to external service

---

**Current Status:** ⚠️ **NOT PRODUCTION READY** - 2 critical blockers remain

**Estimated Time to Production-Ready:** 2-3 days (critical fixes only)
