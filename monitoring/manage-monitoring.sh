#!/bin/bash

# Server Monitoring Management Script
# Location: /home/monitoring/

NETDATA_DIR="/home/monitoring/netdata"
PROMETHEUS_DIR="/home/monitoring/prometheus"
GRAFANA_DIR="/home/monitoring/grafana"
ZABBIX_DIR="/home/monitoring/zabbix"

show_status() {
    echo "=========================================="
    echo "  MONITORING SERVICES STATUS"
    echo "=========================================="
    echo ""
    docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | grep -E "NAMES|netdata|prometheus|grafana|zabbix|node-exporter"
    echo ""
}

start_all() {
    echo "Starting all monitoring services..."
    echo ""

    echo "→ Starting Netdata..."
    cd "$NETDATA_DIR" && docker-compose up -d

    echo "→ Starting Prometheus + Node Exporter..."
    cd "$PROMETHEUS_DIR" && docker-compose up -d

    echo "→ Starting Grafana..."
    cd "$GRAFANA_DIR" && docker-compose up -d

    echo "→ Starting Zabbix..."
    cd "$ZABBIX_DIR" && docker-compose up -d

    echo ""
    echo "✓ All services started!"
    echo ""
    sleep 2
    show_status
}

stop_all() {
    echo "Stopping all monitoring services..."
    echo ""

    echo "→ Stopping Zabbix..."
    cd "$ZABBIX_DIR" && docker-compose down

    echo "→ Stopping Grafana..."
    cd "$GRAFANA_DIR" && docker-compose down

    echo "→ Stopping Prometheus..."
    cd "$PROMETHEUS_DIR" && docker-compose down

    echo "→ Stopping Netdata..."
    cd "$NETDATA_DIR" && docker-compose down

    echo ""
    echo "✓ All services stopped!"
}

restart_all() {
    echo "Restarting all monitoring services..."
    stop_all
    echo ""
    sleep 2
    start_all
}

show_urls() {
    echo "=========================================="
    echo "  MONITORING ACCESS URLS"
    echo "=========================================="
    echo ""
    echo "Netdata:       http://localhost:19999"
    echo "Prometheus:    http://localhost:9090"
    echo "Grafana:       http://localhost:3000"
    echo "               → Username: admin"
    echo "               → Password: admin"
    echo "Zabbix:        http://localhost:8080"
    echo "               → Username: Admin"
    echo "               → Password: zabbix"
    echo "Node Exporter: http://localhost:9100/metrics"
    echo ""
}

show_logs() {
    if [ -z "$1" ]; then
        echo "Available services: netdata, prometheus, grafana, zabbix-server, zabbix-web"
        echo "Usage: $0 logs <service-name>"
        return
    fi

    echo "Showing logs for $1 (Ctrl+C to exit)..."
    docker logs -f "$1"
}

show_stats() {
    echo "=========================================="
    echo "  CONTAINER RESOURCE USAGE"
    echo "=========================================="
    echo ""
    docker stats --no-stream netdata prometheus node-exporter grafana zabbix-server zabbix-web zabbix-mysql 2>/dev/null || echo "Some containers may not be running"
}

case "$1" in
    start)
        start_all
        ;;
    stop)
        stop_all
        ;;
    restart)
        restart_all
        ;;
    status)
        show_status
        ;;
    urls)
        show_urls
        ;;
    logs)
        show_logs "$2"
        ;;
    stats)
        show_stats
        ;;
    *)
        echo "=========================================="
        echo "  MONITORING MANAGEMENT SCRIPT"
        echo "=========================================="
        echo ""
        echo "Usage: $0 {start|stop|restart|status|urls|logs|stats}"
        echo ""
        echo "Commands:"
        echo "  start   - Start all monitoring services"
        echo "  stop    - Stop all monitoring services"
        echo "  restart - Restart all monitoring services"
        echo "  status  - Show status of all services"
        echo "  urls    - Show access URLs and credentials"
        echo "  logs    - Show logs for a specific service"
        echo "            Example: $0 logs grafana"
        echo "  stats   - Show resource usage for all containers"
        echo ""
        echo "Quick Links:"
        echo "  Netdata:    http://localhost:19999"
        echo "  Prometheus: http://localhost:9090"
        echo "  Grafana:    http://localhost:3000 (admin/admin)"
        echo "  Zabbix:     http://localhost:8080 (Admin/zabbix)"
        echo ""
        exit 1
        ;;
esac
