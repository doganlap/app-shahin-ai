# Complete Monitoring Infrastructure Structure

## 📁 Directory Overview

```
/home/monitoring/
├── 🐳 Docker-based Monitoring Solutions
│   ├── netdata/
│   ├── prometheus/
│   ├── grafana/
│   └── zabbix/
│
├── 🎯 Custom NOC Hub Application (.NET)
│   └── noc-hub/
│
├── 📚 Documentation
│   ├── README.md
│   ├── QUICK-START.md
│   ├── INSTALLATION-SUMMARY.md
│   └── COMPLETE-STRUCTURE.md (this file)
│
└── 🛠️ Management Scripts
    └── manage-monitoring.sh
```

---

## 🔍 Detailed Structure

### 1. Netdata (Real-time Monitoring)
```
/home/monitoring/netdata/
└── docker-compose.yml          # Netdata container configuration
    └── Services:
        └── netdata:19999       # Web interface
    └── Volumes:
        ├── netdataconfig       # Configuration data
        ├── netdatalib          # Library data
        └── netdatacache        # Cache data
```

**Access:** http://localhost:19999
**Purpose:** Sub-second real-time monitoring with auto-generated dashboards

---

### 2. Prometheus (Metrics Database)
```
/home/monitoring/prometheus/
├── docker-compose.yml          # Prometheus + Node Exporter
├── prometheus.yml              # Scrape configuration
└── Services:
    ├── prometheus:9090         # Time-series database
    └── node-exporter:9100      # System metrics exporter
└── Volumes:
    └── prometheus-data         # Metric storage
```

**Access:**
- Prometheus: http://localhost:9090
- Node Exporter: http://localhost:9100/metrics

**Purpose:** Metrics collection, storage, and querying

---

### 3. Grafana (Visualization Platform)
```
/home/monitoring/grafana/
├── docker-compose.yml          # Grafana container
├── provisioning/
│   ├── datasources/
│   │   └── prometheus.yml      # Auto-config Prometheus connection
│   └── dashboards/
│       └── dashboard.yml       # Dashboard provider config
└── Services:
    └── grafana:3000            # Web interface
└── Volumes:
    └── grafana-data            # Dashboard & config storage
```

**Access:** http://localhost:3000
**Credentials:** admin / admin
**Purpose:** Beautiful dashboards and data visualization

---

### 4. Zabbix (Enterprise Monitoring)
```
/home/monitoring/zabbix/
└── docker-compose.yml          # Full Zabbix stack
    └── Services:
        ├── mysql-server:3306       # Database
        ├── zabbix-server:10051     # Monitoring server
        ├── zabbix-web:8080         # Web interface
        └── zabbix-agent:10050      # Local monitoring agent
    └── Volumes:
        ├── mysql-data              # Database storage
        └── zabbix-server-data      # Server data
```

**Access:** http://localhost:8080
**Credentials:** Admin / zabbix
**Purpose:** Traditional enterprise monitoring with advanced alerting

---

### 5. NOC Hub (Custom .NET Application)
```
/home/monitoring/noc-hub/
├── 📄 Project Files
│   ├── NocHub.csproj           # .NET 8.0 project file
│   ├── Program.cs              # Application entry point
│   ├── monitoring.sln          # Solution file (parent dir)
│   └── appsettings.json        # Configuration
│   └── appsettings.Development.json
│
├── 📁 Application Structure
│   ├── Controllers/
│   │   └── MonitoringController.cs    # API endpoints
│   │
│   ├── Services/
│   │   └── MonitoringService.cs       # Business logic
│   │
│   ├── Pages/                  # Razor Pages
│   │   ├── Index.cshtml        # Home page
│   │   ├── Dashboard.cshtml    # Monitoring dashboard
│   │   ├── Privacy.cshtml
│   │   ├── Error.cshtml
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _Layout.cshtml.css
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   ├── _ViewImports.cshtml
│   │   └── _ViewStart.cshtml
│   │
│   ├── wwwroot/                # Static files
│   │   ├── css/
│   │   │   └── site.css
│   │   ├── js/
│   │   │   └── site.js
│   │   ├── lib/
│   │   │   ├── bootstrap/      # Bootstrap CSS framework
│   │   │   ├── jquery/         # jQuery library
│   │   │   ├── jquery-validation/
│   │   │   └── jquery-validation-unobtrusive/
│   │   └── favicon.ico
│   │
│   ├── Properties/
│   │   └── launchSettings.json # Development settings
│   │
│   ├── config/                 # Additional configuration
│   ├── public/                 # Public assets
│   │
│   ├── bin/Debug/net8.0/       # Build output
│   └── obj/                    # Build artifacts
│
└── 🎯 Purpose: Custom monitoring dashboard/hub application
```

