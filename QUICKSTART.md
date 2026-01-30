# SSMS Docker Quick Start Guide

## Prerequisites

- Windows Server 2019 or Windows 10/11 Pro
- Docker Desktop with Linux containers enabled
- At least 8GB RAM
- At least 50GB free disk space

---

## Quick Start (5 Minutes)

### 1. Clone and Configure

```bash
cd QuanLyQuyTrinhAnToan_BieuMauQHSE

cp .env.example .env
```

### 2. Edit Environment Variables

Open `.env` and change:

```bash
DB_SA_PASSWORD=YourStrong@Password123!
JWT_SECRET_KEY=Your-Very-Long-Secret-Key-At-Least-32-Characters
```

### 3. Start Services

```bash
docker compose up -d
```

Wait 1-2 minutes for all services to become healthy.

### 4. Check Status

```bash
docker compose ps
```

Expected output:
```
NAME       SERVICE   STATUS              PORTS
ssms-api   api       Up (healthy)        0.0.0.0:5000->80/tcp
ssms-db    db        Up (healthy)        0.0.0.0:1433->1433/tcp
ssms-web   web       Up (healthy)        0.0.0.0:80->80/tcp, 0.0.0.0:443->443/tcp
```

### 5. Access Application

Open browser: `http://localhost`

API health check: `http://localhost/api/health`

---

## Initial Database Setup

### Option A: Run Migrations

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "CREATE DATABASE SSMS_KhaiThacTau"

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -d SSMS_KhaiThacTau \
  -i /docker-entrypoint-initdb.d/002_dynamic_permissions.sql
```

### Option B: Restore Backup

```bash
docker cp backup.bak ssms-db:/var/opt/mssql/backups/

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "RESTORE DATABASE SSMS_KhaiThacTau FROM DISK = '/var/opt/mssql/backups/backup.bak' WITH REPLACE"
```

---

## Development Mode

For local development with hot reload:

### 1. Create Override File

```bash
cp docker-compose.override.yml.example docker-compose.override.yml
```

### 2. Start Development Mode

```bash
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

This exposes:
- Database: `localhost:1433`
- API: `localhost:5000`
- Web: `localhost:8080`

---

## Common Commands

### View Logs

```bash
docker compose logs -f
docker compose logs -f api
docker compose logs -f db
```

### Restart Service

```bash
docker compose restart api
docker compose restart web
```

### Stop All Services

```bash
docker compose down
```

### Rebuild and Restart

```bash
docker compose down
docker compose build
docker compose up -d
```

### Access Database

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C
```

---

## Backup Database

```bash
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "BACKUP DATABASE SSMS_KhaiThacTau TO DISK = '/var/opt/mssql/backups/backup_$(date +%Y%m%d).bak' WITH COMPRESSION"

docker cp ssms-db:/var/opt/mssql/backups/backup_$(date +%Y%m%d).bak ./
```

---

## Troubleshooting

### Services Not Starting

```bash
docker compose logs

docker compose ps -a
```

### Database Connection Issues

```bash
docker exec -it ssms-api ping db

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourPassword" -C \
  -Q "SELECT @@VERSION"
```

### Reset Everything

```bash
docker compose down -v
docker compose up -d
```

---

## Next Steps

1. Read [DEPLOYMENT.md](./DEPLOYMENT.md) for detailed production deployment
2. Configure SSL certificates for HTTPS
3. Set up automated backups
4. Configure monitoring and logging
5. Review security hardening steps

---

**Support**: See [DEPLOYMENT.md](./DEPLOYMENT.md) for comprehensive guide
