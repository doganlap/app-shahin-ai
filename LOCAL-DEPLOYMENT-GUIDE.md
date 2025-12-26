# 🖥️ Local Server Deployment Guide
## Deploy on This Server Using Railway Databases

## Architecture

```
┌─────────────────────────────────────────────────────┐
│  This Server (Linux)                                │
│  /root/app.shahin-ai.com                            │
├─────────────────────────────────────────────────────┤
│                                                     │
│  🌐 Nginx (Port 80)                                 │
│      └─→ Web App (Angular)                          │
│      └─→ Proxy to API                               │
│                                                     │
│  🚀 GRC API (Port 5000)                             │
│      ├─→ Railway PostgreSQL (mainline:46662) ✅     │
│      ├─→ Railway Redis (caboose:26002) ✅           │
│      └─→ Railway S3 Storage ✅                      │
│                                                     │
└─────────────────────────────────────────────────────┘
```

**Benefits**:
- ✅ Application runs on your server (full control)
- ✅ Databases on Railway (managed, backed up)
- ✅ No database management overhead
- ✅ Free local compute, pay only for Railway databases

---

## 🚀 Quick Deployment (1 Command)

```bash
cd /root/app.shahin-ai.com/Shahin-ai
sudo ./deploy-on-this-server.sh
```

This will:
1. Create deployment directory `/var/www/grc-platform`
2. Build .NET API (if project exists)
3. Build Angular frontend
4. Create systemd service for API
5. Configure nginx
6. Start all services

---

## 📋 Step-by-Step Deployment

### Prerequisites Check
```bash
# Check required tools
dotnet --version    # Should be 8.0+
node --version      # Should be 18+
nginx -v           # Should be installed
```

### Install Missing Tools (if needed)
```bash
# Install nginx
sudo apt update && sudo apt install -y nginx

# Ensure .NET 8 SDK
# Already installed: 8.0.122 ✅

# Ensure Node.js 18+
# Already installed: v20.19.6 ✅
```

### Step 1: Deploy
```bash
cd /root/app.shahin-ai.com/Shahin-ai
sudo ./deploy-on-this-server.sh
```

### Step 2: Run Database Migrations
```bash
cd /var/www/grc-platform/api

# Run migrations (connects to Railway PostgreSQL)
dotnet ef database update
```

### Step 3: Seed Default Products
```bash
cd /var/www/grc-platform/api

# Seed products (Trial, Standard, Professional, Enterprise)
dotnet Grc.HttpApi.Host.dll --seed
```

### Step 4: Verify Deployment
```bash
# Check API health
curl http://localhost:5000/health

# Check services
sudo systemctl status grc-api
sudo systemctl status nginx

# Test web app
curl http://localhost
```

---

## 🔧 Manual Start/Stop

### Start Services
```bash
./start-local-production.sh
```

### Stop Services
```bash
./stop-local-production.sh
```

### Restart API Only
```bash
sudo systemctl restart grc-api
```

---

## 📊 Service Status

### Check Running Services
```bash
# API service
sudo systemctl status grc-api

# Nginx
sudo systemctl status nginx

# Check ports
sudo netstat -tuln | grep -E ':(80|5000|8080)'
```

### View Logs
```bash
# API logs (realtime)
sudo journalctl -u grc-api -f

# Nginx access logs
sudo tail -f /var/log/nginx/access.log

# Nginx error logs
sudo tail -f /var/log/nginx/error.log
```

---

## 🌐 Access URLs

After deployment, access your application:

| Service | URL | Description |
|---------|-----|-------------|
| **Web App** | `http://localhost` | Main application (via nginx) |
| **Web App** | `http://YOUR_SERVER_IP` | From other machines |
| **API** | `http://localhost:5000` | API directly |
| **Swagger** | `http://localhost:5000/swagger` | API documentation |
| **Health Check** | `http://localhost:5000/health` | Health endpoint |

---

## 🗄️ Database Connections

All data is stored on Railway (managed services):

