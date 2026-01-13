# Docker Hub Setup Instructions

## 📋 Prerequisites

1. **Docker Hub Account**: You need a Docker Hub account
2. **Docker Hub Credentials**: Username and access token/password

## 🐳 Build and Push to Docker Hub

### Step 1: Login to Docker Hub

```bash
docker login -u YOUR_DOCKERHUB_USERNAME
# Enter your password or access token when prompted
```

### Step 2: Build the Docker Image

```bash
cd /home/Shahin-ai/Shahin-Jan-2026
docker build -f src/GrcMvc/Dockerfile -t YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:latest .
```

Or with version tag:
```bash
docker build -f src/GrcMvc/Dockerfile -t YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:v1.0.0 .
```

### Step 3: Tag the Image (Optional)

```bash
docker tag YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:latest YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:v1.0.0
```

### Step 4: Push to Docker Hub

```bash
# Push latest
docker push YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:latest

# Push versioned tag
docker push YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:v1.0.0
```

## 🔐 Using Docker Hub Access Token

Instead of password, use an access token:
1. Go to: https://hub.docker.com/settings/security
2. Create new access token
3. Use it as password when running `docker login`

## 📝 Example Complete Workflow

```bash
# 1. Login
docker login -u doganlap

# 2. Build
cd /home/Shahin-ai/Shahin-Jan-2026
docker build -f src/GrcMvc/Dockerfile -t doganlap/shahin-grc-platform:latest .

# 3. Push
docker push doganlap/shahin-grc-platform:latest
```

## 🚀 Quick Test

After pushing, test the image:
```bash
docker pull YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:latest
docker run -p 5000:80 YOUR_DOCKERHUB_USERNAME/shahin-grc-platform:latest
```

## 📦 Image Details

- **Base Image**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Build Image**: `mcr.microsoft.com/dotnet/sdk:8.0`
- **Exposed Ports**: 80, 443
- **User**: Non-root user (appuser:1000)
- **Entry Point**: `dotnet GrcMvc.dll`
