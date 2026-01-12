# Refresh PATH and push to GitHub
$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

Write-Host "Refreshing PATH..." -ForegroundColor Cyan
Write-Host ""

# Verify Git is accessible
$gitVersion = git --version 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "Git found: $gitVersion" -ForegroundColor Green
    Write-Host ""
    
    # Change to repository directory
    Set-Location C:\Shahin-ai
    
    Write-Host "Pushing to GitHub..." -ForegroundColor Cyan
    Write-Host "Repository: https://github.com/doganlap/app-shahin-ai.git" -ForegroundColor White
    Write-Host ""
    Write-Host "You will be prompted for credentials:" -ForegroundColor Yellow
    Write-Host "  Username: doganlap" -ForegroundColor White
    Write-Host "  Password: Use your Personal Access Token" -ForegroundColor White
    Write-Host ""
    
    # Push to GitHub
    git push -u origin main
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "SUCCESS! Files pushed to GitHub!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "View repository: https://github.com/doganlap/app-shahin-ai" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "Push failed. Check your credentials." -ForegroundColor Red
        Write-Host "Make sure you use a Personal Access Token, not your GitHub password." -ForegroundColor Yellow
    }
} else {
    Write-Host "Git not found. Please restart PowerShell or run:" -ForegroundColor Red
    Write-Host "  .\install-git.bat" -ForegroundColor White
}


