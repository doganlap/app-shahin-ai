# Saudi GRC Platform - ABP.io Implementation

A comprehensive Governance, Risk, and Compliance (GRC) platform built with ABP.io Framework, specifically designed for Saudi Arabian business requirements.

## 🚀 Project Overview

This project implements a modular monolith architecture using ABP.io Framework 8.3+ with .NET 8.0, providing a robust foundation for GRC operations with multi-tenancy support, localization (Arabic/English), and subscription-based product management.

## 📋 Table of Contents

- [Architecture](#architecture)
- [Modules](#modules)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [API Documentation](#api-documentation)
- [Development](#development)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🏗️ Architecture

### Modular Monolith with DDD
- **Domain-Driven Design (DDD)**: Aggregate Roots, Entities, Value Objects, Domain Services
- **Multi-tenancy**: Domain-based tenant resolution with Row-Level Security (RLS)
- **Localization**: Bilingual support (Arabic/English) using `LocalizedString`
- **CQRS Ready**: Application services prepared for command/query separation

### Layers
1. **Domain Layer**: Core business logic, entities, domain services
2. **Application Layer**: Use cases, DTOs, application services
3. **Infrastructure Layer**: EF Core, repositories, external services
4. **API Layer**: RESTful HTTP APIs, OpenAPI/Swagger
5. **Frontend Layer**: Angular 17+ with PrimeNG (planned)

## 📦 Modules

### Core Modules (16 Total)

1. **Product Module** ✨ (NEW)
   - Product catalog management
   - Feature definitions
   - Quota management
   - Pricing plans
   - Tenant subscriptions
   - Quota usage tracking

2. **Audit Module**
   - Audit trails
   - Compliance tracking
   - Evidence management

3. **Risk Module**
   - Risk assessment
   - Risk mitigation
   - Risk reporting

4. **Compliance Module**
   - Regulatory compliance
   - Standards management
   - Compliance monitoring

5. **Policy Module**
   - Policy management
   - Policy lifecycle
   - Policy enforcement

6. **Incident Module**
   - Incident tracking
   - Incident response
   - Incident reporting

... and 10 more modules (see `00-PROJECT-SPEC.yaml` for complete list)

## 🛠️ Technology Stack

### Backend
- **Framework**: ABP.io 8.3+
- **Runtime**: .NET 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL 16+
- **Identity**: OpenIddict
- **Real-time**: SignalR
- **Messaging**: RabbitMQ
- **Caching**: Redis
- **Search**: Elasticsearch
- **Storage**: MinIO

### Frontend (Planned)
- **Framework**: Angular 17+
- **UI Library**: PrimeNG
- **State Management**: NgRx (planned)

### Infrastructure
- **Containerization**: Docker
- **Orchestration**: Kubernetes
- **CI/CD**: GitHub Actions
- **Cloud**: DigitalOcean, AWS, Azure

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 16+
- Docker Desktop (optional, for infrastructure)
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/shahin-ai.git
   cd shahin-ai
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database**
   - Update connection string in `appsettings.json`
   - Run migrations:
     ```bash
     dotnet ef database update --project src/Grc.EntityFrameworkCore
     ```

4. **Run the application**
   ```bash
   dotnet run --project src/Grc.HttpApi.Host
   ```

5. **Access Swagger UI**
   - Navigate to: `https://localhost:44300/swagger`

## 📁 Project Structure

```
shahin-ai/
├── src/                          # Source code
│   ├── Grc.Domain.Shared/        # Shared domain contracts
│   ├── Grc.Product.Domain/       # Product domain layer
│   ├── Grc.Product.Application/  # Product application layer
│   ├── Grc.Product.EntityFrameworkCore/ # Product data access
│   ├── Grc.Product.HttpApi/      # Product API controllers
│   ├── Grc.EntityFrameworkCore/   # Main DbContext
│   └── Grc.HttpApi.Host/         # API host application
├── scripts/                       # Utility scripts
│   ├── setup-github.ps1          # GitHub setup script
│   ├── cloud-build.ps1           # Cloud build script
│   └── list-ssh-servers.ps1      # SSH server list
├── config/                        # Configuration files
├── 00-PROJECT-SPEC.yaml          # Project specification
├── 01-ENTITIES.yaml              # Domain entities definition
├── 02-DATABASE-SCHEMA.sql        # Database schema
├── 03-API-SPEC.yaml              # OpenAPI specification
├── 05-TASK-BREAKDOWN.yaml        # Implementation tasks
└── README.md                     # This file
```

## 🗄️ Database Schema

The database schema is defined in `02-DATABASE-SCHEMA.sql` and includes:

- **Products**: Product catalog entries
- **Product Features**: Feature definitions per product
- **Product Quotas**: Quota limits per product
- **Pricing Plans**: Pricing tiers and billing periods
- **Tenant Subscriptions**: Active tenant subscriptions
- **Quota Usage**: Real-time quota usage tracking

All tenant-scoped tables include Row-Level Security (RLS) policies.

## 📡 API Documentation

API specifications are available in:
- **OpenAPI Spec**: `03-API-SPEC.yaml`
- **Swagger UI**: Available when running the application

### Key Endpoints

#### Products
- `GET /api/products` - List all products
- `POST /api/products` - Create product
- `GET /api/products/{id}` - Get product details
- `PUT /api/products/{id}` - Update product
- `GET /api/products/{id}/features` - Get product features
- `GET /api/products/{id}/quotas` - Get product quotas
- `GET /api/products/{id}/pricing` - Get pricing plans

#### Subscriptions
- `GET /api/subscriptions/current` - Get current tenant subscription
- `POST /api/subscriptions/subscribe` - Subscribe to a product
- `POST /api/subscriptions/{id}/cancel` - Cancel subscription
- `POST /api/subscriptions/upgrade` - Upgrade subscription
- `GET /api/subscriptions/quota-usage` - Get quota usage
- `POST /api/subscriptions/check-quota` - Check quota availability

## 💻 Development

### Adding a New Module

1. Create domain entities in `Grc.<Module>.Domain`
2. Create application services in `Grc.<Module>.Application`
3. Create EF Core configurations in `Grc.<Module>.EntityFrameworkCore`
4. Create API controllers in `Grc.<Module>.HttpApi`
5. Register modules in `GrcHttpApiHostModule`

### Running Migrations

```bash
# Create migration
dotnet ef migrations add AddProductModule --project src/Grc.EntityFrameworkCore

# Update database
dotnet ef database update --project src/Grc.EntityFrameworkCore
```

### Running Tests

```bash
dotnet test
```

## 🚢 Deployment

### Cloud Setup

See `CLOUD-SERVER-SETUP.md` for detailed cloud deployment instructions.

Quick setup:
```powershell
# Build and deploy to cloud
.\scripts\cloud-build.ps1 -ServerAddress your-server.com -Username root
```

### Docker

```bash
docker-compose up -d
```

## 📝 Specification Files

This project uses YAML specification files for documentation and code generation:

- `00-PROJECT-SPEC.yaml` - Overall project structure
- `01-ENTITIES.yaml` - Domain entities and enums
- `02-DATABASE-SCHEMA.sql` - Database schema
- `03-API-SPEC.yaml` - API endpoints and DTOs
- `05-TASK-BREAKDOWN.yaml` - Implementation tasks

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Contact

For questions or support, please open an issue in the GitHub repository.

## 🙏 Acknowledgments

- ABP.io Framework team
- .NET community
- All contributors

---

**Built with ❤️ for Saudi GRC Platform**

