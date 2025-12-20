# GitHub Repository Setup Guide

## Prerequisites

1. **Install Git** (if not already installed)
   - Download from: https://git-scm.com/download/win
   - Or use: `winget install Git.Git`
   - Restart PowerShell after installation

2. **GitHub Account**
   - Create account at: https://github.com
   - Generate Personal Access Token (PAT) with `repo` scope:
     - Go to: https://github.com/settings/tokens
     - Click "Generate new token (classic)"
     - Select `repo` scope
     - Copy the token (you'll need it)

## Quick Setup (Automated)

Once Git is installed, run:

```powershell
# Option 1: With GitHub credentials
.\scripts\setup-github.ps1 -GitHubUser your-username -GitHubToken your-token

# Option 2: Public repository
.\scripts\setup-github.ps1 -GitHubUser your-username -GitHubToken your-token -Public

# Option 3: Manual repository creation
.\scripts\setup-github.ps1
```

## Manual Setup (Step by Step)

### Step 1: Initialize Git Repository

```powershell
cd C:\Shahin-ai
git init
```

### Step 2: Configure Git User (if not already set)

```powershell
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### Step 3: Add All Files

```powershell
git add .
```

### Step 4: Create Initial Commit

```powershell
git commit -m "Initial commit: Saudi GRC Platform - ABP.io Project with Product/Subscription Module"
```

### Step 5: Create Repository on GitHub

1. Go to: https://github.com/new
2. Repository name: `shahin-ai` (or your preferred name)
3. Description: `Saudi GRC Platform - ABP.io Framework Implementation`
4. Choose visibility: Public or Private
5. **DO NOT** initialize with README, .gitignore, or license (we already have these)
6. Click "Create repository"

### Step 6: Connect Local Repository to GitHub

```powershell
# Add remote (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/shahin-ai.git

# Rename branch to main (if needed)
git branch -M main

# Push to GitHub
git push -u origin main
```

If prompted for credentials:
- Username: Your GitHub username
- Password: Use your Personal Access Token (PAT), not your GitHub password

### Step 7: Verify

1. Visit your repository: `https://github.com/YOUR_USERNAME/shahin-ai`
2. Verify all files are uploaded
3. Check that README.md displays correctly

## Repository Contents

This repository includes:

### Source Code
- `src/` - All C# source files
  - Domain layer entities and services
  - Application layer DTOs and services
  - EntityFrameworkCore configurations
  - HTTP API controllers

### Specifications
- `00-PROJECT-SPEC.yaml` - Project overview
- `01-ENTITIES.yaml` - Domain entities definition
- `02-DATABASE-SCHEMA.sql` - Database schema
- `03-API-SPEC.yaml` - OpenAPI API specification
- `05-TASK-BREAKDOWN.yaml` - Implementation tasks

### Documentation
- `README.md` - Main project documentation
- `CLOUD-SERVER-SETUP.md` - Cloud deployment guide
- `GITHUB-SETUP.md` - This file
- Various documentation files

### Scripts
- `scripts/` - Utility scripts for:
  - GitHub setup
  - Cloud deployment
  - SSH server management
  - Build automation

### Configuration
- `.gitignore` - Git ignore rules
- `config/` - Configuration templates

## Current Status

✅ **Completed:**
- Product/Subscription module domain layer
- Product/Subscription module application layer
- Product/Subscription module infrastructure layer
- Product/Subscription module API layer
- Database schema and migrations
- Documentation

🔄 **In Progress:**
- Frontend implementation
- Additional modules
- Testing

## Next Steps After Setup

1. **Clone on other machines:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/shahin-ai.git
   cd shahin-ai
   ```

2. **Set up development environment:**
   ```powershell
   dotnet restore
   dotnet build
   ```

3. **Run database migrations:**
   ```powershell
   dotnet ef database update --project src/Grc.EntityFrameworkCore
   ```

4. **Start development:**
   ```powershell
   dotnet run --project src/Grc.HttpApi.Host
   ```

## Troubleshooting

### Git not recognized
- Install Git: https://git-scm.com/download/win
- Restart PowerShell after installation
- Verify: `git --version`

### Authentication failed
- Use Personal Access Token (PAT) instead of password
- Ensure PAT has `repo` scope
- Consider using SSH keys: https://docs.github.com/en/authentication/connecting-to-github-with-ssh

### Branch name issues
- Use: `git branch -M main` to rename to main
- Or: `git push -u origin master` if using master branch

### Large file uploads
- Ensure `.gitignore` excludes build artifacts
- Review files before committing: `git status`

## Repository Information

- **Repository Name**: shahin-ai
- **Description**: Saudi GRC Platform - ABP.io Framework Implementation with Product/Subscription Management
- **License**: MIT (recommended)
- **Visibility**: Private (recommended for commercial projects)

---

**Need Help?** Open an issue in the repository or check the documentation files.

