# 🔐 URGENT: Credential Rotation Guide

**Date:** 2026-01-11
**Status:** 🔴 **CRITICAL ACTION REQUIRED**
**Reason:** Sensitive credentials exposed in git history (commit 803dd31d and earlier)

---

## ⚠️ EXPOSED CREDENTIALS

The following files were committed to git history with real credentials:
- `.env.backup` - Database passwords
- `.env.production.secure` - Claude API keys
- `.env.grcmvc.production` - Azure tenant/client IDs, secrets
- `.env.grcmvc.secure` - JWT secrets, connection strings

**Even though these files were removed** in commit 803dd31d, **they remain accessible in git history** and must be considered compromised.

---

## 🚨 IMMEDIATE ACTIONS (Do Today)

### 1. Rotate Database Passwords

**PostgreSQL/SQL Server:**
```sql
-- Connect to your database as admin
ALTER USER grc_app WITH PASSWORD 'NEW_STRONG_PASSWORD_HERE';

-- Update connection strings in:
-- - Azure Key Vault (if using)
-- - Kubernetes secrets (if using)
-- - .env files (local only, never commit)
```

**Generate strong password:**
```bash
# Use this to generate a 32-character password
openssl rand -base64 32
```

**Update in:**
- Azure Key Vault: `grc-db-password`
- Kubernetes Secret: `grc-secrets`
- Local `.env` files (DO NOT COMMIT)

---

### 2. Rotate Claude API Keys

**Action:**
1. Log in to https://console.anthropic.com
2. Go to API Keys section
3. **Revoke** the exposed API key immediately
4. Generate a new API key
5. Update the key in:

**Update locations:**
```bash
# Azure Key Vault
az keyvault secret set \
  --vault-name shahin-prod-kv \
  --name claude-api-key \
  --value "NEW_KEY_HERE"

# Kubernetes (if applicable)
kubectl create secret generic claude-secrets \
  --from-literal=api-key=NEW_KEY_HERE \
  --dry-run=client -o yaml | kubectl apply -f -

# Local .env (DO NOT COMMIT)
echo "ClaudeAgents__ApiKey=NEW_KEY_HERE" >> .env.local
```

---

### 3. Rotate Azure Credentials

**Azure Active Directory Application:**

**If tenant ID was exposed (not sensitive but change anyway):**
- Tenant ID can remain the same (not a secret)

**If client secret was exposed:**
1. Go to Azure Portal → Azure Active Directory → App Registrations
2. Select your application
3. Go to "Certificates & secrets"
4. **Delete** the old client secret
5. Click "New client secret"
6. Save the new secret value
7. Update in all locations:

```bash
# Azure Key Vault
az keyvault secret set \
  --vault-name shahin-prod-kv \
  --name azure-client-secret \
  --value "NEW_SECRET_HERE"

# Kubernetes
kubectl create secret generic azure-secrets \
  --from-literal=client-secret=NEW_SECRET_HERE \
  --dry-run=client -o yaml | kubectl apply -f -
```

---

### 4. Rotate JWT Signing Secrets

**Generate new JWT secret:**
```bash
# Generate a 64-character secret
openssl rand -base64 64
```

**Update in:**
```bash
# appsettings.Production.json (use environment variable)
# In Azure Key Vault or Kubernetes secrets
JWT_SECRET=NEW_SECRET_HERE

# This will invalidate all existing JWT tokens
# Users will need to re-login
```

**Update in code:**
- `src/GrcMvc/appsettings.Production.json` (use env var)
- Azure Key Vault: `jwt-secret`
- Kubernetes: `grc-secrets`

---

### 5. Rotate OpenAI API Keys (if exposed)

If OpenAI keys were in the exposed files:

1. Go to https://platform.openai.com/api-keys
2. **Revoke** old keys
3. Generate new keys
4. Update in:
   - Azure Key Vault: `openai-api-key`
   - Kubernetes secrets
   - Local `.env` files

---

### 6. Update CORS Origins (if domain secrets exposed)

If production URLs were exposed:
- Review and update allowed CORS origins
- Consider changing internal API endpoints
- Update firewall rules if needed

---

## 📋 CREDENTIAL ROTATION CHECKLIST

Use this checklist to track rotation progress:

- [ ] **Database Password** - Rotated and updated everywhere
- [ ] **Claude API Key** - Old key revoked, new key generated
- [ ] **Azure Client Secret** - Old secret deleted, new secret created
- [ ] **JWT Signing Secret** - New secret generated (users must re-login)
- [ ] **OpenAI API Key** - Old key revoked (if exposed)
- [ ] **SMTP Password** - Rotated (if exposed)
- [ ] **Redis Password** - Rotated (if applicable)
- [ ] **Docker Registry** - Credentials rotated (if exposed)
- [ ] **Azure Key Vault Access** - Reviewed and updated
- [ ] **Kubernetes Secrets** - All secrets updated
- [ ] **Verified services restart** - All services picked up new credentials
- [ ] **Tested application** - Confirmed everything works
- [ ] **Updated documentation** - Removed old credential references

---

## 🔧 HOW TO PROPERLY STORE SECRETS

### For Development:
```bash
# Use .env files (NEVER commit these)
cp .env.example .env
# Edit .env with your secrets

# Verify .gitignore
git status # Should NOT show .env files
```