### PostgreSQL (Main Database)
```
Host: mainline.proxy.rlwy.net:46662
Database: railway
User: postgres
Password: sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ
SSL: Required
```

### Redis (Cache)
```
Host: caboose.proxy.rlwy.net:26002
Password: ySTCqQpbNuYVFfJwIIIeqkRgkTvIrslB
SSL: Yes
```

### S3 Storage (Evidence Files)
```
Endpoint: https://storage.railway.app
Bucket: optimized-bin-yvjb9vxnhq1
Access Key: tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD
```

---

## 🧪 Testing After Deployment

### 1. API Health Check
```bash
curl http://localhost:5000/health
```

Expected: `{"status":"Healthy"}`

### 2. Test Products API
```bash
curl http://localhost:5000/api/grc/products
```

### 3. Access Web App
Open browser: `http://localhost` or `http://YOUR_SERVER_IP`

### 4. Check Database Connection
```bash
# Connect to Railway PostgreSQL
psql postgresql://postgres:sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ@mainline.proxy.rlwy.net:46662/railway

# List tables
\dt

# Check products
SELECT * FROM grc.products;
```

---

## 🔒 Firewall Configuration (if needed)

```bash
# Allow HTTP
sudo ufw allow 80/tcp

# Allow HTTPS (for future)
sudo ufw allow 443/tcp

# Allow API port (if accessing directly)
sudo ufw allow 5000/tcp

# Enable firewall
sudo ufw enable
```

---

## 🔄 Update Deployment

To update after code changes:

```bash
cd /root/app.shahin-ai.com/Shahin-ai

# Rebuild
sudo ./deploy-on-this-server.sh

# Restart
sudo systemctl restart grc-api
```

---

## 🆘 Troubleshooting

### API Won't Start
```bash
# Check logs
sudo journalctl -u grc-api -n 100

# Check configuration
cat /var/www/grc-platform/api/appsettings.Production.json

# Test Railway database connection
psql postgresql://postgres:sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ@mainline.proxy.rlwy.net:46662/railway -c "SELECT 1;"
```

### Web App 404 Errors
```bash
# Check nginx configuration
sudo nginx -t

# Check web files exist
ls -la /var/www/grc-platform/web/

# Restart nginx
sudo systemctl restart nginx
```

### Database Connection Errors
```bash
# Test PostgreSQL connection
telnet mainline.proxy.rlwy.net 46662

# Test Redis connection
redis-cli -h caboose.proxy.rlwy.net -p 26002 -a ySTCqQpbNuYVFfJwIIIeqkRgkTvIrslB --tls PING
```

---

## 📊 Monitoring

### System Resources
```bash
# CPU and Memory
htop

# Disk usage
df -h

# Check API process
ps aux | grep Grc.HttpApi.Host
```

### Application Metrics
```bash
# API logs
sudo journalctl -u grc-api --since "1 hour ago"

# Nginx access logs
sudo tail -100 /var/log/nginx/access.log

# Count API requests
sudo grep "GET /api/" /var/log/nginx/access.log | wc -l
```

---

## ✨ Summary

**Deployment Location**: `/var/www/grc-platform`  
**Application**: Runs on this server  
**Databases**: Railway managed services  
**Web Server**: Nginx (port 80)  
**API Server**: .NET 8 (port 5000)  

**Start**: `./start-local-production.sh`  
**Stop**: `./stop-local-production.sh`  
**Deploy**: `./deploy-on-this-server.sh`  

**Configuration**: `local-production-config.json`

---

## 🎯 Next Steps After Deployment

1. Run migrations: `cd /var/www/grc-platform/api && dotnet ef database update`
2. Seed data: `dotnet Grc.HttpApi.Host.dll --seed`
3. Test: `curl http://localhost:5000/health`
4. Access: `http://localhost` in browser
5. Monitor: `sudo journalctl -u grc-api -f`

---

**Ready to deploy!** Run: `sudo ./deploy-on-this-server.sh`

