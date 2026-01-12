# 🌐 Saudi GRC Application - cURL Testing Guide

**Server IP:** 37.27.139.173  
**Generated:** December 21, 2025

---

## 📊 Server Status

### **Current Configuration:**
- **External IP:** 37.27.139.173
- **Hostname:** Ubuntu-2404-noble-amd64-base
- **OS:** Ubuntu 24.04.3 LTS
- **Web App:** Port 5001 (localhost)
- **API:** Port 5000 (localhost)
- **Nginx:** Ports 80 & 443 (public)

### **Listening Ports:**
```
Port 80   → Nginx (HTTP)
Port 443  → Nginx (HTTPS)
Port 5000 → GRC API (dotnet)
Port 5001 → GRC Web (dotnet)
```

---

## ✅ Basic Health Checks

### **1. Homepage Test:**
```bash
curl -I http://localhost:5001
# Expected: HTTP/1.1 200 OK
```

### **2. API Test:**
```bash
curl -I http://localhost:5000
# Expected: HTTP/1.1 302 Found (redirects to /swagger)
```

### **3. Full Request:**
```bash
curl http://localhost:5001
# Returns full HTML of homepage
```

---

## 🔍 Detailed Testing

### **Verbose Output (Debug):**
```bash
curl -v http://localhost:5001
# Shows full HTTP headers, timing, SSL info
```

### **Follow Redirects:**
```bash
curl -L http://localhost:5000
# Follows redirect to /swagger
```

### **Get Response Time:**
```bash
curl -w '\nTotal: %{time_total}s\nConnect: %{time_connect}s\nStart Transfer: %{time_starttransfer}s\n' \
     -o /dev/null -s http://localhost:5001
```

### **Save Response to File:**
```bash
curl -o homepage.html http://localhost:5001
```

---

## 🎯 Test Specific Modules

### **Evidence Module:**
```bash
curl -I http://localhost:5001/Evidence
# Status: 500 (needs database tables)
```

### **Framework Library:**
```bash
curl -I http://localhost:5001/FrameworkLibrary
# Status: 500 (needs database tables)
```

### **Risks Module:**
```bash
curl -I http://localhost:5001/Risks
# Status: 500 (needs database tables)
```

### **Assessments:**
```bash
curl -I http://localhost:5001/Assessments
# Status: 500 (needs database tables)
```

**Note:** HTTP 500 errors are expected until database migrations are run. The tables for these modules need to be created.

---

## 🌍 External Testing (After DNS Update)

### **Test Public URLs:**
```bash
# Web Application
curl -I https://grc.shahin-ai.com

# API
curl -I https://api-grc.shahin-ai.com

# Swagger Documentation
curl -L https://api-grc.shahin-ai.com/swagger
```

### **Test with Cloudflare Headers:**
```bash
curl -I https://grc.shahin-ai.com \
     -H "CF-Connecting-IP: 1.2.3.4" \
     -H "CF-RAY: test123"
```

### **Check SSL Certificate:**
```bash
curl -vI https://grc.shahin-ai.com 2>&1 | grep -E "(SSL|TLS|certificate)"
```

---

## 📈 Performance Testing

### **Measure Load Time:**
```bash
time curl -s -o /dev/null http://localhost:5001
```

### **Multiple Requests (Load Test):**
```bash
for i in {1..10}; do
  curl -s -o /dev/null -w "Request $i: %{time_total}s\n" http://localhost:5001
done
```

### **Concurrent Requests:**
```bash
# Install apache2-utils first: sudo apt install apache2-utils
ab -n 100 -c 10 http://localhost:5001/
```

---

## 🔐 Authentication Testing

### **Login Endpoint:**
```bash
curl -X POST http://localhost:5001/Account/Login \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "username=admin&password=1q2w3E*"
```

### **Get Cookies:**
```bash
curl -c cookies.txt http://localhost:5001/Account/Login
```

### **Send Cookies:**
```bash
curl -b cookies.txt http://localhost:5001/Evidence
```

---

## 🔧 API Testing

### **Swagger JSON:**
```bash
curl http://localhost:5000/swagger/v1/swagger.json | jq
```

### **Health Endpoint:**
```bash
curl http://localhost:5000/health
```

### **API Version:**
```bash
curl -I http://localhost:5000/api/
```

