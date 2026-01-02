# SSH Setup for Hetzner Servers

## Your Servers

| Name | Server | IP | Auth |
|------|--------|-----|------|
| **shahin-ai** | EX63 #2818891 | 157.180.105.48 | SSH Key (ready!) |
| dogan-ai | GEX44 #2882091 | 148.251.246.221 | Password |

## Quick Start (shahin-ai - Main Server)

Your SSH key is already configured on shahin-ai! Just connect:

### Step 1: Update SSH Config (Windows)

```powershell
.\setup-ssh-windows.ps1
```

### Step 2: Connect to Server

```bash
ssh shahin-ai
```

No password needed - your SSH key is already authorized!

### Step 3: Install Claude Code

```bash
# Upload the script
scp ssh-setup/install-claude-code.sh shahin-ai:/root/

# Run it
ssh shahin-ai "chmod +x /root/install-claude-code.sh && /root/install-claude-code.sh"
```

Or run directly:

```bash
ssh shahin-ai "curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && apt-get install -y nodejs && npm install -g @anthropic-ai/claude-code"
```

### Step 4: Authenticate Claude

```bash
ssh shahin-ai

# Option 1: Interactive login
claude

# Option 2: Use API key
export ANTHROPIC_API_KEY='your-api-key'
echo 'export ANTHROPIC_API_KEY="your-api-key"' >> ~/.bashrc
```

## Files

| File | Description |
|------|-------------|
| `setup-ssh-windows.ps1` | Windows PowerShell script to configure local SSH |
| `install-claude-code.sh` | Script to install Claude Code CLI on server |
| `initial-setup.sh` | Linux script for initial server setup |

## Claude Code Commands

After installation:

```bash
claude                  # Start interactive mode
claude -p "prompt"      # One-shot prompt
claude config           # Configure settings
claude --help           # Show all options
```
