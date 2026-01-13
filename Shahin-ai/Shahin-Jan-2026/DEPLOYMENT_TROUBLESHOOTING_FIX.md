# 🔧 Deployment Troubleshooting & Quick Fixes

**Date:** 2026-01-11
**Status:** Fixing deployment issues

---

## 🚨 CURRENT ISSUES IDENTIFIED

### Issue 1: JWT_SECRET Not Set (Exit 139)
**Error:** `JWT settings are invalid or missing. Please set JwtSettings__Secret`

**Root Cause:** `.env` file has empty JWT_SECRET value

**Fix:**
```bash
# Generate a strong secret
JWT_SECRET=$(openssl rand -base64 48)

# Update .env file
cd /home/Shahin-ai/Shahin-Jan-2026
cat > .env << EOF
# CRITICAL: JWT Settings (REQUIRED)
JwtSettings__Secret="${JWT_SECRET}"
JwtSettings__Issuer="GrcSystem"
JwtSettings__Audience="GrcSystemUsers"

# Database Connection (REQUIRED)
ConnectionStrings__DefaultConnection="Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true"
ConnectionStrings__GrcAuthDb="Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true"

# Application URLs
App__BaseUrl="https://portal.shahin-ai.com"
App__LandingUrl="https://shahin-ai.com"

# Claude Agents (Optional)
ClaudeAgents__Enabled="false"
ClaudeAgents__ApiKey=""

# Logging
Logging__LogLevel__Default="Information"
Logging__LogLevel__Microsoft="Warning"
Logging__LogLevel__Microsoft.AspNetCore="Warning"

# Environment
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="http://+:8888"
EOF

echo "✅ .env file created with generated JWT_SECRET"
```

---

### Issue 2: Docker Container Image Problem
**Error:** `KeyError: 'ContainerConfig'`

**Root Cause:** Stale docker container or corrupted image

**Fix:**
```bash
# Stop and remove all containers
cd /home/Shahin-ai/Shahin-Jan-2026
docker-compose down -v

# Remove the problematic image
docker rmi shahin-jan-2026_grcmvc || true

# Rebuild from scratch
docker-compose build --no-cache grcmvc

# Start services
docker-compose up -d
```

---

### Issue 3: Build Compilation Error (Already Fixed)
**Error:** Duplicate logger variable

**Status:** ✅ Already fixed in Program.cs line 1387

---

## 🚀 COMPLETE RECOVERY PROCEDURE

### Step 1: Clean Environment
```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Stop everything
docker-compose down -v

# Remove problematic containers and images
docker rm -f $(docker ps -aq) 2>/dev/null || true
docker rmi shahin-jan-2026_grcmvc 2>/dev/null || true

# Clean docker build cache
docker builder prune -af
```

### Step 2: Create Proper .env File
```bash
# Generate JWT secret
JWT_SECRET=$(openssl rand -base64 48)

# Create .env with all required variables
cat > /home/Shahin-ai/Shahin-Jan-2026/.env << 'ENVEOF'
# ===========================================
# PRODUCTION ENVIRONMENT VARIABLES
# ===========================================

# JWT Settings (REQUIRED)
JwtSettings__Secret=REPLACE_WITH_GENERATED_SECRET
JwtSettings__Issuer=GrcSystem
JwtSettings__Audience=GrcSystemUsers

# Database Connection (REQUIRED)
ConnectionStrings__DefaultConnection=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true
ConnectionStrings__GrcAuthDb=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true

# Application Settings
App__BaseUrl=https://portal.shahin-ai.com
App__LandingUrl=https://shahin-ai.com

# SMTP Settings (Optional - Configure if needed)
SmtpSettings__Host=smtp.office365.com
SmtpSettings__Port=587
SmtpSettings__EnableSsl=true
SmtpSettings__FromEmail=info@shahin-ai.com
SmtpSettings__Username=info@shahin-ai.com
SmtpSettings__Password=

# Claude Agents (Optional)
ClaudeAgents__Enabled=false
ClaudeAgents__ApiKey=
ClaudeAgents__Model=claude-sonnet-4-20250514

# Feature Flags
ENABLE_AUTO_MIGRATION=false
FeatureFlags__EnableSwagger=false
FeatureFlags__EnableDetailedErrors=false

# Rate Limiting
RateLimiting__GlobalPermitLimit=100
RateLimiting__ApiPermitLimit=50

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft__AspNetCore=Warning

# ASP.NET Core Settings
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8888
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Database Initialization
POSTGRES_DB=GrcMvcDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres_2026
ENVEOF

# Replace the JWT secret with generated one
sed -i "s|REPLACE_WITH_GENERATED_SECRET|${JWT_SECRET}|g" /home/Shahin-ai/Shahin-Jan-2026/.env

echo "✅ .env file created with JWT_SECRET: ${JWT_SECRET:0:20}..."
```

### Step 3: Verify Configuration
```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Check .env file
echo "Checking .env file..."
grep -E "JwtSettings__Secret|ConnectionStrings__DefaultConnection" .env

# Verify it's not empty
if grep -q "JwtSettings__Secret=$" .env || grep -q "REPLACE_WITH_GENERATED_SECRET" .env; then
    echo "❌ ERROR: JWT_SECRET is still empty or placeholder"
    exit 1
else
    echo "✅ JWT_SECRET is set"
fi
```

