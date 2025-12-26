# Cloud Server Setup Guide for ABP.io GRC Platform

This guide will help you connect to a cloud server and build the ABP.io project.

## Prerequisites

### On Your Local Machine

1. **SSH Client** (Windows 10+ includes OpenSSH)
2. **SSH Key Pair** (generate if you don't have one)
3. **Server Credentials** (IP address, username, password or SSH key)

### On Cloud Server

- Ubuntu 20.04+ / Debian 11+ (recommended)
- Or Windows Server 2019+ (for PowerShell scripts)
- Minimum 2 GB RAM, 2 CPU cores
- 20 GB+ disk space

## Step 1: Generate SSH Key (if needed)

### Windows (PowerShell)
```powershell
ssh-keygen -t rsa -b 4096 -C "your_email@example.com"
# Save to: C:\Users\YourUsername\.ssh\id_rsa
```

### Linux/Mac
```bash
ssh-keygen -t rsa -b 4096 -C "your_email@example.com"
```

## Step 2: Connect to Cloud Server

### Option A: SSH with Password

```bash
ssh username@your-server-ip
# Or with custom port
ssh -p 2222 username@your-server-ip
```

### Option B: SSH with Key (Recommended)

```bash
# Linux/Mac
ssh -i ~/.ssh/id_rsa username@your-server-ip

# Windows (PowerShell)
ssh -i C:\Users\YourUsername\.ssh\id_rsa username@your-server-ip
```

### Option C: Using the Provided Script

Edit `scripts/ssh-connect.sh` and update the variables:

```bash
SERVER_USER="ubuntu"
SERVER_HOST="your-server-ip"
SSH_KEY="~/.ssh/id_rsa"
SERVER_PORT="22"
```

Then run:
```bash
chmod +x scripts/ssh-connect.sh
./scripts/ssh-connect.sh
```

## Step 3: Initial Server Setup (Ubuntu/Debian)

Once connected, run these commands on the server:

```bash
# Update system
sudo apt-get update && sudo apt-get upgrade -y

# Install basic tools
sudo apt-get install -y git curl wget unzip

# Install .NET 8.0 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
export PATH=$PATH:$HOME/.dotnet
export DOTNET_ROOT=$HOME/.dotnet

# Add to .bashrc for persistence
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

# Install Docker (optional, for containerized infrastructure)
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
```

Or use the automated setup script:

```bash
# Upload the script to server first
chmod +x cloud-build-setup.sh
./cloud-build-setup.sh
```

## Step 4: Transfer Project Files to Server

### Option A: Using Git (Recommended)

```bash
# On server
cd /opt
sudo mkdir grc-platform
sudo chown $USER:$USER grc-platform
cd grc-platform
git clone https://github.com/your-org/grc-platform.git .
```

### Option B: Using SCP

```bash
# From local machine
scp -r C:\Shahin-ai\* username@server-ip:/opt/grc-platform/
```

### Option C: Using SFTP

```bash
# Windows (WinSCP) or Linux/Mac (sftp command)
sftp username@server-ip
put -r C:\Shahin-ai\* /opt/grc-platform/
```

## Step 5: Build the Project on Server

### For Linux Server

```bash
cd /opt/grc-platform

# Run the build script
chmod +x scripts/cloud-build-setup.sh
./scripts/cloud-build-setup.sh
```

Or manually:

```bash
cd /opt/grc-platform

# Restore packages
dotnet restore

# Build solution
dotnet build --configuration Release

# Setup infrastructure (if using Docker)
cd docker
docker-compose -f docker-compose.infrastructure.yml up -d
cd ..

# Apply database migrations
cd src/Grc.EntityFrameworkCore
export ConnectionStrings__Default="Host=localhost;Database=grc_db;Username=postgres;Password=postgres"
dotnet ef database update --startup-project ../../Grc.HttpApi.Host
cd ../..
```

### For Windows Server

```powershell
cd C:\grc-platform

# Run the PowerShell build script
.\scripts\cloud-build.ps1
```

Or manually:

```powershell
# Restore packages
dotnet restore

# Build solution
dotnet build --configuration Release

# Apply database migrations
cd src\Grc.EntityFrameworkCore
$env:ConnectionStrings__Default = "Host=localhost;Database=grc_db;Username=postgres;Password=postgres"
dotnet ef database update --startup-project ..\..\Grc.HttpApi.Host
cd ..\..

# Publish application
dotnet publish src\Grc.HttpApi.Host\Grc.HttpApi.Host.csproj -c Release -o ./publish
```

## Step 6: Configure Application Settings

Edit `appsettings.json` or `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=grc_db;Username=postgres;Password=your_password;Port=5432"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "RabbitMQ": {
    "Connections": {
      "Default": {
        "HostName": "localhost",
        "Port": 5672,
        "UserName": "guest",
        "Password": "guest"
      }
    }
  }
}
```

## Step 7: Run the Application

### Development Mode

```bash
cd src/Grc.HttpApi.Host
dotnet run
```

### Production Mode (Linux)

```bash
cd publish
dotnet Grc.HttpApi.Host.dll
```

### As a Service (systemd - Linux)

Create `/etc/systemd/system/grc-api.service`:

```ini
[Unit]
Description=GRC API Service
After=network.target postgresql.service

[Service]
Type=notify
User=www-data
WorkingDirectory=/opt/grc-platform/publish
ExecStart=/usr/bin/dotnet /opt/grc-platform/publish/Grc.HttpApi.Host.dll
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

Then:
```bash
sudo systemctl daemon-reload
sudo systemctl enable grc-api
sudo systemctl start grc-api
sudo systemctl status grc-api
```

## Step 8: Setup Reverse Proxy (Nginx - Optional)

Create `/etc/nginx/sites-available/grc-api`:

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable:
```bash
sudo ln -s /etc/nginx/sites-available/grc-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## Troubleshooting

### Connection Issues

- **Permission denied**: Check SSH key permissions (`chmod 600 ~/.ssh/id_rsa`)
- **Connection timeout**: Verify firewall rules allow SSH (port 22)
- **Host key verification failed**: Remove old host key: `ssh-keygen -R server-ip`

### Build Issues

- **.NET SDK not found**: Verify PATH includes `$HOME/.dotnet`
- **NuGet restore fails**: Check internet connection, verify NuGet sources
- **Build errors**: Check logs for missing dependencies or configuration issues

### Database Issues

- **Connection refused**: Ensure PostgreSQL is running and accessible
- **Migration fails**: Check connection string and database permissions
- **Seed data not loaded**: Verify ProductSeedData is registered in module

## Security Recommendations

1. **Firewall**: Configure UFW (Ubuntu) or firewall rules
   ```bash
   sudo ufw allow 22/tcp   # SSH
   sudo ufw allow 80/tcp   # HTTP
   sudo ufw allow 443/tcp  # HTTPS
   sudo ufw enable
   ```

2. **SSH Hardening**: Disable password authentication, use keys only
3. **SSL/TLS**: Use Let's Encrypt for HTTPS certificates
4. **Database**: Use strong passwords, restrict network access
5. **Secrets**: Use environment variables or Azure Key Vault for sensitive data

## Next Steps

- Setup CI/CD pipeline (GitHub Actions, Azure DevOps)
- Configure monitoring (Application Insights, Prometheus)
- Setup log aggregation (Seq, ELK Stack)
- Configure auto-scaling for high availability


