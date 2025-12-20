# Quick Start: Connect to Cloud Server and Build

## Fastest Method (Using Provided Scripts)

### 1. Connect to Server (Windows)

```powershell
# Edit the script with your server details first, or use parameters:
.\scripts\quick-connect.ps1 -ServerHost "your-server-ip" -Username "ubuntu"
```

### 2. On Server - Run Setup Script

```bash
# Download and run the setup script
wget https://raw.githubusercontent.com/your-repo/scripts/cloud-build-setup.sh
chmod +x cloud-build-setup.sh
./cloud-build-setup.sh
```

### 3. Build Project

```bash
cd /opt/grc-platform
dotnet restore
dotnet build -c Release
dotnet ef database update --startup-project src/Grc.HttpApi.Host
```

### 4. Run Application

```bash
cd src/Grc.HttpApi.Host
dotnet run
```

## Manual Connection (Step by Step)

### Step 1: Generate SSH Key (if needed)
```powershell
ssh-keygen -t rsa -b 4096
# Press Enter for default location
# Enter passphrase (optional)
```

### Step 2: Copy Public Key to Server
```powershell
type $env:USERPROFILE\.ssh\id_rsa.pub | ssh username@server-ip "cat >> .ssh/authorized_keys"
```

### Step 3: Connect
```powershell
ssh username@server-ip
```

### Step 4: Install .NET 8.0 SDK
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
export PATH=$PATH:$HOME/.dotnet
```

### Step 5: Clone/Build Project
```bash
cd /opt
git clone your-repo-url grc-platform
cd grc-platform
dotnet restore
dotnet build -c Release
```

## Common Commands Cheat Sheet

```bash
# Check .NET version
dotnet --version

# Check EF Core tools
dotnet ef --version

# Restore packages
dotnet restore

# Build solution
dotnet build -c Release

# Run tests
dotnet test

# Create migration
dotnet ef migrations add MigrationName --startup-project src/Grc.HttpApi.Host

# Update database
dotnet ef database update --startup-project src/Grc.HttpApi.Host

# Publish application
dotnet publish src/Grc.HttpApi.Host/Grc.HttpApi.Host.csproj -c Release -o ./publish

# Run published app
cd publish
dotnet Grc.HttpApi.Host.dll
```

## Troubleshooting Quick Fixes

**"dotnet: command not found"**
```bash
export PATH=$PATH:$HOME/.dotnet
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```

**"Permission denied" (SSH)**
```bash
chmod 600 ~/.ssh/id_rsa
chmod 644 ~/.ssh/id_rsa.pub
```

**"Connection refused" (Database)**
```bash
# Check if PostgreSQL is running
sudo systemctl status postgresql
# Or if using Docker
docker ps | grep postgres
```

**"Build failed"**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear
# Restore again
dotnet restore
```


