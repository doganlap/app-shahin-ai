# 📊 Post-Production Monitoring & Troubleshooting Guide

**Duration:** First 2 Weeks After Production Deployment
**Purpose:** Ensure stability before rotating credentials
**Date Created:** 2026-01-11

---

## 🎯 OVERVIEW

This guide covers the first 2 weeks of production operation to ensure the application is stable and functioning correctly before performing credential rotation. Once you verify everything works properly, follow [SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md).

---

## 📅 DEPLOYMENT TIMELINE

### Week 1: Intensive Monitoring
- **Days 1-3:** Critical monitoring (24/7 on-call)
- **Days 4-7:** Active monitoring (business hours)

### Week 2: Validation & Preparation
- **Days 8-10:** Performance validation
- **Days 11-12:** Security audit
- **Days 13-14:** Credential rotation preparation

### Week 3: Credential Rotation
- **After 2 weeks of stable operation:** Execute credential rotation

---

## 🚀 DAY 1: DEPLOYMENT DAY

### Pre-Deployment Checklist

```bash
# 1. Verify all environment variables are set
echo "Checking environment variables..."
[ ! -z "$JWT_SECRET" ] && echo "✅ JWT_SECRET set" || echo "❌ JWT_SECRET missing"
[ ! -z "$GRCMVC_DB_CONNECTION" ] && echo "✅ DB connection set" || echo "❌ DB connection missing"
[ ! -z "$CLAUDE_API_KEY" ] && echo "✅ Claude API key set" || echo "❌ Claude API key missing"

# 2. Run database migrations
./scripts/run-migrations.sh production

# 3. Build verification
cd grc-frontend && npm run build
cd ../grc-app && npm run build
cd ../src/GrcMvc && dotnet build -c Release

# 4. Deploy application
# (Your deployment command here)
```

### Post-Deployment Verification (First Hour)

```bash
# 1. Health Checks
curl -f https://app.shahin-ai.com/health/ready || echo "❌ Health check failed"
curl -f https://app.shahin-ai.com/health/live || echo "❌ Liveness check failed"

# 2. Test Authentication
curl -X POST https://app.shahin-ai.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"your-password"}' \
  | jq '.'

# 3. Test API endpoints
curl -H "Authorization: Bearer YOUR_TOKEN" \
  https://app.shahin-ai.com/api/diagnostics/health \
  | jq '.'

# 4. Check logs for startup errors
kubectl logs -f deployment/grc-api --tail=100 | grep -i "error\|exception\|fatal"

# 5. Verify database connectivity
kubectl exec -it deployment/grc-api -- \
  dotnet ef database update --dry-run
```

### Day 1 Monitoring Checklist

- [ ] Application started without errors
- [ ] Database connection successful
- [ ] Health endpoints responding (200 OK)
- [ ] User login working
- [ ] API endpoints responding
- [ ] No critical errors in logs
- [ ] Response times under 2 seconds
- [ ] CPU usage under 70%
- [ ] Memory usage stable

---

## 📊 DAILY MONITORING (DAYS 1-7)

### Morning Check (9:00 AM)

```bash
#!/bin/bash
# Save as: scripts/morning-health-check.sh

echo "=== Daily Health Check - $(date) ==="

# 1. Check application status
echo "1. Application Status:"
kubectl get pods -l app=grc-api
kubectl get pods -l app=grc-frontend

# 2. Health endpoints
echo -e "\n2. Health Endpoints:"
curl -f https://app.shahin-ai.com/health/ready && echo "✅ Ready" || echo "❌ Not Ready"
curl -f https://app.shahin-ai.com/health/live && echo "✅ Live" || echo "❌ Not Live"

# 3. Error count in last 24 hours
echo -e "\n3. Error Count (Last 24h):"
kubectl logs deployment/grc-api --since=24h | grep -c "ERROR"

# 4. Database connectivity
echo -e "\n4. Database Status:"
kubectl exec -it deployment/grc-api -- \
  psql "$GRCMVC_DB_CONNECTION" -c "SELECT 1" 2>&1 | grep -q "1 row" && \
  echo "✅ Database connected" || echo "❌ Database issue"

# 5. Response time check
echo -e "\n5. Response Times:"
time curl -o /dev/null -s -w "Health endpoint: %{time_total}s\n" \
  https://app.shahin-ai.com/health/live

# 6. Active users (if available)
echo -e "\n6. Active Sessions:"
kubectl exec -it deployment/grc-api -- \
  redis-cli KEYS "session:*" | wc -l 2>/dev/null || echo "N/A"

echo -e "\n=== Check Complete ===\n"
```

