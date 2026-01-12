# GRC Platform - Production Deployment Guide

## Current Status

This repository contains the **complete specification and codebase** for the GRC Platform SaaS application. All 42 tasks across Phases 3, 4, and 5 have been completed (100%).

## What's Included

### ✅ Backend (.NET 8 / C#)
- Domain entities and aggregates
- Application services
- API controllers
- EF Core configurations
- Repositories
- Event handlers
- Quota enforcement system
- All business logic

### ✅ Frontend (Angular 17+)
- Components (Product List, Subscription Management, etc.)
- Services (Product, Subscription, etc.)
- Models and DTOs
- PWA configuration
- Responsive UI

### ✅ Infrastructure
- Kubernetes manifests
- Docker configurations
- CI/CD scripts
- Database migrations
- Seed data

### ✅ Testing & Security
- k6 performance test scripts
- Security audit tools
- OWASP checklist

## Production Deployment Steps

### Prerequisites

1. **.NET 8 SDK** - Install from: https://dotnet.microsoft.com/download/dotnet/8.0
2. **Node.js 18+** - Install from: https://nodejs.org/
3. **PostgreSQL 15+** - Database server
4. **Redis** - Cache server
5. **MinIO or S3** - Object storage
6. **RabbitMQ** (optional) - Message queue

### Step 1: Setup Infrastructure

#### Using Docker Compose (Recommended for Development)
```bash
cd /root/app.shahin-ai.com/Shahin-ai/release
docker-compose up -d
```

This starts:
- PostgreSQL on port 5432
- Redis on port 6379
- MinIO on ports 9000 (API) and 9001 (Console)

#### Or Manual Setup
- Install PostgreSQL and create database `grc_platform`
- Install Redis
- Install MinIO or configure AWS S3

### Step 2: Build .NET Backend

```bash
cd /root/app.shahin-ai.com/Shahin-ai

# Restore NuGet packages
find src -name "*.csproj" -exec dotnet restore {} \;

# Build all projects
find src -name "*.csproj" -exec dotnet build {} --configuration Release \;

# Publish the API (adjust path if needed)
dotnet publish src/Grc.HttpApi.Host/*.csproj \
    --configuration Release \
    --output /root/app.shahin-ai.com/Shahin-ai/release/api
```

### Step 3: Build Angular Frontend

```bash
cd /root/app.shahin-ai.com/Shahin-ai/angular

# Install dependencies
npm install --legacy-peer-deps

# Build for production
npm run build -- --configuration production

# Output will be in dist/
```

### Step 4: Database Migration

```bash
cd /root/app.shahin-ai.com/Shahin-ai/release/api

# Update connection string in appsettings.Production.json
# Then run migrations
dotnet ef database update
```

### Step 5: Seed Data

```bash
# Run the API with seed flag
dotnet Grc.HttpApi.Host.dll --seed
```

This will create:
- 4 default products (Trial, Standard, Professional, Enterprise)
- Default frameworks
- Initial data

### Step 6: Start the Application

#### API Server
```bash
cd /root/app.shahin-ai.com/Shahin-ai/release

export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5000"

./start-production.sh
```

#### Web Server (Option 1: Using nginx)
```bash
# Copy nginx configuration
sudo cp release/nginx-grc.conf /etc/nginx/sites-available/grc
sudo ln -s /etc/nginx/sites-available/grc /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### Web Server (Option 2: Simple HTTP Server)
```bash
cd /root/app.shahin-ai.com/Shahin-ai/angular/dist
python3 -m http.server 8080
```

### Step 7: Verify Deployment

1. **API Health Check**
   ```bash
   curl http://localhost:5000/health
   ```

2. **Test API Endpoints**
   ```bash
   # Get products
   curl http://localhost:5000/api/grc/products
   ```

3. **Access Web App**
   - Open browser: http://localhost:8080
   - Or: http://localhost (if using nginx)

## Production Kubernetes Deployment

For production, use the provided Kubernetes manifests:

```bash
cd /root/app.shahin-ai.com/Shahin-ai

# Deploy to Kubernetes
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml  # Update secrets first!
kubectl apply -f k8s/deployment-api.yaml
kubectl apply -f k8s/deployment-web.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/ingress.yaml
kubectl apply -f k8s/hpa.yaml
```

Or use the deployment script:
```bash
cd scripts/deployment
./deploy-production.sh production grc-platform
```

## Configuration

### API Configuration (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=grc_platform;Username=grc_user;Password=YOUR_PASSWORD"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "YOUR_ACCESS_KEY",
    "SecretKey": "YOUR_SECRET_KEY"
  }
}
```

### Environment Variables
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__Default="Host=localhost;..."
export Redis__Configuration="localhost:6379"
```

## Performance Testing

```bash
cd scripts/performance
./run-performance-tests.sh load https://api.yourcompany.com YOUR_API_TOKEN
```

## Security Audit

```bash
cd scripts/security
./security-audit.sh https://api.yourcompany.com
```

## Monitoring

### Application Logs
```bash
# View API logs
tail -f /var/log/grc/api.log

# Or if using systemd
journalctl -u grc-api -f
```

### Health Endpoints
- Health: `GET /health`
- Ready: `GET /health/ready`
- Live: `GET /health/live`

## Troubleshooting

### API Won't Start
1. Check database connection
2. Verify PostgreSQL is running
3. Check appsettings.Production.json
4. Review logs

### Database Migration Fails
1. Ensure PostgreSQL is accessible
2. Verify connection string
3. Check user permissions
4. Run manually: `dotnet ef database update --verbose`

### Frontend Build Fails
1. Clear node_modules: `rm -rf node_modules`
2. Clear cache: `npm cache clean --force`
3. Reinstall: `npm install --legacy-peer-deps`
4. Build: `npm run build -- --configuration production`

## Support Files Location

All code files are in: `/root/app.shahin-ai.com/Shahin-ai/`
- Backend: `src/`
- Frontend: `angular/`
- Scripts: `scripts/`
- K8s: `k8s/`
- Docs: `docs/`

## Next Steps

1. Review all code files
2. Customize for your environment
3. Update branding and styling
4. Configure domain and SSL
5. Setup CI/CD pipeline
6. Configure monitoring (Prometheus/Grafana)
7. Setup backup procedures
8. Configure logging aggregation

## Architecture Overview

- **Multi-tenant SaaS** with complete tenant isolation
- **Subscription-based** with quota enforcement
- **Microservices-ready** modular architecture
- **Event-driven** with domain events
- **Cloud-native** Kubernetes deployment
- **Bilingual** English/Arabic support
- **PWA-enabled** mobile-responsive

## Contact & Support

For issues or questions:
1. Review documentation in `docs/`
2. Check scripts in `scripts/`
3. Review `ALL-TASKS-COMPLETE.md` for implementation status

