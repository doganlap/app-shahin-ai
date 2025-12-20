#!/bin/bash
# ============================================================
# SSH Connection Script for Cloud Server
# ============================================================

# Configuration - Update these values
SERVER_USER="${SERVER_USER:-ubuntu}"
SERVER_HOST="${SERVER_HOST:-your-server.com}"
SSH_KEY="${SSH_KEY:-~/.ssh/id_rsa}"
SERVER_PORT="${SERVER_PORT:-22}"

echo "=========================================="
echo "Connecting to Cloud Server"
echo "=========================================="
echo "Server: $SERVER_USER@$SERVER_HOST"
echo "Port: $SERVER_PORT"
echo "=========================================="

# Check if SSH key exists
if [ ! -f "$SSH_KEY" ]; then
    echo "ERROR: SSH key not found at $SSH_KEY"
    echo "Please generate an SSH key first:"
    echo "  ssh-keygen -t rsa -b 4096 -C 'your_email@example.com'"
    exit 1
fi

# Connect to server
ssh -i "$SSH_KEY" -p "$SERVER_PORT" "$SERVER_USER@$SERVER_HOST"


