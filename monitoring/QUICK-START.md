# 🚀 Quick Start Guide

## Access Your Monitoring Tools NOW

### 1️⃣ Netdata - Real-time Monitoring
```
http://localhost:19999
```
**No login required** - Just open and explore!
Best for: Quick troubleshooting and real-time metrics

---

### 2️⃣ Grafana - Beautiful Dashboards
```
http://localhost:3000
Username: admin
Password: admin
```
**First-time setup:**
1. Login with admin/admin
2. Change password (or skip)
3. Click "+" → Import Dashboard
4. Enter ID: **1860** (Node Exporter Full)
5. Select "Prometheus" as data source
6. Click "Import"

---

### 3️⃣ Prometheus - Metrics Database
```
http://localhost:9090
```
**Quick test:**
- Go to Status → Targets (verify all are UP)
- Try query: `node_cpu_seconds_total`

---

### 4️⃣ Zabbix - Enterprise Monitoring
```
http://localhost:8080
Username: Admin
Password: zabbix
```
**Note:** Wait 1-2 minutes for initial setup after first start

---

## 🎮 Management Commands

### One-line status check
```bash
/home/monitoring/manage-monitoring.sh status
```

### Start/Stop/Restart all services
```bash
/home/monitoring/manage-monitoring.sh start
/home/monitoring/manage-monitoring.sh stop
/home/monitoring/manage-monitoring.sh restart
```

### Show all URLs
```bash
/home/monitoring/manage-monitoring.sh urls
```

### View logs
```bash
/home/monitoring/manage-monitoring.sh logs grafana
/home/monitoring/manage-monitoring.sh logs prometheus
/home/monitoring/manage-monitoring.sh logs netdata
```

### Check resource usage
```bash
/home/monitoring/manage-monitoring.sh stats
```

---

## 🎯 Which Tool Should I Use?

| Situation | Recommended Tool |
|-----------|-----------------|
| "Show me everything NOW" | **Netdata** (http://localhost:19999) |
| "I want beautiful custom dashboards" | **Grafana** (http://localhost:3000) |
| "I need to query specific metrics" | **Prometheus** (http://localhost:9090) |
| "I need enterprise features & alerts" | **Zabbix** (http://localhost:8080) |

---

## 📊 Top 5 Things to Check

1. **CPU Usage** - Available in all tools
2. **Memory Usage** - Available in all tools
3. **Disk Space** - Available in all tools
4. **Network Traffic** - Available in all tools
5. **Top Processes** - Best in Netdata

---

## 🔥 Pro Tips

### Netdata
- Use the search bar to find any metric instantly
- Click any chart to zoom and drill down
- Everything auto-updates in real-time

### Grafana
- Import dashboard **1860** for comprehensive system monitoring
- Browse https://grafana.com/grafana/dashboards/ for more
- Press 'd' then 's' for shortcuts menu

### Prometheus
- Learn PromQL: https://prometheus.io/docs/prometheus/latest/querying/basics/
- Check targets: http://localhost:9090/targets
- Useful query: `rate(node_cpu_seconds_total[5m])`

### Zabbix
- Pre-configured templates available in Configuration → Templates
- Set up email notifications in Administration → Media types
- Check latest data: Monitoring → Latest data

---

## 🆘 Quick Troubleshooting

### Service not accessible?
```bash
# Check if containers are running
docker ps | grep -E "netdata|prometheus|grafana|zabbix"

# Restart a specific service
cd /home/monitoring/<service-name>
docker-compose restart
```

### Grafana can't connect to Prometheus?
```bash
# Test connection
curl http://localhost:9090/api/v1/query?query=up

# If that fails, restart both
cd /home/monitoring/prometheus && docker-compose restart
cd /home/monitoring/grafana && docker-compose restart
```

### Zabbix not loading?
```bash
# Wait 2 minutes, then check logs
docker logs zabbix-server

# Restart if needed
cd /home/monitoring/zabbix && docker-compose restart
```

---

## 📱 Bookmark These

- **This guide:** `file:///home/monitoring/QUICK-START.md`
- **Full README:** `file:///home/monitoring/README.md`
- **Management script:** `/home/monitoring/manage-monitoring.sh`

---

**Need help?** Read the full [README.md](README.md) for detailed instructions.
