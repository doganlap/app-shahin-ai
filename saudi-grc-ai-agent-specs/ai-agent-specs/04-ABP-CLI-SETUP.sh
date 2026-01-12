# ============================================================
# ABP.io OPEN SOURCE - CLI COMMANDS & SETUP
# AI Agent: Execute these commands to scaffold the project
# ============================================================

# ============================================================
# STEP 1: CREATE ABP SOLUTION
# ============================================================

# Install ABP CLI (if not installed)
dotnet tool install -g Volo.Abp.Cli

# Create new ABP solution with Angular UI
abp new Grc -t app -u angular -d ef -dbms PostgreSQL --no-random-port

# Navigate to solution
cd Grc

# ============================================================
# STEP 2: ADD ADDITIONAL MODULES
# ============================================================

# Add SignalR module
abp add-package Volo.Abp.AspNetCore.SignalR --project src/Grc.HttpApi.Host

# Add Redis caching
abp add-package Volo.Abp.Caching.StackExchangeRedis --project src/Grc.HttpApi.Host

# Add RabbitMQ for background jobs
abp add-package Volo.Abp.BackgroundJobs.RabbitMQ --project src/Grc.HttpApi.Host

# Add RabbitMQ event bus
abp add-package Volo.Abp.EventBus.RabbitMQ --project src/Grc.HttpApi.Host

# Add MinIO blob storage
abp add-package Volo.Abp.BlobStoring.Minio --project src/Grc.HttpApi.Host

# Add OpenIddict for authentication
# (Already included in ABP template)

# ============================================================
# STEP 3: CREATE DOMAIN MODULES
# ============================================================

# Create FrameworkLibrary module
abp add-module Grc.FrameworkLibrary --new --add-to-solution-file

# Create Assessment module
abp add-module Grc.Assessment --new --add-to-solution-file

# Create Evidence module
abp add-module Grc.Evidence --new --add-to-solution-file

# Create Risk module
abp add-module Grc.Risk --new --add-to-solution-file

# Create ActionPlan module
abp add-module Grc.ActionPlan --new --add-to-solution-file

# Create Workflow module
abp add-module Grc.Workflow --new --add-to-solution-file

# Create Audit module
abp add-module Grc.Audit --new --add-to-solution-file

# Create Reporting module
abp add-module Grc.Reporting --new --add-to-solution-file

# Create Policy module
abp add-module Grc.Policy --new --add-to-solution-file

# Create Notification module
abp add-module Grc.Notification --new --add-to-solution-file

# Create Integration module
abp add-module Grc.Integration --new --add-to-solution-file

# Create AI module
abp add-module Grc.AI --new --add-to-solution-file

# Create Calendar module
abp add-module Grc.Calendar --new --add-to-solution-file

# Create Vendor module
abp add-module Grc.Vendor --new --add-to-solution-file

# ============================================================
# STEP 4: ADD NUGET PACKAGES
# ============================================================

# Add ML.NET for AI features
cd src/Grc.AI.Application
dotnet add package Microsoft.ML
dotnet add package Microsoft.ML.TensorFlow
cd ../..

# Add HotChocolate for GraphQL
cd src/Grc.HttpApi.Host
dotnet add package HotChocolate.AspNetCore
dotnet add package HotChocolate.Data.EntityFramework
cd ../..

# Add Elasticsearch
cd src/Grc.HttpApi.Host
dotnet add package Elastic.Clients.Elasticsearch
cd ../..

# Add Tesseract OCR
cd src/Grc.AI.Application
dotnet add package Tesseract
cd ../..

# Add PDF generation
cd src/Grc.Reporting.Application
dotnet add package QuestPDF
cd ../..

# ============================================================
# STEP 5: CONFIGURE MULTI-TENANCY
# ============================================================

# Multi-tenancy is already enabled in ABP by default
# Configure domain-based resolution in Program.cs:

# File: src/Grc.HttpApi.Host/Program.cs
# Add this configuration:
<<EOF
// Add after builder.Services.AddApplication<GrcHttpApiHostModule>()

