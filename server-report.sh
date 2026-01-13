#!/bin/bash

# Server Uptime and Usage Report
# Generated on: $(date)

echo "========================================"
echo "  SERVER UPTIME AND USAGE REPORT"
echo "========================================"
echo ""

# System Information
echo "--- SYSTEM INFORMATION ---"
echo "Hostname: $(hostname)"
echo "OS: $(lsb_release -d 2>/dev/null | cut -f2 || cat /etc/os-release | grep PRETTY_NAME | cut -d'"' -f2)"
echo "Kernel: $(uname -r)"
echo "Architecture: $(uname -m)"
echo ""

# Uptime
echo "--- UPTIME ---"
uptime
echo ""

# CPU Usage
echo "--- CPU USAGE ---"
echo "Load Average: $(uptime | awk -F'load average:' '{print $2}')"
echo "CPU Cores: $(nproc)"
echo ""
top -bn1 | grep "Cpu(s)" | sed "s/.*, *\([0-9.]*\)%* id.*/\1/" | awk '{print "CPU Usage: " 100 - $1"%"}'
echo ""

# Memory Usage
echo "--- MEMORY USAGE ---"
free -h
echo ""
free | grep Mem | awk '{printf "Memory Usage: %.2f%% (%s used of %s total)\n", $3/$2 * 100.0, $3, $2}'
echo ""

# Disk Usage
echo "--- DISK USAGE ---"
df -h | grep -E '^/dev/' | grep -v 'docker'
echo ""

# Top Processes by CPU
echo "--- TOP 5 PROCESSES BY CPU ---"
ps aux --sort=-%cpu | head -6 | awk 'NR==1 || NR<=6 {printf "%-10s %-8s %-5s %-5s %s\n", $1, $2, $3, $4, $11}'
echo ""

# Top Processes by Memory
echo "--- TOP 5 PROCESSES BY MEMORY ---"
ps aux --sort=-%mem | head -6 | awk 'NR==1 || NR<=6 {printf "%-10s %-8s %-5s %-5s %s\n", $1, $2, $3, $4, $11}'
echo ""

# Network Connections
echo "--- NETWORK CONNECTIONS ---"
echo "Active connections: $(netstat -an 2>/dev/null | grep ESTABLISHED | wc -l || ss -tan | grep ESTAB | wc -l)"
echo "Listening ports: $(netstat -tuln 2>/dev/null | grep LISTEN | wc -l || ss -tuln | grep LISTEN | wc -l)"
echo ""

# Logged in Users
echo "--- LOGGED IN USERS ---"
who
echo ""

# System Load History (if available)
if command -v sar &> /dev/null; then
    echo "--- SYSTEM LOAD HISTORY (Last hour) ---"
    sar -u 1 1
    echo ""
fi

echo "========================================"
echo "Report generated at: $(date '+%Y-%m-%d %H:%M:%S')"
echo "========================================"
