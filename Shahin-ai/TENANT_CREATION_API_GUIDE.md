# Public Tenant Creation API Guide

## Endpoint Details

**URL**: `POST /api/agent/tenant/create`
**Base URL**: `http://localhost:5137` (Development) or `https://app.shahin-ai.com` (Production)
**Authentication**: None required (`[AllowAnonymous]`)
**Rate Limiting**: 5 requests per 5 minutes per IP
**Content-Type**: `application/json`

---

## Request Format

### JSON Body

```json
{
  "tenantName": "acme-corp",
  "adminEmail": "admin@acme.com",
  "adminPassword": "StrongPassword123!",
  "recaptchaToken": "",
  "deviceFingerprint": ""
}
```

### Field Descriptions

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `tenantName` | string | ✅ Yes | Unique tenant name (will be sanitized: lowercase, hyphens, alphanumeric) |
| `adminEmail` | string | ✅ Yes | Admin user email address (must be unique) |
| `adminPassword` | string | ✅ Yes | Strong password (ABP default: min 6 chars, 1 uppercase, 1 lowercase, 1 digit) |
| `recaptchaToken` | string | ❌ No | reCAPTCHA token (bypassed if `Recaptcha:Enabled=false`) |
| `deviceFingerprint` | string | ❌ No | Device fingerprint for fraud detection (optional) |

---

## Response Format

### Success Response (200 OK)

```json
{
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tenantName": "acme-corp",
  "adminEmail": "admin@acme.com",
  "message": "Tenant created successfully"
}
```

### Error Responses

#### 400 Bad Request (Validation Failed)
```json
{
  "error": "Security validation failed: CAPTCHA validation required"
}
```

#### 409 Conflict (Duplicate Tenant/Email)
```json
{
  "error": "A tenant with the given name already exists."
}
```

#### 429 Too Many Requests (Rate Limit)
```json
{
  "error": "Rate limit exceeded. Please try again later."
}
```

#### 500 Internal Server Error
```json
{
  "error": "An error occurred while creating the tenant. Please try again later."
}
```

---

## cURL Examples

### Basic Tenant Creation
```bash
curl -X POST http://localhost:5137/api/agent/tenant/create \
  -H "Content-Type: application/json" \
  -d '{
    "tenantName": "test-company",
    "adminEmail": "admin@test-company.com",
    "adminPassword": "TestPass123!"
  }'
```

### With Device Fingerprint
```bash
curl -X POST http://localhost:5137/api/agent/tenant/create \
  -H "Content-Type: application/json" \
  -d '{
    "tenantName": "acme-inc",
    "adminEmail": "john@acme.com",
    "adminPassword": "SecurePass456!",
    "deviceFingerprint": "fp_abc123xyz789"
  }'
```

---

## PowerShell Example

```powershell
$body = @{
    tenantName = "my-company"
    adminEmail = "admin@mycompany.com"
    adminPassword = "MySecure123!"
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Uri "http://localhost:5137/api/agent/tenant/create" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"

Write-Host "Tenant ID: $($response.tenantId)"
Write-Host "Tenant Name: $($response.tenantName)"
```

---

## JavaScript/Fetch Example

```javascript
const response = await fetch('http://localhost:5137/api/agent/tenant/create', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    tenantName: 'startup-co',
    adminEmail: 'founder@startup.co',
    adminPassword: 'Startup2024!'
  })
});

const result = await response.json();
console.log('Tenant ID:', result.tenantId);
console.log('Tenant Name:', result.tenantName);
```

---

## Python Example

```python
import requests

url = "http://localhost:5137/api/agent/tenant/create"
payload = {
    "tenantName": "tech-corp",
    "adminEmail": "admin@techcorp.com",
    "adminPassword": "TechPass789!"
}

response = requests.post(url, json=payload)
data = response.json()

print(f"Tenant ID: {data['tenantId']}")
print(f"Tenant Name: {data['tenantName']}")
```

---

## What Happens After Creation?

