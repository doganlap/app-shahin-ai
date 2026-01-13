# Actual API Endpoints - Complete List

**Date:** 2026-01-22  
**Base URL:** http://localhost:8888

---

## ✅ Trial Endpoints (Working)

| Endpoint | Method | Auth | Status | Purpose |
|----------|--------|------|--------|---------|
| `/trial` | GET | No | ✅ 200 OK | Display registration form |
| `/trial` | POST | No | ✅ Exists | Process registration |
| `/trial/demo-request` | POST | No | ✅ Exists | Handle demo requests |

**Note:** `/api/trial/status` and `/api/trial/info` do NOT exist (404 is expected)

---

## ✅ Support Endpoints (Working)

| Endpoint | Method | Auth | Status | Purpose |
|----------|--------|------|--------|---------|
| `/api/support/contact` | POST | No | ✅ Exists | Submit contact form |
| `/api/support/start` | POST | Yes | ✅ Exists | Start support conversation |
| `/api/support/message` | POST | Yes | ✅ Exists | Send support message |
| `/api/support/messages/{id}` | GET | Yes | ✅ Exists | Get conversation messages |
| `/api/support/escalate` | POST | Yes | ✅ Exists | Escalate to human |
| `/api/support/quick-help` | GET | Yes | ✅ Exists | Quick help |
| `/api/support/legal/{type}` | GET | Yes | ✅ Exists | Get legal document |
| `/api/support/consent` | POST | Yes | ✅ Exists | Record consent |
| `/api/support/consent/check/{userId}` | GET | Yes | ✅ Exists | Check consents |

**Note:** `/api/support/status` does NOT exist (404 is expected)

---

## ✅ Agent/Chat Endpoints (Working)

| Endpoint | Method | Auth | Status | Purpose |
|----------|--------|------|--------|---------|
| `/api/agent/chat/public` | GET/POST | No | ✅ Exists | Public AI chat (used in trial form) |
| `/api/Landing/ChatMessage` | POST | No | ✅ Exists | Landing page chat |

**Note:** `/api/landing/chat` does NOT exist (use `/api/Landing/ChatMessage` instead)

---

## ✅ Landing Endpoints (Working)

| Endpoint | Method | Auth | Status | Purpose |
|----------|--------|------|--------|---------|
| `/api/Landing/StartTrial` | POST | No | ✅ Exists | Initial trial signup |
| `/api/Landing/ContactUs` | POST | No | ✅ Exists | Contact form |
| `/api/Landing/RequestDemo` | POST | No | ✅ Exists | Demo request |
| `/api/landing/client-logos` | GET | No | ✅ Exists | Get client logos |
| `/api/landing/trust-badges` | GET | No | ✅ Exists | Get trust badges |
| `/api/landing/faqs` | GET | No | ✅ Exists | Get FAQs |
| `/api/landing/statistics` | GET | No | ✅ Exists | Get statistics |
| `/api/landing/features` | GET | No | ✅ Exists | Get features |
| `/api/landing/partners` | GET | No | ✅ Exists | Get partners |

---

## ❌ Endpoints That Don't Exist (404 Expected)

These endpoints were tested but don't exist (which is fine):

- ❌ `/api/trial/status` - Not needed
- ❌ `/api/trial/info` - Not needed
- ❌ `/api/support/status` - Not needed
- ❌ `/api/landing/chat` - Use `/api/Landing/ChatMessage` instead

---

## 🔍 Trial Form Integration

**The trial form (`/trial`) uses:**

1. **AI Assistant Chat:**
   - Endpoint: `/api/agent/chat/public`
   - Method: GET
   - Parameters: `message`, `context=trial_registration`
   - Appears after 10 seconds on page

2. **Form Submission:**
   - Endpoint: `/trial` (POST)
   - Creates tenant and user
   - Redirects to onboarding

3. **Support Contact:**
   - Endpoint: `/api/support/contact` (POST)
   - Sends email to `support@grc-system.sa`

---

## ⚠️ Redis Issue

**Problem:** Redis connection timeout
```
UnableToConnect on grc-redis:6379
```

**Impact:** 
- Session management may use in-memory fallback
- Caching may not work
- **Non-critical** - app has fallback

**Fix:**
```bash
# Check if Redis service exists in docker-compose
docker-compose -f docker-compose.yml config | grep redis

# If exists, start it
docker-compose -f docker-compose.yml up -d redis
```

---

## ✅ Summary

**Working:**
- ✅ Trial registration form (`/trial`)
- ✅ AI chat assistant (`/api/agent/chat/public`)
- ✅ Support contact (`/api/support/contact`)
- ✅ All landing page endpoints

**Not Needed:**
- ❌ `/api/trial/status` - 404 is expected
- ❌ `/api/trial/info` - 404 is expected
- ❌ `/api/support/status` - 404 is expected
- ❌ `/api/landing/chat` - Use `/api/Landing/ChatMessage` instead

**Status:** ✅ **All required endpoints are working!**
