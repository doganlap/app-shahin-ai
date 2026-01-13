# 🚨 PRODUCTION READINESS - CRITICAL ACTIONS REQUIRED

**Date**: January 11, 2026  
**Status**: ❌ NOT PRODUCTION READY (53% readiness, need 85%)  
**Severity**: CRITICAL

---

## IMMEDIATE ACTIONS (BLOCKING PRODUCTION)

### ✅ 1. React Hook Error - FIXED
**File**: `grc-frontend/src/components/dashboard/SupersetEmbed.tsx`  
**Status**: ✅ ALREADY FIXED (hooks called before all early returns)

### 🔴 2. EXPOSED SECRETS IN GIT - MUST FIX NOW

**Critical Exposed Files** (DO NOT DEPLOY):
```
.env.backup - Contains: DB password, JWT secrets, AWS keys
.env.production.secure - Contains: Claude API key, credentials
.env.grcmvc.production - Contains: Admin passwords, Azure tenant IDs
.env.grcmvc.secure - Contains: Production secrets
.env.production.complete - Contains: All secrets
.env.production.final - Contains: All secrets
```

**REQUIRED IMMEDIATE STEPS**:

#### Step 1: Remove secrets from git history
```bash
# Install BFG Repo-Cleaner
wget https://repo1.maven.org/maven2/com/madgag/bfg/1.14.0/bfg-1.14.0.jar
java -jar bfg-1.14.0.jar --delete-files '.env*' .

# Clean git history
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# Force push (⚠️ ALL COLLABORATORS MUST PULL --rebase)
git push origin main --force
```

#### Step 2: Rotate ALL exposed credentials
- [ ] Database password (change in RDS/PostgreSQL)
- [ ] JWT Secret (regenerate 64+ byte key)
- [ ] Claude API key (revoke at console.anthropic.com, create new key)
- [ ] Azure tenant IDs/client secrets (rotate in Azure Portal)
- [ ] AWS keys (if exposed)
- [ ] Admin passwords (reset AccountController & PlatformAdmin)
- [ ] SMTP credentials (update in EmailSettings)

#### Step 3: Update `.gitignore` (✅ DONE)
```
.env
.env.*
!.env.example
!.env.production.template
.env.local
secrets/
vaults/
credentials/
```

#### Step 4: Configure safe environment variables
Create `.env.production` (NOT in git):
```bash
# Copy template and fill in NEW rotated values
cp .env.production.template .env.production
# Edit .env.production with rotated credentials
nano .env.production
```

**Files to KEEP in git** (NO secrets):
- `.env.example` - Template with placeholder values
- `.env.production.template` - Template with `${VAR_NAME}` placeholders

**Files to DELETE from git**:
- `.env.backup` ❌ DELETE
- `.env.production.secure` ❌ DELETE
- `.env.grcmvc.production` ❌ DELETE
- `.env.grcmvc.secure` ❌ DELETE
- `.env.production.complete` ❌ DELETE
- `.env.production.final` ❌ DELETE
- `.env` ❌ DELETE (if exists)

---

### ✅ 3. Missing Environment Variables - FIXED

**Created**: `grc-app/.env.production`  
**Content**: All NEXT_PUBLIC_* variables configured with production URLs

---

## HIGH PRIORITY FIXES (BEFORE DEPLOYMENT)

### 4. Hardcoded Configuration in appsettings.json
**Issue**: Production URLs hardcoded, not using environment variables

**Files to Fix**:
- `src/GrcMvc/appsettings.json` - Remove hardcoded BaseUrl, JWT Secret
- `src/GrcMvc/appsettings.Production.json` - Create with production overrides

### 5. Security Header Weakness
**File**: `src/GrcMvc/Middleware/SecurityHeadersMiddleware.cs`  
**Issue**: CSP allows `'unsafe-eval'` - XSS vulnerability  
**Fix**: Remove `'unsafe-eval'`, implement nonce-based inline scripts

### 6. Insecure CORS
**File**: `src/GrcMvc/appsettings.json`  
**Issue**: `"AllowedHosts": "*"`  
**Fix**: Change to specific domains only

---

## DEPLOYMENT BLOCKERS CHECKLIST

- [ ] Remove all `.env*` files with secrets from git history
- [ ] Rotate ALL exposed credentials
- [ ] Verify `.gitignore` prevents future commits
- [ ] Create `.env.production` locally (NOT in git)
- [ ] Test build succeeds: `dotnet build --configuration Release`
- [ ] Test grc-frontend build: `npm run build`
- [ ] Test grc-app build: `npm run build`
- [ ] Verify Docker containers start: `docker-compose -f docker-compose.production.yml up -d`
- [ ] Check health endpoints return 200
- [ ] Verify no localhost fallbacks in logs
- [ ] Security scan passes (no exposed credentials)

---

## TIMELINE

**Immediate (TODAY)**:
1. Remove secrets from git (1-2 hours with BFG)
2. Rotate all exposed credentials (30 mins per service)
3. Test builds succeed

**Tomorrow**:
4. Fix hardcoded configuration
5. Fix security headers
6. Test deployment locally

**Production Readiness**: ~24-48 hours

---

## AUTHORIZATION REQUIRED

🚨 **CRITICAL DECISION NEEDED**:

1. **Remove secrets from git history?** (MUST DO - irreversible)
   - YES ✅ / NO ❌
   
2. **Rotate all exposed credentials?** (REQUIRED for production)
   - YES ✅ / NO ❌
   
3. **Deploy to production after fixes?**
   - YES ✅ / NO ❌

---

## VERIFICATION COMMANDS

```bash
# Check if secrets removed from git
git log --all --full-history -- '.env.backup' | wc -l  # Should be 0

# Verify .gitignore works
echo "TEST_SECRET=123" > .env.test
git status  # Should show .env.test as ignored

# Test builds
cd grc-frontend && npm run build && cd ..
cd grc-app && npm run build && cd ..
dotnet build --configuration Release

# Docker test
docker-compose -f docker-compose.production.yml up -d
sleep 60
curl http://localhost:8888/health/ready  # Should return 200
```

---

## CONTACT & ESCALATION

If credentials exposed on public cloud:
1. Check git push logs (did it push to GitHub/GitLab public?)
2. If YES: Immediately revoke all credentials
3. If YES: File incident report
4. Notify: Security team, DevOps, Database admin

---

**Status**: Awaiting authorization to proceed with secret removal and credential rotation.
