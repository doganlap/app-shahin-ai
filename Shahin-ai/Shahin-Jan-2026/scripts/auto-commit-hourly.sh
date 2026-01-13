#!/bin/bash

# Auto-commit and push script - runs every hour
# This script automatically commits and pushes all changes to GitHub

set -e

# Configuration
REPO_PATH="/home/Shahin-ai/Shahin-Jan-2026"
BRANCH_NAME=$(cd "$REPO_PATH" && git branch --show-current)
LOG_FILE="/home/Shahin-ai/.auto-commit.log"
# GitHub token from environment variable (set in crontab or .env)
GITHUB_TOKEN="${GITHUB_TOKEN:-}"

# Logging function
log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

log "=== Auto-commit started ==="

# Change to repository directory
cd "$REPO_PATH" || {
    log "ERROR: Cannot change to repository directory: $REPO_PATH"
    exit 1
}

# Check if there are any changes
if git diff --quiet && git diff --cached --quiet; then
    log "No changes to commit. Skipping."
    exit 0
fi

# Get current branch
CURRENT_BRANCH=$(git branch --show-current)
if [ -z "$CURRENT_BRANCH" ]; then
    log "ERROR: Not on any branch. Skipping."
    exit 1
fi

log "Current branch: $CURRENT_BRANCH"

# Stage all changes
git add -A
STAGED_COUNT=$(git diff --cached --name-only | wc -l)

if [ "$STAGED_COUNT" -eq 0 ]; then
    log "No changes to commit after staging. Skipping."
    exit 0
fi

log "Staged $STAGED_COUNT file(s) for commit"

# Create commit message with timestamp and file count
COMMIT_MSG="chore: Auto-commit $(date '+%Y-%m-%d %H:%M:%S') - $STAGED_COUNT file(s) changed

Auto-committed changes:
$(git diff --cached --name-only | head -10 | sed 's/^/  - /')

$(if [ "$STAGED_COUNT" -gt 10 ]; then echo "  ... and $((STAGED_COUNT - 10)) more file(s)"; fi)"

# Commit changes
if git commit -m "$COMMIT_MSG"; then
    log "✅ Commit successful: $(git log -1 --format='%h')"
else
    log "ERROR: Commit failed"
    exit 1
fi

# Update remote URL with token (if provided)
if [ -n "$GITHUB_TOKEN" ]; then
    git remote set-url origin "https://doganlap:${GITHUB_TOKEN}@github.com/doganlap/app-shahin-ai.git"
else
    log "WARNING: GITHUB_TOKEN not set. Using existing remote configuration."
fi

# Push to GitHub
if git push origin "$CURRENT_BRANCH" 2>&1; then
    log "✅ Push successful to origin/$CURRENT_BRANCH"
else
    log "ERROR: Push failed"
    exit 1
fi

log "=== Auto-commit completed successfully ==="
exit 0