### Metrics to Track Daily

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| Uptime | 99.9% | < 99% |
| Response Time (p95) | < 500ms | > 2000ms |
| Error Rate | < 0.1% | > 1% |
| CPU Usage | < 70% | > 85% |
| Memory Usage | < 80% | > 90% |
| Database Connections | < 50 | > 80 |
| Failed Logins | < 5/hour | > 20/hour |

---

## 🔍 KEY AREAS TO MONITOR

### 1. Application Logs

**Location:** `/app/logs/grcmvc-*.log`

**What to Watch:**
```bash
# Real-time error monitoring
tail -f /app/logs/grcmvc-*.log | grep -i "error\|exception\|fatal\|critical"

# Count errors by type
grep "ERROR" /app/logs/grcmvc-$(date +%Y%m%d).log | \
  awk '{print $NF}' | sort | uniq -c | sort -rn

# Database connection issues
grep -i "database\|connection\|timeout" /app/logs/grcmvc-*.log

# Claude API failures
grep -i "claude\|anthropic\|api.*fail" /app/logs/grcmvc-*.log

# Rate limiting hits
grep -i "rate.*limit\|429\|too many requests" /app/logs/grcmvc-*.log
```

### 2. Database Performance

```sql
-- Active connections
SELECT count(*) FROM pg_stat_activity WHERE state = 'active';

-- Long-running queries (> 30 seconds)
SELECT pid, usename, state, query, now() - query_start as duration
FROM pg_stat_activity
WHERE state != 'idle' AND now() - query_start > interval '30 seconds'
ORDER BY duration DESC;

-- Database size growth
SELECT pg_size_pretty(pg_database_size('GrcMvcDb')) as size;

-- Table sizes
SELECT schemaname, tablename,
       pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC
LIMIT 10;
```

### 3. API Performance

```bash
# Test critical endpoints
curl -w "\n%{http_code} - %{time_total}s\n" \
  -o /dev/null -s \
  https://app.shahin-ai.com/api/health

curl -w "\n%{http_code} - %{time_total}s\n" \
  -o /dev/null -s \
  -H "Authorization: Bearer YOUR_TOKEN" \
  https://app.shahin-ai.com/api/dashboard/stats

# Test rate limiting
for i in {1..105}; do
  curl -s -o /dev/null -w "%{http_code}\n" \
    https://app.shahin-ai.com/api/health
  sleep 0.1
done | grep "429" && echo "✅ Rate limiting working"
```

### 4. Frontend Performance

```bash
# Check frontend build
curl -I https://shahin-ai.com | grep "200 OK"

# Test page load times
curl -w "DNS: %{time_namelookup}s\nConnect: %{time_connect}s\nTotal: %{time_total}s\n" \
  -o /dev/null -s https://shahin-ai.com

# Check for JavaScript errors (requires browser)
# Open DevTools Console on https://shahin-ai.com
# Look for red error messages
```

---

## 🚨 COMMON ISSUES & SOLUTIONS

### Issue 1: Application Won't Start

**Symptoms:**
- Pod in CrashLoopBackOff
- "JWT_SECRET environment variable is required" error
- "CLAUDE_API_KEY environment variable is required" error

