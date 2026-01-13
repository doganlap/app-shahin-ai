# 🔐 Environment Variables Configuration Guide

**Last Updated:** 2026-01-11
**Purpose:** Complete reference for all environment variables needed in production

---

## 🎯 OVERVIEW

This guide lists all environment variables required for production deployment. The simplified `appsettings.Production.json` relies on these being set in your deployment environment.

---

## ✅ REQUIRED ENVIRONMENT VARIABLES

These **MUST** be set or the application will fail to start:

### 1. JWT Configuration
```bash
# JWT Secret - REQUIRED (minimum 32 characters)
export JWT_SECRET="your-secure-secret-at-least-32-characters-long"

# JWT Issuer - REQUIRED
export JWT_ISSUER="ShahinAI"

# JWT Audience - REQUIRED
export JWT_AUDIENCE="ShahinAIUsers"
```

**Validation:** Application will throw `InvalidOperationException` if `JWT_SECRET` is missing.

---

### 2. Database Connection
```bash
# Main database - REQUIRED
export CONNECTION_STRING="Host=postgres-host;Database=GrcMvcDb;Username=grc_user;Password=secure_password;Port=5432"

# Auth database - REQUIRED (can be same as main)
export CONNECTION_STRING_GrcAuthDb="Host=postgres-host;Database=GrcMvcDb;Username=grc_user;Password=secure_password;Port=5432"
```

**Alternative format:**
```bash
# Using individual components (will be combined in Program.cs)
export DB_HOST="postgres-host"
export DB_NAME="GrcMvcDb"
export DB_USER="grc_user"
export DB_PASSWORD="secure_password"
export DB_PORT="5432"
```

---

### 3. Application URLs
```bash
# Backend API URL - REQUIRED
export APP_BASE_URL="https://app.shahin-ai.com"

# Landing/Marketing site URL - REQUIRED
export APP_LANDING_URL="https://shahin-ai.com"

# Allowed hosts for request validation - REQUIRED
export ALLOWED_HOSTS="app.shahin-ai.com;shahin-ai.com;*.shahin-ai.com"
```

---

## 🔧 OPTIONAL BUT RECOMMENDED

### 4. Claude AI Agents
```bash
# Enable Claude agents - Optional (default: false)
export CLAUDE_AGENTS_ENABLED="true"

# Claude API Key - REQUIRED if CLAUDE_AGENTS_ENABLED=true
export CLAUDE_API_KEY="sk-ant-api03-your-key-here"

# Claude Model - Optional (default: claude-sonnet-4-20250514)
export CLAUDE_MODEL="claude-sonnet-4-20250514"

# Max tokens - Optional (default: 4096)
export CLAUDE_MAX_TOKENS="4096"
```

**Important:** If `CLAUDE_AGENTS_ENABLED=true` but `CLAUDE_API_KEY` is missing, the application will fail to start.

---

### 5. Rate Limiting Configuration
```bash
# Global rate limit - Optional (default: 100 requests per minute)
export RATELIMITING__GLOBALPERMITLIMIT="100"

# API rate limit - Optional (default: 50 requests per minute)
export RATELIMITING__APIPERMITLIMIT="50"
```

**Note:** Uses double underscore (`__`) for nested configuration.

---

### 6. Feature Flags
```bash
# Auto-migrations - Optional (default: false, KEEP FALSE IN PRODUCTION)
export ENABLE_AUTO_MIGRATION="false"

# Enable Swagger in production - Optional (default: false, recommended: false)
export FEATUREFLAGS__ENABLESWAGGER="false"

# Detailed errors - Optional (default: false, recommended: false)
export FEATUREFLAGS__ENABLEDETAILEDERRORS="false"
```

---

