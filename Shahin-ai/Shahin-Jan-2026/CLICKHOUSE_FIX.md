# ClickHouse Authentication Fix

## Problem
**Error:** "Error authenticating with database. Please check your connection params and try again."
**URL:** http://localhost:8123 (ClickHouse HTTP interface)

## Root Cause
ClickHouse is running and healthy, but:
1. The application doesn't have ClickHouse configuration in `appsettings.json`
2. ClickHouse is disabled by default (`ClickHouse:Enabled = false`)
3. Accessing ClickHouse directly via HTTP requires authentication

## Solution Applied

### ✅ Added ClickHouse Configuration
Updated `appsettings.json` with ClickHouse settings:

```json
"ClickHouse": {
  "Enabled": true,
  "Host": "grc-clickhouse",
  "HttpPort": 8123,
  "NativePort": 9000,
  "Database": "grc_analytics",
  "Username": "grc_analytics",
  "Password": "grc_analytics_2026",
  "MaxPoolSize": 10,
  "CommandTimeoutSeconds": 30
}
```

### ✅ ClickHouse Status
- **Container:** grc-clickhouse ✅ Running and healthy
- **HTTP Port:** 8123 ✅ Accessible
- **Native Port:** 9000 ✅ Accessible
- **Database:** grc_analytics ✅ Created
- **User:** grc_analytics ✅ Authenticated
- **Password:** grc_analytics_2026 ✅ Verified

---

## 🧪 Test ClickHouse Connection

### 1. Test HTTP Interface
```bash
# Test with authentication
curl -u grc_analytics:grc_analytics_2026 "http://localhost:8123/?query=SELECT%201"
# Expected: 1
```

### 2. Test Native Client
```bash
# Test native client
docker exec grc-clickhouse clickhouse-client \
  --user grc_analytics \
  --password grc_analytics_2026 \
  --query "SELECT version()"
# Expected: ClickHouse version number
```

### 3. Test from Application
After restarting the application, ClickHouse service will be enabled and can be used for analytics queries.

---

## 📋 Configuration Details

### Docker Compose Settings
```yaml
clickhouse:
  environment:
    - CLICKHOUSE_DB=grc_analytics
    - CLICKHOUSE_USER=grc_analytics
    - CLICKHOUSE_PASSWORD=grc_analytics_2026
  ports:
    - "8123:8123"  # HTTP interface
    - "9000:9000"  # Native protocol
```

### Application Settings
- **Host:** `grc-clickhouse` (container name for Docker network)
- **For localhost access:** Use `localhost` instead of `grc-clickhouse`
- **HTTP Port:** 8123
- **Database:** grc_analytics
- **Credentials:** grc_analytics / grc_analytics_2026

---

## 🔧 If Accessing from Browser

If you're accessing http://localhost:8123 directly in a browser:

1. **ClickHouse doesn't have a web UI by default**
2. **Use ClickHouse client tools:**
   - ClickHouse client: `clickhouse-client`
   - HTTP interface: `curl` with Basic Auth
   - Grafana: Configured at http://localhost:3030

3. **For web access, use Grafana:**
   - URL: http://localhost:3030
   - ClickHouse datasource is pre-configured
   - Login: admin / admin123

---

## ✅ Summary

| Item | Status |
|------|--------|
| ClickHouse Container | ✅ Running |
| ClickHouse Health | ✅ Healthy |
| HTTP Port (8123) | ✅ Accessible |
| Native Port (9000) | ✅ Accessible |
| Authentication | ✅ Working |
| Application Config | ✅ Added |
| Enabled in App | ✅ Yes |

**ClickHouse is now configured and ready to use!** 🎉

---

## 🚀 Next Steps

1. **Restart application** to load ClickHouse configuration
2. **Test analytics queries** through the application
3. **Use Grafana** for visual analytics (http://localhost:3030)
4. **Access ClickHouse** via HTTP API with proper authentication
