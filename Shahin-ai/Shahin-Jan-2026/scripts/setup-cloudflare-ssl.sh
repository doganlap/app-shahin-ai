#!/bin/bash
# ══════════════════════════════════════════════════════════════════════════════
# SHAHIN AI GRC - Cloudflare Origin Certificate Setup Script
# ══════════════════════════════════════════════════════════════════════════════
# This script helps you set up Cloudflare Origin Certificate for SSL
# ══════════════════════════════════════════════════════════════════════════════

set -e

NGINX_SSL_DIR="/etc/nginx/ssl"
BACKUP_DIR="/etc/nginx/ssl/backup-$(date +%Y%m%d-%H%M%S)"

echo "══════════════════════════════════════════════════════════════════════════════"
echo "Cloudflare Origin Certificate Setup"
echo "══════════════════════════════════════════════════════════════════════════════"
echo ""

# Create backup directory
mkdir -p "$BACKUP_DIR"

# Backup existing certificates
if [ -f "$NGINX_SSL_DIR/fullchain.pem" ]; then
    echo "📦 Backing up existing certificates..."
    sudo cp "$NGINX_SSL_DIR/fullchain.pem" "$BACKUP_DIR/fullchain.pem.backup"
    sudo cp "$NGINX_SSL_DIR/privkey.pem" "$BACKUP_DIR/privkey.pem.backup"
    echo "✅ Backup created in: $BACKUP_DIR"
    echo ""
fi

echo "📋 INSTRUCTIONS:"
echo "══════════════════════════════════════════════════════════════════════════════"
echo ""
echo "1. Go to Cloudflare Dashboard: https://dash.cloudflare.com"
echo "2. Select domain: shahin-ai.com"
echo "3. Navigate to: SSL/TLS → Origin Server"
echo "4. Click: 'Create Certificate'"
echo "5. Configure:"
echo "   - Hostnames: shahin-ai.com, *.shahin-ai.com"
echo "   - Validity: 15 years"
echo "   - Private key type: RSA (2048)"
echo "6. Click: 'Create'"
echo "7. Copy the 'Origin Certificate' and 'Private Key'"
echo ""
echo "══════════════════════════════════════════════════════════════════════════════"
echo ""
read -p "Press Enter when you have copied the certificate and key from Cloudflare..."

echo ""
echo "📝 Paste the Origin Certificate (including BEGIN/END lines):"
echo "   (Press Ctrl+D when done)"
echo ""
read -d '' -r CERT || true

echo ""
echo "📝 Paste the Private Key (including BEGIN/END lines):"
echo "   (Press Ctrl+D when done)"
echo ""
read -d '' -r KEY || true

# Validate certificate format
if ! echo "$CERT" | grep -q "BEGIN CERTIFICATE"; then
    echo "❌ Error: Certificate format invalid. Must include BEGIN/END CERTIFICATE lines."
    exit 1
fi

if ! echo "$KEY" | grep -q "BEGIN.*PRIVATE KEY"; then
    echo "❌ Error: Private key format invalid. Must include BEGIN/END PRIVATE KEY lines."
    exit 1
fi

# Save certificate files
echo ""
echo "💾 Saving certificate files..."
echo "$CERT" | sudo tee "$NGINX_SSL_DIR/fullchain.pem" > /dev/null
echo "$KEY" | sudo tee "$NGINX_SSL_DIR/privkey.pem" > /dev/null

# Set permissions
sudo chmod 600 "$NGINX_SSL_DIR/privkey.pem"
sudo chmod 644 "$NGINX_SSL_DIR/fullchain.pem"

echo "✅ Certificate files saved"
echo ""

# Verify certificate
echo "🔍 Verifying certificate..."
if openssl x509 -in "$NGINX_SSL_DIR/fullchain.pem" -text -noout > /dev/null 2>&1; then
    echo "✅ Certificate is valid"
    openssl x509 -in "$NGINX_SSL_DIR/fullchain.pem" -text -noout | grep -E "Issuer:|Subject:|Not After" | head -3
else
    echo "❌ Error: Certificate validation failed"
    exit 1
fi

# Test nginx configuration
echo ""
echo "🧪 Testing nginx configuration..."
if sudo nginx -t; then
    echo "✅ Nginx configuration is valid"
else
    echo "❌ Error: Nginx configuration test failed"
    exit 1
fi

# Reload nginx
echo ""
echo "🔄 Reloading nginx..."
sudo systemctl reload nginx
echo "✅ Nginx reloaded"

# Test HTTPS
echo ""
echo "🌐 Testing HTTPS connection..."
if curl -s -I https://www.shahin-ai.com 2>&1 | grep -q "HTTP"; then
    echo "✅ HTTPS is working"
else
    echo "⚠️  Warning: HTTPS test failed (may need to wait for DNS propagation)"
fi

echo ""
echo "══════════════════════════════════════════════════════════════════════════════"
echo "✅ Setup Complete!"
echo "══════════════════════════════════════════════════════════════════════════════"
echo ""
echo "Next steps:"
echo "1. Visit https://www.shahin-ai.com in your browser"
echo "2. Verify you see a green padlock (✅)"
echo "3. If you still see errors, clear browser cache (Ctrl+Shift+Delete)"
echo ""
echo "Backup location: $BACKUP_DIR"
echo ""
