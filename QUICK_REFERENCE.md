# 🚀 Quick Deployment Reference Card

## ⚡ Quick Commands

### Deploy Everything (After DNS configured):
```bash
cd /root/app.shahin-ai.com/Shahin-ai
sudo ./setup-ssl.sh && sudo ./deploy-production.sh
```

### Check Status:
```bash
sudo systemctl status grc-web grc-api nginx
curl -I https://grc.shahin-ai.com
curl -I https://api-grc.shahin-ai.com
```

### View Logs:
```bash
sudo journalctl -u grc-web -f
sudo journalctl -u grc-api -f
```

---

## 🔑 Production Credentials

### Database (Railway PostgreSQL):
```
Host: mainline.proxy.rlwy.net
Port: 46662
Database: railway
Username: postgres
Password: sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ
SSL: Required
```

### Default Admin:
```
Username: admin
Password: 1q2w3E*
⚠️ CHANGE IMMEDIATELY AFTER FIRST LOGIN
```

### URLs:
```
Web: https://grc.shahin-ai.com
API: https://api-grc.shahin-ai.com
Swagger: https://api-grc.shahin-ai.com/swagger
```

---

## 📋 DNS Configuration

Add these A records to your DNS:

| Record | Value | TTL |
|--------|-------|-----|
| grc.shahin-ai.com | YOUR_SERVER_IP | 300 |
| api-grc.shahin-ai.com | YOUR_SERVER_IP | 300 |

Verify:
```bash
host grc.shahin-ai.com
host api-grc.shahin-ai.com
```

---

## 🛠️ Service Management

```bash
# Start
sudo systemctl start grc-web grc-api

# Stop
sudo systemctl stop grc-web grc-api

# Restart
sudo systemctl restart grc-web grc-api

# Status
sudo systemctl status grc-web grc-api

# Logs (real-time)
sudo journalctl -u grc-web -f
sudo journalctl -u grc-api -f

# Logs (last 100 lines)
sudo journalctl -u grc-web -n 100
sudo journalctl -u grc-api -n 100
```

---

## 🗄️ Database Commands

```bash
# Connect to database
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres -d railway

# List tables
\dt

# Check GRC tables
SELECT tablename FROM pg_tables WHERE tablename LIKE '%grc_%';

# Exit
\q
```

---

## 📁 Important Paths

```
Application:     /var/www/grc/web
                 /var/www/grc/api

Logs:            /var/log/grc/

SSL Certs:       /etc/letsencrypt/live/grc.shahin-ai.com/

Systemd Units:   /etc/systemd/system/grc-web.service
                 /etc/systemd/system/grc-api.service

Nginx Config:    /etc/nginx/sites-available/grc-web
                 /etc/nginx/sites-available/grc-api

Backups:         /var/backups/grc/
```

---

## 🔧 Troubleshooting

### Application won't start:
```bash
# Check logs
sudo journalctl -u grc-web -n 50
sudo journalctl -u grc-api -n 50

# Test manually
cd /var/www/grc/web
dotnet Grc.Web.dll
```

### Database connection failed:
```bash
# Test connection
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres -d railway -c "SELECT 1;"

# Check connection string in:
cat /var/www/grc/web/appsettings.Production.json
```

### SSL certificate issues:
```bash
# Check certificate status
sudo certbot certificates

# Test renewal
sudo certbot renew --dry-run

# Force renewal
sudo certbot renew --force-renewal
```

### Nginx issues:
```bash
# Test config
sudo nginx -t

# Reload
sudo systemctl reload nginx

# Restart
sudo systemctl restart nginx

# Check logs
sudo tail -f /var/log/nginx/error.log
```

---

## 📊 Monitoring

### Check if services are listening:
```bash
sudo netstat -tulpn | grep -E ':(5000|5001|80|443)'
```

### Check application health:
```bash
curl https://grc.shahin-ai.com/health
curl https://api-grc.shahin-ai.com/health
```

### Check SSL:
```bash
openssl s_client -connect grc.shahin-ai.com:443 -servername grc.shahin-ai.com
```

### Test endpoints:
```bash
curl -I https://grc.shahin-ai.com
curl -I https://grc.shahin-ai.com/Evidence
curl -I https://grc.shahin-ai.com/FrameworkLibrary
curl -I https://grc.shahin-ai.com/Risks
curl -I https://api-grc.shahin-ai.com/api/app/evidence
```

---

## 🔄 Update/Redeploy

```bash
cd /root/app.shahin-ai.com/Shahin-ai

# Stop services
sudo systemctl stop grc-web grc-api

# Rebuild
cd aspnet-core
dotnet build --configuration Release
dotnet publish src/Grc.Web/Grc.Web.csproj -c Release -o /var/www/grc/web
dotnet publish src/Grc.HttpApi.Host/Grc.HttpApi.Host.csproj -c Release -o /var/www/grc/api

# Start services
sudo systemctl start grc-web grc-api
```

---

## 💾 Backup

### Manual Database Backup:
```bash
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
pg_dump -h mainline.proxy.rlwy.net -p 46662 -U postgres railway > grc_backup_$(date +%Y%m%d).sql
```

### Restore:
```bash
export PGPASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"
psql -h mainline.proxy.rlwy.net -p 46662 -U postgres railway < grc_backup_YYYYMMDD.sql
```

---

## 📖 Documentation Files

All in: `/root/app.shahin-ai.com/Shahin-ai/`

- **PRODUCTION_CONFIGURATION_COMPLETE.md** - This guide
- **PRODUCTION_DEPLOYMENT_GUIDE.md** - Full deployment guide
- **PRODUCTION_READY.md** - Build & readiness summary
- **DATABASE_MIGRATION_NOTES.md** - Migration instructions
- **COMPILATION_FIX_SUMMARY.md** - All fixes applied

---

## ✅ Post-Deployment Checklist

- [ ] DNS configured and propagated
- [ ] SSL certificates obtained
- [ ] Application deployed
- [ ] Services running
- [ ] HTTPS working
- [ ] Database tables created
- [ ] Admin password changed
- [ ] All modules tested
- [ ] Backups configured
- [ ] Monitoring set up

---

**Quick Help:**
- Docs: `/root/app.shahin-ai.com/Shahin-ai/*.md`
- Scripts: `./deploy-production.sh`, `./setup-ssl.sh`, `./run-migrations.sh`
- Logs: `sudo journalctl -u grc-web -f`

**Emergency Contacts:**
- Development: dev@shahin-ai.com
- DevOps: devops@shahin-ai.com

---

*Last Updated: December 21, 2025*



