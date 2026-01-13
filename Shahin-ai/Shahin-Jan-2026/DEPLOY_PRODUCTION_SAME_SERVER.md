# Production Deployment Guide - Same Server (localhost)

## 🎯 Quick Summary

Deploy GRC system to production **on the same server** where you are now:
- **Server**: shahin-ai (localhost / 46.224.68.73)
- **User**: root
- **Path**: /home/Shahin-ai/Shahin-Jan-2026
- **Docker**: ✅ Installed (version 28.2.2)
- **Docker Compose**: ✅ Installed (version 1.29.2)

---

## 🚀 Quick Deployment (5 Steps)

### Step 1: Navigate to Project
```bash
cd /home/Shahin-ai/Shahin-Jan-2026
```

### Step 2: Build Application
```bash
cd src/GrcMvc
dotnet build -c Release
cd ../..
```

### Step 3: Start with Docker Compose
```bash
docker-compose -f docker-compose.grcmvc.yml up -d --build
```

### Step 4: Verify Status
```bash
docker-compose -f docker-compose.grcmvc.yml ps
docker-compose -f docker-compose.grcmvc.yml logs -f grcmvc
```

### Step 5: Access Application
- **URL**: http://localhost:5137
- **Health**: http://localhost:5137/health
- **Login**: Info@doganconsult.com / AhmEma$$123456

---

## 📋 Detailed Deployment Steps

### Prerequisites Check

```bash
# Verify Docker
docker --version
docker compose version

# Check disk space
df -h

# Check available memory
free -h

# Verify .NET SDK (if building locally)
dotnet --version
```

### Option 1: Docker Compose Deployment (Recommended)

```bash
# 1. Navigate to project root
cd /home/Shahin-ai/Shahin-Jan-2026

# 2. Stop any existing containers
docker-compose -f docker-compose.grcmvc.yml down

# 3. Build and start all services
docker-compose -f docker-compose.grcmvc.yml up -d --build

# 4. Check logs (wait 30 seconds for startup)
sleep 30
docker-compose -f docker-compose.grcmvc.yml logs --tail=50 grcmvc

# 5. Verify health endpoint
curl http://localhost:5137/health
```

### Option 2: Direct .NET Deployment (Without Docker)

```bash
# 1. Navigate to application
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc

# 2. Restore dependencies
dotnet restore

# 3. Build release
dotnet build -c Release

# 4. Publish
dotnet publish -c Release -o ./publish

# 5. Set environment variables
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:8080"
export ConnectionStrings__DefaultConnection="Host=localhost;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"

# 6. Run
cd publish
dotnet GrcMvc.dll
```

---

## 🔧 Configuration

### Environment Variables

Create `.env` file or set environment variables:

```bash
# Production Environment
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://+:5137"

# Database Connection
export ConnectionStrings__DefaultConnection="Host=db;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"

# JWT Settings
export JwtSettings__Secret="ProdSecretKeyMustBeVeryLongAndSecureForProductionUse_2026_Portal!"
export JwtSettings__Issuer="https://portal.shahin-ai.com"
export JwtSettings__Audience="https://portal.shahin-ai.com"

# Allowed Hosts
export AllowedHosts="localhost;46.224.68.73;portal.shahin-ai.com;app.shahin-ai.com"
```

### appsettings.Production.json

Update `/home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc/appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GrcMvcDb;Username=postgres;Password=YOUR_SECURE_PASSWORD;Port=5432"
  },
  "JwtSettings": {
    "Secret": "YOUR_VERY_LONG_SECURE_SECRET_KEY_HERE_MINIMUM_32_CHARS",
    "Issuer": "https://portal.shahin-ai.com",
    "Audience": "https://portal.shahin-ai.com",
    "ExpiryMinutes": 60
  },
  "AllowedHosts": "*"
}
```

---

## 🔐 Security Hardening

### 1. Change Default Passwords

```bash
# Change database password
docker exec -it grcmvc-db psql -U postgres -c "ALTER USER postgres WITH PASSWORD 'YOUR_SECURE_PASSWORD';"

# Update docker-compose.grcmvc.yml with new password
```

### 2. Secure JWT Secret

Generate strong secret:
```bash
openssl rand -base64 64
```

### 3. Firewall Setup

```bash
# Allow HTTP/HTTPS
ufw allow 80/tcp
ufw allow 443/tcp

# Allow application port (if exposing directly)
ufw allow 5137/tcp

# Enable firewall
ufw enable
ufw status
```

### 4. SSL/TLS Setup (Optional - for HTTPS)

If you want HTTPS on localhost:

```bash
# Generate self-signed certificate
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet dev-certs https --trust

# Or use Let's Encrypt (requires domain pointing to server)
sudo certbot --nginx -d portal.shahin-ai.com
```

---

## 📊 Monitoring & Maintenance

### Check Application Status

```bash
# Container status
docker-compose -f docker-compose.grcmvc.yml ps

# Application logs
docker-compose -f docker-compose.grcmvc.yml logs -f grcmvc

# Database logs
docker-compose -f docker-compose.grcmvc.yml logs -f db

# Health check
curl http://localhost:5137/health

# Resource usage
docker stats
```

### Database Backups

