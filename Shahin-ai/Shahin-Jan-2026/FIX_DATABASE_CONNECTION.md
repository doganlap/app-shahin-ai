# Fix Database Connection Error

## Problem
Error: "Error authenticating with database. Please check your connection params and try again."

## Root Cause
The `.env` file has incorrect database connection parameters:
- Wrong IP address (172.18.0.6 instead of 172.18.0.2)
- Should use container name instead of IP for reliability

## Solution

### Option 1: Use Container Name (Recommended)
Update `.env` file to use container name `grc-db`:

```bash
CONNECTION_STRING=Host=grc-db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres;Include Error Detail=true
```

### Option 2: Use Correct IP Address
Update `.env` file with correct IP (172.18.0.2):

```bash
CONNECTION_STRING=Host=172.18.0.2;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres;Include Error Detail=true
```

### Option 3: Use localhost (If running from host)
If running the application from the host machine (not in Docker):

```bash
CONNECTION_STRING=Host=localhost;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres;Include Error Detail=true
```

## Quick Fix Command

```bash
cd /home/Shahin-ai/Shahin-Jan-2026

# Option 1: Use container name (if app runs in Docker)
sed -i 's/Host=172.18.0.6/Host=grc-db/g' .env

# Option 2: Use correct IP
sed -i 's/Host=172.18.0.6/Host=172.18.0.2/g' .env

# Option 3: Use localhost (if app runs on host)
sed -i 's/Host=172.18.0.6/Host=localhost/g' .env
```

## Verify Connection

```bash
# Test connection from host
docker exec grc-db psql -U postgres -d GrcMvcDb -c "SELECT version();"

# Test connection string format
cd src/GrcMvc
dotnet ef database update --dry-run
```

## Current Database Info

- **Container Name:** grc-db
- **Container IP:** 172.18.0.2
- **Port:** 5432
- **Database:** GrcMvcDb
- **Username:** postgres
- **Password:** postgres
- **Status:** ✅ Running and healthy

## After Fix

1. Update `.env` file with correct connection string
2. Restart application
3. Run migration: `dotnet ef migrations add AddAbpFrameworkTablesAndOnboarding`
4. Apply migration: `dotnet ef database update`