### Step 4: Rebuild and Start
```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Build with no cache
docker-compose build --no-cache grcmvc

# Start database first
docker-compose up -d grc-db

# Wait for database
echo "Waiting for database..."
sleep 10

# Start the application
docker-compose up -d grcmvc

# Watch logs
docker-compose logs -f grcmvc
```

### Step 5: Verify Application
```bash
# Wait for startup
sleep 15

# Check if running
docker-compose ps

# Test health endpoint
curl -f http://localhost:8888/health/ready && echo "✅ Application is healthy" || echo "❌ Application is not responding"

# Check logs for errors
docker-compose logs grcmvc | grep -i error
```

---

## 🔍 DIAGNOSTIC COMMANDS

### Check Environment Variables
```bash
# In running container
docker-compose exec grcmvc env | grep -E "Jwt|Connection|Claude"

# Expected output should show:
# JwtSettings__Secret=<long-string>
# ConnectionStrings__DefaultConnection=Host=grc-db...
```

### Check Application Startup
```bash
# View startup logs
docker-compose logs grcmvc | head -100

# Look for:
# ✅ "Application started"
# ❌ "JWT settings are invalid"
# ❌ "InvalidOperationException"
```

### Test Database Connection
```bash
# Connect to database
docker-compose exec grc-db psql -U postgres -d GrcMvcDb -c "SELECT version();"

# Expected: PostgreSQL version output
```

### Check Port Binding
```bash
# Check if port 8888 is listening
netstat -tlnp | grep 8888

# Or
lsof -i :8888

# Expected: Docker proxy listening on 8888
```

---

## 🔧 ALTERNATIVE FIX: Direct .env Template

If automated script fails, manually create `.env`:

```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Generate secret manually
openssl rand -base64 48

# Create .env and paste the secret
nano .env
```

Then paste this content (replace YOUR_GENERATED_SECRET_HERE):

```env
JwtSettings__Secret=YOUR_GENERATED_SECRET_HERE
JwtSettings__Issuer=GrcSystem
JwtSettings__Audience=GrcSystemUsers
ConnectionStrings__DefaultConnection=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026
ConnectionStrings__GrcAuthDb=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026
App__BaseUrl=https://portal.shahin-ai.com
App__LandingUrl=https://shahin-ai.com
ClaudeAgents__Enabled=false
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8888
```

Save and rebuild:
```bash
docker-compose down
docker-compose build --no-cache grcmvc
docker-compose up -d
```

---

## ✅ SUCCESS INDICATORS

Your deployment is successful when you see:

1. **Container Running:**
```bash
docker-compose ps
# grcmvc should show "Up" state
```

2. **No Errors in Logs:**
```bash
docker-compose logs grcmvc | grep -i error
# Should show minimal or no errors
```

3. **Health Check Passes:**
```bash
curl http://localhost:8888/health/ready
# Should return 200 OK
```

4. **Application Accessible:**
```bash
curl -I http://localhost:8888
# Should return HTTP headers
```

---

## 🚨 IF STILL FAILING

### Last Resort: Check Program.cs Validation

The validation is on line ~468 in Program.cs. Temporarily disable for testing:

**NOT RECOMMENDED FOR PRODUCTION** - Only for debugging:

```bash
# Edit Program.cs
nano /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc/Program.cs

# Find line ~130-135:
# if (string.IsNullOrWhiteSpace(jwtSecret))
# {
#     throw new InvalidOperationException("JWT_SECRET environment variable is required...");
# }

# Comment it out TEMPORARILY:
# // if (string.IsNullOrWhiteSpace(jwtSecret))
# // {
# //     throw new InvalidOperationException("JWT_SECRET environment variable is required...");
# // }

# Rebuild
docker-compose build --no-cache grcmvc
docker-compose up -d
```

**IMPORTANT:** This is only for debugging. Re-enable validation after fixing the .env issue.

---

## 📞 SUPPORT CHECKLIST

If you need to escalate, provide:

1. **Environment file (sanitized):**
```bash
grep -v "Secret\|Password" /home/Shahin-ai/Shahin-Jan-2026/.env
```

2. **Container status:**
```bash
docker-compose ps
```

3. **Application logs:**
```bash
docker-compose logs grcmvc | tail -100
```

4. **Database status:**
```bash
docker-compose exec grc-db psql -U postgres -c "\l"
```

---

## 🎯 EXPECTED FINAL STATE

After following this guide:

- ✅ `.env` file exists with valid JWT_SECRET (48+ characters)
- ✅ `docker-compose ps` shows grcmvc as "Up"
- ✅ `curl http://localhost:8888/health/ready` returns 200
- ✅ No "JWT settings are invalid" errors in logs
- ✅ Application accessible on port 8888

---

**Quick Command Summary:**
```bash
# 1. Generate secret
JWT_SECRET=$(openssl rand -base64 48)

# 2. Create .env with secret
echo "JwtSettings__Secret=${JWT_SECRET}" > .env
echo "ConnectionStrings__DefaultConnection=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026" >> .env
echo "ASPNETCORE_URLS=http://+:8888" >> .env

# 3. Rebuild
docker-compose down -v
docker-compose build --no-cache grcmvc
docker-compose up -d

# 4. Verify
sleep 15
curl http://localhost:8888/health/ready
```

---

**Created:** 2026-01-11
**Status:** Active Troubleshooting
**Priority:** 🔴 CRITICAL
