# 🔍 "Fetch Failed" Error Diagnostic Guide

**Issue**: "localmmm" typo causing fetch failures  
**Date**: 2026-01-11

---

## ✅ Search Results

**No "localmmm" found in codebase** - This confirms it's likely:
1. A typo in a browser URL you typed manually
2. An environment variable in `.env` file (not committed)
3. A runtime configuration value
4. A browser cache/storage value

---

## 🔍 Where to Check

### 1. Browser Address Bar / DevTools

**Check**:
- Open browser DevTools (F12)
- Go to **Network** tab
- Look for failed requests
- Check the **Request URL** - does it say `localmmm`?

**Common typos**:
- ❌ `http://localmmm:8080`
- ✅ `http://localhost:8080`

---

### 2. Environment Variables (.env files)

**Check these files**:
```bash
# Main .env file
cat /home/Shahin-ai/Shahin-Jan-2026/.env | grep -i "local"

# Frontend .env files
cat /home/Shahin-ai/Shahin-Jan-2026/grc-app/.env.local
cat /home/Shahin-ai/Shahin-Jan-2026/grc-frontend/.env.local
cat /home/Shahin-ai/Shahin-Jan-2026/shahin-ai-website/.env.local
```

**Look for**:
```bash
# ❌ Wrong
NEXT_PUBLIC_API_URL=http://localmmm:8080
VITE_API_BASE_URL=http://localmmm:8888

# ✅ Correct
NEXT_PUBLIC_API_URL=http://localhost:8080
VITE_API_BASE_URL=http://localhost:8888
```

---

### 3. Browser LocalStorage / SessionStorage

**Check in browser console**:
```javascript
// Check localStorage
Object.keys(localStorage).forEach(key => {
  const value = localStorage.getItem(key);
  if (value && value.includes('localmmm')) {
    console.log(key, value);
  }
});

// Check sessionStorage
Object.keys(sessionStorage).forEach(key => {
  const value = sessionStorage.getItem(key);
  if (value && value.includes('localmmm')) {
    console.log(key, value);
  }
});
```

---

### 4. Monitoring Services Status

**Expected ports**:
- **Netdata**: `localhost:19999`
- **Prometheus**: `localhost:9090`
- **Grafana**: `localhost:3000` (or `3030` in docker-compose)
- **Zabbix/NocHub**: `localhost:8080`
- **Kafka UI**: `localhost:9080`

**Check if services are running**:
```bash
# Check Docker containers
docker-compose ps | grep -E "grafana|prometheus|netdata|zabbix"

# Check ports
netstat -tuln | grep -E "19999|9090|3000|8080|9080"

# Test services
curl http://localhost:19999  # Netdata
curl http://localhost:9090    # Prometheus
curl http://localhost:3000    # Grafana
curl http://localhost:8080    # Zabbix/Kafka UI
```

---

## 🔧 Frontend API Configuration

### Current Default Values

**grc-app** (`src/lib/config.ts`):
```typescript
BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
  (process.env.NODE_ENV === 'production' 
    ? 'https://portal.shahin-ai.com'
    : 'http://localhost:8080')
```

**grc-frontend** (`src/lib/config.ts`):
```typescript
BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
  (process.env.NODE_ENV === 'production'
    ? 'https://portal.shahin-ai.com'
    : 'http://localhost:8080')
```

**shahin-ai-website** (`lib/config.ts`):
```typescript
BASE_URL: process.env.NEXT_PUBLIC_API_URL || 
  (process.env.NODE_ENV === 'production'
    ? 'https://app.shahin-ai.com'
    : 'http://localhost:8080')
```

---

## 🛠️ Fix Steps

### Step 1: Check Browser Console

1. Open DevTools (F12)
2. Go to **Console** tab
3. Look for errors like:
   ```
   Failed to fetch from http://localmmm:8080/api/...
   ```

### Step 2: Check Environment Variables

```bash
# Check main .env
cd /home/Shahin-ai/Shahin-Jan-2026
grep -r "localmmm" .env* 2>/dev/null

# Check frontend .env files
grep -r "localmmm" grc-app/.env* grc-frontend/.env* shahin-ai-website/.env* 2>/dev/null
```

### Step 3: Fix if Found

**If found in .env file**:
```bash
# Replace localmmm with localhost
sed -i 's/localmmm/localhost/g' .env
sed -i 's/localmmm/localhost/g' grc-app/.env.local
sed -i 's/localmmm/localhost/g' grc-frontend/.env.local
```

### Step 4: Clear Browser Cache

1. **Chrome/Edge**: Ctrl+Shift+Delete → Clear cache
2. **Firefox**: Ctrl+Shift+Delete → Clear cache
3. **Or**: Hard refresh (Ctrl+F5)

### Step 5: Restart Services

```bash
# Restart frontend apps
cd /home/Shahin-ai/Shahin-Jan-2026/grc-app
npm run dev  # or docker-compose up -d

cd /home/Shahin-ai/Shahin-Jan-2026/grc-frontend
npm run dev  # or docker-compose up -d
```

---

## 📋 Quick Diagnostic Commands

```bash
# 1. Search for "localmmm" in all files
cd /home/Shahin-ai/Shahin-Jan-2026
grep -r "localmmm" . --exclude-dir=node_modules --exclude-dir=.git 2>/dev/null

# 2. Check environment variables
env | grep -i "local"

# 3. Check running services
docker-compose ps
netstat -tuln | grep -E "19999|9090|3000|8080|9080"

# 4. Test API endpoints
curl http://localhost:8888/health
curl http://localhost:8080/health 2>/dev/null
```

---

## 🎯 Most Likely Causes

| Cause | Probability | Fix |
|-------|-------------|-----|
| Typo in browser URL | 80% | Type `localhost` correctly |
| .env file typo | 15% | Fix `.env` file |
| Browser cache | 4% | Clear cache |
| Service not running | 1% | Start services |

---

## ✅ Verification

After fixing, verify:

1. **Browser DevTools** → Network tab → No failed requests
2. **Console** → No "fetch failed" errors
3. **API calls** → All return 200 OK

---

## 📝 Next Steps

1. **Check browser console** for exact error message
2. **Check .env files** for typos
3. **Clear browser cache** and hard refresh
4. **Restart services** if needed

If you can share the **exact error message** from browser console, I can provide a more specific fix.

---

**Created**: 2026-01-11  
**Status**: Diagnostic guide ready
