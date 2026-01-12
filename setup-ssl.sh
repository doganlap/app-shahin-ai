#!/bin/bash

###############################################################################
# SSL Certificate Setup Script for Saudi GRC Application
# This script sets up SSL certificates using Let's Encrypt (Certbot)
###############################################################################

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
DOMAIN_WEB="grc.shahin-ai.com"
DOMAIN_API="api-grc.shahin-ai.com"
EMAIL="admin@shahin-ai.com"  # Update with your email

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_root() {
    if [ "$EUID" -ne 0 ]; then 
        print_error "Please run as root (use sudo)"
        exit 1
    fi
}

check_domains() {
    print_info "Checking domain DNS resolution..."
    
    for domain in $DOMAIN_WEB $DOMAIN_API; do
        if host $domain > /dev/null 2>&1; then
            IP=$(host $domain | grep "has address" | awk '{print $4}' | head -1)
            print_info "✓ $domain resolves to $IP"
        else
            print_warning "✗ $domain does not resolve. Please configure DNS first!"
            echo "   Add an A record pointing to your server's IP address"
            exit 1
        fi
    done
}

install_certbot() {
    print_info "Installing Certbot..."
    
    if ! command -v certbot &> /dev/null; then
        apt update
        apt install -y certbot python3-certbot-nginx
        print_info "Certbot installed successfully"
    else
        print_info "Certbot is already installed"
    fi
}

install_nginx() {
    print_info "Installing Nginx..."
    
    if ! command -v nginx &> /dev/null; then
        apt update
        apt install -y nginx
        systemctl enable nginx
        systemctl start nginx
        print_info "Nginx installed and started"
    else
        print_info "Nginx is already installed"
    fi
}

configure_nginx_http() {
    print_info "Configuring Nginx for HTTP (for SSL verification)..."
    
    # Web application HTTP config
    cat > /etc/nginx/sites-available/grc-web << EOF
server {
    listen 80;
    server_name $DOMAIN_WEB;
    
    location /.well-known/acme-challenge/ {
        root /var/www/html;
    }
    
    location / {
        return 301 https://\$server_name\$request_uri;
    }
}
EOF

    # API HTTP config
    cat > /etc/nginx/sites-available/grc-api << EOF
server {
    listen 80;
    server_name $DOMAIN_API;
    
    location /.well-known/acme-challenge/ {
        root /var/www/html;
    }
    
    location / {
        return 301 https://\$server_name\$request_uri;
    }
}
EOF

    # Enable sites
    ln -sf /etc/nginx/sites-available/grc-web /etc/nginx/sites-enabled/
    ln -sf /etc/nginx/sites-available/grc-api /etc/nginx/sites-enabled/
    
    # Remove default site
    rm -f /etc/nginx/sites-enabled/default
    
    # Test and reload
    nginx -t
    systemctl reload nginx
    
    print_info "Nginx HTTP configuration complete"
}

obtain_certificates() {
    print_info "Obtaining SSL certificates from Let's Encrypt..."
    
    # Obtain certificates for both domains
    certbot certonly --nginx \
        -d $DOMAIN_WEB \
        -d $DOMAIN_API \
        --email $EMAIL \
        --agree-tos \
        --non-interactive \
        --redirect
    
    if [ $? -eq 0 ]; then
        print_info "SSL certificates obtained successfully!"
    else
        print_error "Failed to obtain SSL certificates"
        exit 1
    fi
}

