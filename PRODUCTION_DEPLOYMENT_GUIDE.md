# Saudi GRC Application - Production Deployment Guide

## 📋 **Pre-Deployment Checklist**

### ✅ **Build Status**
- [x] Solution builds successfully (0 errors)
- [x] All modules functional (Evidence, FrameworkLibrary, Risk, Assessment)
- [x] Release build completed
- [x] Production artifacts published
- [ ] Database migrations prepared
- [ ] Production configuration reviewed

---

## 🏗️ **System Architecture**

### **Components:**
1. **Grc.Web** - ASP.NET Core Razor Pages Web Application (73 MB)
2. **Grc.HttpApi.Host** - RESTful API Backend (75 MB)
3. **PostgreSQL Database** - Data persistence
4. **BlobStorage** - File storage for evidence documents

### **Technology Stack:**
- **.NET 8.0**
- **ABP Framework 8.3.0**
- **Entity Framework Core 8.0**
- **PostgreSQL** (via Npgsql)
- **OpenIddict** (Authentication)
- **BlobStoring** (File management)

---

## 📦 **Production Artifacts**

### **Web Application:**
```
Location: /tmp/grc-production/
Size: 73 MB
Entry Point: Grc.Web.dll
```

### **API Host:**
```
Location: /tmp/grc-api-production/
Size: 75 MB
Entry Point: Grc.HttpApi.Host.dll
```

---

## 🔧 **Environment Configuration**

### **1. appsettings.Production.json**

Create or update the following configuration files:

#### **Web Application** (`Grc.Web/appsettings.Production.json`):

