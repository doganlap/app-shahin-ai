#!/bin/bash
# Security Check: Prevent Database Port Exposure in Production
# This script checks docker-compose files for exposed database ports
# Exit code 1 = Security violation found
# Exit code 0 = All checks passed

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

VIOLATIONS=0
FILES_CHECKED=0

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║     Docker Compose Security Check - Port Exposure              ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

# Check for exposed database ports
check_file() {
    local file=$1
    local is_production=$2
    
    if [ ! -f "$file" ]; then
        return 0
    fi
    
    FILES_CHECKED=$((FILES_CHECKED + 1))
    local violations_in_file=0
    
    # Check for PostgreSQL port exposure
    if grep -qE '^\s*-\s*["\047]?[0-9]+:5432["\047]?' "$file" && ! grep -qE '^\s*#.*5432' "$file"; then
        echo -e "${RED}❌ SECURITY VIOLATION${NC}: $file exposes PostgreSQL port 5432"
        grep -nE '^\s*-\s*["\047]?[0-9]+:5432["\047]?' "$file" | head -3
        violations_in_file=$((violations_in_file + 1))
        VIOLATIONS=$((VIOLATIONS + 1))
    fi
    
    # Check for Redis port exposure
    if grep -qE '^\s*-\s*["\047]?[0-9]+:6379["\047]?' "$file" && ! grep -qE '^\s*#.*6379' "$file"; then
        echo -e "${RED}❌ SECURITY VIOLATION${NC}: $file exposes Redis port 6379"
        grep -nE '^\s*-\s*["\047]?[0-9]+:6379["\047]?' "$file" | head -3
        violations_in_file=$((violations_in_file + 1))
        VIOLATIONS=$((VIOLATIONS + 1))
    fi
    
    # Check for MySQL port exposure
    if grep -qE '^\s*-\s*["\047]?[0-9]+:3306["\047]?' "$file" && ! grep -qE '^\s*#.*3306' "$file"; then
        echo -e "${RED}❌ SECURITY VIOLATION${NC}: $file exposes MySQL port 3306"
        grep -nE '^\s*-\s*["\047]?[0-9]+:3306["\047]?' "$file" | head -3
        violations_in_file=$((violations_in_file + 1))
        VIOLATIONS=$((VIOLATIONS + 1))
    fi
    
    if [ $violations_in_file -eq 0 ]; then
        echo -e "${GREEN}✅ PASSED${NC}: $file"
    fi
    
    echo ""
}

# Check all docker-compose files
echo "Checking docker-compose files..."
echo ""

check_file "docker-compose.production.yml" true
check_file "docker-compose.grcmvc.yml" true
check_file "docker-compose.yml" false
check_file "deploy/docker-compose.yml" true

# Summary
echo "╔════════════════════════════════════════════════════════════════╗"
echo "║                         SUMMARY                                 ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""
echo "Files checked: $FILES_CHECKED"
echo "Violations found: $VIOLATIONS"
echo ""

if [ $VIOLATIONS -gt 0 ]; then
    echo -e "${RED}❌ SECURITY CHECK FAILED${NC}"
    echo ""
    echo "Database ports (5432, 6379, 3306) should NOT be exposed publicly."
    echo "They should only be accessible within Docker network."
    echo ""
    echo "Fix: Comment out or remove 'ports:' sections for databases."
    echo "Example:"
    echo "  # ports:"
    echo "  #   - \"5432:5432\""
    exit 1
else
    echo -e "${GREEN}✅ ALL SECURITY CHECKS PASSED${NC}"
    echo ""
    echo "No database ports are exposed publicly."
    echo "All databases are only accessible within Docker network."
    exit 0
fi