### 7. SMTP Email Configuration
```bash
# SMTP Host - Optional (default: smtp.office365.com)
export SMTP_HOST="smtp.office365.com"

# SMTP Port - Optional (default: 587)
export SMTP_PORT="587"

# From Email - Required if email enabled
export SMTP_FROM_EMAIL="info@shahin-ai.com"

# SMTP Username - Required if email enabled
export SMTP_USERNAME="info@shahin-ai.com"

# SMTP Password - Required if email enabled
export SMTP_PASSWORD="your-email-password"

# Azure OAuth2 for SMTP (if using OAuth)
export AZURE_TENANT_ID="your-tenant-id"
export SMTP_CLIENT_ID="your-client-id"
export SMTP_CLIENT_SECRET="your-client-secret"
```

---

### 8. Azure Integration (Optional)
```bash
# Azure Key Vault
export AZURE_KEYVAULT_ENABLED="true"
export AZURE_KEYVAULT_URI="https://your-vault.vault.azure.net/"
export AZURE_CLIENT_ID="your-app-client-id"
export AZURE_CLIENT_SECRET="your-app-secret"

# Microsoft Graph API
export MSGRAPH_CLIENT_ID="your-msgraph-client-id"
export MSGRAPH_CLIENT_SECRET="your-msgraph-secret"
export MSGRAPH_APP_ID_URI="api://your-app-id"

# Copilot Agent
export COPILOT_ENABLED="false"
export COPILOT_CLIENT_ID="your-copilot-client-id"
export COPILOT_CLIENT_SECRET="your-copilot-secret"
```

---

### 9. Logging Configuration
```bash
# Log level - Optional (default: Information)
export LOGGING__LOGLEVEL__DEFAULT="Information"

# Seq logging server - Optional
export SEQ_URL="http://seq-server:5341"
export SEQ_API_KEY="your-seq-api-key"

# Log file path - Optional (default: /app/logs/)
export LOG_PATH="/app/logs/"
```

---

### 10. CORS Configuration
```bash
# Allowed CORS origins - Required if serving SPA
export CORS_ALLOWED_ORIGINS="https://shahin-ai.com,https://www.shahin-ai.com,https://portal.shahin-ai.com"
```

---

### 11. Workflow & Background Jobs
```bash
# Kafka (optional)
export KAFKA_ENABLED="false"
export KAFKA_BOOTSTRAP_SERVERS="localhost:9092"

# Camunda BPM (optional)
export CAMUNDA_ENABLED="false"
export CAMUNDA_BASE_URL="http://camunda:8080/camunda"
export CAMUNDA_USERNAME="admin"
export CAMUNDA_PASSWORD="admin"
```

---

## 📋 COMPLETE PRODUCTION TEMPLATE

Save this as `.env.production` (DO NOT COMMIT TO GIT):

