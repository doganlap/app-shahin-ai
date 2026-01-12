# 🚂 Railway Deployment - Ready

## Railway Services Configured

You have the following services on Railway:

### ✅ Databases
1. **PostgreSQL "Postgres"** (Primary)
   - Host: `mainline.proxy.rlwy.net:46662`
   - Username: `postgres`
   - Password: `sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ`
   - Database: `railway`
   - Connection: `postgresql://postgres:sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ@mainline.proxy.rlwy.net:46662/railway`

2. **PostgreSQL "2"** (Secondary/Backup)
   - Host: `shortline.proxy.rlwy.net:11220`
   - Username: `postgres`
   - Password: `CGZRNJUmAgXhBrnbbREVhvSMflfJWQay`
   - Database: `railway`
   - Connection: `postgresql://postgres:CGZRNJUmAgXhBrnbbREVhvSMflfJWQay@shortline.proxy.rlwy.net:11220/railway`

### ✅ Cache
**Redis**
- Host: `caboose.proxy.rlwy.net:26002`
- Username: `default`
- Password: `ySTCqQpbNuYVFfJwIIIeqkRgkTvIrslB`
- Connection: `redis://default:ySTCqQpbNuYVFfJwIIIeqkRgkTvIrslB@caboose.proxy.rlwy.net:26002`

### ✅ NoSQL Database
**MongoDB** (Optional)
- Host: `interchange.proxy.rlwy.net:20886`
- Username: `mongo`
- Password: `PoDrVaTRkDBGaQbeaTpjELWkWPIbxvMz`
- Connection: `mongodb://mongo:PoDrVaTRkDBGaQbeaTpjELWkWPIbxvMz@interchange.proxy.rlwy.net:20886`

### ✅ Object Storage
**S3-Compatible (optimized-bin)**
- Endpoint: `https://storage.railway.app`
- Bucket: `optimized-bin-yvjb9vxnhq1`
- Access Key: `tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD`
- Secret Key: `tsec_KnsFqr0JZOsYqQl1LRMMo46kqAbGVcHgR-vADqBcGxAbQzmt44MakhvpYceOi3Z7ggUnC9`

---

## 🚀 Deploy to Railway - Quick Start

### Step 1: Create API Service in Railway

1. Go to Railway Dashboard
2. Click **"New" → "Empty Service"**
3. Name it: **"grc-api"**

### Step 2: Add Environment Variables

Copy all variables from `.env.railway` file:

**Required Variables:**
```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Database - Using Primary PostgreSQL
ConnectionStrings__Default=Host=mainline.proxy.rlwy.net;Port=46662;Database=railway;Username=postgres;Password=sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ;SSL Mode=Require;Trust Server Certificate=true

# Redis
Redis__Configuration=caboose.proxy.rlwy.net:26002,password=ySTCqQpbNuYVFfJwIIIeqkRgkTvIrslB,ssl=true,abortConnect=false

# S3 Storage
S3__Endpoint=https://storage.railway.app
S3__BucketName=optimized-bin-yvjb9vxnhq1
S3__AccessKeyId=tid_NjtZXPqCgdJPDgZIwAdsFThHeqPwtBIrRyIetsqjHHCuMnwiCD
S3__SecretAccessKey=tsec_KnsFqr0JZOsYqQl1LRMMo46kqAbGVcHgR-vADqBcGxAbQzmt44MakhvpYceOi3Z7ggUnC9

# App URLs (Update after deployment)
App__SelfUrl=https://grc-api.up.railway.app
App__CorsOrigins=https://grc-web.up.railway.app

# JWT Secret (Generate a strong random string)
Jwt__SecretKey=GENERATE_A_STRONG_SECRET_KEY_HERE_AT_LEAST_32_CHARACTERS
```

### Step 3: Deploy Using Railway CLI

```bash
# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# Link to your project
cd /root/app.shahin-ai.com/Shahin-ai
railway link

# Deploy
railway up
```

### Step 4: Run Database Migrations

After deployment:

```bash
# Connect to your Railway project
railway link

# Run migrations
railway run dotnet ef database update

# Seed products
railway run dotnet run --seed
```

---

## 📝 Alternative: Deploy via GitHub

### Option 1: Connect GitHub Repository

1. **Push code to GitHub**:
   ```bash
   cd /root/app.shahin-ai.com/Shahin-ai
   git add .
   git commit -m "Add Railway deployment configuration"
   git push origin main
   ```

2. **In Railway Dashboard**:
   - Click "New" → "GitHub Repo"
   - Select your repository
   - Railway will auto-detect the Dockerfile

3. **Configure Build**:
   - Root Directory: `/`
   - Dockerfile Path: `Dockerfile`
   - Railway will automatically use `railway.json`

4. **Add Environment Variables** (from Step 2 above)

5. **Deploy** - Railway will build and deploy automatically

