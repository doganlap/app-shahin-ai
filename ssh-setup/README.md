# SSH Setup for dogan-ai Server

## Server Details

| Field | Value |
|-------|-------|
| Server | GEX44 #2882091 (Hetzner) |
| IPv4 | 148.251.246.221 |
| IPv6 | 2a01:4f8:192:8342::2 |
| Username | root / dogan |

## Quick Start

### Step 1: Configure SSH on Windows

Run the PowerShell script on your local Windows machine:

```powershell
.\setup-ssh-windows.ps1
```

This will:
- Create SSH config entries for easy connection
- Generate an SSH key if you don't have one
- Save server details for reference

### Step 2: Connect to Server

```bash
ssh dogan-ai-root
# Password: (initial password from server details)
```

### Step 3: Run Server Setup

Upload and run the initial setup script:

```bash
# From Windows PowerShell
scp ssh-setup/initial-setup.sh dogan-ai-root:/root/
ssh dogan-ai-root "chmod +x /root/initial-setup.sh && /root/initial-setup.sh"
```

### Step 4: Add SSH Key

Copy your public key to the server for passwordless login:

```powershell
type $env:USERPROFILE\.ssh\id_ed25519.pub | ssh dogan-ai-root "cat >> /home/dogan/.ssh/authorized_keys"
```

### Step 5: Connect as dogan

```bash
ssh dogan-ai
```

## Files

| File | Description |
|------|-------------|
| `setup-ssh-windows.ps1` | Windows PowerShell script to configure local SSH |
| `initial-setup.sh` | Linux script to run on the server for initial setup |

## What initial-setup.sh Does

1. Updates system packages
2. Creates `dogan` user with sudo access
3. Sets up SSH directory structure
4. Installs essential packages (git, curl, htop, etc.)
5. Configures UFW firewall
6. Secures SSH configuration
