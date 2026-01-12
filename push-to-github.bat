@echo off
REM ============================================================
REM Push All Files to GitHub Repository
REM ============================================================

echo ==========================================
echo Push to GitHub Repository
echo ==========================================
echo Repository: https://github.com/doganlap/app-shahin-ai.git
echo.

cd /d "C:\Shahin-ai"

REM Check if git is installed
git --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Git is not installed or not in PATH.
    echo.
    echo Please install Git:
    echo   1. Download from: https://git-scm.com/download/win
    echo   2. Or run: winget install Git.Git
    echo   3. Restart Command Prompt after installation
    echo.
    pause
    exit /b 1
)

echo Git found!
echo.

REM Initialize git repository if needed
if not exist ".git" (
    echo Initializing git repository...
    git init
    if errorlevel 1 (
        echo Failed to initialize git repository.
        pause
        exit /b 1
    )
    echo Repository initialized.
) else (
    echo Git repository already initialized.
)

REM Configure remote
echo Configuring remote 'origin'...
git remote remove origin >nul 2>&1
git remote add origin https://github.com/doganlap/app-shahin-ai.git
echo Remote 'origin' configured.
echo.

REM Stage all files
echo Staging all files...
git add .
if errorlevel 1 (
    echo Failed to stage files.
    pause
    exit /b 1
)
echo Files staged.
echo.

REM Check if there are changes
git diff --cached --quiet
if errorlevel 1 (
    echo Creating commit...
    git commit -m "Initial commit: Saudi GRC Platform - ABP.io Project"
    if errorlevel 1 (
        echo Failed to create commit.
        pause
        exit /b 1
    )
    echo Commit created.
) else (
    echo No changes to commit.
)

REM Set default branch
git branch -M main >nul 2>&1
echo.

REM Push to GitHub
echo ==========================================
echo Pushing to GitHub...
echo ==========================================
echo.
echo This will prompt for GitHub credentials.
echo Use your Personal Access Token (PAT) as password.
echo.
echo Get PAT from: https://github.com/settings/tokens
echo.

git push -u origin main

if errorlevel 1 (
    echo.
    echo Push failed. Please check:
    echo   1. You have a Personal Access Token
    echo   2. Token has 'repo' scope
    echo   3. You used the token as password (not GitHub password)
    echo.
    echo You can also push manually:
    echo   git push -u origin main
    echo.
) else (
    echo.
    echo ==========================================
    echo SUCCESS! Repository pushed to GitHub
    echo ==========================================
    echo.
    echo View online: https://github.com/doganlap/app-shahin-ai
    echo.
)

pause

