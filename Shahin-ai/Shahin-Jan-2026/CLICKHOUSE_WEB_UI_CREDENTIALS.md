# ClickHouse Web UI - Correct Credentials

## 🔐 Login Credentials

When accessing ClickHouse Web UI at **http://localhost:8123**, use these credentials:

### ✅ Correct Settings:
- **Server:** `http://localhost:8123`
- **Username:** `grc_analytics` (NOT "default")
- **Password:** `grc_analytics_2026`

### ❌ Wrong Settings (What you had):
- **Username:** `default` ❌
- **Password:** (empty) ❌

---

## 📋 Quick Fix

1. **Change Username:**
   - From: `default`
   - To: `grc_analytics`

2. **Enter Password:**
   - Password: `grc_analytics_2026`

3. **Click "Reload"** button

---

## ✅ Verification

After entering correct credentials, you should:
- ✅ See ClickHouse interface
- ✅ Be able to run queries
- ✅ Access ClickHouse database

---

## 🧪 Test Connection

You can also test the connection via command line:

```bash
curl -u grc_analytics:grc_analytics_2026 \
  "http://localhost:8123/?query=SELECT%201"
```

Expected output: `1`

---

## 📊 ClickHouse Configuration

**Container:** grc-clickhouse
**Database:** grc_analytics
**User:** grc_analytics
**Password:** grc_analytics_2026
**HTTP Port:** 8123
**Native Port:** 9000

---

## 🎯 Summary

| Field | Value |
|-------|-------|
| Server | http://localhost:8123 |
| Username | **grc_analytics** |
| Password | **grc_analytics_2026** |

**Use these credentials to access ClickHouse Web UI!** 🚀
