#!/bin/bash
# Deploy Nginx for Public Domain Access
# Domain: shahin-ai.com
# Server IP: 46.224.68.73

set -e

echo "🚀 Starting Nginx Deployment for shahin-ai.com"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo -e "${RED}❌ Please run as root (use sudo)${NC}"
    exit 1
fi

# 1. Install Nginx
echo -e "${YELLOW}📦 Installing Nginx...${NC}"
if ! command -v nginx &> /dev/null; then
    apt-get update
    apt-get install -y nginx
    echo -e "${GREEN}✅ Nginx installed${NC}"
else
    echo -e "${GREEN}✅ Nginx already installed${NC}"
fi

# 2. Backup existing nginx config
echo -e "${YELLOW}💾 Backing up existing nginx configuration...${NC}"
if [ -f /etc/nginx/nginx.conf ]; then
    cp /etc/nginx/nginx.conf /etc/nginx/nginx.conf.backup.$(date +%Y%m%d_%H%M%S)
    echo -e "${GREEN}✅ Backup created${NC}"
fi

# 3. Create SSL directory
echo -e "${YELLOW}📁 Creating SSL certificate directory...${NC}"
mkdir -p /etc/nginx/ssl
chmod 755 /etc/nginx/ssl
echo -e "${GREEN}✅ SSL directory created${NC}"

# 4. Copy nginx configuration
echo -e "${YELLOW}📋 Copying nginx configuration...${NC}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [ -f "$SCRIPT_DIR/nginx/nginx.conf" ]; then
    cp "$SCRIPT_DIR/nginx/nginx.conf" /etc/nginx/nginx.conf
    echo -e "${GREEN}✅ Configuration copied${NC}"
else
    echo -e "${RED}❌ nginx.conf not found in $SCRIPT_DIR/nginx/${NC}"
    exit 1
fi

# 5. Check SSL certificates
echo -e "${YELLOW}🔒 Checking SSL certificates...${NC}"
if [ -f /etc/nginx/ssl/fullchain.pem ] && [ -f /etc/nginx/ssl/privkey.pem ]; then
    echo -e "${GREEN}✅ SSL certificates found${NC}"
else
    echo -e "${YELLOW}⚠️  SSL certificates not found${NC}"
    echo -e "${YELLOW}   You can either:${NC}"
    echo -e "${YELLOW}   1. Copy existing certificates to /etc/nginx/ssl/${NC}"
    echo -e "${YELLOW}   2. Use Let's Encrypt (certbot)${NC}"
    echo ""
    read -p "Do you want to set up Let's Encrypt certificates now? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        # Install certbot
        if ! command -v certbot &> /dev/null; then
            apt-get install -y certbot python3-certbot-nginx
        fi
        
        echo -e "${YELLOW}📝 Running certbot...${NC}"
        certbot --nginx -d shahin-ai.com -d www.shahin-ai.com -d app.shahin-ai.com -d portal.shahin-ai.com -d login.shahin-ai.com --non-interactive --agree-tos --email admin@shahin-ai.com
        
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ SSL certificates obtained${NC}"
        else
            echo -e "${RED}❌ Failed to obtain SSL certificates${NC}"
            echo -e "${YELLOW}   Make sure DNS records are configured correctly${NC}"
        fi
    else
        echo -e "${YELLOW}⚠️  Skipping SSL setup. You'll need to configure certificates manually.${NC}"
    fi
fi

# 6. Test nginx configuration
echo -e "${YELLOW}🧪 Testing nginx configuration...${NC}"
if nginx -t; then
    echo -e "${GREEN}✅ Configuration test passed${NC}"
else
    echo -e "${RED}❌ Configuration test failed${NC}"
    exit 1
fi

# 7. Configure firewall
echo -e "${YELLOW}🔥 Configuring firewall...${NC}"
if command -v ufw &> /dev/null; then
    ufw allow 80/tcp
    ufw allow 443/tcp
    echo -e "${GREEN}✅ Firewall configured${NC}"
else
    echo -e "${YELLOW}⚠️  UFW not found. Please configure firewall manually.${NC}"
fi

# 8. Start/Reload nginx
echo -e "${YELLOW}🔄 Starting/Reloading nginx...${NC}"
systemctl enable nginx
if systemctl is-active --quiet nginx; then
    systemctl reload nginx
    echo -e "${GREEN}✅ Nginx reloaded${NC}"
else
    systemctl start nginx
    echo -e "${GREEN}✅ Nginx started${NC}"
fi

# 9. Verify nginx is running
echo -e "${YELLOW}✅ Verifying nginx status...${NC}"
if systemctl is-active --quiet nginx; then
    echo -e "${GREEN}✅ Nginx is running${NC}"
else
    echo -e "${RED}❌ Nginx failed to start${NC}"
    systemctl status nginx
    exit 1
fi

# 10. Test local access
echo -e "${YELLOW}🧪 Testing local access...${NC}"
sleep 2
if curl -s -o /dev/null -w "%{http_code}" http://localhost/health | grep -q "200\|301\|302"; then
    echo -e "${GREEN}✅ Local access working${NC}"
else
    echo -e "${YELLOW}⚠️  Local access test failed (this might be normal if app is still starting)${NC}"
fi

echo ""
echo -e "${GREEN}🎉 Nginx deployment complete!${NC}"
echo ""
echo "📋 Next Steps:"
echo "   1. Verify DNS records point to 46.224.68.73"
echo "   2. Wait for DNS propagation (can take up to 48 hours)"
echo "   3. Test public access: https://shahin-ai.com"
echo "   4. Check nginx logs: tail -f /var/log/nginx/error.log"
echo ""
echo "🌐 Public URLs:"
echo "   - https://shahin-ai.com"
echo "   - https://app.shahin-ai.com"
echo "   - https://portal.shahin-ai.com"
echo "   - https://login.shahin-ai.com"
echo ""
