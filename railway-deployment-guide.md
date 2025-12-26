# 🚂 Railway Deployment Guide for GRC Platform

## Current Railway Infrastructure

You already have the following services deployed on Railway:

### ✅ Services Running
1. **PostgreSQL** - Database (multiple deployments)
2. **Redis** - Cache (2 instances: Redis-zP4r and Redis)
3. **S3-Compatible Storage** (optimized-bin) - Object storage

### S3 Storage Credentials (Already Configured)
- **Endpoint**: `https://storage.railway.app`
- **Region**: `auto`
- **Bucket**: `optimized-bin-yvjb9vxnhq1`
- **Access Key**: `tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD`
- **Secret Key**: `tsec_KnsFqr0JZOsYqQl1LRMMo46kqAbGVcHgR-vADqBcGxAbQzmt44MakhvpYceOi3Z7ggUnC9`

---

## 🚀 Deploy GRC Platform to Railway

### Step 1: Get Railway Service Connection Strings

In your Railway dashboard, for each service get the connection details:

#### PostgreSQL
```bash
# In Railway PostgreSQL service → Connect → Copy Connection String
# Format: postgresql://postgres:password@host:port/railway
```

#### Redis  
```bash
# In Railway Redis service → Connect → Copy Connection String
# Format: redis://:password@host:port
```

### Step 2: Create Railway Environment Variables

Create a new service in Railway for the GRC API:

1. **Click "New" → "Empty Service"**
2. **Name it**: `grc-api`
3. **Add the following environment variables**:

```bash
# App Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Database (PostgreSQL from your Railway PostgreSQL service)
ConnectionStrings__Default=postgresql://postgres:PASSWORD@HOST:PORT/railway?sslmode=require

# Redis (from your Railway Redis service)
Redis__Configuration=HOST:PORT,password=PASSWORD,ssl=true

# S3 Storage (Already have credentials)
S3__Endpoint=https://storage.railway.app
S3__Region=auto
S3__BucketName=optimized-bin-yvjb9vxnhq1
S3__AccessKeyId=tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD
S3__SecretAccessKey=tsec_KnsFqr0JZOsYqQl1LRMMo46kqAbGVcHgR-vADqBcGxAbQzmt44MakhvpYceOi3Z7ggUnC9
S3__UseVirtualHostStyle=true

# CORS
App__CorsOrigins=https://YOUR-WEB-DOMAIN.railway.app
```

### Step 3: Deploy API to Railway

#### Option A: Using GitHub (Recommended)

1. **Push code to GitHub**:
   ```bash
   cd /root/app.shahin-ai.com/Shahin-ai
   git add .
   git commit -m "Complete Phases 3, 4, 5 implementation"
   git push origin main
   ```

2. **In Railway**:
   - Click "New" → "GitHub Repo"
   - Select your repository
   - Root directory: `src/Grc.HttpApi.Host`
   - Build command: `dotnet publish -c Release -o out`
   - Start command: `dotnet out/Grc.HttpApi.Host.dll`

#### Option B: Using Railway CLI

1. **Install Railway CLI**:
   ```bash
   npm install -g @railway/cli
   railway login
   ```

2. **Link project**:
   ```bash
   cd /root/app.shahin-ai.com/Shahin-ai
   railway link
   ```

3. **Deploy**:
   ```bash
   railway up
   ```

### Step 4: Deploy Angular Frontend

1. **Create new service**: `grc-web`

2. **Set environment variables**:
   ```bash
   API_URL=https://grc-api.railway.app
   ```

3. **Deploy from GitHub** or **use static hosting**:
   - Build locally: `cd angular && npm run build -- --configuration production`
   - Deploy `dist/` folder to Railway static hosting

### Step 5: Run Database Migrations

Once API is deployed:

```bash
# Using Railway CLI
railway run dotnet ef database update

# Or exec into the container
railway shell
dotnet ef database update
```

### Step 6: Seed Data

```bash
railway run dotnet run --seed
```

---

## 🔧 Railway Configuration Template

### Dockerfile for API (Optional)

Create `Dockerfile` in root:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY src/ ./

# Restore and build
WORKDIR /src/Grc.HttpApi.Host
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Railway will set $PORT)
EXPOSE $PORT

ENTRYPOINT ["dotnet", "Grc.HttpApi.Host.dll"]
```

### railway.json (Railway Configuration)

Create `railway.json` in root:

```json
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "DOCKERFILE",
    "dockerfilePath": "Dockerfile"
  },
  "deploy": {
    "startCommand": "dotnet Grc.HttpApi.Host.dll",
    "restartPolicyType": "ON_FAILURE",
    "restartPolicyMaxRetries": 10
  }
}
```

---

## 🔗 Connecting Services

### Update appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "Default": "${{DATABASE_URL}}"
  },
  "Redis": {
    "Configuration": "${{REDIS_URL}}"
  },
  "S3": {
    "Endpoint": "https://storage.railway.app",
    "BucketName": "optimized-bin-yvjb9vxnhq1",
    "AccessKeyId": "${{S3_ACCESS_KEY}}",
    "SecretAccessKey": "${{S3_SECRET_KEY}}"
  }
}
```

