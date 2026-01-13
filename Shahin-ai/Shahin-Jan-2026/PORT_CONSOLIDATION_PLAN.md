# Port Consolidation Plan - Docker Only

**Date**: 2025-01-22  
**Goal**: Run everything through Docker only, reduce exposed ports

---

## 🔍 Current Ports (Too Many!)

### Docker Containers
- **80/443**: Nginx (shahin-nginx) - ✅ Keep (main entry point)
- **3000**: Grafana - ❓ Remove if not needed
- **8080**: Zabbix Web - ❓ Remove if not needed
- **9090**: Prometheus - ❓ Remove if not needed
- **10050/10051**: Zabbix Agent/Server - ❓ Remove if not needed
- **19999**: Netdata - ❓ Remove if not needed
- **3306**: MySQL (Zabbix) - ❓ Remove if not needed
- **5432**: PostgreSQL (shahin-postgres) - ⚠️ Expose only internally
- **6379**: Redis (shahin-redis) - ⚠️ Expose only internally

### Direct Processes (NOT Docker)
- **5000**: Direct .NET process - ❌ **REMOVE** (use Docker instead)
- **5137**: Docker container (not running) - ✅ Start this instead

---

## ✅ Action Plan: Docker Only

### Step 1: Stop Direct .NET Process on Port 5000
```bash
# Stop the direct process
sudo kill 981822

# Verify stopped
netstat -tulpn | grep 5000  # Should show nothing
```

### Step 2: Start Docker Container on Port 5137
```bash
cd /home/Shahin-ai/Shahin-Jan-2026
docker-compose -f docker-compose.grcmvc.yml up -d

# Verify running
docker ps | grep grcmvc
curl http://localhost:5137/health
```

### Step 3: Secure Internal Services
**Remove public exposure for internal services**:

```yaml
# docker-compose.yml - Change these:
services:
  postgres:
    ports:
      - "5432:5432"  # ❌ Remove this
      # ✅ Only accessible within Docker network
  
  redis:
    ports:
      - "6379:6379"  # ❌ Remove this
      # ✅ Only accessible within Docker network
```

**Only expose what users need**:
- ✅ **Port 80/443**: Nginx (public entry point)
- ✅ **Port 5137**: GRC Application (internal or via Nginx proxy)
- ❌ **Remove monitoring ports** (3000, 8080, 9090, etc.) if not needed
- ❌ **Remove database ports** (5432, 6379) from public exposure

### Step 4: Configure Nginx to Proxy to Docker
```nginx
# /etc/nginx/sites-available/shahin-ai
server {
    listen 80;
    server_name portal.shahin-ai.com;
    
    location / {
        proxy_pass http://localhost:5137;  # Docker container
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

---

## 🎯 Final Port Configuration

### Public Ports (Exposed)
- **80/443**: Nginx only (all traffic goes through this)

### Internal Ports (Docker Network Only)
- **5137**: GRC Application (accessed via Nginx or localhost)
- **5432**: PostgreSQL (internal Docker network only)
- **6379**: Redis (internal Docker network only)
- **5001**: shahin-grc-app (internal only)

### Optional Monitoring (Remove if Not Needed)
- **3000**: Grafana - Remove if not using
- **8080**: Zabbix - Remove if not using
- **9090**: Prometheus - Remove if not using
- **19999**: Netdata - Remove if not using

---

## 📋 Steps to Consolidate

1. ✅ Stop direct .NET process (port 5000)
2. ✅ Start Docker container (port 5137)
3. ✅ Remove public exposure of PostgreSQL (port 5432)
4. ✅ Remove public exposure of Redis (port 6379)
5. ✅ Configure Nginx to proxy to Docker container
6. ✅ Remove unused monitoring ports (if not needed)

---

## 🔒 Security Benefits

After consolidation:
- ✅ **Single entry point**: Only port 80/443 exposed
- ✅ **Internal services hidden**: Databases not accessible from outside
- ✅ **Docker network isolation**: Services communicate internally
- ✅ **Easier firewall rules**: Only allow 80/443
- ✅ **Better security**: No direct database access from internet

---

## 📊 Before vs After

### Before (Current - Too Many Ports)
```
Public: 80, 443, 3000, 3306, 5432, 6379, 8080, 9090, 10050, 10051, 19999, 5000
Internal Docker: 5137 (not running)
```

### After (Docker Only - Minimal Ports)
```
Public: 80, 443 only
Internal Docker: 5137, 5432, 6379 (not exposed)
All traffic → Nginx → Docker containers
```

---

## ✅ Verification Checklist

- [ ] Direct .NET process stopped (port 5000 closed)
- [ ] Docker container running (port 5137)
- [ ] PostgreSQL not publicly exposed
- [ ] Redis not publicly exposed
- [ ] Nginx configured to proxy to Docker
- [ ] Monitoring ports removed (if not needed)
- [ ] Application accessible via Nginx only

---

**Last Updated**: 2025-01-22  
**Status**: Ready to execute  
**Estimated Time**: 10-15 minutes
