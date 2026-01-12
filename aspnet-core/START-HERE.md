# 🚀 Saudi GRC Platform - Unified Source

## ✅ ONE Source Location - NO Mixed Code

**Everything runs from:** `/root/app.shahin-ai.com/Shahin-ai/aspnet-core/`

---

## 🎯 Quick Start (3 Steps)

### 1. Start Application
```bash
cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core
./start-unified.sh
```

### 2. Open Browser
```
http://localhost:5500
```

### 3. Login
```
Username: admin
Password: 1q2w3E*
```

**Done! 🎉**

---

## 📂 Unified Directory Structure

```
aspnet-core/                          ← ONE ROOT PATH
├── src/                              ← All source code
│   ├── Grc.Web/                      ← Web UI (runs on port 5500)
│   ├── Grc.HttpApi.Host/             ← API (if needed)
│   ├── Grc.Domain/                   ← Business logic
│   ├── Grc.Application/              ← Services
│   ├── Grc.EntityFrameworkCore/      ← Database
│   └── Grc.*.Domain/                 ← Feature modules
├── Logs/                             ← Application logs
│   └── unified-app.log              ← Current session log
├── start-unified.sh                  ← Startup script
└── .env                              ← Configuration
```

**All code in ONE place - NO deployment directories!**

---

## 🔧 Management Commands

```bash
# Start application
./start-unified.sh

# Stop application
pkill -f "Grc.Web"

# View logs
tail -f Logs/unified-app.log

# Rebuild
dotnet build

# Run database migrations
cd src/Grc.DbMigrator && dotnet run
```

---

## 📊 Access All Features

| Module | URL |
|--------|-----|
| Dashboard | http://localhost:5500/Dashboard |
| Framework Library | http://localhost:5500/FrameworkLibrary |
| Assessments | http://localhost:5500/Assessments |
| Risks | http://localhost:5500/Risks |
| Evidence | http://localhost:5500/Evidence |
| Policies | http://localhost:5500/Policies |

---

## ⚙️ Configuration

**Database:** PostgreSQL (localhost:5434)  
**Ports:** 5500 (HTTP), 5501 (HTTPS)  
**Environment:** Production  
**Source:** aspnet-core/ directory

Edit `.env` file for configuration changes.

---

**✨ Everything unified in ONE path - Clean, simple, no hiccups!**
