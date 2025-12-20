#!/bin/bash
# ============================================================
# List Configured SSH Servers
# ============================================================

SSH_CONFIG="$HOME/.ssh/config"

echo "=========================================="
echo "Configured SSH Servers"
echo "=========================================="
echo "Config File: $SSH_CONFIG"
echo ""

if [ ! -f "$SSH_CONFIG" ]; then
    echo "SSH config file not found at: $SSH_CONFIG"
    echo ""
    echo "Creating sample SSH config structure..."
    
    # Create .ssh directory if it doesn't exist
    mkdir -p "$HOME/.ssh"
    chmod 700 "$HOME/.ssh"
    
    # Create sample config
    cat > "$SSH_CONFIG" << 'EOF'
# SSH Config for shahin-ai.com servers
# Example entries - update with your actual server details

Host shahin-ai-prod
    HostName your-production-server.digitalocean.com
    User ubuntu
    IdentityFile ~/.ssh/id_rsa
    Port 22

Host shahin-ai-staging
    HostName your-staging-server.digitalocean.com
    User ubuntu
    IdentityFile ~/.ssh/id_rsa
    Port 22

Host shahin-ai-dev
    HostName your-dev-server.digitalocean.com
    User root
    IdentityFile ~/.ssh/id_rsa
    Port 22
EOF
    
    chmod 600 "$SSH_CONFIG"
    echo "Sample SSH config created at: $SSH_CONFIG"
    echo "Please edit it with your actual server details."
    echo ""
fi

if [ -f "$SSH_CONFIG" ]; then
    echo "Configured Hosts:"
    echo "----------------------------------------"
    
    # Extract and display host information
    awk '
    /^Host / {
        if (host) {
            printf "Host: %-20s | User: %-10s | HostName: %s | Port: %s\n", 
                host, user, hostname, port
        }
        host = $2
        user = ""
        hostname = ""
        port = "22"
    }
    /^    User / { user = $2 }
    /^    HostName / { hostname = $2 }
    /^    Port / { port = $2 }
    END {
        if (host) {
            printf "Host: %-20s | User: %-10s | HostName: %s | Port: %s\n", 
                host, user, hostname, port
        }
    }
    ' "$SSH_CONFIG" | grep -v "^Host: *$" | while IFS= read -r line; do
        if [ ! -z "$line" ]; then
            echo "$line"
        fi
    done
    
    echo ""
    echo "Total configured hosts: $(grep -c '^Host ' "$SSH_CONFIG" 2>/dev/null || echo 0)"
    echo ""
    echo "To connect to a server, use: ssh <Host>"
    echo "Example: ssh shahin-ai-prod"
    echo ""
    echo "=========================================="
    echo "SSH Config File Content:"
    echo "=========================================="
    cat "$SSH_CONFIG"
fi