configure_nginx_https() {
    print_info "Configuring Nginx with SSL..."
    
    # Web application HTTPS config
    cat > /etc/nginx/sites-available/grc-web << EOF
server {
    listen 80;
    server_name $DOMAIN_WEB;
    return 301 https://\$server_name\$request_uri;
}

server {
    listen 443 ssl http2;
    server_name $DOMAIN_WEB;

    ssl_certificate /etc/letsencrypt/live/$DOMAIN_WEB/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/$DOMAIN_WEB/privkey.pem;
    
    # SSL Configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-RSA-AES128-SHA256:ECDHE-RSA-AES256-SHA384;
    ssl_prefer_server_ciphers on;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    
    # Security Headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Content-Type-Options nosniff always;
    add_header X-Frame-Options SAMEORIGIN always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    client_max_body_size 100M;
    
    location / {
        proxy_pass http://localhost:5001;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_set_header X-Real-IP \$remote_addr;
        
        # Timeouts
        proxy_connect_timeout 600;
        proxy_send_timeout 600;
        proxy_read_timeout 600;
        send_timeout 600;
    }
}
EOF

    # API HTTPS config
    cat > /etc/nginx/sites-available/grc-api << EOF
server {
    listen 80;
    server_name $DOMAIN_API;
    return 301 https://\$server_name\$request_uri;
}

server {
    listen 443 ssl http2;
    server_name $DOMAIN_API;

    ssl_certificate /etc/letsencrypt/live/$DOMAIN_WEB/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/$DOMAIN_WEB/privkey.pem;
    
    # SSL Configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-RSA-AES128-SHA256:ECDHE-RSA-AES256-SHA384;
    ssl_prefer_server_ciphers on;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    
    # Security Headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Content-Type-Options nosniff always;
    add_header X-Frame-Options SAMEORIGIN always;
    
    client_max_body_size 100M;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_cache_bypass \$http_upgrade;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_set_header X-Real-IP \$remote_addr;
    }
}
EOF

    # Test and reload
    nginx -t
    systemctl reload nginx
    
    print_info "Nginx HTTPS configuration complete"
}

setup_auto_renewal() {
    print_info "Setting up automatic certificate renewal..."
    
    # Test renewal
    certbot renew --dry-run
    
    # Certbot automatically sets up a systemd timer for renewal
    systemctl list-timers | grep certbot
    
    print_info "Auto-renewal configured. Certificates will renew automatically."
}

configure_firewall() {
    print_info "Configuring firewall..."
    
    if command -v ufw &> /dev/null; then
        ufw allow 80/tcp
        ufw allow 443/tcp
        ufw --force enable
        print_info "Firewall configured"
    else
        print_warning "UFW not installed. Please configure firewall manually."
    fi
}

print_summary() {
    echo ""
    echo "================================================================"
    echo -e "${GREEN}  SSL Certificates Setup Complete!${NC}"
    echo "================================================================"
    echo ""
    echo "SSL Certificates:"
    echo "  • Web: /etc/letsencrypt/live/$DOMAIN_WEB/fullchain.pem"
    echo "  • API: Uses same certificate"
    echo ""
    echo "Certificate Details:"
    certbot certificates
    echo ""
    echo "Domains Configured:"
    echo "  • $DOMAIN_WEB → https (port 443) → localhost:5001"
    echo "  • $DOMAIN_API → https (port 443) → localhost:5000"
    echo ""
    echo "Auto-Renewal:"
    echo "  • Certificates will auto-renew 30 days before expiration"
    echo "  • Check status: sudo certbot renew --dry-run"
    echo ""
    echo "Next Steps:"
    echo "  1. Deploy application: sudo ./deploy-production.sh"
    echo "  2. Test HTTPS: curl https://$DOMAIN_WEB"
    echo "  3. Test SSL rating: https://www.ssllabs.com/ssltest/"
    echo ""
    echo "================================================================"
}

main() {
    echo ""
    echo "================================================================"
    echo "  SSL Certificate Setup for Saudi GRC Application"
    echo "================================================================"
    echo ""
    
    check_root
    
    print_warning "This will:"
    print_warning "  1. Install Certbot and Nginx"
    print_warning "  2. Obtain SSL certificates from Let's Encrypt"
    print_warning "  3. Configure Nginx as reverse proxy with HTTPS"
    print_warning ""
    print_warning "Before running, ensure:"
    print_warning "  • DNS records point to this server"
    print_warning "  • Ports 80 and 443 are accessible"
    print_warning "  • Email address is correct: $EMAIL"
    echo ""
    read -p "Continue? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Setup cancelled"
        exit 0
    fi
    
    check_domains
    install_nginx
    install_certbot
    configure_nginx_http
    obtain_certificates
    configure_nginx_https
    setup_auto_renewal
    configure_firewall
    print_summary
}

main