**Solution:**
```bash
# Check environment variables
kubectl describe pod <pod-name> | grep -A 20 "Environment:"

# Verify secrets exist
kubectl get secrets

# Check secret values (be careful - this shows secrets!)
kubectl get secret grc-secrets -o yaml

# Fix: Update secret
kubectl create secret generic grc-secrets \
  --from-literal=jwt-secret="$JWT_SECRET" \
  --from-literal=claude-api-key="$CLAUDE_API_KEY" \
  --dry-run=client -o yaml | kubectl apply -f -

# Restart pods
kubectl rollout restart deployment/grc-api
```

---

### Issue 2: Database Connection Failures

**Symptoms:**
- "Connection refused" errors
- "Timeout expired" errors
- Slow queries

**Solution:**
```bash
# Test database connectivity
kubectl run -it --rm debug --image=postgres:15 --restart=Never -- \
  psql "$GRCMVC_DB_CONNECTION" -c "SELECT version();"

# Check database pod status
kubectl get pods -l app=postgres

# View database logs
kubectl logs -f postgres-0

# Verify connection string format
# Should be: Host=postgres-service;Database=GrcMvcDb;Username=user;Password=pass;Port=5432

# Check if connection pooling is exhausted
kubectl exec -it deployment/grc-api -- \
  dotnet ef dbcontext info

# Restart database connection pool
kubectl rollout restart deployment/grc-api
```

---

### Issue 3: Claude API Failures

**Symptoms:**
- "Claude agent is not configured" errors
- 401 Unauthorized from Anthropic
- Rate limit exceeded

**Solution:**
```bash
# Test API key
curl https://api.anthropic.com/v1/messages \
  -H "x-api-key: $CLAUDE_API_KEY" \
  -H "anthropic-version: 2023-06-01" \
  -H "content-type: application/json" \
  -d '{"model":"claude-sonnet-4-20250514","max_tokens":10,"messages":[{"role":"user","content":"test"}]}'

# Check if key is valid
# Expected: 200 OK with response
# If 401: Key is invalid - need to rotate
# If 429: Rate limited - reduce usage

# Disable Claude temporarily if needed
kubectl set env deployment/grc-api CLAUDE_ENABLED=false

# View Claude API usage
kubectl logs deployment/grc-api | grep -i "claude" | tail -50
```

---

### Issue 4: Rate Limiting Too Aggressive

**Symptoms:**
- Legitimate users getting 429 errors
- "Too Many Requests" in logs

**Solution:**
```bash
# Check current rate limit config
kubectl exec -it deployment/grc-api -- \
  cat /app/appsettings.Production.json | grep -A 5 "RateLimiting"

# Temporarily increase limits (requires app restart)
# Edit appsettings.Production.json or set environment variable
kubectl set env deployment/grc-api \
  RateLimiting__GlobalPermitLimit=200 \
  RateLimiting__ApiPermitLimit=100

# Monitor rate limit hits
kubectl logs -f deployment/grc-api | grep "429\|rate limit"

# Analyze which IPs are hitting limits
kubectl logs deployment/grc-api | \
  grep "429" | \
  awk '{print $5}' | \
  sort | uniq -c | sort -rn
```

---

### Issue 5: Memory Leaks

**Symptoms:**
- Memory usage continuously increasing
- OutOfMemoryException
- Pod restarts frequently

**Solution:**
```bash
# Monitor memory usage
kubectl top pods -l app=grc-api

# Check memory limits
kubectl describe pod <pod-name> | grep -A 5 "Limits:"

# Increase memory limit if needed
kubectl set resources deployment/grc-api \
  --limits=memory=2Gi \
  --requests=memory=1Gi

# Analyze memory dump (advanced)
kubectl exec -it deployment/grc-api -- \
  dotnet-dump collect -p 1

# Force garbage collection (temporary fix)
kubectl exec -it deployment/grc-api -- \
  curl http://localhost/api/diagnostics/gc
```

---

### Issue 6: Slow Database Queries

**Symptoms:**
- API requests taking > 5 seconds
- Database CPU at 100%
- Timeout errors

