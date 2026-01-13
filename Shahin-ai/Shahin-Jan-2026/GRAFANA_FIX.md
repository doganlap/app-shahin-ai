# Grafana Port Mount Fix

## Problem
**Error:** Grafana not mounted at any port
**Status:** Container exited with database authentication error

## Root Cause
1. Grafana was trying to connect to database host `db` but container name is `grc-db`
2. Database password authentication failed
3. Container exited before port could be exposed

## Solution Applied

### ✅ Fixed Database Host
Changed in `docker-compose.yml`:
- **Before:** `GF_DATABASE_HOST=db`
- **After:** `GF_DATABASE_HOST=grc-db`

### ✅ Container Restarted
- Removed old container
- Recreated with correct database host
- Port mapping: `3030:3000`

---

## 📊 Grafana Configuration

### Port Mapping
- **Host Port:** 3030
- **Container Port:** 3000
- **URL:** http://localhost:3030

### Login Credentials
- **Username:** `admin`
- **Password:** `admin123`

### Database Connection
- **Type:** PostgreSQL
- **Host:** `grc-db`
- **Database:** `grafana`
- **User:** `postgres`
- **Password:** `postgres`

---

## 🧪 Verify Grafana

### 1. Check Container Status
```bash
docker ps | grep grafana
# Expected: grc-grafana Up (healthy)
```

### 2. Check Port
```bash
netstat -tlnp | grep 3030
# Expected: 0.0.0.0:3030
```

### 3. Access Grafana
- **URL:** http://localhost:3030
- **Login:** admin / admin123

---

## ✅ Expected Status

| Item | Status |
|------|--------|
| Container | ✅ Running |
| Port 3030 | ✅ Exposed |
| Database Connection | ✅ Fixed (grc-db) |
| Web UI | ✅ Accessible |

---

## 🚀 Next Steps

1. **Wait for Grafana to start** (30-60 seconds)
2. **Access Grafana:** http://localhost:3030
3. **Login with:** admin / admin123
4. **Check datasources:**
   - ClickHouse (pre-configured)
   - PostgreSQL (pre-configured)

**Grafana should now be accessible on port 3030!** 🎉
