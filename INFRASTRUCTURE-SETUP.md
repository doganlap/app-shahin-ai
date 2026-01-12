# Infrastructure Setup Guide

This document describes how to configure Redis, RabbitMQ, and MinIO for the GRC Platform.

## Redis Caching Configuration

### Package Installation
```bash
abp add-package Volo.Abp.Caching.StackExchangeRedis --project src/Grc.HttpApi.Host
```

### Configuration in Module
Add to `GrcHttpApiHostModule.cs`:
```csharp
context.Services.AddStackExchangeRedisCache(options =>
{
    var configuration = context.Services.GetConfiguration();
    options.Configuration = configuration["Redis:Configuration"];
});
```

### Usage
Redis is automatically used by ABP's distributed caching system. No additional code needed.

## RabbitMQ Configuration

### Package Installation
```bash
abp add-package Volo.Abp.BackgroundJobs.RabbitMQ --project src/Grc.HttpApi.Host
abp add-package Volo.Abp.EventBus.RabbitMQ --project src/Grc.HttpApi.Host
```

### Configuration in Module
Add to `GrcHttpApiHostModule.cs`:
```csharp
Configure<AbpRabbitMqOptions>(options =>
{
    var configuration = context.Services.GetConfiguration();
    options.Connections.Default.HostName = configuration["RabbitMQ:Connections:Default:HostName"];
    options.Connections.Default.Port = configuration.GetValue<int>("RabbitMQ:Connections:Default:Port", 5672);
    options.Connections.Default.UserName = configuration["RabbitMQ:Connections:Default:UserName"];
    options.Connections.Default.Password = configuration["RabbitMQ:Connections:Default:Password"];
});

Configure<AbpRabbitMqEventBusOptions>(options =>
{
    options.ClientName = "GrcPlatform";
    options.ExchangeName = "GrcEvents";
});
```

## MinIO Blob Storage Configuration

### Package Installation
```bash
abp add-package Volo.Abp.BlobStoring.Minio --project src/Grc.HttpApi.Host
```

### Configuration in Module
Add to `GrcHttpApiHostModule.cs`:
```csharp
Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseMinio(config =>
        {
            var configuration = context.Services.GetConfiguration();
            config.EndPoint = configuration["Minio:EndPoint"];
            config.AccessKey = configuration["Minio:AccessKey"];
            config.SecretKey = configuration["Minio:SecretKey"];
            config.BucketName = configuration["Minio:BucketName"];
            config.UseSSL = configuration.GetValue<bool>("Minio:UseSSL", false);
        });
    });
});
```

### Create Bucket
The bucket will be created automatically on first use, or you can create it manually:
```bash
docker exec -it grc-minio mc alias set myminio http://localhost:9000 minioadmin minioadmin
docker exec -it grc-minio mc mb myminio/grc-evidence
```

## PostgreSQL Configuration

The DbContext is already configured in `GrcDbContext.cs`. Ensure the connection string in `appsettings.json` is correct.

## Starting Infrastructure

Use Docker Compose to start all infrastructure services:
```bash
docker-compose -f docker/docker-compose.infrastructure.yml up -d
```

This will start:
- PostgreSQL on port 5432
- Redis on port 6379
- RabbitMQ on ports 5672 (AMQP) and 15672 (Management UI)
- Elasticsearch on port 9200
- MinIO on ports 9000 (API) and 9001 (Console)
- Seq on port 5341

## Verification

### Redis
```bash
docker exec -it grc-redis redis-cli ping
# Should return: PONG
```

### RabbitMQ
Access management UI at: http://localhost:15672
- Username: guest
- Password: guest

### MinIO
Access console at: http://localhost:9001
- Username: minioadmin
- Password: minioadmin

### Elasticsearch
```bash
curl http://localhost:9200
# Should return cluster information
```

### PostgreSQL
```bash
docker exec -it grc-postgres psql -U postgres -d grc_db -c "SELECT version();"
```

