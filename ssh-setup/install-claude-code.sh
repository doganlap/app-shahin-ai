#!/bin/bash
#=======================================
# Claude Code CLI Installation Script
# For: shahin-ai Server (EX63 - 157.180.105.48)
#=======================================

set -e

echo "========================================"
echo "  Claude Code CLI Installation"
echo "========================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

#---------------------------------------
# 1. Install Node.js 20 LTS
#---------------------------------------
echo -e "${YELLOW}[1/4] Installing Node.js 20 LTS...${NC}"

# Check if Node.js is already installed
if command -v node &> /dev/null; then
    NODE_VERSION=$(node -v)
    echo -e "${CYAN}Node.js already installed: $NODE_VERSION${NC}"
else
    # Install Node.js 20 via NodeSource
    curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
    sudo apt-get install -y nodejs
    echo -e "${GREEN}Node.js installed: $(node -v)${NC}"
fi

# Verify npm
echo "npm version: $(npm -v)"
echo ""

#---------------------------------------
# 2. Install Claude Code CLI
#---------------------------------------
echo -e "${YELLOW}[2/4] Installing Claude Code CLI...${NC}"

# Install globally
sudo npm install -g @anthropic-ai/claude-code

echo -e "${GREEN}Claude Code installed!${NC}"
echo ""

#---------------------------------------
# 3. Verify Installation
#---------------------------------------
echo -e "${YELLOW}[3/4] Verifying installation...${NC}"

if command -v claude &> /dev/null; then
    echo -e "${GREEN}Claude Code CLI is available!${NC}"
    claude --version
else
    echo -e "${RED}Claude command not found. Try restarting your shell.${NC}"
fi
echo ""

#---------------------------------------
# 4. Setup Instructions
#---------------------------------------
echo -e "${YELLOW}[4/4] Setup Instructions${NC}"
echo ""
echo "========================================"
echo -e "${GREEN}  Installation Complete!${NC}"
echo "========================================"
echo ""
echo "To authenticate Claude Code, you have two options:"
echo ""
echo -e "${CYAN}Option 1: Interactive Login (Recommended)${NC}"
echo "  claude"
echo "  # Follow the prompts to login with your Anthropic account"
echo ""
echo -e "${CYAN}Option 2: API Key${NC}"
echo "  export ANTHROPIC_API_KEY='your-api-key-here'"
echo "  # Add to ~/.bashrc for persistence:"
echo "  echo 'export ANTHROPIC_API_KEY=\"your-api-key\"' >> ~/.bashrc"
echo ""
echo -e "${CYAN}Quick Commands:${NC}"
echo "  claude              # Start interactive mode"
echo "  claude -p 'prompt'  # One-shot prompt"
echo "  claude --help       # Show all options"
echo ""
echo -e "${CYAN}Configuration:${NC}"
echo "  claude config       # Configure settings"
echo "  claude mcp          # Manage MCP servers"
echo ""
