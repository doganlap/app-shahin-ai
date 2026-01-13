---
description: Repository Information Overview
alwaysApply: true
---

# Repository Information Overview

## Repository Summary
This repository contains a comprehensive Governance, Risk, and Compliance (GRC) ecosystem and a supporting server monitoring suite. The GRC system is a multi-layered enterprise application with a .NET backend and multiple Next.js frontends, integrated with high-performance data analytics and workflow automation tools.

## Repository Structure
- **Shahin-ai/Shahin-Jan-2026/**: Main GRC ecosystem directory.
  - **src/GrcMvc/**: Core ASP.NET Core 8.0 MVC backend.
  - **grc-frontend/**: Main Next.js 14 frontend.
  - **grc-app/**: Next.js 16 mobile/web application.
  - **shahin-ai-website/**: Next.js 14 marketing website.
  - **tests/**: Unit and integration tests for the backend.
- **monitoring/**: Infrastructure monitoring stack.
  - **grafana/**, **prometheus/**, **netdata/**, **zabbix/**: Dockerized monitoring tools.
  - **noc-hub/**: .NET 8.0 monitoring hub.

### Main Repository Components
- **GRC Backend**: Modular ASP.NET Core 8.0 application handling business logic, security, and integrations.
- **Data & Analytics Stack**: Integrated ClickHouse, Kafka, Apache Superset, and Metabase for OLAP and BI.
- **Workflow & Automation**: Camunda BPM and n8n for business process management.
- **Monitoring Suite**: Comprehensive observability stack for real-time and historical system metrics.

## Projects

### GRC MVC System (Core Backend)
**Configuration File**: `Shahin-ai/Shahin-Jan-2026/src/GrcMvc/GrcMvc.csproj`

#### Language & Runtime
**Language**: C#  
**Version**: .NET 8.0  
**Build System**: MSBuild / dotnet CLI  
**Package Manager**: NuGet

#### Dependencies
**Main Dependencies**:
- **ORM**: Entity Framework Core 8.0 (PostgreSQL)
- **Messaging**: MassTransit with RabbitMQ & Confluent.Kafka
- **Background Jobs**: Hangfire
- **Security**: ASP.NET Core Identity & JWT Bearer Authentication
- **Analytics**: ClickHouse.Client
- **Logging**: Serilog (Console, File)

#### Build & Installation
```bash
cd Shahin-ai/Shahin-Jan-2026/src/GrcMvc
dotnet restore
dotnet build
```

#### Docker
**Dockerfile**: `Shahin-ai/Shahin-Jan-2026/src/GrcMvc/Dockerfile`
**Configuration**: Multi-stage build (build/publish/final) with mandatory quality checks for controllers and views. Includes non-root user setup.

#### Testing
**Framework**: xUnit
**Test Location**: `Shahin-ai/Shahin-Jan-2026/tests/GrcMvc.Tests/`
**Run Command**:
```bash
dotnet test
```

### GRC Frontend (Main Web UI)
**Configuration File**: `Shahin-ai/Shahin-Jan-2026/grc-frontend/package.json`

#### Language & Runtime
**Language**: TypeScript  
**Version**: Node.js (Next.js 14.2)  
**Package Manager**: npm

#### Dependencies
**Main Dependencies**:
- **UI**: React 18, Tailwind CSS, Framer Motion, Radix UI
- **State/Data**: TanStack Query, Zustand, Zod
- **Backend**: Prisma (client-side)
- **Visuals**: Three.js (@react-three/fiber)

#### Build & Installation
```bash
cd Shahin-ai/Shahin-Jan-2026/grc-frontend
npm install
npm run build
```

### Server Monitoring Suite
**Type**: Infrastructure as Code / Containerized Stack

#### Specification & Tools
**Type**: Docker Compose based monitoring stack
**Required Tools**: Docker, Docker Compose
**Key Components**: Netdata (19999), Prometheus (9090), Grafana (3000), Zabbix (8080)

#### Key Resources
**Main Files**:
- `monitoring/manage-monitoring.sh`: Central management script.
- `monitoring/*/docker-compose.yml`: Individual service definitions.

#### Usage & Operations
**Key Commands**:
```bash
# Start all monitoring services
./monitoring/manage-monitoring.sh start

# View specific service logs
docker logs -f grafana
```

#### Validation
**Quality Checks**: Docker healthchecks defined for all core services in `docker-compose.yml`.
**Testing Approach**: Automated container health checks and connectivity verification between Prometheus and Node Exporter.