---

## 🌐 All Access Points

| Service | URL | Credentials | Port |
|---------|-----|-------------|------|
| **Netdata** | http://localhost:19999 | None | 19999 |
| **Prometheus** | http://localhost:9090 | None | 9090 |
| **Node Exporter** | http://localhost:9100/metrics | None | 9100 |
| **Grafana** | http://localhost:3000 | admin / admin | 3000 |
| **Zabbix Web** | http://localhost:8080 | Admin / zabbix | 8080 |
| **Zabbix Server** | tcp://localhost:10051 | - | 10051 |
| **Zabbix Agent** | tcp://localhost:10050 | - | 10050 |
| **MySQL (Zabbix)** | tcp://localhost:3306 | zabbix / zabbix_password | 3306 |
| **NOC Hub** | (Not running - needs build) | - | TBD |

---

## 🐳 Docker Volumes

### Persistent Data Storage
```
Docker Volumes Created:
├── netdata_netdataconfig       # Netdata configuration
├── netdata_netdatalib          # Netdata library data
├── netdata_netdatacache        # Netdata cache
├── prometheus_prometheus-data  # Prometheus metrics
├── grafana_grafana-data        # Grafana dashboards
├── zabbix_mysql-data          # Zabbix database
└── zabbix_zabbix-server-data  # Zabbix server data
```

**View volumes:**
```bash
docker volume ls | grep -E "netdata|prometheus|grafana|zabbix"
```

**Inspect volume:**
```bash
docker volume inspect <volume-name>
```

---

## 🌉 Docker Networks

```
Networks Created:
├── netdata_default             # Isolated Netdata network
├── prometheus_monitoring       # Prometheus + Node Exporter + Grafana
└── zabbix_zabbix-network      # Zabbix full stack
```

**View networks:**
```bash
docker network ls | grep -E "netdata|prometheus|grafana|zabbix"
```

---

## 📊 Running Containers

```bash
# Check all monitoring containers
docker ps --format "table {{.Names}}\t{{.Image}}\t{{.Status}}\t{{.Ports}}"

# Expected containers:
# - netdata
# - prometheus
# - node-exporter
# - grafana
# - zabbix-server
# - zabbix-web
# - zabbix-agent
# - zabbix-mysql
```

---

## 🛠️ Management Tools

### Main Management Script
```
/home/monitoring/manage-monitoring.sh

Commands:
├── start       # Start all Docker services
├── stop        # Stop all Docker services
├── restart     # Restart all services
├── status      # Show container status
├── urls        # Display access URLs
├── logs        # View service logs
└── stats       # Show resource usage
```

**Usage:**
```bash
/home/monitoring/manage-monitoring.sh status
/home/monitoring/manage-monitoring.sh logs grafana
/home/monitoring/manage-monitoring.sh stats
```

---

## 🔐 Data Persistence

### What Happens on Restart?
- ✅ **Configuration**: Preserved in Docker volumes
- ✅ **Metrics Data**: Retained (Prometheus, Zabbix DB)
- ✅ **Dashboards**: Saved (Grafana)
- ✅ **Custom Settings**: Kept
- ⚠️ **Real-time Data**: Netdata streams live data (minimal history)

### Backup Important Data
```bash
# Backup Grafana dashboards
docker exec grafana grafana-cli admin export-dashboard

# Backup Prometheus data
docker exec prometheus promtool tsdb snapshot /prometheus

# Backup Zabbix database
docker exec zabbix-mysql mysqldump -u root -proot_password zabbix > zabbix_backup.sql
```

---

## 🔄 Integration Map

