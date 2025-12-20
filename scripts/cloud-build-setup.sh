#!/bin/bash
# ============================================================
# Cloud Server Build Setup Script for ABP.io GRC Platform
# ============================================================

set -e

echo "=========================================="
echo "ABP.io GRC Platform - Cloud Build Setup"
echo "=========================================="

# Configuration
PROJECT_DIR="/opt/grc-platform"
GIT_REPO_URL="${GIT_REPO_URL:-https://github.com/your-org/grc-platform.git}"
BRANCH="${BRANCH:-main}"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Step 1: Installing prerequisites...${NC}"

# Update package list
sudo apt-get update -y

# Install .NET 8.0 SDK
if ! command -v dotnet &> /dev/null; then
    echo "Installing .NET 8.0 SDK..."
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 8.0
    export PATH=$PATH:$HOME/.dotnet
    export DOTNET_ROOT=$HOME/.dotnet
    echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
    echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
else
    echo ".NET SDK already installed: $(dotnet --version)"
fi

# Install EF Core tools
echo "Installing EF Core tools..."
dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef

# Install Docker
if ! command -v docker &> /dev/null; then
    echo "Installing Docker..."
    curl -fsSL https://get.docker.com -o get-docker.sh
    sudo sh get-docker.sh
    sudo usermod -aG docker $USER
    rm get-docker.sh
else
    echo "Docker already installed: $(docker --version)"
fi

# Install Docker Compose
if ! command -v docker-compose &> /dev/null; then
    echo "Installing Docker Compose..."
    sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
    sudo chmod +x /usr/local/bin/docker-compose
else
    echo "Docker Compose already installed: $(docker-compose --version)"
fi

# Install PostgreSQL client (optional, for direct DB access)
sudo apt-get install -y postgresql-client

# Install Git if not present
if ! command -v git &> /dev/null; then
    sudo apt-get install -y git
fi

echo -e "${GREEN}Step 2: Setting up project directory...${NC}"

# Create project directory
sudo mkdir -p $PROJECT_DIR
sudo chown -R $USER:$USER $PROJECT_DIR

# Clone repository (or update if exists)
if [ -d "$PROJECT_DIR/.git" ]; then
    echo "Repository exists, updating..."
    cd $PROJECT_DIR
    git fetch origin
    git checkout $BRANCH
    git pull origin $BRANCH
else
    echo "Cloning repository..."
    git clone -b $BRANCH $GIT_REPO_URL $PROJECT_DIR
    cd $PROJECT_DIR
fi

echo -e "${GREEN}Step 3: Setting up infrastructure services...${NC}"

# Start infrastructure services with Docker Compose
if [ -f "docker/docker-compose.infrastructure.yml" ]; then
    cd docker
    docker-compose -f docker-compose.infrastructure.yml up -d
    echo "Waiting for services to start..."
    sleep 10
    cd ..
else
    echo -e "${YELLOW}Warning: docker-compose.infrastructure.yml not found${NC}"
fi

echo -e "${GREEN}Step 4: Restoring NuGet packages...${NC}"
dotnet restore

echo -e "${GREEN}Step 5: Building solution...${NC}"
dotnet build --configuration Release --no-restore

echo -e "${GREEN}Step 6: Running database migrations...${NC}"

# Check if connection string is set
if [ -z "$ConnectionStrings__Default" ]; then
    echo -e "${YELLOW}Warning: ConnectionStrings__Default not set. Using default: Host=localhost;Database=grc_db;Username=postgres;Password=postgres${NC}"
    export ConnectionStrings__Default="Host=localhost;Database=grc_db;Username=postgres;Password=postgres"
fi

# Apply migrations
cd src/Grc.EntityFrameworkCore
dotnet ef database update --startup-project ../../Grc.HttpApi.Host
cd ../..

echo -e "${GREEN}=========================================="
echo "Setup complete! Next steps:"
echo "=========================================="
echo "1. Configure appsettings.json with your settings"
echo "2. Run: dotnet run --project src/Grc.HttpApi.Host"
echo "3. Or: dotnet publish -c Release -o ./publish"
echo "==========================================${NC}"


