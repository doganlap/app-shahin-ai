@echo off
REM ============================================================
REM Download and Install Git
REM ============================================================

echo ==========================================
echo Git Installation
echo ==========================================
echo.

REM Check if Git is already installed
git --version >nul 2>&1
if not errorlevel 1 (
    echo Git is already installed!
    git --version
    echo.
    echo You can now run: push-to-github.bat
    pause
    exit /b 0
)

echo Git is not installed.
echo.

REM Try winget
echo Checking for winget...
winget --version >nul 2>&1
if not errorlevel 1 (
    echo winget found. Installing Git...
    echo.
    winget install --id Git.Git -e --source winget --accept-package-agreements --accept-source-agreements
    if not errorlevel 1 (
        echo.
        echo Git installed successfully!
        echo Please restart PowerShell/Command Prompt and run: push-to-github.bat
        pause
        exit /b 0
    )
)

REM Try chocolatey
echo Checking for Chocolatey...
choco --version >nul 2>&1
if not errorlevel 1 (
    echo Chocolatey found. Installing Git...
    echo.
    choco install git -y
    if not errorlevel 1 (
        echo.
        echo Git installed successfully!
        echo Please restart PowerShell/Command Prompt and run: push-to-github.bat
        pause
        exit /b 0
    )
)

REM Manual installation instructions
echo ==========================================
echo Manual Installation Required
echo ==========================================
echo.
echo Please download and install Git manually:
echo.
echo 1. Visit: https://git-scm.com/download/win
echo 2. Download Git for Windows
echo 3. Run the installer
echo 4. Restart PowerShell/Command Prompt
echo 5. Run: push-to-github.bat
echo.

REM Try to open download page
echo Opening download page in browser...
start https://git-scm.com/download/win

echo.
echo Browser opened. Please download and install Git.
echo.
pause

