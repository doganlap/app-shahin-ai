#!/bin/bash

###############################################################################
# Saudi GRC Application - Production Deployment Script
# Version: 1.0.0
# Date: December 21, 2025
###############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
APP_NAME="Saudi GRC"
APP_USER="grcapp"
WEB_DIR="/var/www/grc/web"
API_DIR="/var/www/grc/api"
BLOB_DIR="/var/lib/grc/blobstorage"
BACKUP_DIR="/var/backups/grc"
SOURCE_DIR="/root/app.shahin-ai.com/Shahin-ai/aspnet-core"

# Functions
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

create_backup() {
    print_info "Creating backup..."
    DATE=$(date +%Y%m%d_%H%M%S)
    
    if [ -d "$WEB_DIR" ]; then
        mkdir -p "$BACKUP_DIR"
        tar -czf "$BACKUP_DIR/web_backup_$DATE.tar.gz" -C "$WEB_DIR" . 2>/dev/null || true
        tar -czf "$BACKUP_DIR/api_backup_$DATE.tar.gz" -C "$API_DIR" . 2>/dev/null || true
        print_info "Backup created: $BACKUP_DIR/*_backup_$DATE.tar.gz"
    fi
}

install_prerequisites() {
    print_info "Installing prerequisites..."
    
    # Check if .NET 8.0 is installed
    if ! command -v dotnet &> /dev/null; then
        print_warning ".NET 8.0 not found. Installing..."
        wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
        chmod +x /tmp/dotnet-install.sh
        /tmp/dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/share/dotnet
        ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
    else
        print_info ".NET 8.0 already installed"
    fi
    
    # Install PostgreSQL if needed
    if ! command -v psql &> /dev/null; then
        print_warning "PostgreSQL not found. Please install manually:"
        print_info "  sudo apt update && sudo apt install postgresql postgresql-contrib"
    fi
}

setup_directories() {
    print_info "Setting up directories..."
    
    # Create application user if doesn't exist
    if ! id "$APP_USER" &>/dev/null; then
        useradd -m -s /bin/bash "$APP_USER"
        print_info "Created user: $APP_USER"
    fi
    
    # Create directories
    mkdir -p "$WEB_DIR" "$API_DIR" "$BLOB_DIR" "$BACKUP_DIR"
    chown -R "$APP_USER:$APP_USER" /var/www/grc /var/lib/grc
    print_info "Directories created"
}

build_application() {
    print_info "Building application..."
    cd "$SOURCE_DIR"
    
    # Clean and restore
    dotnet clean
    dotnet restore
    
    # Build in Release mode
    dotnet build --configuration Release
    
    if [ $? -eq 0 ]; then
        print_info "Build successful"
    else
        print_error "Build failed"
        exit 1
    fi
}

publish_application() {
    print_info "Publishing application..."
    cd "$SOURCE_DIR"
    
    # Publish Web
    dotnet publish src/Grc.Web/Grc.Web.csproj \
        --configuration Release \
        --output /tmp/grc-web-publish \
        --no-restore
    
    # Publish API
    dotnet publish src/Grc.HttpApi.Host/Grc.HttpApi.Host.csproj \
        --configuration Release \
        --output /tmp/grc-api-publish \
        --no-restore
    
    if [ $? -eq 0 ]; then
        print_info "Publish successful"
    else
        print_error "Publish failed"
        exit 1
    fi
}