**Solution:**
```sql
-- Find slow queries
SELECT pid, usename, state, query, now() - query_start as duration
FROM pg_stat_activity
WHERE state != 'idle' AND now() - query_start > interval '5 seconds'
ORDER BY duration DESC;

-- Kill slow query (if needed)
SELECT pg_terminate_backend(pid) FROM pg_stat_activity
WHERE query_start < now() - interval '30 seconds' AND state = 'active';

-- Check for missing indexes
SELECT schemaname, tablename, indexname
FROM pg_indexes
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY tablename;

-- Add index if needed (example)
CREATE INDEX CONCURRENTLY idx_users_email ON users(email);

-- Update statistics
ANALYZE;
```

---

## 📈 WEEK 2: PERFORMANCE VALIDATION

### Day 8-10: Load Testing

```bash
# Install k6 (load testing tool)
# https://k6.io/docs/getting-started/installation/

# Create load test script: load-test.js
cat > load-test.js << 'EOF'
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '2m', target: 10 },  // Ramp up to 10 users
    { duration: '5m', target: 10 },  // Stay at 10 users
    { duration: '2m', target: 50 },  // Ramp up to 50 users
    { duration: '5m', target: 50 },  // Stay at 50 users
    { duration: '2m', target: 0 },   // Ramp down
  ],
};

export default function () {
  let response = http.get('https://app.shahin-ai.com/health/live');
  check(response, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
  sleep(1);
}
EOF

# Run load test
k6 run load-test.js

# Analyze results
# - Look for error rate < 1%
# - p95 response time < 1s
# - p99 response time < 2s
```

### Day 11-12: Security Audit

```bash
# 1. Check for exposed secrets
grep -r "password\|secret\|key" src/ --include="*.cs" --include="*.json" | \
  grep -v "Password\|Secret" | \
  grep -v "appsettings.Development.json"

# 2. Verify HTTPS is enforced
curl -I http://app.shahin-ai.com | grep "301\|308"

# 3. Check security headers
curl -I https://app.shahin-ai.com | \
  grep -E "X-Frame-Options|X-Content-Type-Options|Strict-Transport-Security"

# 4. Test SQL injection (should be prevented)
curl "https://app.shahin-ai.com/api/users?name=admin'--" | \
  grep -i "error\|exception" || echo "✅ Protected"

# 5. Test XSS (should be prevented)
curl "https://app.shahin-ai.com/api/search?q=<script>alert(1)</script>" | \
  grep "<script>" && echo "❌ Vulnerable" || echo "✅ Protected"

# 6. Verify CORS is configured
curl -H "Origin: https://evil.com" \
  -H "Access-Control-Request-Method: GET" \
  -X OPTIONS https://app.shahin-ai.com/api/health | \
  grep "Access-Control-Allow-Origin: https://evil.com" && \
  echo "❌ CORS misconfigured" || echo "✅ CORS correct"
```

### Day 13-14: Credential Rotation Preparation

```bash
# Create checklist for credential rotation
cat > credential-rotation-checklist.md << 'EOF'
# Credential Rotation Preparation Checklist

## Pre-Rotation
- [ ] Application stable for 2 weeks
- [ ] No critical errors in logs
- [ ] Performance metrics within targets
- [ ] Security audit passed
- [ ] Database backup completed
- [ ] Rollback plan documented
- [ ] Team notified of maintenance window

## During Rotation
- [ ] Follow SECURITY_CREDENTIAL_ROTATION_GUIDE.md
- [ ] Rotate database password
- [ ] Rotate Claude API key
- [ ] Rotate Azure credentials
- [ ] Rotate JWT secret (users will need to re-login)
- [ ] Update all secrets in Kubernetes/Key Vault
- [ ] Restart all services

## Post-Rotation
- [ ] Verify application starts
- [ ] Test authentication
- [ ] Test API endpoints
- [ ] Monitor logs for errors
- [ ] Verify no services using old credentials
- [ ] Document rotation in audit log

## Rollback (if needed)
- [ ] Restore database from backup
- [ ] Revert credentials to previous values
- [ ] Restart services
- [ ] Verify application works
- [ ] Document issues encountered
EOF
```

