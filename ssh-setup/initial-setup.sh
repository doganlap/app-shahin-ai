#!/bin/bash
#=======================================
# dogan-ai Server Initial Setup Script
# Server: GEX44 #2882091 (Hetzner)
# IP: 148.251.246.221
#=======================================

set -e

echo "========================================"
echo "  dogan-ai Server Initial Setup"
echo "========================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo -e "${RED}Please run as root${NC}"
    exit 1
fi

#---------------------------------------
# 1. Update System
#---------------------------------------
echo -e "${YELLOW}[1/7] Updating system packages...${NC}"
apt update && apt upgrade -y
echo -e "${GREEN}System updated!${NC}"
echo ""

#---------------------------------------
# 2. Create dogan user
#---------------------------------------
echo -e "${YELLOW}[2/7] Creating user 'dogan'...${NC}"
if id "dogan" &>/dev/null; then
    echo -e "${YELLOW}User 'dogan' already exists${NC}"
else
    useradd -m -s /bin/bash -G sudo dogan
    echo "dogan:ChangeThisPassword123!" | chpasswd
    echo -e "${GREEN}User 'dogan' created!${NC}"
fi
echo ""

#---------------------------------------
# 3. Setup SSH directory for dogan
#---------------------------------------
echo -e "${YELLOW}[3/7] Setting up SSH for user 'dogan'...${NC}"
mkdir -p /home/dogan/.ssh
chmod 700 /home/dogan/.ssh
touch /home/dogan/.ssh/authorized_keys
chmod 600 /home/dogan/.ssh/authorized_keys
chown -R dogan:dogan /home/dogan/.ssh
echo -e "${GREEN}SSH directory configured!${NC}"
echo ""

#---------------------------------------
# 4. Configure sudo without password (optional)
#---------------------------------------
echo -e "${YELLOW}[4/7] Configuring sudo access...${NC}"
echo "dogan ALL=(ALL) NOPASSWD:ALL" > /etc/sudoers.d/dogan
chmod 440 /etc/sudoers.d/dogan
echo -e "${GREEN}Sudo configured for dogan!${NC}"
echo ""

#---------------------------------------
# 5. Install essential packages
#---------------------------------------
echo -e "${YELLOW}[5/7] Installing essential packages...${NC}"
apt install -y \
    git \
    curl \
    wget \
    htop \
    vim \
    nano \
    unzip \
    zip \
    net-tools \
    ufw \
    fail2ban
echo -e "${GREEN}Essential packages installed!${NC}"
echo ""

#---------------------------------------
# 6. Configure Firewall (UFW)
#---------------------------------------
echo -e "${YELLOW}[6/7] Configuring firewall...${NC}"
ufw default deny incoming
ufw default allow outgoing
ufw allow ssh
ufw allow 80/tcp
ufw allow 443/tcp
ufw --force enable
echo -e "${GREEN}Firewall configured!${NC}"
echo ""

#---------------------------------------
# 7. Secure SSH Configuration
#---------------------------------------
echo -e "${YELLOW}[7/7] Securing SSH configuration...${NC}"
# Backup original config
cp /etc/ssh/sshd_config /etc/ssh/sshd_config.backup

# Update SSH config
sed -i 's/#PermitRootLogin yes/PermitRootLogin prohibit-password/' /etc/ssh/sshd_config
sed -i 's/#PubkeyAuthentication yes/PubkeyAuthentication yes/' /etc/ssh/sshd_config
sed -i 's/#PasswordAuthentication yes/PasswordAuthentication yes/' /etc/ssh/sshd_config

# Restart SSH service
systemctl restart sshd
echo -e "${GREEN}SSH secured!${NC}"
echo ""

#---------------------------------------
# Setup Complete
#---------------------------------------
echo "========================================"
echo -e "${GREEN}  Setup Complete!${NC}"
echo "========================================"
echo ""
echo "Server Info:"
echo "  - IP: 148.251.246.221"
echo "  - User: dogan"
echo "  - Sudo: Enabled (passwordless)"
echo ""
echo "Next Steps:"
echo "  1. Add your SSH public key to /home/dogan/.ssh/authorized_keys"
echo "  2. Change dogan's password: passwd dogan"
echo "  3. Test login: ssh dogan@148.251.246.221"
echo ""
echo "To add your SSH key from Windows PowerShell:"
echo '  type $env:USERPROFILE\.ssh\id_ed25519.pub | ssh root@148.251.246.221 "cat >> /home/dogan/.ssh/authorized_keys"'
echo ""