Railway will automatically inject environment variables.

---

## 📊 Expected Railway Services

After deployment, you should have:

```
┌─────────────────────┬──────────────────────┬─────────────┐
│ Service             │ Type                 │ Status      │
├─────────────────────┼──────────────────────┼─────────────┤
│ PostgreSQL          │ Database             │ ✅ Running  │
│ Redis               │ Cache                │ ✅ Running  │
│ optimized-bin       │ S3 Storage           │ ✅ Running  │
│ grc-api             │ .NET API             │ 🔄 Deploy   │
│ grc-web             │ Angular Frontend     │ 🔄 Deploy   │
└─────────────────────┴──────────────────────┴─────────────┘
```

---

## 🎯 Migration Commands for Railway

### Using Railway CLI

```bash
# Connect to your project
railway link

# Run migration
railway run --service grc-api dotnet ef database update

# Seed data
railway run --service grc-api dotnet run --seed

# View logs
railway logs --service grc-api
```

---

## 📝 Environment Variables Needed

Copy these to Railway `grc-api` service:

```env
# Required
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Database (Get from Railway PostgreSQL)
DATABASE_URL=postgresql://postgres:password@host:port/railway

# Redis (Get from Railway Redis)
REDIS_URL=redis://:password@host:port

# S3 (Your existing credentials)
S3_ENDPOINT=https://storage.railway.app
S3_BUCKET=optimized-bin-yvjb9vxnhq1
S3_ACCESS_KEY=tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD
S3_SECRET_KEY=tsec_KnsFqr0JZOsYqQl1LRMMo46kqAbGVcHgR-vADqBcGxAbQzmt44MakhvpYceOi3Z7ggUnC9

# App
APP_SELF_URL=https://grc-api.railway.app
APP_CORS_ORIGINS=https://grc-web.railway.app

# Optional
JWT_SECRET_KEY=YOUR_LONG_RANDOM_STRING_HERE
```

---

## 🔍 Verify Deployment

### Check API Health
```bash
curl https://grc-api.railway.app/health
```

### Check Services
```bash
railway status
```

### View Logs
```bash
railway logs
```

---

## 🎨 Frontend Deployment

### Build Angular App
```bash
cd /root/app.shahin-ai.com/Shahin-ai/angular

# Update environment with Railway API URL
# Edit src/environments/environment.prod.ts
# apiUrl: 'https://grc-api.railway.app'

# Build
npm run build -- --configuration production
```

### Deploy to Railway

1. Create new service: `grc-web`
2. Use **Static** hosting
3. Publish directory: `dist/`
4. Or use Nginx with custom Dockerfile

---

## 💾 Database Setup on Railway

Your Railway PostgreSQL is ready. Just need to:

1. **Run migrations**:
   ```bash
   railway run --service grc-api dotnet ef database update
   ```

2. **Seed default products**:
   ```bash
   railway run --service grc-api dotnet run --seed
   ```

This will create:
- All database tables
- 4 products (Trial, Standard, Professional, Enterprise)
- Default frameworks and data

---

## 🎯 Quick Railway Deployment

### Using Railway CLI (Fastest)

```bash
# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# Link to your project
cd /root/app.shahin-ai.com/Shahin-ai
railway link

# Deploy API
railway up --service grc-api

# Run migrations
railway run --service grc-api dotnet ef database update

# Seed data
railway run --service grc-api dotnet run --seed
```

---

## 📦 What's Already Done

✅ **Infrastructure**: PostgreSQL, Redis, S3 (optimized-bin) on Railway  
✅ **Code**: All 42 tasks (265+ files)  
✅ **Config**: S3 credentials ready  
✅ **Automation**: Scripts ready  

**Just need to**: Deploy the API and Web services to Railway

---

## 🔐 Security Notes

**IMPORTANT**: 
- ✅ S3 credentials already configured
- ⚠️ Store other secrets in Railway's environment variables
- ⚠️ Don't commit secrets to Git
- ✅ Use Railway's built-in secret management

---

## 📞 Support

For Railway-specific issues:
- Railway Docs: https://docs.railway.app
- Railway Discord: https://discord.gg/railway

For GRC Platform:
- See: [PRODUCTION-DEPLOYMENT-GUIDE.md](PRODUCTION-DEPLOYMENT-GUIDE.md)
- API Docs: [docs/API-REFERENCE.md](docs/API-REFERENCE.md)

---

**Status**: Ready to deploy to Railway  
**Infrastructure**: ✅ Already running on Railway  
**Code**: ✅ Complete  
**Next**: Deploy API and Web services to Railway

