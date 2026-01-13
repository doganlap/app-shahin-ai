#!/bin/bash
# ============================================
# SSL Certificate Setup with Caddy
# ============================================
# Automatically configures Let's Encrypt SSL
# certificates for all shahin-ai.com domains
# ============================================

set -e

echo "🔐 SSL Certificate Setup with Caddy"
echo "===================================="
echo ""

# Step 1: Create Caddyfile
echo "📝 Step 1: Creating Caddyfile..."
cat > /etc/caddy/Caddyfile << 'EOF'
# ============================================
# Caddy Configuration for Shahin GRC
# Automatic HTTPS with Let's Encrypt
# ============================================

# Main application domains
portal.shahin-ai.com, app.shahin-ai.com {
    # Reverse proxy to Docker container
    reverse_proxy localhost:8888 {
        # Forward original headers
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}

        # Health check
        health_uri /health/ready
        health_interval 30s
        health_timeout 5s
    }

    # Security headers
    header {
        # Enable HSTS
        Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
        # Prevent clickjacking
        X-Frame-Options "SAMEORIGIN"
        # Prevent MIME sniffing
        X-Content-Type-Options "nosniff"
        # XSS protection
        X-XSS-Protection "1; mode=block"
        # Referrer policy
        Referrer-Policy "strict-origin-when-cross-origin"
    }

    # Logging
    log {
        output file /var/log/caddy/access.log
        format json
    }
}

# Landing page
shahin-ai.com, www.shahin-ai.com {
    # Reverse proxy to Docker container
    reverse_proxy localhost:8888 {
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}
    }

    # Security headers
    header {
        Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
        X-Frame-Options "SAMEORIGIN"
        X-Content-Type-Options "nosniff"
        X-XSS-Protection "1; mode=block"
        Referrer-Policy "strict-origin-when-cross-origin"
    }

    log {
        output file /var/log/caddy/landing.log
        format json
    }
}

# Login subdomain
login.shahin-ai.com {
    # Reverse proxy to Docker container
    reverse_proxy localhost:8888 {
        header_up Host {host}
        header_up X-Real-IP {remote}
        header_up X-Forwarded-For {remote}
        header_up X-Forwarded-Proto {scheme}
    }

    # Security headers
    header {
        Strict-Transport-Security "max-age=31536000; includeSubDomains; preload"
        X-Frame-Options "SAMEORIGIN"
        X-Content-Type-Options "nosniff"
        X-XSS-Protection "1; mode=block"
        Referrer-Policy "strict-origin-when-cross-origin"
    }

    log {
        output file /var/log/caddy/login.log
        format json
    }
}

# Global options
{
    # Email for Let's Encrypt notifications
    email admin@shahin-ai.com

    # Automatic HTTPS
    auto_https on

    # Use Let's Encrypt production
    # Remove 'staging' after testing
    # acme_ca https://acme-staging-v02.api.letsencrypt.org/directory
}
EOF

echo "✅ Caddyfile created"
echo ""

# Step 2: Create log directory
echo "📁 Step 2: Creating log directory..."
mkdir -p /var/log/caddy
chown caddy:caddy /var/log/caddy
echo "✅ Log directory created"
echo ""

# Step 3: Validate Caddyfile
echo "🔍 Step 3: Validating Caddyfile..."
if caddy validate --config /etc/caddy/Caddyfile; then
    echo "✅ Caddyfile is valid"
else
    echo "❌ ERROR: Caddyfile validation failed"
    exit 1
fi
echo ""

# Step 4: Enable and start Caddy
echo "🚀 Step 4: Starting Caddy service..."
systemctl enable caddy 2>/dev/null || true
systemctl restart caddy

# Wait for Caddy to start
sleep 5

if systemctl is-active --quiet caddy; then
    echo "✅ Caddy is running"
else
    echo "❌ ERROR: Caddy failed to start"
    echo "Checking logs:"
    journalctl -u caddy --no-pager -n 50
    exit 1
fi
echo ""

# Step 5: Check SSL certificate provisioning
echo "🔐 Step 5: Checking SSL certificates..."
echo "Note: Let's Encrypt may take 1-2 minutes to issue certificates"
echo ""

# Wait for certificates
MAX_WAIT=120
WAIT_TIME=0
while [ $WAIT_TIME -lt $MAX_WAIT ]; do
    if [ -d "/var/lib/caddy/certificates" ] && [ "$(ls -A /var/lib/caddy/certificates 2>/dev/null)" ]; then
        echo "✅ SSL certificates are being provisioned"
        break
    fi
    echo "  Waiting for certificates... ($WAIT_TIME/$MAX_WAIT seconds)"
    sleep 10
    WAIT_TIME=$((WAIT_TIME + 10))
done

echo ""

# Step 6: Test HTTPS endpoints
echo "🧪 Step 6: Testing HTTPS endpoints..."
echo ""

test_https() {
    local domain=$1
    echo "Testing https://$domain..."

    if curl -s -o /dev/null -w "%{http_code}" --max-time 10 "https://$domain/health/ready" | grep -q "200\|301\|302"; then
        echo "  ✅ https://$domain is accessible"
        return 0
    else
        echo "  ⚠️  https://$domain not ready yet (this may be normal during initial setup)"
        return 1
    fi
}

# Test each domain
test_https "portal.shahin-ai.com" || true
test_https "app.shahin-ai.com" || true
test_https "shahin-ai.com" || true
test_https "www.shahin-ai.com" || true
test_https "login.shahin-ai.com" || true

echo ""

# Step 7: Display status
echo "=========================================="
echo "✅ SSL SETUP COMPLETE"
echo "=========================================="
echo ""
echo "📊 Status:"
systemctl status caddy --no-pager -l
echo ""
echo "🔗 Your domains with HTTPS:"
echo "  - https://portal.shahin-ai.com"
echo "  - https://app.shahin-ai.com"
echo "  - https://shahin-ai.com"
echo "  - https://www.shahin-ai.com"
echo "  - https://login.shahin-ai.com"
echo ""
echo "📋 Next steps:"
echo "  1. Wait 1-2 minutes for Let's Encrypt to issue certificates"
echo "  2. Test in browser: https://portal.shahin-ai.com"
echo "  3. Check Caddy logs: journalctl -u caddy -f"
echo "  4. View certificates: ls -la /var/lib/caddy/certificates/"
echo ""
echo "🔧 Useful commands:"
echo "  - Reload config: systemctl reload caddy"
echo "  - Check status: systemctl status caddy"
echo "  - View logs: journalctl -u caddy -f"
echo "  - Test config: caddy validate --config /etc/caddy/Caddyfile"
echo ""
echo "⚠️  IMPORTANT:"
echo "  - Ensure DNS records point to this server's IP"
echo "  - Ports 80 and 443 must be open in firewall"
echo "  - Let's Encrypt requires valid DNS resolution"
echo ""

exit 0