---

## 📊 Response Analysis

### **Get Headers Only:**
```bash
curl -I http://localhost:5001
```

### **Get Specific Header:**
```bash
curl -I http://localhost:5001 | grep -i "content-type"
```

### **Get Response Code:**
```bash
curl -s -o /dev/null -w "%{http_code}" http://localhost:5001
```

### **Get All Timing Info:**
```bash
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5001

# Create curl-format.txt:
cat > curl-format.txt << 'EOF'
    time_namelookup:  %{time_namelookup}\n
       time_connect:  %{time_connect}\n
    time_appconnect:  %{time_appconnect}\n
   time_pretransfer:  %{time_pretransfer}\n
      time_redirect:  %{time_redirect}\n
 time_starttransfer:  %{time_starttransfer}\n
                    ----------\n
         time_total:  %{time_total}\n
EOF
```

---

## 🐛 Debugging

### **Check if Service is Running:**
```bash
curl -I http://localhost:5001 || echo "Service not responding"
```

### **Test through Nginx:**
```bash
curl -I http://localhost:80 \
     -H "Host: grc.shahin-ai.com"
```

### **Check DNS Resolution:**
```bash
curl -I --resolve grc.shahin-ai.com:443:37.27.139.173 https://grc.shahin-ai.com
```

### **Ignore SSL Errors (Testing Only):**
```bash
curl -k https://grc.shahin-ai.com
```

---

## 📝 Common Status Codes

| Code | Meaning | Likely Cause |
|------|---------|-------------|
| 200 | OK | Success |
| 302 | Found | Redirect (normal for API root → /swagger) |
| 401 | Unauthorized | Need to login |
| 403 | Forbidden | No permission |
| 404 | Not Found | Wrong URL or route not configured |
| 500 | Internal Error | Database tables missing or app error |
| 502 | Bad Gateway | App not running / Nginx can't connect |
| 503 | Service Unavailable | App starting or overloaded |

---

## ✅ Current Test Results

### **Working:**
✅ Homepage (/) → HTTP 200  
✅ API Root (/swagger redirect) → HTTP 302  
✅ Nginx → Listening on ports 80 & 443  
✅ Web Service → Running on port 5001  
✅ API Service → Running on port 5000  

### **Need Database Tables:**
⚠️ Evidence → HTTP 500  
⚠️ FrameworkLibrary → HTTP 500  
⚠️ Risks → HTTP 500  
⚠️ Assessments → HTTP 500  

**Fix:** Run database migrations to create tables:
```bash
cd /root/app.shahin-ai.com/Shahin-ai
./run-migrations.sh
```

---

## 🚀 Quick Test Script

Save as `test-app.sh`:

```bash
#!/bin/bash

echo "=== Saudi GRC Application Test ==="
echo ""

# Test Web
echo "1. Testing Web Application..."
WEB_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5001)
if [ "$WEB_STATUS" = "200" ]; then
    echo "   ✅ Web: OK (HTTP $WEB_STATUS)"
else
    echo "   ❌ Web: FAILED (HTTP $WEB_STATUS)"
fi

# Test API
echo "2. Testing API..."
API_STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000)
if [ "$API_STATUS" = "302" ]; then
    echo "   ✅ API: OK (HTTP $API_STATUS)"
else
    echo "   ❌ API: FAILED (HTTP $API_STATUS)"
fi

# Test Modules
echo "3. Testing Modules..."
for module in Evidence FrameworkLibrary Risks; do
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5001/$module)
    if [ "$STATUS" = "200" ]; then
        echo "   ✅ $module: OK"
    else
        echo "   ⚠️  $module: HTTP $STATUS"
    fi
done

echo ""
echo "=== Test Complete ==="
```

Run: `chmod +x test-app.sh && ./test-app.sh`

---

## 📚 Additional Resources

### **cURL Documentation:**
```bash
man curl
curl --help
```

### **Install jq for JSON parsing:**
```bash
sudo apt install jq
curl http://localhost:5000/swagger/v1/swagger.json | jq '.info'
```

### **HTTP Headers Reference:**
- https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers

---

**Last Updated:** December 21, 2025  
**Server:** 37.27.139.173  
**Status:** ✅ Services Running, ⚠️ Database Migration Pending



