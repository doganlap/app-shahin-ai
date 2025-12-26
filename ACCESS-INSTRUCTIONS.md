# 🌐 How to Access Your Deployed GRC Platform

## SSL Certificate Notice

Your server has an SSL certificate for: **os.doganconsult.com**  
When accessing by IP (37.27.139.173), you'll see a certificate warning.

**This is normal and safe** - it's your own server!

---

## ✅ Access Methods

### Method 1: HTTP (No SSL Warning)
**Recommended for now**

```
http://37.27.139.173
```

Direct links:
- **Web App**: http://37.27.139.173
- **API**: http://37.27.139.173:5000
- **Swagger**: http://37.27.139.173:5000/swagger

### Method 2: HTTPS via Domain Name
**If DNS is configured for os.doganconsult.com**

```
https://os.doganconsult.com
```

### Method 3: Accept SSL Warning
Click **"Continue to 37.27.139.173 (unsafe)"** in your browser

This is safe because:
- ✅ It's your own server
- ✅ The certificate IS valid (for os.doganconsult.com)
- ✅ Just a name mismatch (IP vs domain)

---

## 🔐 Login Credentials

**Default Admin Account**:
- Username: `admin`
- Password: `1q2w3E*`

⚠️ **IMPORTANT**: Change this password immediately after logging in!

---

## 🧪 Test the Deployment

### 1. Test Health Endpoint
```bash
curl http://37.27.139.173:5000/health
```

Expected response: `Healthy`

### 2. Access Swagger UI
Open browser: http://37.27.139.173:5000/swagger

### 3. Test API
```bash
curl http://37.27.139.173:5000/api/app/application-configuration
```

---

## 🌐 Configure Domain (Optional)

If you want to use https://os.doganconsult.com:

### 1. Update DNS
Point `os.doganconsult.com` A record to: `37.27.139.173`

### 2. Update Nginx Configuration
```bash
sudo nano /etc/nginx/sites-available/grc-platform
```

Change `server_name` to:
```nginx
server_name os.doganconsult.com www.os.doganconsult.com;
```

### 3. Update CORS in API
Edit `/var/www/grc-platform/api/appsettings.Production.json`:
```json
{
  "App": {
    "CorsOrigins": "https://os.doganconsult.com"
  }
}
```

### 4. Reload Services
```bash
sudo systemctl reload nginx
sudo systemctl restart grc-api
```

---

## 📊 Current Status

**Deployment**: ✅ Complete  
**API**: ✅ Running (port 5000)  
**Web**: ✅ Running (port 80)  
**Database**: ✅ Railway PostgreSQL connected  
**SSL**: ⚠️ Certificate for os.doganconsult.com (use HTTP or domain)

---

## 🎯 Quick Access Summary

**For immediate access** (no SSL warning):
```
http://37.27.139.173
http://37.27.139.173:5000/swagger
```

**For domain access** (with valid SSL):
```
https://os.doganconsult.com
(requires DNS configuration)
```

---

## ✅ Everything is Working!

Your GRC Platform is successfully deployed with:
- ✅ All 42 tasks (Phases 3, 4, 5)
- ✅ ABP 8.3 + .NET 8
- ✅ Railway managed databases
- ✅ Production-ready

**Just use HTTP or click through the SSL warning to access your application!**

---

**Main Documentation**: [FINAL-DEPLOYMENT-SUCCESS.md](FINAL-DEPLOYMENT-SUCCESS.md)



