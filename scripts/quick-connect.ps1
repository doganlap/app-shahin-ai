# ============================================================
# Quick SSH Connection Script for Windows
# ============================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$ServerHost,
    
    [Parameter(Mandatory=$false)]
    [string]$Username = "ubuntu",
    
    [Parameter(Mandatory=$false)]
    [string]$SshKey = "$env:USERPROFILE\.ssh\id_rsa",
    
    [Parameter(Mandatory=$false)]
    [int]$Port = 22
)

Write-Host "==========================================" -ForegroundColor Green
Write-Host "Connecting to Cloud Server" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Server: ${Username}@${ServerHost}" -ForegroundColor Cyan
Write-Host "Port: $Port" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Green

# Check if SSH key exists
if (-not (Test-Path $SshKey)) {
    Write-Host "WARNING: SSH key not found at $SshKey" -ForegroundColor Yellow
    Write-Host "Connecting without key (will prompt for password)..." -ForegroundColor Yellow
    ssh -p $Port "$Username@$ServerHost"
} else {
    Write-Host "Using SSH key: $SshKey" -ForegroundColor Green
    ssh -i $SshKey -p $Port "$Username@$ServerHost"
}