### For Production:

**Option 1: Azure Key Vault (Recommended)**
```csharp
// In Program.cs
var azureKeyVaultEndpoint = builder.Configuration["AzureKeyVaultEndpoint"];
if (!string.IsNullOrEmpty(azureKeyVaultEndpoint))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(azureKeyVaultEndpoint),
        new DefaultAzureCredential());
}
```

**Option 2: Kubernetes Secrets**
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: grc-secrets
type: Opaque
data:
  db-password: BASE64_ENCODED_PASSWORD
  claude-api-key: BASE64_ENCODED_KEY
  jwt-secret: BASE64_ENCODED_SECRET
```

**Option 3: Environment Variables**
```bash
# In deployment script or CI/CD
export ConnectionStrings__DefaultConnection="Server=..."
export ClaudeAgents__ApiKey="sk-ant-..."
export JwtSettings__Secret="..."
```

---

## 🛡️ PREVENTING FUTURE EXPOSURES

### 1. Pre-commit Hook

Create `.git/hooks/pre-commit`:
```bash
#!/bin/bash

# Prevent committing files with secrets
if git diff --cached --name-only | grep -E '\.env$|\.env\..*' | grep -v '.env.example'; then
    echo "ERROR: Attempting to commit .env files with secrets!"
    echo "These files should NEVER be committed to git."
    exit 1
fi

# Check for common secret patterns
if git diff --cached | grep -E 'sk-ant-|postgres://.*:.*@|password.*=.*[^_example]'; then
    echo "WARNING: Potential secrets detected in commit!"
    echo "Please review your changes carefully."
    exit 1
fi

exit 0
```

Make executable:
```bash
chmod +x .git/hooks/pre-commit
```

### 2. Use git-secrets

```bash
# Install git-secrets
brew install git-secrets  # macOS
# or
apt-get install git-secrets  # Linux

# Initialize
git secrets --install
git secrets --register-aws

# Add custom patterns
git secrets --add 'sk-ant-[a-zA-Z0-9]{48}'  # Claude keys
git secrets --add 'postgres://.*:.*@'       # DB connection strings
git secrets --add 'Bearer [A-Za-z0-9\-\._~\+\/]+=*'  # JWT tokens
```

### 3. Update .gitignore

Already done in [.gitignore](.gitignore) - verify:
```bash
grep -E '\.env' .gitignore
```

Should see:
```
.env
.env.*
!.env.example
!.env.production.template
```

---

## 🔍 AUDIT TRAIL

Keep track of when credentials were rotated:

| Credential | Rotated Date | Rotated By | Notes |
|------------|--------------|------------|-------|
| DB Password | YYYY-MM-DD | Name | Reason |
| Claude API Key | YYYY-MM-DD | Name | Reason |
| Azure Client Secret | YYYY-MM-DD | Name | Reason |
| JWT Secret | YYYY-MM-DD | Name | Users re-login required |

---

## 📞 INCIDENT RESPONSE

If you discover additional exposed credentials:

1. **Immediately revoke** the credential
2. **Assess impact** - was it accessed?
3. **Rotate** following this guide
4. **Document** in audit trail above
5. **Review** how it was exposed
6. **Implement** preventive measures

---

## ✅ VERIFICATION STEPS

After rotating all credentials:

```bash
# 1. Test database connection
psql "postgresql://grc_app:NEW_PASSWORD@localhost:5432/grc_db" -c "SELECT 1"

# 2. Test API with new secrets
curl -H "Authorization: Bearer NEW_JWT" http://localhost:8888/api/health

# 3. Verify no old secrets in environment
env | grep -i "secret\|password\|key" | grep -v "OLD_VALUE"

# 4. Check application logs for auth failures
tail -f /var/log/grc-app/app.log | grep -i "401\|403\|auth"

# 5. Verify Azure Key Vault connection
az keyvault secret show --vault-name shahin-prod-kv --name claude-api-key
```

---

## 🚀 POST-ROTATION DEPLOYMENT

After rotating all credentials:

```bash
# 1. Update Kubernetes secrets
kubectl apply -f k8s/secrets.yaml

# 2. Restart deployments to pick up new secrets
kubectl rollout restart deployment/grc-api
kubectl rollout restart deployment/grc-worker

# 3. Verify pods are running
kubectl get pods -w

# 4. Check logs for errors
kubectl logs -f deployment/grc-api

# 5. Run smoke tests
./scripts/smoke-tests.sh production
```

---

## 📚 REFERENCES

- [Azure Key Vault Best Practices](https://learn.microsoft.com/azure/key-vault/general/best-practices)
- [OWASP Secrets Management](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)
- [Kubernetes Secrets](https://kubernetes.io/docs/concepts/configuration/secret/)
- [ASP.NET Core Secret Manager](https://learn.microsoft.com/aspnet/core/security/app-secrets)

---

## 🔴 REMEMBER

**Secrets in git history are PERMANENT unless you:**
1. Use BFG Repo-Cleaner or git-filter-branch (destructive)
2. Force-push to all remote branches (breaks everyone's clones)
3. Consider the repository compromised forever

**Best practice:** Rotate all exposed credentials and move forward with proper secret management.

---

**Status after completion:** Update this file with rotation dates and mark as ✅ **COMPLETED**