```bash
# ============================================
# PRODUCTION ENVIRONMENT VARIABLES
# ============================================
# DO NOT COMMIT THIS FILE TO VERSION CONTROL
# ============================================

# --- REQUIRED VARIABLES ---

# JWT Configuration (REQUIRED)
JWT_SECRET="REPLACE_WITH_STRONG_SECRET_AT_LEAST_32_CHARS"
JWT_ISSUER="ShahinAI"
JWT_AUDIENCE="ShahinAIUsers"

# Database (REQUIRED)
CONNECTION_STRING="Host=postgres-prod;Database=GrcMvcDb;Username=grc_prod;Password=REPLACE_WITH_DB_PASSWORD;Port=5432"
CONNECTION_STRING_GrcAuthDb="Host=postgres-prod;Database=GrcMvcDb;Username=grc_prod;Password=REPLACE_WITH_DB_PASSWORD;Port=5432"

# Application URLs (REQUIRED)
APP_BASE_URL="https://app.shahin-ai.com"
APP_LANDING_URL="https://shahin-ai.com"
ALLOWED_HOSTS="app.shahin-ai.com;shahin-ai.com;*.shahin-ai.com"

# --- OPTIONAL BUT RECOMMENDED ---

# Claude AI Agents
CLAUDE_AGENTS_ENABLED="false"  # Set to "true" to enable
CLAUDE_API_KEY=""  # Required if enabled
CLAUDE_MODEL="claude-sonnet-4-20250514"
CLAUDE_MAX_TOKENS="4096"

# Rate Limiting
RATELIMITING__GLOBALPERMITLIMIT="100"
RATELIMITING__APIPERMITLIMIT="50"

# Feature Flags
ENABLE_AUTO_MIGRATION="false"  # KEEP FALSE IN PRODUCTION
FEATUREFLAGS__ENABLESWAGGER="false"
FEATUREFLAGS__ENABLEDETAILEDERRORS="false"

# SMTP Email (if needed)
SMTP_HOST="smtp.office365.com"
SMTP_PORT="587"
SMTP_FROM_EMAIL="info@shahin-ai.com"
SMTP_USERNAME="info@shahin-ai.com"
SMTP_PASSWORD=""  # Set if using SMTP

# Azure (if using)
AZURE_TENANT_ID=""
AZURE_KEYVAULT_ENABLED="false"
AZURE_KEYVAULT_URI=""
AZURE_CLIENT_ID=""
AZURE_CLIENT_SECRET=""

# CORS
CORS_ALLOWED_ORIGINS="https://shahin-ai.com,https://www.shahin-ai.com,https://portal.shahin-ai.com"

# Logging
LOGGING__LOGLEVEL__DEFAULT="Information"
LOG_PATH="/app/logs/"

# Seq (optional centralized logging)
SEQ_URL=""
SEQ_API_KEY=""
```

---

## 🐳 DOCKER COMPOSE EXAMPLE

```yaml
version: '3.8'

services:
  grc-api:
    image: shahin/grc-api:latest
    environment:
      # Required
      - JWT_SECRET=${JWT_SECRET}
      - JWT_ISSUER=ShahinAI
      - JWT_AUDIENCE=ShahinAIUsers
      - CONNECTION_STRING=${CONNECTION_STRING}
      - CONNECTION_STRING_GrcAuthDb=${CONNECTION_STRING}
      - APP_BASE_URL=https://app.shahin-ai.com
      - APP_LANDING_URL=https://shahin-ai.com
      - ALLOWED_HOSTS=app.shahin-ai.com;shahin-ai.com

      # Optional
      - CLAUDE_AGENTS_ENABLED=false
      - RATELIMITING__GLOBALPERMITLIMIT=100
      - RATELIMITING__APIPERMITLIMIT=50
      - ENABLE_AUTO_MIGRATION=false

    volumes:
      - ./logs:/app/logs
    ports:
      - "8888:8080"
    depends_on:
      - postgres
```

---

## ☸️ KUBERNETES SECRETS EXAMPLE

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: grc-secrets
type: Opaque
stringData:
  jwt-secret: "your-secure-secret-here"
  connection-string: "Host=postgres;Database=GrcMvcDb;Username=user;Password=pass"
  claude-api-key: "sk-ant-your-key"

---
apiVersion: v1
kind: ConfigMap
metadata:
  name: grc-config
data:
  APP_BASE_URL: "https://app.shahin-ai.com"
  APP_LANDING_URL: "https://shahin-ai.com"
  JWT_ISSUER: "ShahinAI"
  JWT_AUDIENCE: "ShahinAIUsers"
  CLAUDE_AGENTS_ENABLED: "false"
  RATELIMITING__GLOBALPERMITLIMIT: "100"

---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: grc-api
spec:
  template:
    spec:
      containers:
      - name: api
        image: shahin/grc-api:latest
        envFrom:
        - configMapRef:
            name: grc-config
        env:
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: grc-secrets
              key: jwt-secret
        - name: CONNECTION_STRING
          valueFrom:
            secretKeyRef:
              name: grc-secrets
              key: connection-string
        - name: CLAUDE_API_KEY
          valueFrom:
            secretKeyRef:
              name: grc-secrets
              key: claude-api-key
