#!/bin/bash
# Apply Security Fixes - Restart Containers with Secure Configuration
# This script restarts containers to apply the new secure port configuration

set -e

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║     Applying Security Fixes - Restarting Containers          ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

cd /home/Shahin-ai/Shahin-Jan-2026

echo "📋 Step 1: Checking current port exposure..."
EXPOSED=$(netstat -tulpn | grep LISTEN | grep -E ":5432|:6379" | wc -l)
if [ "$EXPOSED" -gt 0 ]; then
    echo "⚠️  Found $EXPOSED database ports still exposed"
    echo "   This is from running containers with old configuration"
else
    echo "✅ No database ports exposed"
fi
echo ""

echo "📋 Step 2: Restarting GRC containers with secure config..."
docker-compose -f docker-compose.grcmvc.yml down
docker-compose -f docker-compose.grcmvc.yml up -d
echo ""

echo "📋 Step 3: Checking for other containers exposing database ports..."
OTHER_CONTAINERS=$(docker ps --format "{{.Names}}" | grep -E "shahin-postgres|shahin-redis" || true)
if [ -n "$OTHER_CONTAINERS" ]; then
    echo "⚠️  Found other containers: $OTHER_CONTAINERS"
    echo "   These may need to be restarted separately"
    echo "   Check: docker-compose -f docker-compose.production.yml down && up -d"
else
    echo "✅ No other containers found"
fi
echo ""

echo "📋 Step 4: Verifying security..."
sleep 3
./scripts/security-check-ports.sh
echo ""

echo "📋 Step 5: Checking exposed ports after restart..."
EXPOSED_AFTER=$(netstat -tulpn | grep LISTEN | grep -E "0.0.0.0:5432|0.0.0.0:6379" | wc -l)
if [ "$EXPOSED_AFTER" -gt 0 ]; then
    echo "⚠️  Still exposed: $EXPOSED_AFTER ports"
    echo "   These may be from other docker-compose files"
    echo "   Run: docker-compose -f docker-compose.production.yml down && up -d"
else
    echo "✅ No database ports exposed publicly"
fi
echo ""

echo "✅ Security fixes applied!"
echo ""
echo "📝 Next steps:"
echo "   1. Restart other containers if needed"
echo "   2. Reload Nginx: sudo systemctl reload nginx"
echo "   3. Test application: curl http://localhost:5137/health"
