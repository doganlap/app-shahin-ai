#!/bin/bash
# Full Stack Production Deployment Script
# Deploy to: 46.224.68.73 (portal.shahin-ai.com)

set -e

# Configuration
SERVER_IP="46.224.68.73"
SERVER_USER="root"
DEPLOY_PATH="/opt/grc-system"
DOMAIN="portal.shahin-ai.com"

echo "🚀 GRC System - Full Stack Production Deployment"
echo "================================================"
echo "Server: $SERVER_IP"
echo "Domain: $DOMAIN"
echo ""

# Step 1: Clean and rebuild
echo "[1/5] Building application..."
cd src/GrcMvc
rm -rf publish bin/Release obj/Release
dotnet publish -c Release -o ./publish --no-incremental
echo "✅ Build complete"
cd ../..

# Step 2: Create deployment package
echo ""
echo "[2/5] Creating deployment package..."
cd src/GrcMvc/publish
tar -czf ../../../grc-deploy-$(date +%Y%m%d-%H%M%S).tar.gz .
cd ../../..
echo "✅ Package created"

# Step 3: Show deployment summary
echo ""
echo "[3/5] Deployment Summary:"
echo "  📦 Build output: src/GrcMvc/publish/"
echo "  🌐 Target server: $SERVER_IP"
echo "  📍 Domain: $DOMAIN"
echo ""
echo "DNS Configuration (already set):"
echo "  ✅ shahin-ai.com → $SERVER_IP"
echo "  ✅ portal.shahin-ai.com → $SERVER_IP"
echo "  ✅ app.shahin-ai.com → $SERVER_IP"
echo "  ✅ login.shahin-ai.com → $SERVER_IP"
echo ""

echo "[4/5] Next steps (manual or via SSH):"
echo "  1. Transfer files to server:"
echo "     scp -r src/GrcMvc/publish/* $SERVER_USER@$SERVER_IP:$DEPLOY_PATH/"
echo ""
echo "  2. On server, restart service:"
echo "     ssh $SERVER_USER@$SERVER_IP 'cd $DEPLOY_PATH && systemctl restart grcmvc'"
echo ""

echo "[5/5] ✅ Build complete - Ready for deployment!"
echo ""
echo "📋 Changes included in this deployment:"
echo "  ✅ Full i18n implementation (all text dynamic)"
echo "  ✅ Language switcher (global)"
echo "  ✅ Chat widget localization"
echo "  ✅ Claude AI agent integration"
echo "  ✅ All accessibility attributes localized"