```json
{
  "App": {
    "SelfUrl": "https://grc.yourdomain.com",
    "CorsOrigins": "https://grc.yourdomain.com",
    "BlobStoragePath": "/var/lib/grc/blobstorage"
  },
  "ConnectionStrings": {
    "Default": "Host=your-db-server;Port=5432;Database=GrcProduction;Username=grc_user;Password=YOUR_SECURE_PASSWORD;SSL Mode=Require;"
  },
  "Redis": {
    "Configuration": "your-redis-server:6379,password=YOUR_REDIS_PASSWORD"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

#### **API Host** (`Grc.HttpApi.Host/appsettings.Production.json`):

```json
{
  "App": {
    "SelfUrl": "https://api-grc.yourdomain.com",
    "CorsOrigins": "https://grc.yourdomain.com",
    "BlobStoragePath": "/var/lib/grc/blobstorage"
  },
  "ConnectionStrings": {
    "Default": "Host=your-db-server;Port=5432;Database=GrcProduction;Username=grc_user;Password=YOUR_SECURE_PASSWORD;SSL Mode=Require;"
  },
  "AuthServer": {
    "Authority": "https://api-grc.yourdomain.com",
    "RequireHttpsMetadata": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

---

## 🗄️ **Database Setup**

### **1. Create Database:**

```sql
-- Connect to PostgreSQL as admin
CREATE DATABASE GrcProduction;
CREATE USER grc_user WITH PASSWORD 'YOUR_SECURE_PASSWORD';
GRANT ALL PRIVILEGES ON DATABASE GrcProduction TO grc_user;
```

### **2. Run Migrations:**

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.DbMigrator

# Update connection string in appsettings.json
dotnet run
```

**Or manually:**

```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.EntityFrameworkCore

# Add migration (if not exists)
dotnet ef migrations add InitialCreate --startup-project ../Grc.Web

# Update database
dotnet ef database update --startup-project ../Grc.Web
```

---

## 🐧 **Linux Server Deployment**

### **1. Install Prerequisites:**

```bash
# Install .NET 8.0 Runtime
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0 --runtime aspnetcore

# Install PostgreSQL
sudo apt update
sudo apt install postgresql postgresql-contrib

# Install Nginx (for reverse proxy)
sudo apt install nginx

# Install SSL certificate (Let's Encrypt)
sudo apt install certbot python3-certbot-nginx
```

### **2. Create Application User:**

```bash
sudo useradd -m -s /bin/bash grcapp
sudo mkdir -p /var/www/grc/{web,api}
sudo mkdir -p /var/lib/grc/blobstorage
sudo chown -R grcapp:grcapp /var/www/grc /var/lib/grc
```

### **3. Deploy Application Files:**

```bash
# Copy Web Application
sudo cp -r /tmp/grc-production/* /var/www/grc/web/
sudo chown -R grcapp:grcapp /var/www/grc/web/

# Copy API Host
sudo cp -r /tmp/grc-api-production/* /var/www/grc/api/
sudo chown -R grcapp:grcapp /var/www/grc/api/
```

### **4. Create Systemd Services:**

#### **Web Application Service** (`/etc/systemd/system/grc-web.service`):

```ini
[Unit]
Description=Saudi GRC Web Application
After=network.target

[Service]
Type=notify
User=grcapp
Group=grcapp
WorkingDirectory=/var/www/grc/web
ExecStart=/usr/bin/dotnet /var/www/grc/web/Grc.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=grc-web
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

#### **API Host Service** (`/etc/systemd/system/grc-api.service`):

```ini
[Unit]
Description=Saudi GRC API Host
After=network.target

[Service]
Type=notify
User=grcapp
Group=grcapp
WorkingDirectory=/var/www/grc/api
ExecStart=/usr/bin/dotnet /var/www/grc/api/Grc.HttpApi.Host.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=grc-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### **5. Enable and Start Services:**

```bash
sudo systemctl daemon-reload
sudo systemctl enable grc-web grc-api
sudo systemctl start grc-web grc-api

# Check status
sudo systemctl status grc-web
sudo systemctl status grc-api

# View logs
sudo journalctl -u grc-web -f
sudo journalctl -u grc-api -f
```

---

## 🌐 **Nginx Configuration**

### **Web Application** (`/etc/nginx/sites-available/grc-web`):

```nginx
server {
    listen 80;
    server_name grc.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name grc.yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/grc.yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/grc.yourdomain.com/privkey.pem;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    client_max_body_size 100M;

    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;
        
        # Timeouts
        proxy_connect_timeout 600;
        proxy_send_timeout 600;
        proxy_read_timeout 600;
        send_timeout 600;
    }
}
```

### **API Host** (`/etc/nginx/sites-available/grc-api`):

```nginx
server {
    listen 80;
    server_name api-grc.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api-grc.yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/api-grc.yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api-grc.yourdomain.com/privkey.pem;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    client_max_body_size 100M;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### **Enable Sites:**

```bash
sudo ln -s /etc/nginx/sites-available/grc-web /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/grc-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

---

## 🔒 **SSL Certificate Setup**

```bash
# Obtain SSL certificates
sudo certbot --nginx -d grc.yourdomain.com -d api-grc.yourdomain.com

# Auto-renewal
sudo certbot renew --dry-run
```

---

## 🔐 **Security Hardening**

### **1. Firewall Configuration:**

```bash
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 5432/tcp  # PostgreSQL (restrict to specific IPs)
sudo ufw enable
```

### **2. PostgreSQL Security:**

```bash
# Edit postgresql.conf
sudo nano /etc/postgresql/*/main/postgresql.conf

# Set:
listen_addresses = 'localhost'  # Or specific IPs
ssl = on
```

### **3. Application Security:**

- [ ] Change default admin password
- [ ] Configure HTTPS only
- [ ] Enable CORS properly
- [ ] Set up rate limiting
- [ ] Configure audit logging
- [ ] Enable two-factor authentication

---

## 📊 **Monitoring & Logging**

### **1. Application Logs:**

```bash
# View logs
sudo journalctl -u grc-web -f
sudo journalctl -u grc-api -f

# Log location
/var/log/nginx/access.log
/var/log/nginx/error.log
```

### **2. Health Checks:**

```bash
# Web App
curl https://grc.yourdomain.com/health

# API
curl https://api-grc.yourdomain.com/health
```

### **3. Database Backups:**

```bash
# Create backup script
sudo nano /usr/local/bin/backup-grc-db.sh
```

```bash
#!/bin/bash
BACKUP_DIR="/var/backups/grc"
DATE=$(date +%Y%m%d_%H%M%S)
mkdir -p $BACKUP_DIR

pg_dump -U grc_user -h localhost GrcProduction | gzip > $BACKUP_DIR/grc_backup_$DATE.sql.gz

# Keep last 30 days
find $BACKUP_DIR -name "grc_backup_*.sql.gz" -mtime +30 -delete
```

```bash
chmod +x /usr/local/bin/backup-grc-db.sh

# Add to crontab
sudo crontab -e
# Add: 0 2 * * * /usr/local/bin/backup-grc-db.sh
```

---

## 🐳 **Docker Deployment (Alternative)**

### **Dockerfile.Web:**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Grc.Web/Grc.Web.csproj", "src/Grc.Web/"]
RUN dotnet restore "src/Grc.Web/Grc.Web.csproj"
COPY . .
WORKDIR "/src/src/Grc.Web"
RUN dotnet build "Grc.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Grc.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Grc.Web.dll"]
```

### **docker-compose.yml:**

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: GrcProduction
      POSTGRES_USER: grc_user
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  grc-web:
    build:
      context: .
      dockerfile: Dockerfile.Web
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__Default: "Host=postgres;Port=5432;Database=GrcProduction;Username=grc_user;Password=${DB_PASSWORD}"
    ports:
      - "5001:80"
    depends_on:
      - postgres
    volumes:
      - blob-storage:/var/lib/grc/blobstorage

  grc-api:
    build:
      context: .
      dockerfile: Dockerfile.Api
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__Default: "Host=postgres;Port=5432;Database=GrcProduction;Username=grc_user;Password=${DB_PASSWORD}"
    ports:
      - "5000:80"
    depends_on:
      - postgres
    volumes:
      - blob-storage:/var/lib/grc/blobstorage

volumes:
  postgres-data:
  blob-storage:
```

### **Deploy with Docker:**

```bash
docker-compose up -d
```

---

## ✅ **Post-Deployment Verification**

### **1. Test All Modules:**

```bash
curl https://grc.yourdomain.com
curl https://grc.yourdomain.com/Evidence
curl https://grc.yourdomain.com/FrameworkLibrary
curl https://grc.yourdomain.com/Risks
curl https://grc.yourdomain.com/Assessments
curl https://api-grc.yourdomain.com/api/app/evidence
```

### **2. Performance Testing:**

```bash
# Install Apache Bench
sudo apt install apache2-utils

# Test load
ab -n 1000 -c 10 https://grc.yourdomain.com/
```

### **3. Security Scan:**

```bash
# SSL Test
https://www.ssllabs.com/ssltest/analyze.html?d=grc.yourdomain.com

# Security headers
curl -I https://grc.yourdomain.com
```

---

## 🚨 **Troubleshooting**

### **Common Issues:**

1. **Application won't start:**
   ```bash
   sudo journalctl -u grc-web -n 50
   dotnet /var/www/grc/web/Grc.Web.dll  # Test manually
   ```

2. **Database connection failed:**
   ```bash
   psql -h localhost -U grc_user -d GrcProduction
   # Check connection string in appsettings.Production.json
   ```

3. **Permission denied:**
   ```bash
   sudo chown -R grcapp:grcapp /var/www/grc /var/lib/grc
   sudo chmod -R 755 /var/www/grc
   ```

4. **502 Bad Gateway:**
   ```bash
   sudo systemctl status grc-web
   sudo netstat -tulpn | grep :5001
   ```

---

## 📞 **Support & Maintenance**

### **Regular Maintenance Tasks:**

- [ ] Daily: Check application logs
- [ ] Daily: Verify database backups
- [ ] Weekly: Review security logs
- [ ] Monthly: Update SSL certificates (auto)
- [ ] Monthly: Review and optimize database
- [ ] Quarterly: Update dependencies
- [ ] Annually: Security audit

### **Emergency Contacts:**

- **Development Team:** dev@yourdomain.com
- **Database Admin:** dba@yourdomain.com
- **DevOps:** devops@yourdomain.com

---

## 📝 **Rollback Procedure**

```bash
# Stop services
sudo systemctl stop grc-web grc-api

# Restore previous version
sudo cp -r /var/www/grc/web.backup/* /var/www/grc/web/
sudo cp -r /var/www/grc/api.backup/* /var/www/grc/api/

# Restore database
gunzip < /var/backups/grc/grc_backup_YYYYMMDD_HHMMSS.sql.gz | psql -U grc_user GrcProduction

# Start services
sudo systemctl start grc-web grc-api
```

---

## 🎉 **Deployment Complete!**

Your Saudi GRC Application is now deployed to production.

**Access URLs:**
- Web Application: https://grc.yourdomain.com
- API: https://api-grc.yourdomain.com
- Swagger/API Docs: https://api-grc.yourdomain.com/swagger

**Default Admin Credentials:**
- Username: admin
- Password: 1q2w3E* (⚠️ **CHANGE IMMEDIATELY**)

---

**Last Updated:** December 21, 2025  
**Version:** 1.0.0  
**Deployment Status:** ✅ Ready for Production