builder.Services.Configure<AbpTenantResolveOptions>(options =>
{
    options.TenantResolvers.Clear();
    options.TenantResolvers.Add(new DomainTenantResolveContributor("{0}.grc-platform.sa"));
    options.TenantResolvers.Add(new HeaderTenantResolveContributor());
    options.TenantResolvers.Add(new CookieTenantResolveContributor());
});
EOF

# ============================================================
# STEP 6: DATABASE SETUP
# ============================================================

# Update connection string in appsettings.json
# File: src/Grc.HttpApi.Host/appsettings.json
<<EOF
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=grc_db;Username=postgres;Password=postgres"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "RabbitMQ": {
    "Connections": {
      "Default": {
        "HostName": "localhost"
      }
    }
  },
  "Minio": {
    "EndPoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "grc-evidence"
  },
  "Elasticsearch": {
    "Url": "http://localhost:9200"
  }
}
EOF

# Create database migration
cd src/Grc.EntityFrameworkCore
dotnet ef migrations add InitialCreate
dotnet ef database update
cd ../..

# ============================================================
# STEP 7: ANGULAR SETUP
# ============================================================

cd angular

# Install dependencies
npm install

# Install PrimeNG
npm install primeng primeicons

# Install NgRx
npm install @ngrx/store @ngrx/effects @ngrx/entity @ngrx/store-devtools

# Install SignalR client
npm install @microsoft/signalr

# Install chart libraries
npm install chart.js ng2-charts

# Generate components
ng generate module features/dashboard --routing
ng generate module features/frameworks --routing
ng generate module features/assessments --routing
ng generate module features/controls --routing
ng generate module features/evidence --routing
ng generate module features/risks --routing
ng generate module features/reports --routing
ng generate module features/workflows --routing
ng generate module features/ai-assistant --routing
ng generate module features/admin --routing

# Generate services
ng generate service core/services/framework
ng generate service core/services/assessment
ng generate service core/services/control-assessment
ng generate service core/services/evidence
ng generate service core/services/dashboard
ng generate service core/services/signalr
ng generate service core/services/ai

# Build Angular app
npm run build

cd ..

# ============================================================
# STEP 8: RUN THE APPLICATION
# ============================================================

# Start infrastructure services with Docker
docker-compose -f docker/docker-compose.infrastructure.yml up -d

# Run the API
cd src/Grc.HttpApi.Host
dotnet run

# In another terminal, run Angular
cd angular
npm start

# ============================================================
# STEP 9: SEED DATA
# ============================================================

# Run database seeder
cd src/Grc.DbMigrator
dotnet run

# ============================================================
# PROJECT STRUCTURE AFTER SETUP
# ============================================================

