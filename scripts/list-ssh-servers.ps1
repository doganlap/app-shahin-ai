# ============================================================
# List Configured SSH Servers
# ============================================================

$sshConfigPath = "$env:USERPROFILE\.ssh\config"

if (-not (Test-Path $sshConfigPath)) {
    Write-Host "SSH config file not found at: $sshConfigPath" -ForegroundColor Yellow
    Write-Host "`nCreating sample SSH config structure..." -ForegroundColor Cyan
    
    # Create .ssh directory if it doesn't exist
    $sshDir = "$env:USERPROFILE\.ssh"
    if (-not (Test-Path $sshDir)) {
        New-Item -ItemType Directory -Path $sshDir -Force | Out-Null
    }
    
    # Create sample config
    $sampleConfig = @"
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
"@
    
    Set-Content -Path $sshConfigPath -Value $sampleConfig
    Write-Host "Sample SSH config created at: $sshConfigPath" -ForegroundColor Green
    Write-Host "Please edit it with your actual server details." -ForegroundColor Yellow
}

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "Configured SSH Servers" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Config File: $sshConfigPath`n" -ForegroundColor Cyan

if (Test-Path $sshConfigPath) {
    $content = Get-Content $sshConfigPath
    
    $servers = @()
    $currentHost = $null
    $currentConfig = @{}
    
    foreach ($line in $content) {
        $trimmed = $line.Trim()
        
        # Skip comments and empty lines
        if ($trimmed -match '^\s*#' -or $trimmed -eq '') {
            continue
        }
        
        # Check for Host entry
        if ($trimmed -match '^Host\s+(.+)$') {
            # Save previous host if exists
            if ($currentHost) {
                $servers += [PSCustomObject]@{
                    Host = $currentHost
                    HostName = $currentConfig['HostName']
                    User = $currentConfig['User']
                    Port = $currentConfig['Port']
                    IdentityFile = $currentConfig['IdentityFile']
                }
            }
            
            $currentHost = $matches[1]
            $currentConfig = @{}
        }
        # Parse config entries
        elseif ($trimmed -match '^(\w+)\s+(.+)$') {
            $key = $matches[1]
            $value = $matches[2]
            $currentConfig[$key] = $value
        }
    }
    
    # Add last host
    if ($currentHost) {
        $servers += [PSCustomObject]@{
            Host = $currentHost
            HostName = $currentConfig['HostName']
            User = $currentConfig['User']
            Port = $currentConfig['Port']
            IdentityFile = $currentConfig['IdentityFile']
        }
    }
    
    if ($servers.Count -eq 0) {
        Write-Host "No SSH servers configured." -ForegroundColor Yellow
        Write-Host "`nTo add a server, edit: $sshConfigPath" -ForegroundColor Cyan
    } else {
        $servers | Format-Table -AutoSize Host, HostName, User, Port, IdentityFile
        Write-Host "`nTotal servers configured: $($servers.Count)" -ForegroundColor Green
        Write-Host "`nTo connect to a server, use: ssh <Host>" -ForegroundColor Cyan
        Write-Host "Example: ssh shahin-ai-prod" -ForegroundColor Cyan
    }
    
    # Display raw config for reference
    Write-Host "`n==========================================" -ForegroundColor Green
    Write-Host "SSH Config File Content" -ForegroundColor Green
    Write-Host "==========================================" -ForegroundColor Green
    Get-Content $sshConfigPath | ForEach-Object {
        if ($_ -match '^\s*#') {
            Write-Host $_ -ForegroundColor DarkGray
        } elseif ($_ -match '^Host\s+') {
            Write-Host $_ -ForegroundColor Green
        } else {
            Write-Host $_ -ForegroundColor White
        }
    }
}


