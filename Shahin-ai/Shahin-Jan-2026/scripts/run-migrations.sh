#!/bin/bash
# =============================================================================
# Database Migration Script for GRC Application
# =============================================================================
# This script should be run BEFORE deploying new versions to production
# It applies EF Core migrations to both GrcDbContext and GrcAuthDbContext
#
# Usage:
#   ./scripts/run-migrations.sh [environment]
#
# Examples:
#   ./scripts/run-migrations.sh production
#   ./scripts/run-migrations.sh staging
#
# Prerequisites:
#   - dotnet CLI installed
#   - Database connection string set in environment or appsettings
#   - Database backup completed (verify before running!)
# =============================================================================

set -e  # Exit on error

# Configuration
ENVIRONMENT="${1:-Production}"
PROJECT_PATH="src/GrcMvc/GrcMvc.csproj"
BACKUP_DIR="./database-backups"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "🚀 GRC Database Migration Script"
echo "=================================="
echo "Environment: $ENVIRONMENT"
echo "Timestamp: $TIMESTAMP"
echo ""

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: dotnet CLI is not installed"
    exit 1
fi

# Check if project file exists
if [ ! -f "$PROJECT_PATH" ]; then
    echo "❌ Error: Project file not found at $PROJECT_PATH"
    exit 1
fi

# Function to create database backup
create_backup() {
    local db_name=$1
    local backup_file="${BACKUP_DIR}/${db_name}_backup_${TIMESTAMP}.sql"

    echo "📦 Creating backup of database: $db_name"

    # Create backup directory if it doesn't exist
    mkdir -p "$BACKUP_DIR"

    # PostgreSQL backup
    if [ ! -z "$DB_HOST" ] && [ ! -z "$DB_USER" ]; then
        PGPASSWORD="$DB_PASSWORD" pg_dump \
            -h "$DB_HOST" \
            -U "$DB_USER" \
            -p "${DB_PORT:-5432}" \
            -d "$db_name" \
            -F c \
            -f "$backup_file.backup"

        if [ $? -eq 0 ]; then
            echo "✅ Backup created: $backup_file.backup"
            return 0
        else
            echo "❌ Backup failed for $db_name"
            return 1
        fi
    else
        echo "⚠️ Database credentials not set. Skipping backup."
        echo "   Set DB_HOST, DB_USER, DB_PASSWORD environment variables to enable backups."
    fi
}

# Function to verify database connection
verify_connection() {
    echo "🔌 Verifying database connection..."

    if [ ! -z "$GRCMVC_DB_CONNECTION" ]; then
        # Try to connect using connection string
        echo "   Using GRCMVC_DB_CONNECTION"
        return 0
    elif [ ! -z "$DB_HOST" ]; then
        echo "   Using DB_HOST: $DB_HOST"
        return 0
    else
        echo "❌ No database connection string found."
        echo "   Set GRCMVC_DB_CONNECTION or DB_HOST/DB_USER/DB_PASSWORD"
        return 1
    fi
}

# Function to run migrations
run_migrations() {
    local context=$1
    echo ""
    echo "🔄 Running migrations for $context..."

    dotnet ef database update \
        --project "$PROJECT_PATH" \
        --context "$context" \
        --configuration "$ENVIRONMENT" \
        --verbose

    if [ $? -eq 0 ]; then
        echo "✅ Migrations applied successfully for $context"
        return 0
    else
        echo "❌ Migration failed for $context"
        return 1
    fi
}

# Function to list pending migrations
list_pending() {
    local context=$1
    echo ""
    echo "📋 Pending migrations for $context:"

    dotnet ef migrations list \
        --project "$PROJECT_PATH" \
        --context "$context" \
        --no-build \
        | grep -E "^\s+\(Pending\)" || echo "   No pending migrations"
}

# Main execution
main() {
    echo ""
    echo "⚠️  WARNING: This will modify your database!"
    echo "   Make sure you have a recent backup before proceeding."
    echo ""
    read -p "Do you want to continue? (yes/no): " confirm

    if [ "$confirm" != "yes" ]; then
        echo "❌ Migration cancelled"
        exit 0
    fi

    # Verify connection
    if ! verify_connection; then
        exit 1
    fi

    # Create backups (optional but recommended)
    if [ ! -z "$DB_HOST" ]; then
        echo ""
        echo "📦 Creating database backups..."
        create_backup "GrcMvcDb" || echo "⚠️ Backup skipped"
    fi

    # List pending migrations
    echo ""
    echo "📋 Checking for pending migrations..."
    list_pending "GrcDbContext"
    list_pending "GrcAuthDbContext"

    echo ""
    read -p "Apply these migrations? (yes/no): " apply_confirm

    if [ "$apply_confirm" != "yes" ]; then
        echo "❌ Migration cancelled"
        exit 0
    fi

    # Run migrations
    echo ""
    echo "🚀 Starting migration process..."

    # Migrate GrcDbContext (main database)
    if ! run_migrations "GrcDbContext"; then
        echo ""
        echo "❌ Migration failed for GrcDbContext!"
        echo "   Check the error messages above."
        echo "   Database may be in an inconsistent state."
        exit 1
    fi

    # Migrate GrcAuthDbContext (auth database)
    if ! run_migrations "GrcAuthDbContext"; then
        echo ""
        echo "❌ Migration failed for GrcAuthDbContext!"
        echo "   GrcDbContext was migrated successfully."
        echo "   You may need to manually fix GrcAuthDbContext."
        exit 1
    fi

    echo ""
    echo "✅ All migrations completed successfully!"
    echo ""
    echo "📊 Next steps:"
    echo "   1. Verify the application starts correctly"
    echo "   2. Run smoke tests"
    echo "   3. Monitor application logs"
    echo "   4. Keep the backup file: $BACKUP_DIR/*_${TIMESTAMP}*"
    echo ""
}

# Run main function
main

exit 0