```bash
# Manual backup
docker exec grcmvc-db pg_dump -U postgres GrcMvcDb > backup_$(date +%Y%m%d_%H%M%S).sql

# Automated backup script
cat > /home/Shahin-ai/backup-db.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/home/Shahin-ai/backups"
mkdir -p $BACKUP_DIR
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
docker exec grcmvc-db pg_dump -U postgres GrcMvcDb | gzip > $BACKUP_DIR/grc_$TIMESTAMP.sql.gz
find $BACKUP_DIR -name "grc_*.sql.gz" -mtime +30 -delete
EOF

chmod +x /home/Shahin-ai/backup-db.sh

# Schedule daily backup (2 AM)
(crontab -l 2>/dev/null; echo "0 2 * * * /home/Shahin-ai/backup-db.sh >> /var/log/grc-backup.log 2>&1") | crontab -
```

### Logs Location

```bash
# Application logs (inside container)
docker exec grcmvc-app ls -la /app/logs

# Copy logs to host
docker cp grcmvc-app:/app/logs /home/Shahin-ai/grc-logs

# View real-time logs
docker-compose -f docker-compose.grcmvc.yml logs -f --tail=100
```

---

## 🔄 Updates & Maintenance

### Update Application

```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Pull latest code
git pull origin main

# Rebuild and restart
docker-compose -f docker-compose.grcmvc.yml down
docker-compose -f docker-compose.grcmvc.yml up -d --build

# Verify
docker-compose -f docker-compose.grcmvc.yml ps
curl http://localhost:5137/health
```

### Database Migrations

```bash
# Run migrations (if using EF Core)
cd /home/Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet ef database update --connection "Host=localhost;Database=GrcMvcDb;Username=postgres;Password=postgres;Port=5432"
```

### Restart Services

```bash
# Restart all services
docker-compose -f docker-compose.grcmvc.yml restart

# Restart specific service
docker-compose -f docker-compose.grcmvc.yml restart grcmvc

# Stop all services
docker-compose -f docker-compose.grcmvc.yml down

# Stop and remove volumes (⚠️ DANGER: deletes data)
docker-compose -f docker-compose.grcmvc.yml down -v
```

---

## 🌐 Expose to Internet (Optional)

### Using Nginx Reverse Proxy

If you want to expose the application publicly:

```bash
# Install Nginx
apt update
apt install -y nginx

# Create Nginx config
cat > /etc/nginx/sites-available/grc-app << 'EOF'
server {
    listen 80;
    server_name portal.shahin-ai.com;

    location / {
        proxy_pass http://localhost:5137;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
EOF

# Enable site
ln -s /etc/nginx/sites-available/grc-app /etc/nginx/sites-enabled/
nginx -t
systemctl reload nginx

# Setup SSL with Let's Encrypt
certbot --nginx -d portal.shahin-ai.com
```

---

## ✅ Verification Checklist

After deployment, verify:

- [ ] Containers are running: `docker-compose ps`
- [ ] Application responds: `curl http://localhost:5137/health`
- [ ] Can access web interface: http://localhost:5137
- [ ] Can login with admin credentials
- [ ] Database is accessible and has data
- [ ] Logs show no critical errors
- [ ] Health endpoint returns 200 OK
- [ ] Permissions are seeded (check ApplicationInitializer logs)
- [ ] Policy file loads successfully (check PolicyStore logs)

---

## 🆘 Troubleshooting

### Application Won't Start

```bash
# Check logs
docker-compose -f docker-compose.grcmvc.yml logs grcmvc

# Check if port is in use
netstat -tulpn | grep 5137

# Restart containers
docker-compose -f docker-compose.grcmvc.yml restart
```

### Database Connection Issues

```bash
# Check database is running
docker-compose -f docker-compose.grcmvc.yml ps db

# Test connection
docker exec -it grcmvc-db psql -U postgres -d GrcMvcDb -c "SELECT 1"

# Check connection string in logs
docker-compose -f docker-compose.grcmvc.yml logs grcmvc | grep -i connection
```

### Port Already in Use

```bash
# Find process using port
lsof -i :5137

# Kill process (if needed)
kill -9 <PID>

# Or change port in docker-compose.grcmvc.yml
```

### Out of Memory

```bash
# Check memory usage
free -h
docker stats

# Increase Docker memory limits or add swap
```

---

## 📞 Quick Reference

**Current Server**: shahin-ai (localhost)
**Application URL**: http://localhost:5137
**Health Check**: http://localhost:5137/health
**Database**: localhost:5432 (inside Docker network: `db:5432`)
**Login**: Info@doganconsult.com / AhmEma$$123456

**Useful Commands**:
```bash
# Start
docker-compose -f docker-compose.grcmvc.yml up -d

# Stop
docker-compose -f docker-compose.grcmvc.yml down

# Logs
docker-compose -f docker-compose.grcmvc.yml logs -f

# Status
docker-compose -f docker-compose.grcmvc.yml ps

# Restart
docker-compose -f docker-compose.grcmvc.yml restart
```

---

**Last Updated**: 2025-01-22
**Status**: Ready for deployment
**Location**: /home/Shahin-ai/Shahin-Jan-2026
