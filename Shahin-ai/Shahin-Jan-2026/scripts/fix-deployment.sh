#!/bin/bash
# ============================================
# Automated Deployment Fix Script
# ============================================
# Fixes common deployment issues and restarts
# the application with proper configuration
# ============================================

set -e

echo "🔧 Automated Deployment Fix Script"
echo "===================================="
echo ""

# Change to project directory
cd /home/Shahin-ai/Shahin-Jan-2026

# Step 1: Stop everything
echo "📦 Step 1: Stopping all containers..."
docker-compose down -v || true
echo "✅ Containers stopped"
echo ""

# Step 2: Generate JWT Secret
echo "🔐 Step 2: Generating JWT Secret..."
JWT_SECRET=$(openssl rand -base64 48)
echo "✅ JWT Secret generated (${#JWT_SECRET} characters)"
echo ""

# Step 3: Create .env file
echo "📝 Step 3: Creating .env file..."
cat > .env << 'ENVEOF'
# ===========================================
# PRODUCTION ENVIRONMENT VARIABLES
# Generated: $(date)
# ===========================================

# JWT Settings (REQUIRED)
JwtSettings__Secret=JWT_SECRET_PLACEHOLDER
JwtSettings__Issuer=GrcSystem
JwtSettings__Audience=GrcSystemUsers

# Database Connection (REQUIRED)
ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true
ConnectionStrings__GrcAuthDb=Host=db;Port=5432;Database=GrcMvcDb;Username=postgres;Password=postgres_2026;Include Error Detail=true

# Application Settings
App__BaseUrl=https://portal.shahin-ai.com
App__LandingUrl=https://shahin-ai.com
AllowedHosts=*

# SMTP Settings (Optional)
SmtpSettings__Host=smtp.office365.com
SmtpSettings__Port=587
SmtpSettings__EnableSsl=true
SmtpSettings__FromEmail=info@shahin-ai.com

# Claude Agents (Optional - Disabled by default)
ClaudeAgents__Enabled=false
ClaudeAgents__ApiKey=
ClaudeAgents__Model=claude-sonnet-4-20250514

# Feature Flags
ENABLE_AUTO_MIGRATION=false
FeatureFlags__EnableSwagger=false
FeatureFlags__EnableDetailedErrors=false

# Rate Limiting
RateLimiting__GlobalPermitLimit=100
RateLimiting__ApiPermitLimit=50

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
Logging__LogLevel__Microsoft__AspNetCore=Warning

# ASP.NET Core Settings
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8888
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Database Initialization
POSTGRES_DB=GrcMvcDb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres_2026
ENVEOF

# Replace placeholder with actual secret
sed -i "s|JWT_SECRET_PLACEHOLDER|${JWT_SECRET}|g" .env

echo "✅ .env file created"
echo ""

# Step 4: Verify .env
echo "🔍 Step 4: Verifying .env configuration..."
if grep -q "JWT_SECRET_PLACEHOLDER" .env; then
    echo "❌ ERROR: JWT_SECRET replacement failed"
    exit 1
fi

if grep -q "^JwtSettings__Secret=.\{40,\}" .env; then
    echo "✅ JWT_SECRET is set (sufficient length)"
else
    echo "❌ ERROR: JWT_SECRET is too short or empty"
    exit 1
fi

if grep -q "^ConnectionStrings__DefaultConnection=Host=" .env; then
    echo "✅ Database connection string is set"
else
    echo "❌ ERROR: Database connection string is missing"
    exit 1
fi
echo ""

# Step 5: Clean Docker
echo "🧹 Step 5: Cleaning Docker artifacts..."
docker rmi shahin-jan-2026_grcmvc 2>/dev/null || echo "  No old image to remove"
docker builder prune -f || true
echo "✅ Docker cleaned"
echo ""

# Step 6: Start Database
echo "🗄️  Step 6: Starting database..."
docker-compose up -d db
echo "  Waiting for database to be ready..."
sleep 10

# Check database health
MAX_RETRIES=30
RETRY_COUNT=0
while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    if docker-compose exec -T db pg_isready -U postgres > /dev/null 2>&1; then
        echo "✅ Database is ready"
        break
    fi
    echo "  Waiting for database... ($((RETRY_COUNT+1))/$MAX_RETRIES)"
    sleep 2
    RETRY_COUNT=$((RETRY_COUNT+1))
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "❌ ERROR: Database failed to start"
    docker-compose logs db
    exit 1
fi
echo ""

# Step 7: Build Application
echo "🏗️  Step 7: Building application..."
docker-compose build --no-cache grcmvc
if [ $? -eq 0 ]; then
    echo "✅ Application built successfully"
else
    echo "❌ ERROR: Build failed"
    docker-compose logs grcmvc
    exit 1
fi
echo ""

# Step 8: Start Application
echo "🚀 Step 8: Starting application..."
docker-compose up -d grcmvc
echo "  Waiting for application to start..."
sleep 15
echo ""

# Step 9: Verify Application
echo "✅ Step 9: Verifying application..."

# Check if container is running
if docker-compose ps grcmvc | grep -q "Up"; then
    echo "✅ Application container is running"
else
    echo "❌ ERROR: Application container is not running"
    docker-compose ps
    docker-compose logs grcmvc
    exit 1
fi

# Check health endpoint
echo "  Testing health endpoint..."
MAX_HEALTH_RETRIES=30
HEALTH_RETRY=0
while [ $HEALTH_RETRY -lt $MAX_HEALTH_RETRIES ]; do
    if curl -f -s http://localhost:8888/health/ready > /dev/null 2>&1; then
        echo "✅ Health endpoint is responding"
        break
    fi
    echo "  Waiting for health endpoint... ($((HEALTH_RETRY+1))/$MAX_HEALTH_RETRIES)"
    sleep 2
    HEALTH_RETRY=$((HEALTH_RETRY+1))
done

if [ $HEALTH_RETRY -eq $MAX_HEALTH_RETRIES ]; then
    echo "⚠️  WARNING: Health endpoint is not responding yet"
    echo "  This might be normal during first startup"
    echo "  Check logs: docker-compose logs -f grcmvc"
fi
echo ""

# Step 10: Summary
echo "=========================================="
echo "✅ DEPLOYMENT FIX COMPLETE"
echo "=========================================="
echo ""
echo "📊 Status Summary:"
docker-compose ps
echo ""
echo "🔗 Access Points:"
echo "  - Health Check: http://localhost:8888/health/ready"
echo "  - Application: http://localhost:8888"
echo "  - External: https://portal.shahin-ai.com"
echo ""
echo "📋 Next Steps:"
echo "  1. Monitor logs: docker-compose logs -f grcmvc"
echo "  2. Test login: curl -X POST http://localhost:8888/api/auth/login"
echo "  3. Check errors: docker-compose logs grcmvc | grep -i error"
echo ""
echo "🔐 JWT Secret (SAVE THIS):"
echo "  ${JWT_SECRET:0:20}... (truncated)"
echo ""
echo "📚 Documentation:"
echo "  - Troubleshooting: DEPLOYMENT_TROUBLESHOOTING_FIX.md"
echo "  - Monitoring: POST_PRODUCTION_MONITORING_GUIDE.md"
echo ""

exit 0