1. **Tenant Created**: A new tenant is created in the ABP multi-tenant database
2. **Admin User Created**: Admin user is created with the specified email/password
3. **Role Assigned**: User is assigned the "TenantAdmin" role automatically
4. **Database Context**: Tenant-specific database context is initialized
5. **ExtraProperties Set**: Metadata tracked:
   - `OnboardingStatus`: "Pending"
   - `CreatedByAgent`: "TenantCreationFacade"
   - `FirstAdminUserId`: The admin user's GUID
   - `DeviceFingerprint`: If provided
   - `CreatedFromIP`: Requester's IP address
6. **Fraud Detection**: Fingerprint tracked in `TenantCreationFingerprints` table
7. **Audit Logging**: Creation logged with IP, user agent, timestamp

---

## Security Features

### Currently Active (with Recaptcha:Enabled=false):
- ✅ Rate limiting (5 requests per 5 minutes per IP)
- ✅ Fraud detection tracking (device fingerprint, IP patterns)
- ✅ Password validation (ABP default rules)
- ✅ Duplicate tenant/email prevention
- ✅ Comprehensive audit logging

### Bypassed (for testing):
- ⚠️ CAPTCHA validation (disabled in config)

### To Enable Full Security:
Update `appsettings.json`:
```json
{
  "Recaptcha": {
    "Enabled": true,
    "SiteKey": "your-recaptcha-site-key",
    "SecretKey": "your-recaptcha-secret-key",
    "MinimumScore": 0.5
  }
}
```

---

## Testing the API

### Quick Test
```bash
# Create a test tenant
curl -X POST http://localhost:5137/api/agent/tenant/create \
  -H "Content-Type: application/json" \
  -d '{
    "tenantName": "test-tenant-001",
    "adminEmail": "test@example.com",
    "adminPassword": "TestPass123!"
  }'

# Expected Response:
# {
#   "tenantId": "...",
#   "tenantName": "test-tenant-001",
#   "adminEmail": "test@example.com",
#   "message": "Tenant created successfully"
# }
```

### Verify in Database
```sql
-- Check tenant created
SELECT "Id", "Name", "ExtraProperties"
FROM "AbpTenants"
WHERE "Name" = 'test-tenant-001';

-- Check admin user created
SELECT "Id", "Email", "TenantId", "EmailConfirmed"
FROM "AbpUsers"
WHERE "Email" = 'test@example.com';

-- Check fingerprint tracked
SELECT "TenantId", "IpAddress", "DeviceId", "IsFlagged"
FROM "TenantCreationFingerprints"
ORDER BY "CreatedAt" DESC
LIMIT 1;
```

---

## Integration with Other Systems

### Zapier Webhook
Use this endpoint as a Zapier webhook action to create tenants automatically when:
- New customer signs up on your website
- Payment is confirmed
- Trial period starts

### Chatbot/GPT Integration
Call this API from your GPT bot when a user requests a demo:
```
User: "I want to try the GRC platform"
Bot: *Calls API* → Creates tenant → Returns login URL
Bot: "Your trial is ready! Visit https://app.shahin-ai.com and login with: test@example.com"
```

### Mobile App
Use this endpoint from your mobile app's registration flow.

---

## Production Deployment Notes

### Before Going Live:
1. ✅ Enable CAPTCHA validation (`Recaptcha:Enabled=true`)
2. ✅ Configure fraud detection thresholds in `appsettings.json`
3. ✅ Set up monitoring/alerting for failed creation attempts
4. ✅ Review rate limiting thresholds (currently 5 per 5 min)
5. ✅ Implement email verification workflow (currently bypassed)
6. ✅ Configure SMTP for admin welcome emails

### Monitoring Endpoints:
- Check flagged tenants: Query `TenantCreationFingerprints` where `IsFlagged=true`
- Review audit logs: Check application logs for "OnboardingAgent:" entries

---

## API Endpoint Location

**Controller**: [OnboardingAgentController.cs](src/GrcMvc/Controllers/Api/OnboardingAgentController.cs)
**Route**: `[Route("api/agent/tenant")]`
**Method**: `[HttpPost("create")]`
**Service**: Uses `ITenantCreationFacadeService` with full security layers

---

## Support

For issues or questions:
- **Email**: support@shahin-ai.com
- **Logs**: Check `/logs/grc-system-*.log` for detailed error messages
- **Health Check**: `GET http://localhost:5137/health`
