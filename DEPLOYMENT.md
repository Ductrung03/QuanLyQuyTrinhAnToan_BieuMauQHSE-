# SSMS Application Deployment Guide

## Target Environment
- **Server**: Windows Server 2019
- **Container Runtime**: Docker Desktop with Linux containers
- **Database**: SQL Server 2022 Linux
- **Application**: .NET 8.0 API + React Frontend

---

## Pre-Deployment Checklist

### 1. Server Requirements
- [ ] Windows Server 2019 with latest updates
- [ ] Docker Desktop installed and configured for Linux containers
- [ ] At least 8GB RAM available (4GB for SQL Server, 2GB for API, 2GB for system)
- [ ] At least 50GB free disk space (20GB for database, 10GB for images, 20GB buffer)
- [ ] Ports 80 and 443 available

### 2. Required Files
- [ ] `.env` file created from `.env.example` with actual values
- [ ] SSL certificates (if using HTTPS) in `./nginx/ssl/`
- [ ] Database migration scripts in `./Database/migrations/`

### 3. Security Checklist
- [ ] Strong SA password set (min 8 chars, upper, lower, numbers, special chars)
- [ ] JWT secret key changed from default (min 32 characters)
- [ ] File upload limits configured appropriately
- [ ] Firewall rules configured (allow 80/443, block 1433/5000 externally)

---

## Initial Deployment

### Step 1: Prepare Environment File

```bash
cp .env.example .env
```

Edit `.env` and set secure values:
```bash
DB_SA_PASSWORD=YourSecurePassword123!
JWT_SECRET_KEY=Your-Very-Long-Secret-Key-Min-32-Characters-Random
```

### Step 2: Create Required Directories

```bash
mkdir -p nginx/ssl
mkdir -p Database/migrations
```

### Step 3: Build and Start Services

```bash
docker compose up -d
```

This will:
1. Pull SQL Server 2022 Linux image
2. Build .NET API image
3. Build Frontend + Nginx image
4. Create persistent volumes
5. Start all services with health checks

### Step 4: Verify Deployment

```bash
docker compose ps
```

All services should show "healthy" status after 1-2 minutes.

### Step 5: Check Logs

```bash
docker compose logs -f db
docker compose logs -f api
docker compose logs -f web
```

### Step 6: Test Application

1. Open browser: `http://your-server-ip`
2. Check API health: `http://your-server-ip/api/health`
3. Login with default credentials (from database seed data)

---

## Database Migration Strategy

### First-Time Database Setup

The database will be automatically created on first startup. SQL Server will:
1. Create the `SSMS_KhaiThacTau` database
2. Run migration scripts from `./Database/migrations/`

**Migration Script Execution:**

Option A: Manual Migration (Recommended for production)

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "CREATE DATABASE SSMS_KhaiThacTau"

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -d SSMS_KhaiThacTau \
  -i /docker-entrypoint-initdb.d/002_dynamic_permissions.sql
```

Option B: Restore from Backup

```bash
docker cp backup.bak ssms-db:/var/opt/mssql/backups/

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "RESTORE DATABASE SSMS_KhaiThacTau FROM DISK = '/var/opt/mssql/backups/backup.bak' WITH REPLACE"
```

### Ongoing Migrations

For schema updates:

1. Add new migration script to `./Database/migrations/`
2. Execute manually:

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -d SSMS_KhaiThacTau \
  -i /docker-entrypoint-initdb.d/003_new_migration.sql
```

3. No service restart needed

---

## Data Persistence & Backup

### Volume Locations

All data is stored in Docker named volumes:

```
mssql-data          -> /var/opt/mssql (Database files)
mssql-backups       -> /var/opt/mssql/backups (SQL backups)
api-uploads         -> /app/uploads (User uploads)
api-logs            -> /app/logs (Application logs)
nginx-logs          -> /var/log/nginx (Web server logs)
```

### Inspect Volume Data

```bash
docker volume inspect ssms-mssql-data
docker volume ls
```

### Backup Database