---

## 🔧 Configuration Files Created

1. **`.env.railway`** - All environment variables with your actual credentials
2. **`Dockerfile`** - Multi-stage build for .NET API
3. **`railway.json`** - Railway deployment configuration
4. **`railway-production-config.json`** - Application settings template
5. **`railway-deploy.sh`** - Automated deployment script

---

## 📊 Deployment Architecture

```
┌─────────────────────────────────────────────┐
│           Railway Platform                  │
├─────────────────────────────────────────────┤
│                                             │
│  ┌──────────────┐      ┌──────────────┐    │
│  │   grc-api    │──────│  PostgreSQL  │    │
│  │  (.NET API)  │      │   (Primary)  │    │
│  └──────┬───────┘      └──────────────┘    │
│         │                                   │
│         ├──────────────┐  ┌──────────────┐ │
│         │    Redis     │  │  PostgreSQL  │ │
│         │   (Cache)    │  │  (Secondary) │ │
│         └──────────────┘  └──────────────┘ │
│         │                                   │
│         ├──────────────┐  ┌──────────────┐ │
│         │  S3 Storage  │  │   MongoDB    │ │
│         │(optimized-bin)│  │  (Optional)  │ │
│         └──────────────┘  └──────────────┘ │
│                                             │
│  ┌──────────────┐                          │
│  │   grc-web    │                          │
│  │  (Angular)   │                          │
│  └──────────────┘                          │
└─────────────────────────────────────────────┘
```

---

## ✅ Pre-Deployment Checklist

- [x] PostgreSQL database available
- [x] Redis cache available
- [x] S3 storage configured
- [x] MongoDB available (optional)
- [x] Credentials documented
- [x] Environment variables file created
- [x] Dockerfile created
- [x] Railway.json created
- [ ] Code pushed to GitHub (optional)
- [ ] Environment variables set in Railway
- [ ] API service deployed
- [ ] Migrations run
- [ ] Seed data loaded
- [ ] Web app deployed

---

## 🎯 Quick Deploy Commands

### Using Railway CLI

```bash
# From project root
cd /root/app.shahin-ai.com/Shahin-ai

# Login and link
railway login
railway link

# Deploy API
railway up

# View status
railway status

# View logs
railway logs

# Run migrations
railway run dotnet ef database update

# Seed data
railway run dotnet run --seed
```

---

## 🔐 Security Notes

✅ **Credentials Configured**:
- All Railway service credentials documented
- S3 credentials ready
- Environment variables prepared

⚠️ **Important**:
- Never commit `.env.railway` to Git
- Add to `.gitignore`
- Use Railway's environment variable UI to set secrets
- Rotate credentials regularly

---

## 📱 Mobile App (Angular PWA)

Deploy the Angular app as a separate Railway service:

1. **Build locally**:
   ```bash
   cd angular
   npm install
   npm run build -- --configuration production
   ```

2. **Deploy to Railway**:
   - Create new service: `grc-web`
   - Use Nginx or Node.js static server
   - Point to `dist/` folder

Or use **Railway static hosting** for the built Angular app.

---

## 🧪 Test Railway Deployment

### After Deployment

1. **Check API Health**:
   ```bash
   curl https://grc-api.up.railway.app/health
   ```

2. **Test API Endpoint**:
   ```bash
   curl https://grc-api.up.railway.app/api/grc/products
   ```

3. **View Logs**:
   ```bash
   railway logs --service grc-api
   ```

---

## 📊 Monitoring

### Railway Dashboard
- View deployment status
- Monitor resource usage
- Check logs
- View metrics

### Application Health
- Health endpoint: `https://grc-api.up.railway.app/health`
- Ready endpoint: `https://grc-api.up.railway.app/health/ready`

---

## 🆘 Troubleshooting

### Deployment Fails
```bash
# Check logs
railway logs

# Check status
railway status

# Redeploy
railway up --detach
```

### Database Connection Issues
1. Verify connection string in Railway environment variables
2. Check PostgreSQL service is running
3. Verify SSL Mode is set to Require
4. Test connection: `railway connect Postgres`

### S3 Storage Issues
1. Verify bucket name is correct
2. Check access credentials
3. Ensure endpoint URL is `https://storage.railway.app`

---

## ✨ Summary

**Railway Infrastructure**: ✅ Ready
- PostgreSQL (2 instances)
- Redis
- S3 Storage
- MongoDB (optional)

**GRC Platform Code**: ✅ Complete
- 265+ files
- All 42 tasks

**Configuration**: ✅ Ready
- `.env.railway` with all credentials
- `Dockerfile` for Railway
- `railway.json` configuration

**Next Step**: Deploy using `railway up` or connect GitHub repository

---

**Ready to deploy!** 🚀

See: [railway-deployment-guide.md](railway-deployment-guide.md) for detailed instructions.

