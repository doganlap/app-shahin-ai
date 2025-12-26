#!/bin/bash

###############################################################################
# Database Migration Script for Saudi GRC Application
# Runs migrations on production database
###############################################################################

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Configuration
DB_HOST="mainline.proxy.rlwy.net"
DB_PORT="46662"
DB_NAME="railway"
DB_USER="postgres"
DB_PASSWORD="sXJTPaceKDGfCkpejiurbDCWjSBmAHnQ"

PROJECT_DIR="/root/app.shahin-ai.com/Shahin-ai/aspnet-core"
MIGRATOR_DIR="$PROJECT_DIR/src/Grc.DbMigrator"

print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_database_connection() {
    print_info "Checking database connection..."
    
    export PGPASSWORD="$DB_PASSWORD"
    
    if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "SELECT 1;" > /dev/null 2>&1; then
        print_info "✓ Database connection successful"
    else
        print_error "✗ Cannot connect to database"
        print_error "  Host: $DB_HOST:$DB_PORT"
        print_error "  Database: $DB_NAME"
        print_error "  User: $DB_USER"
        exit 1
    fi
}

check_existing_tables() {
    print_info "Checking existing database schema..."
    
    export PGPASSWORD="$DB_PASSWORD"
    
    TABLE_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "
        SELECT COUNT(*) 
        FROM information_schema.tables 
        WHERE table_schema = 'public' 
        AND table_type = 'BASE TABLE'
        AND table_name LIKE 'grc_%' OR table_name LIKE 'Abp%';
    " | tr -d ' ')
    
    print_info "Found $TABLE_COUNT existing tables"
    
    if [ "$TABLE_COUNT" -gt "0" ]; then
        print_warning "Database already contains tables. Migration will update schema."
    else
        print_info "Database is empty. Will create new schema."
    fi
}

run_db_migrator() {
    print_info "Running database migrator..."
    
    cd "$MIGRATOR_DIR"
    
    print_info "Building DbMigrator..."
    dotnet build --configuration Release
    
    if [ $? -ne 0 ]; then
        print_error "Build failed"
        exit 1
    fi
    
    print_info "Executing migrations..."
    dotnet run --configuration Release
    
    if [ $? -eq 0 ]; then
        print_info "✓ Migrations executed successfully"
    else
        print_error "✗ Migration failed"
        exit 1
    fi
}

verify_migrations() {
    print_info "Verifying database schema..."
    
    export PGPASSWORD="$DB_PASSWORD"
    
    # Check for key GRC tables
    TABLES=("grc_evidences" "grc_frameworks" "grc_controls" "grc_risks" "grc_regulators")
    
    for table in "${TABLES[@]}"; do
        EXISTS=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_schema = 'public' 
                AND table_name = '$table'
            );
        " | tr -d ' ')
        
        if [ "$EXISTS" = "t" ]; then
            COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM $table;" | tr -d ' ')
            print_info "✓ Table $table exists ($COUNT records)"
        else
            print_warning "✗ Table $table not found"
        fi
    done
}

create_backup() {
    print_info "Creating database backup before migration..."
    
    export PGPASSWORD="$DB_PASSWORD"
    
    BACKUP_FILE="/tmp/grc_backup_$(date +%Y%m%d_%H%M%S).sql"
    
    pg_dump -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" > "$BACKUP_FILE" 2>/dev/null || true
    
    if [ -f "$BACKUP_FILE" ] && [ -s "$BACKUP_FILE" ]; then
        print_info "✓ Backup created: $BACKUP_FILE"
    else
        print_warning "Backup not created (database might be empty)"
    fi
}

print_summary() {
    echo ""
    echo "================================================================"
    echo -e "${GREEN}  Database Migration Complete!${NC}"
    echo "================================================================"
    echo ""
    echo "Database Details:"
    echo "  • Host: $DB_HOST:$DB_PORT"
    echo "  • Database: $DB_NAME"
    echo "  • SSL: Enabled"
    echo ""
    
    export PGPASSWORD="$DB_PASSWORD"
    
    echo "Schema Statistics:"
    TABLE_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "
        SELECT COUNT(*) FROM information_schema.tables 
        WHERE table_schema = 'public' AND table_type = 'BASE TABLE';
    " | tr -d ' ')
    echo "  • Total Tables: $TABLE_COUNT"
    
    echo ""
    echo "Next Steps:"
    echo "  1. Deploy application: sudo ./deploy-production.sh"
    echo "  2. Start services: sudo systemctl start grc-web grc-api"
    echo "  3. Verify application: https://grc.shahin-ai.com"
    echo ""
    echo "================================================================"
}

main() {
    echo ""
    echo "================================================================"
    echo "  Database Migration for Saudi GRC Application"
    echo "================================================================"
    echo ""
    
    print_warning "This will run database migrations on production database."
    print_warning "Database: $DB_HOST:$DB_PORT / $DB_NAME"
    echo ""
    read -p "Continue? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Migration cancelled"
        exit 0
    fi
    
    check_database_connection
    check_existing_tables
    create_backup
    run_db_migrator
    verify_migrations
    print_summary
}

main