---

## 📊 WEEKLY SUMMARY TEMPLATE

```markdown
# Week [1/2] Production Summary

**Date Range:** YYYY-MM-DD to YYYY-MM-DD
**Status:** [🟢 Healthy | 🟡 Issues | 🔴 Critical]

## Metrics
- **Uptime:** XX.X%
- **Error Rate:** X.XX%
- **Avg Response Time:** XXXms
- **Total Requests:** XXX,XXX
- **Peak Concurrent Users:** XXX
- **Database Growth:** XX GB

## Issues Encountered
1. [Issue description]
   - Impact: [High/Medium/Low]
   - Resolution: [What was done]
   - Status: [Resolved/Monitoring/Open]

## Performance Highlights
- [Positive observation]
- [Improvement noted]

## Action Items for Next Week
- [ ] [Action 1]
- [ ] [Action 2]

## Credential Rotation Status
- [ ] Waiting for 2 weeks of stability
- [ ] Preparing rotation checklist
- [ ] Ready to rotate
```

---

## ✅ GO/NO-GO DECISION FOR CREDENTIAL ROTATION

After 2 weeks, use this checklist to decide if you're ready:

### Technical Criteria (Must Pass All)
- [ ] Uptime > 99% over 2 weeks
- [ ] Error rate < 0.5% average
- [ ] No critical bugs discovered
- [ ] Database performance stable
- [ ] No memory leaks detected
- [ ] Load testing passed
- [ ] Security audit passed

### Operational Criteria (Must Pass All)
- [ ] Team trained on rollback procedures
- [ ] Maintenance window scheduled
- [ ] Users notified (if JWT rotation affects them)
- [ ] Database backup completed
- [ ] Monitoring alerts configured
- [ ] Documentation up to date

### If ALL criteria met:
✅ **PROCEED with credential rotation** using [SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md)

### If ANY criteria not met:
⚠️ **DELAY rotation** and address issues first

---

## 📞 ESCALATION CONTACTS

| Severity | Response Time | Contact |
|----------|---------------|---------|
| 🔴 Critical (Site Down) | 15 minutes | On-call engineer |
| 🟡 High (Degraded) | 2 hours | Team lead |
| 🟢 Medium | Next business day | Support team |

---

## 🔗 USEFUL COMMANDS REFERENCE

```bash
# Quick health check
curl https://app.shahin-ai.com/health/ready && echo "✅ Healthy" || echo "❌ Unhealthy"

# View last 100 errors
kubectl logs deployment/grc-api --tail=1000 | grep ERROR | tail -100

# Restart application
kubectl rollout restart deployment/grc-api

# Scale up (if needed for high traffic)
kubectl scale deployment/grc-api --replicas=5

# View resource usage
kubectl top pods
kubectl top nodes

# Port forward for local debugging
kubectl port-forward deployment/grc-api 8888:8888

# Execute command in container
kubectl exec -it deployment/grc-api -- /bin/bash

# View events (useful for troubleshooting)
kubectl get events --sort-by='.lastTimestamp' | tail -20
```

---

## 📚 ADDITIONAL RESOURCES

1. **[PRODUCTION_FIXES_COMPLETED.md](PRODUCTION_FIXES_COMPLETED.md)** - All fixes applied
2. **[SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md)** - Credential rotation procedures
3. **[scripts/run-migrations.sh](scripts/run-migrations.sh)** - Database migration script
4. **ASP.NET Core Logging:** https://learn.microsoft.com/aspnet/core/fundamentals/logging/
5. **PostgreSQL Monitoring:** https://www.postgresql.org/docs/current/monitoring.html

---

**Remember:** The goal is 2 weeks of stable production operation before rotating credentials. Take this time to understand the application's behavior and fix any issues that arise.

**Next Step:** After 2 weeks of successful operation, follow [SECURITY_CREDENTIAL_ROTATION_GUIDE.md](SECURITY_CREDENTIAL_ROTATION_GUIDE.md)

---

**Good luck with your production deployment! 🚀**