```

---

## 🔍 VALIDATION SCRIPT

Create `scripts/validate-env.sh`:

```bash
#!/bin/bash

echo "🔍 Validating Production Environment Variables"
echo "=============================================="

ERRORS=0

# Check required variables
check_var() {
    local var_name=$1
    local var_value="${!var_name}"

    if [ -z "$var_value" ]; then
        echo "❌ MISSING: $var_name"
        ERRORS=$((ERRORS + 1))
    else
        echo "✅ SET: $var_name"
    fi
}

echo -e "\n📋 Required Variables:"
check_var "JWT_SECRET"
check_var "CONNECTION_STRING"
check_var "APP_BASE_URL"
check_var "APP_LANDING_URL"

echo -e "\n🔧 Optional Variables:"
if [ "${CLAUDE_AGENTS_ENABLED}" = "true" ]; then
    check_var "CLAUDE_API_KEY"
fi

echo -e "\n=============================================="
if [ $ERRORS -eq 0 ]; then
    echo "✅ All required variables are set"
    exit 0
else
    echo "❌ $ERRORS required variable(s) missing"
    exit 1
fi
```

Make it executable:
```bash
chmod +x scripts/validate-env.sh
```

Run before deployment:
```bash
./scripts/validate-env.sh
```

---

## 🛡️ SECURITY BEST PRACTICES

### 1. Never Commit Secrets
```bash
# Add to .gitignore
.env
.env.production
.env.local
*.env.backup
```

### 2. Use Strong Secrets
```bash
# Generate strong JWT secret
openssl rand -base64 64

# Generate strong password
openssl rand -base64 32
```

### 3. Rotate Regularly
Follow [SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md) after 2 weeks.

### 4. Use Secret Management
- **Azure:** Azure Key Vault
- **Kubernetes:** Kubernetes Secrets
- **Docker:** Docker Secrets
- **AWS:** AWS Secrets Manager

---

## 📊 COMPATIBILITY NOTES

### ASP.NET Core Configuration Precedence

ASP.NET Core loads configuration in this order (later sources override earlier):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. **Environment Variables** (highest priority)
4. Command-line arguments

### Environment Variable Format

For nested configuration, use double underscore:
```bash
# JSON: { "JwtSettings": { "Secret": "..." } }
export JwtSettings__Secret="value"

# JSON: { "RateLimiting": { "GlobalPermitLimit": 100 } }
export RateLimiting__GlobalPermitLimit="100"
```

### Variable Substitution

The `${VARIABLE_NAME}` syntax in `appsettings.Production.json` is **NOT** natively supported by ASP.NET Core. Our `Program.cs` manually reads and sets these from environment variables.

**Current implementation in Program.cs:**
```csharp
// Program.cs reads environment variables and overrides configuration
builder.Configuration["JwtSettings:Secret"] = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CONNECTION_STRING");
// ... etc
```

---

## 🔗 RELATED DOCUMENTATION

1. [PRODUCTION_FIXES_COMPLETED.md](PRODUCTION_FIXES_COMPLETED.md) - All fixes implemented
2. [FINAL_VALIDATION_CHECKLIST.md](FINAL_VALIDATION_CHECKLIST.md) - Pre-deployment validation
3. [POST_PRODUCTION_MONITORING_GUIDE.md](POST_PRODUCTION_MONITORING_GUIDE.md) - Monitoring guide
4. [SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md) - Credential rotation

---

## ✅ QUICK CHECKLIST

Before deploying, ensure:

- [ ] All REQUIRED variables set
- [ ] JWT_SECRET is at least 32 characters
- [ ] Database connection string is correct
- [ ] URLs match your production domains
- [ ] Claude API key set (if enabled)
- [ ] No secrets committed to git
- [ ] Validation script passes: `./scripts/validate-env.sh`

---

**Created:** 2026-01-11
**Last Updated:** 2026-01-11
**Version:** 1.0
