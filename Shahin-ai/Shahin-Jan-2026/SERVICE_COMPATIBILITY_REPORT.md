# Service Compatibility Report
**Generated:** 2026-01-13 04:05 UTC
**Status:** All services restarted and tested

## Port Mapping & Compatibility Status

| Service | Container Name | Port(s) | Status | Health | HTTP Test | Issue |
|---------|---------------|---------|--------|--------|-----------|-------|
| **GrcMvc App** | `shahin-jan-2026_grcmvc_1` | 5137 (HTTP), 5138 (HTTPS) | ⚠️ Starting | health: starting | HTTP 503 | Still initializing |
| **PostgreSQL DB** | `grc-db` | 5432 (internal) | ✅ Running | healthy | ✅ Ready | None |
| **Camunda BPM** | `grc-camunda` | 8085 | ✅ Running | healthy | HTTP 302 | Working (redirect) |
| **ClickHouse** | `grc-clickhouse` | 8123, 9000 | ✅ Running | healthy | HTTP 200 | None |
| **Redis** | `grc-redis` | 6379 (internal) | ✅ Running | healthy | ✅ PONG | None |
| **Zookeeper** | `grc-zookeeper` | 2181 | ✅ Running | healthy | - | None |
| **Kafka UI** | `grc-kafka-ui` | 9080 | ✅ Running | - | HTTP 200 | None |
| **Kafka Connect** | `grc-kafka-connect` | 8083 | ⚠️ Starting | health: starting | - | Initializing |
| **Kafka** | `grc-kafka` | 9092, 29092 | ❌ Failed | Exit 1 | - | **Cluster ID mismatch** |
| **Grafana** | `grc-grafana` | 3030 | ❌ Failed | Exit 1 | - | **DB password auth failed** |
| **Superset** | `grc-superset` | 8088 | ❌ Failed | Exit 1 | - | **Missing psycopg2 module** |
| **Metabase** | `grc-metabase` | 3033 | ⚠️ Starting | health: starting | - | Initializing |
| **n8n** | `grc-n8n` | 5678 | ⚠️ Starting | health: starting | - | Initializing |

## Detailed Issues

### 1. ❌ Kafka - Cluster ID Mismatch
**Error:** `InconsistentClusterIdException: The Cluster ID doesn't match stored clusterId`
**Fix Required:**
```bash
docker volume rm shahin-jan-2026_kafka_data
docker-compose up -d kafka
```

### 2. ❌ Grafana - Database Authentication Failed
**Error:** `pq: password authentication failed for user "postgres"`
**Fix Required:** Check `.env` file for `DB_PASSWORD` and `GRAFANA_ADMIN_PASSWORD` match

### 3. ❌ Superset - Missing Python Module
**Error:** `ModuleNotFoundError: No module named 'psycopg2'`
**Fix Required:** The docker-compose command installs psycopg2 but may need more time or the image needs rebuild

### 4. ⚠️ GrcMvc App - Still Starting
**Status:** HTTP 503 (Service Unavailable)
**Expected:** Should become healthy after DB migrations complete
**Wait Time:** ~40-60 seconds for health check

## Port Compatibility Check

All ports are properly bound and accessible:
- ✅ Port 5137 (GrcMvc HTTP) - Listening
- ✅ Port 5138 (GrcMvc HTTPS) - Listening  
- ✅ Port 8085 (Camunda) - Listening
- ✅ Port 8123 (ClickHouse HTTP) - Listening
- ✅ Port 9000 (ClickHouse Native) - Listening
- ✅ Port 2181 (Zookeeper) - Listening
- ✅ Port 8083 (Kafka Connect) - Listening
- ✅ Port 9080 (Kafka UI) - Listening
- ✅ Port 3033 (Metabase) - Listening
- ✅ Port 5678 (n8n) - Listening

**No port conflicts detected.**

## Recommendations

1. **Fix Kafka:** Remove and recreate kafka_data volume
2. **Fix Grafana:** Verify DB credentials in `.env`
3. **Fix Superset:** Wait for pip install to complete or rebuild image
4. **Wait for services:** Allow 2-3 minutes for all health checks to pass
5. **Monitor:** Use `docker-compose ps` to track health status

## Next Steps

Run these commands to fix issues:
```bash
# Fix Kafka
docker volume rm shahin-jan-2026_kafka_data
docker-compose up -d kafka

# Check Grafana logs after fixing DB password
docker-compose restart grafana

# Wait for Superset to complete installation
docker-compose logs -f grc-superset
```