deploy_files() {
    print_info "Deploying application files..."
    
    # Stop services if running
    systemctl stop grc-web grc-api 2>/dev/null || true
    
    # Deploy Web
    rm -rf "$WEB_DIR"/*
    cp -r /tmp/grc-web-publish/* "$WEB_DIR"/
    
    # Deploy API
    rm -rf "$API_DIR"/*
    cp -r /tmp/grc-api-publish/* "$API_DIR"/
    
    # Set permissions
    chown -R "$APP_USER:$APP_USER" "$WEB_DIR" "$API_DIR"
    chmod -R 755 "$WEB_DIR" "$API_DIR"
    
    print_info "Files deployed"
}

setup_systemd_services() {
    print_info "Setting up systemd services..."
    
    # Create Web service
    cat > /etc/systemd/system/grc-web.service << 'EOF'
[Unit]
Description=Saudi GRC Web Application
After=network.target

[Service]
Type=notify
User=grcapp
Group=grcapp
WorkingDirectory=/var/www/grc/web
ExecStart=/usr/bin/dotnet /var/www/grc/web/Grc.Web.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=grc-web
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

    # Create API service
    cat > /etc/systemd/system/grc-api.service << 'EOF'
[Unit]
Description=Saudi GRC API Host
After=network.target

[Service]
Type=notify
User=grcapp
Group=grcapp
WorkingDirectory=/var/www/grc/api
ExecStart=/usr/bin/dotnet /var/www/grc/api/Grc.HttpApi.Host.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=grc-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

    systemctl daemon-reload
    systemctl enable grc-web grc-api
    
    print_info "Systemd services configured"
}

start_services() {
    print_info "Starting services..."
    
    systemctl start grc-web
    systemctl start grc-api
    
    sleep 5
    
    # Check service status
    if systemctl is-active --quiet grc-web; then
        print_info "✓ Web service started successfully"
    else
        print_error "✗ Web service failed to start"
        systemctl status grc-web --no-pager
    fi
    
    if systemctl is-active --quiet grc-api; then
        print_info "✓ API service started successfully"
    else
        print_error "✗ API service failed to start"
        systemctl status grc-api --no-pager
    fi
}

verify_deployment() {
    print_info "Verifying deployment..."
    
    sleep 10
    
    # Check if services are listening
    if netstat -tulpn | grep :5001 > /dev/null; then
        print_info "✓ Web application is listening on port 5001"
    else
        print_warning "✗ Web application not responding on port 5001"
    fi
    
    if netstat -tulpn | grep :5000 > /dev/null; then
        print_info "✓ API is listening on port 5000"
    else
        print_warning "✗ API not responding on port 5000"
    fi
}

print_summary() {
    echo ""
    echo "================================================================"
    echo -e "${GREEN}  $APP_NAME - Deployment Complete!${NC}"
    echo "================================================================"
    echo ""
    echo "Services:"
    echo "  • Web Application: http://localhost:5001"
    echo "  • API Host:        http://localhost:5000"
    echo ""
    echo "Service Management:"
    echo "  • Check status:  sudo systemctl status grc-web grc-api"
    echo "  • View logs:     sudo journalctl -u grc-web -f"
    echo "  • Restart:       sudo systemctl restart grc-web grc-api"
    echo ""
    echo "Next Steps:"
    echo "  1. Configure production appsettings (database, URLs)"
    echo "  2. Run database migrations"
    echo "  3. Set up Nginx reverse proxy"
    echo "  4. Configure SSL certificates"
    echo "  5. Change default admin password"
    echo ""
    echo "Documentation:"
    echo "  • Deployment Guide: /root/app.shahin-ai.com/Shahin-ai/PRODUCTION_DEPLOYMENT_GUIDE.md"
    echo "  • Compilation Fixes: /root/app.shahin-ai.com/Shahin-ai/COMPILATION_FIX_SUMMARY.md"
    echo ""
    echo "================================================================"
}

# Main deployment flow
main() {
    echo ""
    echo "================================================================"
    echo "  $APP_NAME - Production Deployment"
    echo "================================================================"
    echo ""
    
    check_root
    
    print_warning "This will deploy the application to production."
    read -p "Continue? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Deployment cancelled"
        exit 0
    fi
    
    create_backup
    install_prerequisites
    setup_directories
    build_application
    publish_application
    deploy_files
    setup_systemd_services
    start_services
    verify_deployment
    print_summary
    
    print_info "Deployment completed successfully!"
}

# Run main function
main



