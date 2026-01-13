# Container Port Conflict Analysis

**Date**: 2025-01-22  
**Issue**: Two applications running on ports 5000 and 5137

---

## 🔍 Current Situation

### Port 5000 - Direct .NET Process (NOT Docker)

**Status**: ✅ **RUNNING**  
**Type**: Direct .NET application (not in Docker container)  
**Process**: `dotnet /var/www/grc-published/GrcMvc.dll`  
**PID**: 981822  
**User**: www-data  
**Port Binding**: `127.0.0.1:5000` (localhost only)

**Location**: `/var/www/grc-published/`

**Configuration**:
- From `deploy/docker-compose.yml` mapping: `5000:8080`
- This appears to be a published .NET app running directly via systemd/service

---

### Port 5137 - Docker Container (Expected)

**Status**: ❓ **NOT RUNNING** (Expected to be running)  
**Type**: Docker container  
**Container**: `grcmvc-app`  
**Configuration**: `5137:80` mapping in `docker-compose.grcmvc.yml`

**Expected**:
- Container should be running from `docker-compose.grcmvc.yml`
- Should expose port 5137

---

## 📊 Summary

| Port | Type | Status | Process/Container | Access |
|------|------|--------|-------------------|--------|
| **5000** | Direct .NET | ✅ Running | PID 981822 (www-data) | localhost only |
| **5137** | Docker | ❓ Not Running | grcmvc-app | Should be accessible |

---

## 🚨 Issues Found

1. **Port 5137 container is not running**
   - Expected: `grcmvc-app` container from `docker-compose.grcmvc.yml`
   - Status: Container not found in `docker ps`

2. **Two different deployment methods**
   - Port 5000: Direct .NET process (old method?)
   - Port 5137: Docker container (new method?)

3. **Possible conflict**
   - Both trying to run the same application
   - Different deployment methods may cause confusion

---

## 🔧 Recommendations

### Option 1: Use Docker Container Only (Recommended)

**Stop direct .NET process and use Docker**:
```bash
# Stop direct .NET process
sudo systemctl stop grcmvc  # or whatever service name
# OR
sudo kill 981822

# Start Docker container
cd /home/Shahin-ai/Shahin-Jan-2026
docker-compose -f docker-compose.grcmvc.yml up -d

# Verify
curl http://localhost:5137/health
```

**Benefits**:
- ✅ Consistent deployment method
- ✅ Easy to manage via docker-compose
- ✅ Better isolation
- ✅ Easy to update/restart

---

### Option 2: Use Direct .NET Process Only

**Stop Docker and keep direct process**:
```bash
# Stop Docker containers
cd /home/Shahin-ai/Shahin-Jan-2026
docker-compose -f docker-compose.grcmvc.yml down

# Keep direct .NET process running on port 5000
# Access via: http://localhost:5000
```

**Benefits**:
- ✅ Simpler (no Docker overhead)
- ✅ Already running and working
- ✅ Systemd managed (auto-restart)

---

### Option 3: Use Both (Different Purposes)

**If both are needed**:
- Port 5000: Development/staging
- Port 5137: Production

**Configure Nginx to route**:
```nginx
# Staging
server {
    listen 80;
    server_name staging.shahin-ai.com;
    proxy_pass http://localhost:5000;
}

# Production
server {
    listen 80;
    server_name portal.shahin-ai.com;
    proxy_pass http://localhost:5137;
}
```

---

## 📋 Action Items

### Immediate Actions

1. ✅ **Check which one you want to keep**
   - Docker container (port 5137) - recommended
   - Direct process (port 5000) - simpler

2. ✅ **Verify port 5137 container status**
   ```bash
   docker-compose -f docker-compose.grcmvc.yml ps
   ```

3. ✅ **Check if direct process is managed by systemd**
   ```bash
   systemctl status grcmvc
   # or
   systemctl list-units | grep grc
   ```

4. ✅ **Decide on single deployment method**

---

## 🔍 Investigation Needed

### Check Direct .NET Process Service

```bash
# Find what service manages the direct process
systemctl list-units --all | grep -i grc
systemctl list-units --all | grep -i dotnet

# Check process details
ps aux | grep 981822
systemctl status <service-name>
```

### Check Docker Container

```bash
# Check if container exists but stopped
docker ps -a | grep grcmvc

# Check logs if exists
docker logs grcmvc-app

# Start if exists but stopped
docker-compose -f docker-compose.grcmvc.yml up -d
```

---

## 🎯 Recommended Solution

**Use Docker container only** (Port 5137):

1. Stop direct .NET process
2. Ensure Docker container is running
3. Configure Nginx to proxy to port 5137
4. Remove or disable direct .NET service

**Benefits**:
- ✅ Consistent with other services (postgres, redis in Docker)
- ✅ Easy deployment and updates
- ✅ Better resource management
- ✅ Easy rollback

---

## 📝 Next Steps

1. **Determine which deployment method you prefer**
2. **Stop the one you don't need**
3. **Verify the chosen one is working**
4. **Update Nginx configuration if needed**
5. **Document the chosen deployment method**

---

**Last Updated**: 2025-01-22  
**Status**: Needs decision on deployment method  
**Priority**: Medium (both can run, but causes confusion)
