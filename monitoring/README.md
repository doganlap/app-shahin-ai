# Server Monitoring Solutions - Complete Guide

All monitoring solutions are installed in `/home/monitoring/` with isolated Docker containers.

## 🚀 Quick Access URLs

### 1. Netdata (Real-time Monitoring)
- **URL**: http://localhost:19999
- **Description**: Real-time, sub-second monitoring with auto-generated dashboards
- **Login**: No authentication required (local access)

### 2. Prometheus (Metrics Collection)
- **URL**: http://localhost:9090
- **Description**: Time-series database and metrics collection
- **Login**: No authentication required

### 3. Grafana (Data Visualization)
- **URL**: http://localhost:3000
- **Username**: admin
- **Password**: admin
- **Description**: Beautiful dashboards and data visualization
- **Note**: You'll be prompted to change password on first login

### 4. Zabbix (Enterprise Monitoring)
- **URL**: http://localhost:8080
- **Username**: Admin
- **Password**: zabbix
- **Description**: Traditional enterprise monitoring with alerting
- **Note**: Initial setup may take 1-2 minutes for database initialization

### 5. Node Exporter (System Metrics for Prometheus)
- **URL**: http://localhost:9100/metrics
- **Description**: Exposes system metrics for Prometheus to scrape

---

## 📊 Feature Comparison

| Feature | Netdata | Prometheus + Grafana | Zabbix |
|---------|---------|---------------------|--------|
| **Setup Time** | ⭐⭐⭐⭐⭐ Instant | ⭐⭐⭐⭐ Quick | ⭐⭐⭐ Moderate |
| **Real-time Data** | ⭐⭐⭐⭐⭐ Sub-second | ⭐⭐⭐ 15 seconds | ⭐⭐⭐ 1 minute |
| **Customization** | ⭐⭐⭐ Limited | ⭐⭐⭐⭐⭐ Highly customizable | ⭐⭐⭐⭐ Flexible |
| **Dashboard** | ⭐⭐⭐⭐⭐ Auto-generated | ⭐⭐⭐⭐⭐ Beautiful, custom | ⭐⭐⭐ Functional |
| **Alerting** | ⭐⭐⭐ Basic | ⭐⭐⭐⭐ Good with Alertmanager | ⭐⭐⭐⭐⭐ Advanced |
| **Resource Usage** | ⭐⭐⭐⭐ Low | ⭐⭐⭐ Moderate | ⭐⭐ Higher |
| **Learning Curve** | ⭐⭐⭐⭐⭐ Very easy | ⭐⭐⭐ Moderate | ⭐⭐ Steeper |
| **Remote Monitoring** | ⭐⭐⭐⭐ Cloud option | ⭐⭐⭐⭐⭐ Excellent | ⭐⭐⭐⭐⭐ Excellent |

---

## 🎯 Which One Should You Use?

### Use **Netdata** if you want:
- Instant monitoring with zero configuration
- Real-time, sub-second data updates
- Beautiful, auto-generated dashboards
- Quick troubleshooting and debugging
- Low maintenance overhead

### Use **Prometheus + Grafana** if you want:
- Industry-standard monitoring stack
- Highly customizable dashboards
- Multiple data sources in one dashboard
- Scalable across many servers
- Integration with modern cloud-native apps

### Use **Zabbix** if you want:
- Traditional enterprise monitoring
- Advanced alerting and escalation
- Agent-based comprehensive monitoring
- Template-based configuration
- Long-term data retention

---

## 🔧 Management Commands

### View All Monitoring Containers
```bash
docker ps | grep -E "netdata|prometheus|grafana|zabbix|node-exporter"
```

### Stop All Monitoring Services
```bash
cd /home/monitoring/netdata && docker-compose down
cd /home/monitoring/prometheus && docker-compose down
cd /home/monitoring/grafana && docker-compose down
cd /home/monitoring/zabbix && docker-compose down
```

### Start All Monitoring Services
```bash
cd /home/monitoring/netdata && docker-compose up -d
cd /home/monitoring/prometheus && docker-compose up -d
cd /home/monitoring/grafana && docker-compose up -d
cd /home/monitoring/zabbix && docker-compose up -d
```

### Restart Individual Service
```bash
# Netdata
cd /home/monitoring/netdata && docker-compose restart

# Prometheus
cd /home/monitoring/prometheus && docker-compose restart

# Grafana
cd /home/monitoring/grafana && docker-compose restart

# Zabbix
cd /home/monitoring/zabbix && docker-compose restart
```

### View Logs
```bash
# Netdata
docker logs -f netdata

# Prometheus
docker logs -f prometheus

# Grafana
docker logs -f grafana

# Zabbix
docker logs -f zabbix-server
docker logs -f zabbix-web
```

### Check Resource Usage
```bash
docker stats netdata prometheus grafana zabbix-server zabbix-web node-exporter
```

---

## 📁 Directory Structure

```
/home/monitoring/
├── netdata/
│   └── docker-compose.yml
├── prometheus/
│   ├── docker-compose.yml
│   └── prometheus.yml
├── grafana/
│   ├── docker-compose.yml
│   └── provisioning/
│       ├── datasources/
│       │   └── prometheus.yml
│       └── dashboards/
│           └── dashboard.yml
├── zabbix/
│   └── docker-compose.yml
└── README.md (this file)
```

---

## 🎨 Getting Started with Each Tool

### Netdata - First Steps
1. Open http://localhost:19999
2. Explore the auto-generated dashboard
3. Click on any metric to drill down
4. Use the search feature to find specific metrics
5. Enjoy real-time monitoring!

