# Setup SSH configuration for dogan-ai server

$sshDir = "$env:USERPROFILE\.ssh"
$configFile = "$sshDir\config"
$serverDetailsFile = "$sshDir\dogan-ai-details.txt"

# Ensure .ssh directory exists
if (!(Test-Path $sshDir)) {
    New-Item -ItemType Directory -Path $sshDir -Force
}

# Save server details
$details = @"
========================================
dogan-ai Server Details
========================================
Server: GEX44 #2882091
Order: B20251215-3317950-2894183
Date: 15/12/2025

IPv4: 148.251.246.221
IPv6: 2a01:4f8:192:8342::2
Username: root
Initial Password: h?7cQcK6r6veF_

Host Key Fingerprints:
- RSA 3072: d4BgU4AmPRRSbTut44VTSdsULzJ4gpzpoysUd/vCvuQ
- ECDSA 256: BXvwHZM3aJDI/AVIRNsVDipUcqGpZjlMPHNBQf1VjOM
- ED25519 256: CBd5Vwiz6lF8XMO8TLHVxCVQPlGFDxJfsmz8/T4p6Mw

========================================
Connection Commands:
========================================
ssh dogan-ai
ssh dogan-ai-root

Or:
ssh root@148.251.246.221
ssh dogan@148.251.246.221

========================================
"@

$details | Out-File -FilePath $serverDetailsFile -Encoding UTF8
Write-Host "Server details saved to: $serverDetailsFile" -ForegroundColor Green

# Create/update SSH config
$sshConfig = @"

# dogan-ai Server (Hetzner GEX44)
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

"@

if (Test-Path $configFile) {
    # Check if config already has dogan-ai entry
    $content = Get-Content $configFile -Raw
    if ($content -notmatch "Host dogan-ai") {
        Add-Content -Path $configFile -Value $sshConfig
        Write-Host "SSH config updated: $configFile" -ForegroundColor Green
    } else {
        Write-Host "SSH config already contains dogan-ai entry" -ForegroundColor Yellow
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
    ssh-keygen -t ed25519 -C "dogan@dogan-ai" -f $sshKeyPath -N '""'
    Write-Host "SSH key generated: $sshKeyPath" -ForegroundColor Green
} else {
    Write-Host "SSH key already exists: $sshKeyPath" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. Connect to server:" -ForegroundColor White
Write-Host "   ssh dogan-ai-root" -ForegroundColor Yellow
Write-Host "   (Password: h?7cQcK6r6veF_)" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Upload and run setup script:" -ForegroundColor White
Write-Host "   scp initial-setup.sh dogan-ai-root:/root/" -ForegroundColor Yellow
Write-Host "   ssh dogan-ai-root 'chmod +x /root/initial-setup.sh && /root/initial-setup.sh'" -ForegroundColor Yellow
Write-Host ""
Write-Host "3. Copy SSH key to server:" -ForegroundColor White
Write-Host "   type `$env:USERPROFILE\.ssh\id_ed25519.pub | ssh dogan-ai-root 'mkdir -p /home/dogan/.ssh && cat >> /home/dogan/.ssh/authorized_keys'" -ForegroundColor Yellow
Write-Host ""
Write-Host "4. After setup, connect as dogan:" -ForegroundColor White
Write-Host "   ssh dogan-ai" -ForegroundColor Yellow
Write-Host ""