```
┌─────────────────────────────────────────────────────────────┐
│                    Monitoring Ecosystem                      │
└─────────────────────────────────────────────────────────────┘

System Resources (CPU, Memory, Disk, Network)
           │
           ├─→ [Node Exporter:9100] → Exports metrics
           │           │
           │           ↓
           │   [Prometheus:9090] → Scrapes & stores metrics
           │           │
           │           ↓
           │   [Grafana:3000] → Visualizes data
           │
           ├─→ [Netdata:19999] → Real-time monitoring
           │
           ├─→ [Zabbix Agent:10050] → Collects data
           │           │
           │           ↓
           │   [Zabbix Server:10051] → Processes data
           │           │
           │           ↓
           │   [Zabbix Web:8080] → Displays dashboard
           │           │
           │           ↓
           │   [MySQL:3306] → Stores historical data
           │
           └─→ [NOC Hub] → Custom monitoring app
```

---

## 📦 Resource Usage

### Current Container Stats
```bash
# View live resource usage
/home/monitoring/manage-monitoring.sh stats

# Or directly:
docker stats netdata prometheus grafana zabbix-server zabbix-web
```

### Typical Resource Footprint
| Container | CPU | Memory | Disk |
|-----------|-----|--------|------|
| Netdata | ~5% | ~100MB | Minimal |
| Prometheus | ~3% | ~200MB | Growing |
| Node Exporter | <1% | ~20MB | Minimal |
| Grafana | ~2% | ~100MB | ~100MB |
| Zabbix Server | ~5% | ~150MB | Minimal |
| Zabbix Web | ~2% | ~50MB | Minimal |
| Zabbix MySQL | ~5% | ~200MB | Growing |
| **Total** | ~22% | ~820MB | ~500MB+ |

---

## 🚀 NOC Hub Application

### Status
⚠️ **Not currently running** - needs to be built and started

### To Run NOC Hub:
```bash
cd /home/monitoring/noc-hub

# Restore dependencies
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run
```

### Access NOC Hub
Once running, check [Properties/launchSettings.json](file:///home/monitoring/noc-hub/Properties/launchSettings.json) for the configured URL.

---

## 📖 Documentation Files

| File | Purpose |
|------|---------|
| [README.md](file:///home/monitoring/README.md) | Comprehensive guide with features, setup, troubleshooting |
| [QUICK-START.md](file:///home/monitoring/QUICK-START.md) | Quick access guide with one-liners |
| [INSTALLATION-SUMMARY.md](file:///home/monitoring/INSTALLATION-SUMMARY.md) | Installation log and summary |
| **COMPLETE-STRUCTURE.md** | This file - full structure reference |

---

## 🔍 Quick Reference Commands

### Container Management
```bash
# Start everything
cd /home/monitoring/netdata && docker-compose up -d
cd /home/monitoring/prometheus && docker-compose up -d
cd /home/monitoring/grafana && docker-compose up -d
cd /home/monitoring/zabbix && docker-compose up -d

# Stop everything
cd /home/monitoring/netdata && docker-compose down
cd /home/monitoring/prometheus && docker-compose down
cd /home/monitoring/grafana && docker-compose down
cd /home/monitoring/zabbix && docker-compose down

# View logs
docker logs -f <container-name>

# Execute commands in container
docker exec -it <container-name> /bin/bash
```

### System Information
```bash
# Current server status
/home/server-report.sh

# Monitoring services status
/home/monitoring/manage-monitoring.sh status

# Docker system info
docker system df
docker ps -a
docker volume ls
docker network ls
```

---

## 🎯 Next Steps

1. **Access each tool** and familiarize yourself with the interface
2. **Import Grafana dashboards** (ID: 1860 for Node Exporter)
3. **Configure alerts** in your preferred tool
4. **Build and run NOC Hub** if you want to use the custom app
5. **Set up remote monitoring** for additional servers
6. **Create backups** of important configurations

---

## 📞 Support & Resources

- **Netdata**: https://learn.netdata.cloud/
- **Prometheus**: https://prometheus.io/docs/
- **Grafana**: https://grafana.com/docs/
- **Zabbix**: https://www.zabbix.com/documentation/
- **.NET**: https://learn.microsoft.com/en-us/aspnet/core/

---

**Generated:** 2026-01-11
**Location:** `/home/monitoring/`
**Server:** shahin-ai (Ubuntu 24.04.3 LTS)