### Prometheus + Grafana - First Steps
1. **Verify Prometheus is collecting metrics:**
   - Open http://localhost:9090
   - Go to Status → Targets
   - Ensure all targets are "UP"

2. **Create your first Grafana dashboard:**
   - Open http://localhost:3000
   - Login with admin/admin (change password)
   - Click "+" → "Dashboard" → "Add new panel"
   - Select "Prometheus" data source
   - Try a query like: `node_cpu_seconds_total`
   - Or import dashboard ID **1860** (Node Exporter Full)

3. **Import pre-built dashboards:**
   - Click "+" → "Import"
   - Enter dashboard ID: **1860** (Node Exporter Full)
   - Select Prometheus data source
   - Click "Import"

### Zabbix - First Steps
1. Open http://localhost:8080
2. Login with Admin/zabbix
3. Wait 1-2 minutes for initial setup
4. Go to Configuration → Hosts
5. Add new host or configure existing agent
6. Set up triggers and alerts
7. View data in Monitoring → Latest data

---

## 🌐 Remote Monitoring Setup

### For Netdata (Remote Servers)
```bash
# On remote server, install Netdata and configure streaming
# Edit netdata.conf:
[global]
    bind socket to IP = 0.0.0.0
```

### For Prometheus (Remote Servers)
```bash
# On remote server, install Node Exporter
docker run -d -p 9100:9100 prom/node-exporter

# Add to prometheus.yml:
scrape_configs:
  - job_name: 'remote-server'
    static_configs:
      - targets: ['remote-server-ip:9100']
```

### For Zabbix (Remote Servers)
```bash
# On remote server, install Zabbix agent
docker run -d \
  -e ZBX_HOSTNAME="remote-server" \
  -e ZBX_SERVER_HOST="your-zabbix-server-ip" \
  -p 10050:10050 \
  zabbix/zabbix-agent
```

---

## 🔐 Security Notes

⚠️ **Important**: These installations are configured for local development/testing.

### For Production Use:
1. **Enable authentication** on all services
2. **Use HTTPS/SSL** certificates
3. **Configure firewall** rules (UFW/iptables)
4. **Change default passwords** immediately
5. **Restrict network access** to trusted IPs
6. **Enable audit logging**
7. **Set up backup strategies**

### Quick Firewall Rules (if needed)
```bash
# Allow only from specific IP
sudo ufw allow from YOUR_IP to any port 19999  # Netdata
sudo ufw allow from YOUR_IP to any port 9090   # Prometheus
sudo ufw allow from YOUR_IP to any port 3000   # Grafana
sudo ufw allow from YOUR_IP to any port 8080   # Zabbix
```

---

## 📊 Key Metrics to Monitor

### System Metrics (All Tools)
- **CPU Usage**: Overall and per-core utilization
- **Memory Usage**: Used, free, cached, swap
- **Disk Usage**: Space, I/O operations, latency
- **Network**: Bandwidth, packets, errors
- **Load Average**: 1min, 5min, 15min

### Application Metrics
- **Process Count**: Running, sleeping, zombie
- **Top Processes**: CPU and memory consumers
- **Open Files**: File descriptors usage
- **Network Connections**: Established, listening

### Docker Metrics
- **Container Stats**: CPU, memory per container
- **Container Health**: Running, stopped, restarts
- **Image Storage**: Disk usage by images

---

## 🆘 Troubleshooting

### Container won't start
```bash
# Check logs
docker logs <container-name>

# Check if port is already in use
sudo netstat -tulpn | grep <port>

# Restart Docker daemon
sudo systemctl restart docker
```

### Prometheus not scraping metrics
```bash
# Check targets status
curl http://localhost:9090/api/v1/targets

# Verify Node Exporter is running
curl http://localhost:9100/metrics
```

### Grafana can't connect to Prometheus
```bash
# Verify both are on same network
docker network inspect prometheus_monitoring

# Test connection from Grafana container
docker exec -it grafana curl http://prometheus:9090
```

### Zabbix web interface not loading
```bash
# Wait for database initialization (1-2 minutes)
docker logs zabbix-server

# Check MySQL is ready
docker logs zabbix-mysql

# Restart Zabbix services
cd /home/monitoring/zabbix && docker-compose restart
```

---

## 📚 Additional Resources

### Netdata
- Official Docs: https://learn.netdata.cloud/
- GitHub: https://github.com/netdata/netdata

### Prometheus
- Official Docs: https://prometheus.io/docs/
- Query Examples: https://prometheus.io/docs/prometheus/latest/querying/examples/

### Grafana
- Official Docs: https://grafana.com/docs/
- Dashboard Library: https://grafana.com/grafana/dashboards/
- Node Exporter Dashboard: https://grafana.com/grafana/dashboards/1860

### Zabbix
- Official Docs: https://www.zabbix.com/documentation/
- Community Templates: https://www.zabbix.com/integrations

---

## 📝 Next Steps

1. **Try each tool** and see which fits your workflow
2. **Import pre-built dashboards** in Grafana (Dashboard ID: 1860 for Node Exporter)
3. **Set up alerts** for critical metrics
4. **Configure remote monitoring** for other servers
5. **Customize dashboards** to show your most important KPIs
6. **Schedule regular reviews** of your monitoring data

---

**Generated on**: 2026-01-11
**Installed in**: /home/monitoring/
**Server**: shahin-ai (Ubuntu 24.04.3 LTS)