<<STRUCTURE
Grc/
├── src/
│   ├── Grc.Domain.Shared/
│   │   ├── GrcDomainSharedModule.cs
│   │   ├── Localization/
│   │   │   ├── Grc/
│   │   │   │   ├── en.json
│   │   │   │   └── ar.json
│   │   └── Enums/
│   │       ├── IndustrySector.cs
│   │       ├── AssessmentType.cs
│   │       ├── ControlAssessmentStatus.cs
│   │       └── ...
│   │
│   ├── Grc.Domain/
│   │   ├── GrcDomainModule.cs
│   │   ├── Regulators/
│   │   │   ├── Regulator.cs
│   │   │   └── IRegulatorRepository.cs
│   │   ├── Frameworks/
│   │   │   ├── Framework.cs
│   │   │   ├── Control.cs
│   │   │   ├── ControlMapping.cs
│   │   │   └── IFrameworkRepository.cs
│   │   ├── Assessments/
│   │   │   ├── Assessment.cs
│   │   │   ├── ControlAssessment.cs
│   │   │   ├── IAssessmentRepository.cs
│   │   │   └── AssessmentManager.cs
│   │   └── ...
│   │
│   ├── Grc.Application.Contracts/
│   │   ├── GrcApplicationContractsModule.cs
│   │   ├── Regulators/
│   │   │   ├── RegulatorDto.cs
│   │   │   └── IRegulatorAppService.cs
│   │   ├── Frameworks/
│   │   │   ├── FrameworkDto.cs
│   │   │   ├── ControlDto.cs
│   │   │   └── IFrameworkAppService.cs
│   │   ├── Assessments/
│   │   │   ├── AssessmentDto.cs
│   │   │   ├── CreateAssessmentInput.cs
│   │   │   └── IAssessmentAppService.cs
│   │   └── ...
│   │
│   ├── Grc.Application/
│   │   ├── GrcApplicationModule.cs
│   │   ├── Regulators/
│   │   │   └── RegulatorAppService.cs
│   │   ├── Frameworks/
│   │   │   └── FrameworkAppService.cs
│   │   ├── Assessments/
│   │   │   ├── AssessmentAppService.cs
│   │   │   └── AssessmentTemplateGenerator.cs
│   │   └── ...
│   │
│   ├── Grc.EntityFrameworkCore/
│   │   ├── GrcDbContext.cs
│   │   ├── GrcEntityFrameworkCoreModule.cs
│   │   ├── Repositories/
│   │   │   ├── RegulatorRepository.cs
│   │   │   ├── FrameworkRepository.cs
│   │   │   └── AssessmentRepository.cs
│   │   └── Migrations/
│   │
│   ├── Grc.HttpApi/
│   │   ├── GrcHttpApiModule.cs
│   │   └── Controllers/
│   │       ├── RegulatorController.cs
│   │       ├── FrameworkController.cs
│   │       └── AssessmentController.cs
│   │
│   ├── Grc.HttpApi.Host/
│   │   ├── Program.cs
│   │   ├── GrcHttpApiHostModule.cs
│   │   ├── appsettings.json
│   │   └── Hubs/
│   │       └── GrcHub.cs
│   │
│   ├── Grc.DbMigrator/
│   │   ├── Program.cs
│   │   └── GrcDbMigratorModule.cs
│   │
│   └── Grc.FrameworkLibrary/
│   │   ├── Domain/
│   │   ├── Domain.Shared/
│   │   ├── Application/
│   │   ├── Application.Contracts/
│   │   ├── EntityFrameworkCore/
│   │   └── HttpApi/
│   │
│   └── ... (other modules)
│
├── angular/
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/
│   │   │   ├── shared/
│   │   │   ├── layout/
│   │   │   └── features/
│   │   ├── assets/
│   │   │   ├── i18n/
│   │   │   └── styles/
│   │   └── environments/
│   └── package.json
│
├── docker/
│   ├── docker-compose.yml
│   ├── docker-compose.infrastructure.yml
│   └── Dockerfile
│
├── test/
│   ├── Grc.Domain.Tests/
│   ├── Grc.Application.Tests/
│   └── Grc.HttpApi.Client.Tests/
│
├── common.props
├── Grc.sln
└── README.md
STRUCTURE

# ============================================================
# DOCKER COMPOSE - INFRASTRUCTURE
# ============================================================

# File: docker/docker-compose.infrastructure.yml
<<DOCKER
version: '3.8'

services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: grc_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7.2-alpine
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3.13-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest

  elasticsearch:
    image: elasticsearch:8.12.0
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
    ports:
      - "9200:9200"
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data

  minio:
    image: minio/minio
    command: server /data --console-address ":9001"
    ports:
      - "9000:9000"
      - "9001:9001"
    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin
    volumes:
      - minio_data:/data

  seq:
    image: datalust/seq
    environment:
      ACCEPT_EULA: "Y"
    ports:
      - "5341:80"
    volumes:
      - seq_data:/data

volumes:
  postgres_data:
  elasticsearch_data:
  minio_data:
  seq_data:
DOCKER

# ============================================================
# END OF SETUP COMMANDS
# ============================================================