**Method 1: SQL Server Native Backup**

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "BACKUP DATABASE SSMS_KhaiThacTau TO DISK = '/var/opt/mssql/backups/ssms_backup_$(date +%Y%m%d_%H%M%S).bak' WITH COMPRESSION"
```

**Method 2: Volume Backup**

```bash
docker run --rm \
  -v ssms-mssql-data:/source \
  -v C:/Backups:/backup \
  alpine tar czf /backup/mssql-data-$(date +%Y%m%d).tar.gz -C /source .
```

### Restore Database

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "RESTORE DATABASE SSMS_KhaiThacTau FROM DISK = '/var/opt/mssql/backups/your_backup.bak' WITH REPLACE"
```

### Automated Backup Script

Create `backup.ps1`:

```powershell
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupName = "ssms_backup_$timestamp.bak"

docker exec ssms-db /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P "$env:DB_SA_PASSWORD" -C `
  -Q "BACKUP DATABASE SSMS_KhaiThacTau TO DISK = '/var/opt/mssql/backups/$backupName' WITH COMPRESSION"

docker cp ssms-db:/var/opt/mssql/backups/$backupName C:/Backups/

Get-ChildItem C:/Backups -Filter "ssms_backup_*.bak" | 
  Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-30) } | 
  Remove-Item
```

Schedule with Task Scheduler: Daily at 2 AM

---

## Updating the Application

### Update API Only

```bash
cd backend
docker compose build api
docker compose up -d api
```

### Update Frontend Only

```bash
cd frontend
docker compose build web
docker compose up -d web
```

### Update All Services

```bash
docker compose build
docker compose up -d
```

### Zero-Downtime Update (Blue-Green)

1. Scale up new version:
```bash
docker compose up -d --scale api=2
```

2. Wait for health check

3. Remove old container:
```bash
docker compose up -d --scale api=1 --no-recreate
```

---

## Monitoring & Maintenance

### View Real-Time Logs

```bash
docker compose logs -f
docker compose logs -f api
docker compose logs -f db
```

### Check Container Health

```bash
docker compose ps
docker inspect --format='{{.State.Health.Status}}' ssms-api
```

### Check Resource Usage

```bash
docker stats
```

### Database Connection Test

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "SELECT @@VERSION"
```

### Cleanup Old Images

```bash
docker image prune -a
```

---

## Troubleshooting

### Database Won't Start

**Symptom**: `ssms-db` shows unhealthy

**Causes**:
1. Weak SA password
2. Insufficient memory
3. Corrupted volume

**Solutions**:
```bash
docker compose logs db

docker inspect ssms-db

docker volume inspect ssms-mssql-data
```

If volume corrupted:
```bash
docker compose down
docker volume rm ssms-mssql-data
docker compose up -d
```

### API Can't Connect to Database

**Symptom**: API logs show connection errors

**Causes**:
1. Database not ready
2. Wrong connection string
3. Network issue

**Solutions**:
```bash
docker exec -it ssms-api ping db

docker exec -it ssms-api cat /etc/hosts | grep db

docker compose restart api
```

### Frontend Can't Reach API

**Symptom**: 502 Bad Gateway

**Causes**:
1. API container down
2. Nginx misconfiguration
3. Network issue

**Solutions**:
```bash
docker compose ps api

docker exec -it ssms-web curl http://ssms-api:80/health

docker compose logs web
```

### Data Loss Prevention

**CRITICAL**: Never run these commands on production:
```bash
docker volume rm ssms-mssql-data
docker compose down -v
```

Always backup before:
- Updating containers
- Removing volumes
- Changing database configuration

---

## SSL/HTTPS Configuration

### 1. Obtain SSL Certificate

Place certificate files:
```
nginx/ssl/certificate.crt
nginx/ssl/private.key
```

### 2. Update Nginx Configuration

Create `nginx/conf.d/ssl.conf`:

