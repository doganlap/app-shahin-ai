#!/bin/bash
# Secure Docker Ports - Remove Public Exposure of Internal Services
# Only keep Nginx (80/443) and GRC App (5137) exposed
# Everything else should be internal Docker network only

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║     Secure Docker Ports - Docker Only Configuration           ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

echo "📋 Current exposed ports:"
docker ps --format "{{.Names}}: {{.Ports}}" | grep "0.0.0.0"
echo ""

echo "🔒 Steps to secure:"
echo ""
echo "1. ✅ GRC App on port 5137: Already running (OK)"
echo "2. ⚠️  PostgreSQL port 5432: Should be INTERNAL only (security risk!)"
echo "3. ⚠️  Redis port 6379: Should be INTERNAL only (security risk!)"
echo "4. ❓ Monitoring ports (3000, 8080, 9090, etc.): Remove if not needed"
echo "5. ✅ Nginx ports 80/443: Keep (main entry point)"
echo ""

echo "📝 To secure PostgreSQL and Redis:"
echo ""
echo "Edit docker-compose files and REMOVE these port mappings:"
echo "  - Remove: '5432:5432' from postgres service"
echo "  - Remove: '6379:6379' from redis service"
echo ""
echo "They will still work inside Docker network, just not publicly exposed."
echo ""

echo "✅ Recommended configuration:"
echo "  - Port 80/443 (Nginx): Public (main entry point)"
echo "  - Port 5137 (GRC App): Can be internal (access via Nginx proxy)"
echo "  - All databases: INTERNAL ONLY (Docker network)"
echo "  - Monitoring: INTERNAL ONLY or remove if not needed"
echo ""

echo "🔐 Security benefits:"
echo "  ✅ Databases not accessible from internet"
echo "  ✅ Single entry point (Nginx)"
echo "  ✅ Better firewall rules (only allow 80/443)"
echo "  ✅ Docker network isolation"
echo ""
