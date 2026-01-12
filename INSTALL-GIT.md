# Install Git - Quick Guide

## Download Git for Windows

### Direct Download Link
**https://git-scm.com/download/win**

### Quick Install Options

#### Option 1: Using winget (if available)
```powershell
winget install Git.Git
```

#### Option 2: Using Chocolatey (if available)
```powershell
choco install git -y
```

#### Option 3: Manual Download (Recommended)

1. **Download Git:**
   - Visit: https://git-scm.com/download/win
   - Click "Download for Windows"
   - The file will be named: `Git-X.XX.X-64-bit.exe`

2. **Run the Installer:**
   - Double-click the downloaded `.exe` file
   - Follow the installation wizard
   - **Recommended settings:**
     - ✅ Use Visual Studio Code as default editor (or your preferred editor)
     - ✅ Let Git decide (default branch name)
     - ✅ Git from the command line and also from 3rd-party software
     - ✅ Use bundled OpenSSH
     - ✅ Use the OpenSSL library
     - ✅ Checkout Windows-style, commit Unix-style line endings
     - ✅ Use MinTTY (default terminal)

3. **Complete Installation:**
   - Click "Install"
   - Wait for installation to complete
   - Click "Finish"

4. **Restart PowerShell/Command Prompt:**
   - Close all PowerShell/CMD windows
   - Open a new PowerShell window
   - This is required for PATH changes to take effect

5. **Verify Installation:**
   ```powershell
   git --version
   ```
   You should see: `git version 2.x.x`

## After Installation

### Configure Git (First time only)

```powershell
git config --global user.name "doganlap"
git config --global user.email "your-email@example.com"
```

### Push to GitHub

Once Git is installed, run:

```powershell
cd C:\Shahin-ai
.\push-to-github.bat
```

Or use the PowerShell script:

```powershell
.\scripts\push-to-github.ps1
```

## Troubleshooting

### "Git is not recognized"
- Git may not be in PATH
- Restart PowerShell/Command Prompt
- If still not working, add Git to PATH manually:
  - Git is usually installed at: `C:\Program Files\Git\cmd\`
  - Add this to System Environment Variables > PATH

### "Permission denied" during installation
- Run PowerShell as Administrator
- Or right-click installer and "Run as administrator"

### Installation fails
- Disable antivirus temporarily
- Check Windows Update
- Try downloading again from: https://git-scm.com/download/win

---

**Need Help?** Visit: https://git-scm.com/download/win