```nginx
server {
    listen 443 ssl http2;
    listen [::]:443 ssl http2;
    server_name your-domain.com;

    ssl_certificate /etc/nginx/ssl/certificate.crt;
    ssl_certificate_key /etc/nginx/ssl/private.key;
    
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers on;

    root /usr/share/nginx/html;
    index index.html;

    location /api/ {
        proxy_pass http://ssms-api:80/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location / {
        try_files $uri $uri/ /index.html;
    }
}

server {
    listen 80;
    listen [::]:80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}
```

### 3. Restart Nginx

```bash
docker compose restart web
```

---

## Production Hardening

### 1. Disable External Database Port

In `docker-compose.yml`, comment out:
```yaml
ports:
  # - "1433:1433"
```

### 2. Restrict API Port

Remove external API port exposure:
```yaml
ports:
  # - "5000:80"
```

### 3. Set Resource Limits

Already configured in `docker-compose.yml`:
- Database: 4GB RAM max
- API: 1GB RAM max
- Web: 512MB RAM max

### 4. Enable Firewall Rules

```powershell
New-NetFirewallRule -DisplayName "Allow HTTP" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "Allow HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow

New-NetFirewallRule -DisplayName "Block SQL Server" -Direction Inbound -LocalPort 1433 -Protocol TCP -Action Block
```

### 5. Regular Security Updates

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker compose up -d db
```

---

## Performance Tuning

### Database Optimization

```sql
-- Enable query store
ALTER DATABASE SSMS_KhaiThacTau SET QUERY_STORE = ON;

-- Update statistics
EXEC sp_updatestats;

-- Rebuild indexes
ALTER INDEX ALL ON [YourTable] REBUILD;
```

### API Optimization

Adjust in `.env`:
```bash
DB_MEMORY_LIMIT_MB=4096
LOG_LEVEL=Warning
```

### Nginx Caching

Add to `nginx.conf`:
```nginx
proxy_cache_path /var/cache/nginx levels=1:2 keys_zone=api_cache:10m max_size=1g inactive=60m;

location /api/ {
    proxy_cache api_cache;
    proxy_cache_valid 200 5m;
}
```

---

## Support & Maintenance

### Health Check Endpoints

- Web: `http://your-server/health`
- API: `http://your-server/api/health`

### Log Locations

```bash
docker compose logs > logs/docker-compose.log

docker cp ssms-api:/app/logs ./api-logs

docker cp ssms-db:/var/opt/mssql/log ./db-logs
```

### Emergency Shutdown

```bash
docker compose stop

docker compose down
```

### Emergency Restart

```bash
docker compose restart

docker compose down && docker compose up -d
```

---

## Migration from Development

### 1. Export Development Database

```bash
docker exec ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "DevPassword" -C \
  -Q "BACKUP DATABASE SSMS_KhaiThacTau TO DISK = '/var/opt/mssql/backups/prod_migration.bak'"

docker cp ssms-db:/var/opt/mssql/backups/prod_migration.bak ./
```

### 2. Transfer to Production Server

```powershell
Copy-Item prod_migration.bak \\production-server\C$\Backups\
```

### 3. Restore on Production

```bash
docker cp prod_migration.bak ssms-db:/var/opt/mssql/backups/

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "ProdPassword" -C \
  -Q "RESTORE DATABASE SSMS_KhaiThacTau FROM DISK = '/var/opt/mssql/backups/prod_migration.bak' WITH REPLACE"
```

### 4. Verify Data

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "ProdPassword" -C \
  -d SSMS_KhaiThacTau \
  -Q "SELECT COUNT(*) FROM Users; SELECT COUNT(*) FROM Permissions;"
```

---

## Quick Reference Commands

```bash
docker compose up -d
docker compose down
docker compose restart
docker compose ps
docker compose logs -f
docker compose build
docker compose pull

docker exec -it ssms-db bash
docker exec -it ssms-api bash
docker exec -it ssms-web sh

docker volume ls
docker volume inspect ssms-mssql-data
docker network inspect ssms-network

docker stats
docker system df
docker system prune
```

---

**Last Updated**: 2026-01-29  
**Version**: 1.0.0
