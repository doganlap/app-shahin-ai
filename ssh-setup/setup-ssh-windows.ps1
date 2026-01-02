# Setup SSH configuration for Hetzner servers
# Servers: dogan-ai (GEX44) and shahin-ai (EX63)

$sshDir = "$env:USERPROFILE\.ssh"
$configFile = "$sshDir\config"
$serverDetailsFile = "$sshDir\hetzner-servers.txt"

# Ensure .ssh directory exists
if (!(Test-Path $sshDir)) {
    New-Item -ItemType Directory -Path $sshDir -Force
}

# Save server details
$details = @"
========================================
HETZNER SERVERS - DOGAN AI
========================================

----------------------------------------
SERVER 1: dogan-ai (GEX44)
----------------------------------------
Server: GEX44 #2882091
Order: B20251215-3317950-2894183
Date: 15/12/2025

IPv4: 148.251.246.221
IPv6: 2a01:4f8:192:8342::2
Username: root
Initial Password: h?7cQcK6r6veF_

Host Keys:
- RSA 3072: d4BgU4AmPRRSbTut44VTSdsULzJ4gpzpoysUd/vCvuQ
- ECDSA 256: BXvwHZM3aJDI/AVIRNsVDipUcqGpZjlMPHNBQf1VjOM
- ED25519 256: CBd5Vwiz6lF8XMO8TLHVxCVQPlGFDxJfsmz8/T4p6Mw

Connection: ssh dogan-ai

----------------------------------------
SERVER 2: shahin-ai (EX63) - MAIN SERVER
----------------------------------------
Server: EX63 #2818891
Order: B20251231-3328645-2904066
Date: 31/12/2025

IPv4: 157.180.105.48
IPv6: 2a01:4f9:3090:23b0::2
Username: root
Auth: SSH Key (DOGAN-ED25519-WIN) - NO PASSWORD NEEDED!

Host Keys:
- RSA 3072: eSmijJaEYlGj0lrYFdwWvNcVyVDwCuV6rW/Ux3QWFUA
- ECDSA 256: 1JPBGY4CMh09i11MN8FiH0PTpwGCB4TFABDXWiD3WI
- ED25519 256: zu9QS+m6QBP5XGuJjQpWnckwSmNOFjtnJLRF4a2fZg

Connection: ssh shahin-ai

========================================
"@

$details | Out-File -FilePath $serverDetailsFile -Encoding UTF8
Write-Host "Server details saved to: $serverDetailsFile" -ForegroundColor Green

# Create/update SSH config
$sshConfig = @"

# ==========================================
# DOGAN AI - HETZNER SERVERS
# ==========================================

# SERVER 1: dogan-ai (GEX44) - 148.251.246.221
Host dogan-ai
    HostName 148.251.246.221
    User dogan
    Port 22
    IdentityFile ~/.ssh/id_ed25519

Host dogan-ai-root
    HostName 148.251.246.221
    User root
    Port 22
    IdentityFile ~/.ssh/id_ed25519

# SERVER 2: shahin-ai (EX63) - 157.180.105.48 [MAIN]
Host shahin-ai
    HostName 157.180.105.48
    User root
    Port 22
    IdentityFile ~/.ssh/id_ed25519

Host shahin
    HostName 157.180.105.48
    User root
    Port 22
    IdentityFile ~/.ssh/id_ed25519

"@

if (Test-Path $configFile) {
    $content = Get-Content $configFile -Raw

    # Add dogan-ai if not exists
    if ($content -notmatch "Host dogan-ai") {
        Add-Content -Path $configFile -Value $sshConfig
        Write-Host "SSH config updated with all servers: $configFile" -ForegroundColor Green
    }
    # Add shahin-ai if not exists
    elseif ($content -notmatch "Host shahin-ai") {
        # Add only shahin-ai config
        $shahinConfig = @"

# SERVER 2: shahin-ai (EX63) - 157.180.105.48 [MAIN]
Host shahin-ai
    HostName 157.180.105.48
    User root
    Port 22
    IdentityFile ~/.ssh/id_ed25519

Host shahin
    HostName 157.180.105.48
    User root
    Port 22
    IdentityFile ~/.ssh/id_ed25519

"@
        Add-Content -Path $configFile -Value $shahinConfig
        Write-Host "SSH config updated with shahin-ai: $configFile" -ForegroundColor Green
    } else {
        Write-Host "SSH config already contains all server entries" -ForegroundColor Yellow
    }
} else {
    $sshConfig | Out-File -FilePath $configFile -Encoding UTF8
    Write-Host "SSH config created: $configFile" -ForegroundColor Green
}

# Check if SSH key exists
$sshKeyPath = "$sshDir\id_ed25519"
if (!(Test-Path $sshKeyPath)) {
    Write-Host ""
    Write-Host "No SSH key found. Generating one..." -ForegroundColor Yellow
    ssh-keygen -t ed25519 -C "dogan@shahin-ai" -f $sshKeyPath -N '""'
    Write-Host "SSH key generated: $sshKeyPath" -ForegroundColor Green
} else {
    Write-Host "SSH key already exists: $sshKeyPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Your Servers:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "SHAHIN-AI (EX63) - Main Server [SSH Key Ready!]" -ForegroundColor Green
Write-Host "  ssh shahin-ai" -ForegroundColor Yellow
Write-Host "  IP: 157.180.105.48" -ForegroundColor Gray
Write-Host ""
Write-Host "DOGAN-AI (GEX44)" -ForegroundColor White
Write-Host "  ssh dogan-ai-root" -ForegroundColor Yellow
Write-Host "  IP: 148.251.246.221" -ForegroundColor Gray
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Quick Start for shahin-ai:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Connect now (no password needed!):" -ForegroundColor White
Write-Host "   ssh shahin-ai" -ForegroundColor Yellow
Write-Host ""
Write-Host "2. Install Claude Code:" -ForegroundColor White
Write-Host "   scp install-claude-code.sh shahin-ai:/root/" -ForegroundColor Yellow
Write-Host "   ssh shahin-ai 'chmod +x /root/install-claude-code.sh && /root/install-claude-code.sh'" -ForegroundColor Yellow
Write-Host ""
