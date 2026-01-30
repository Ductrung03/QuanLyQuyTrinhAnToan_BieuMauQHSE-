# Docker Deployment Files Summary

## Created Files Overview

This deployment package includes all necessary files for deploying the SSMS application using Docker Compose with SQL Server 2022 Linux, .NET 8.0 API, and React frontend served by Nginx.

---

## File Structure

```
QuanLyQuyTrinhAnToan_BieuMauQHSE/
├── docker-compose.yml                      # Main orchestration file
├── .env.example                            # Environment variables template
├── docker-compose.override.yml.example     # Development overrides
├── .gitignore                              # Updated with Docker ignores
│
├── backend/
│   ├── Dockerfile                          # API container build
│   └── .dockerignore                       # Exclude unnecessary files
│
├── frontend/
│   ├── Dockerfile                          # Frontend + Nginx build
│   ├── nginx.conf                          # Nginx configuration
│   └── .dockerignore                       # Exclude unnecessary files
│
├── nginx/
│   ├── ssl/                                # SSL certificates (gitignored)
│   ├── conf.d/
│   │   └── ssl.conf.example               # HTTPS configuration
│   └── README.md                           # SSL setup guide
│
├── scripts/
│   ├── db-backup.ps1                       # Automated backup script
│   ├── db-restore.ps1                      # Database restore script
│   └── validate-deployment.ps1             # Pre-deployment validation
│
├── DEPLOYMENT.md                           # Comprehensive deployment guide
├── QUICKSTART.md                           # Quick start guide
└── DOCKER_DEPLOYMENT_SUMMARY.md           # This file
```

---

## Core Files

### 1. docker-compose.yml
**Purpose**: Main orchestration file defining all services

**Key Features**:
- SQL Server 2022 Linux with persistent volume
- .NET 8.0 API backend
- Nginx web server for frontend
- Health checks on all services
- Restart policies
- Resource limits
- Internal networking

**Services**:
- `db`: SQL Server 2022 (port 1433)
- `api`: .NET API (internal port 5000)
- `web`: Nginx + React (ports 80/443)

**Volumes**:
- `mssql-data`: Database files (persistent)
- `mssql-backups`: SQL backup storage
- `api-uploads`: User uploaded files
- `api-logs`: Application logs
- `nginx-logs`: Web server logs

### 2. .env.example
**Purpose**: Environment variables template

**Critical Variables**:
- `DB_SA_PASSWORD`: SQL Server admin password
- `JWT_SECRET_KEY`: API authentication secret
- `DB_NAME`: Database name
- `ASPNETCORE_ENVIRONMENT`: Production/Development
- `VITE_API_BASE_URL`: API endpoint for frontend

**Usage**: 
```bash
cp .env.example .env
# Edit .env with actual values
```

---

## Docker Files

### 3. backend/Dockerfile
**Purpose**: Multi-stage build for .NET 8.0 API

**Build Stages**:
1. **base**: Runtime environment (mcr.microsoft.com/dotnet/aspnet:8.0)
2. **build**: Build environment (mcr.microsoft.com/dotnet/sdk:8.0)
3. **publish**: Optimized production build
4. **final**: Minimal production image

**Features**:
- Layer caching optimization
- Health check endpoint
- Volume directories for uploads/logs
- Non-root user execution

### 4. frontend/Dockerfile
**Purpose**: Build React app and serve with Nginx

**Build Stages**:
1. **build**: Node.js environment for Vite build
2. **final**: Nginx Alpine for serving static files

**Features**:
- Production-optimized build
- Custom nginx.conf
- Health check endpoint
- Static file caching

### 5. frontend/nginx.conf
**Purpose**: Nginx server configuration

**Features**:
- Reverse proxy to API (/api → ssms-api:80)
- SPA routing (fallback to index.html)
- Gzip compression
- Cache headers for static assets
- Client max body size (20MB for uploads)
- Health check endpoint
- Proxy timeouts (5 minutes)

---

## Documentation

### 6. DEPLOYMENT.md
**Purpose**: Comprehensive production deployment guide

**Sections**:
- Pre-deployment checklist
- Initial deployment steps
- Database migration strategy
- Data persistence & backup
- Application updates
- Monitoring & maintenance
- Troubleshooting
- SSL/HTTPS configuration
- Production hardening
- Performance tuning

### 7. QUICKSTART.md
**Purpose**: Quick start guide for fast deployment

**Covers**:
- Prerequisites
- 5-minute deployment
- Database setup
- Development mode
- Common commands
- Basic troubleshooting

### 8. nginx/README.md
**Purpose**: SSL certificate setup guide

**Methods Covered**:
- Self-signed certificates (development)
- Let's Encrypt (production)
- Commercial certificates

---

## Scripts

### 9. scripts/db-backup.ps1
**Purpose**: Automated database backup

**Features**:
- Compressed SQL Server backups
- Automatic retention (30 days default)
- Timestamp-based naming
- Error handling
- Cleanup of old backups

**Usage**:
```powershell
.\scripts\db-backup.ps1
.\scripts\db-backup.ps1 -RetentionDays 60 -BackupDir "D:\Backups"
```

### 10. scripts/db-restore.ps1
**Purpose**: Database restore from backup

**Features**:
- Interactive confirmation
- Single-user mode for restore
- Automatic verification
- Error recovery
- Safety checks

**Usage**:
```powershell
.\scripts\db-restore.ps1 -BackupFile ".\backups\ssms_backup_20260129.bak"
```

### 11. scripts/validate-deployment.ps1
**Purpose**: Pre-deployment validation

**Checks**:
- Required files exist
- Environment variables configured
- Docker installed and running
- docker-compose.yml valid
- Directory structure
- Security settings
- Password strength
- Network connectivity

**Usage**:
```powershell
.\scripts\validate-deployment.ps1
```

---

## Configuration Examples

### 12. docker-compose.override.yml.example
**Purpose**: Development environment overrides

**Features**:
- Expose all ports for debugging
- Debug build configuration
- Verbose logging
- Volume mounting for hot reload

**Usage**:
```bash
cp docker-compose.override.yml.example docker-compose.override.yml
docker compose up -d
```

### 13. nginx/conf.d/ssl.conf.example
**Purpose**: HTTPS configuration template

**Features**:
- TLS 1.2/1.3 configuration
- Strong cipher suites
- Security headers (HSTS, X-Frame-Options)
- HTTP to HTTPS redirect
- Let's Encrypt support

**Usage**:
```bash
cp nginx/conf.d/ssl.conf.example nginx/conf.d/ssl.conf
# Edit server_name
docker compose restart web
```

---

## Deployment Workflow

### First-Time Production Deployment

1. **Preparation**
   ```bash
   .\scripts\validate-deployment.ps1
   ```

2. **Configure Environment**
   ```bash
   cp .env.example .env
   # Edit .env with production values
   ```

3. **Start Services**
   ```bash
   docker compose up -d
   ```

4. **Initialize Database**
   ```bash
   # Option A: Run migrations
   docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password" -C -i /docker-entrypoint-initdb.d/002_dynamic_permissions.sql
   
   # Option B: Restore backup
   .\scripts\db-restore.ps1 -BackupFile "backup.bak"
   ```

5. **Verify Deployment**
   ```bash
   docker compose ps
   docker compose logs -f
   ```

6. **Configure SSL** (Optional)
   ```bash
   # Place certificates in nginx/ssl/
   cp nginx/conf.d/ssl.conf.example nginx/conf.d/ssl.conf
   docker compose restart web
   ```

### Regular Maintenance

**Daily Backup**:
```bash
.\scripts\db-backup.ps1
```

**Update Application**:
```bash
git pull
docker compose build
docker compose up -d
```

**Monitor Logs**:
```bash
docker compose logs -f api
docker compose logs -f db
```

**Check Health**:
```bash
docker compose ps
curl http://localhost/api/health
```

---

## Data Persistence Strategy

### Volume Mapping

| Volume Name | Container Path | Purpose | Backup Priority |
|------------|---------------|---------|-----------------|
| mssql-data | /var/opt/mssql | Database files | CRITICAL |
| mssql-backups | /var/opt/mssql/backups | SQL backups | HIGH |
| api-uploads | /app/uploads | User files | HIGH |
| api-logs | /app/logs | Application logs | MEDIUM |
| nginx-logs | /var/log/nginx | Web logs | LOW |

### Backup Strategy

**Database**:
- Automated daily backups (2 AM)
- 30-day retention
- Stored in `mssql-backups` volume
- Copied to host `./backups/` directory

**Uploaded Files**:
- Included in `api-uploads` volume
- Manual backup via volume export
- Consider cloud storage sync

**Logs**:
- Rotated automatically
- 7-day retention
- Exported for analysis if needed

---

## Network Architecture

```
Internet
    │
    ▼
[Port 80/443] ─────────────────┐
    │                          │
    ▼                          │
┌─────────────────────┐       │
│   Nginx (web)       │       │
│   - Frontend        │       │
│   - Reverse Proxy   │       │
└─────────────────────┘       │
    │                          │
    │ /api → ssms-api:80      │
    ▼                          │
┌─────────────────────┐       │
│   .NET API (api)    │       │
│   - Internal Port   │       │
└─────────────────────┘       │
    │                          │
    │ Server=db,1433          │
    ▼                          │
┌─────────────────────┐       │
│   SQL Server (db)   │       │
│   - Internal Only   │       │
└─────────────────────┘       │
                               │
All connected via              │
ssms-internal network ─────────┘
(172.20.0.0/16)
```

**Security**:
- Database NOT exposed externally (port 1433 internal only)
- API NOT exposed externally (accessed via Nginx proxy)
- Only Nginx exposes ports 80/443
- Services communicate via internal network using service names

---

## Environment Compatibility

### Target Platform
- **OS**: Windows Server 2019
- **Container Type**: Linux containers
- **Docker Desktop**: Required

### System Requirements
- **CPU**: 4+ cores recommended
- **RAM**: 8GB minimum, 16GB recommended
- **Disk**: 50GB+ free space
- **Network**: Ports 80/443 available

### SQL Server Requirements
- **Memory**: 2GB minimum (configured: 4GB limit)
- **Disk**: 20GB+ for database growth
- **EULA**: Must accept via `ACCEPT_EULA=Y`

---

## Security Considerations

### Password Requirements
- **SA Password**: Min 8 chars, complexity required
- **JWT Secret**: Min 32 chars, random string
- **Never** use default/example passwords

### Network Security
- Database port **not exposed** externally
- API accessed **only via Nginx proxy**
- Internal network isolation
- Firewall rules recommended

### SSL/TLS
- Use HTTPS in production
- TLS 1.2+ only
- Strong cipher suites
- Certificate auto-renewal (Let's Encrypt)

### Data Protection
- Regular backups (automated)
- Volume persistence (data loss prevention)
- Backup retention policy
- Disaster recovery plan

---

## Troubleshooting Guide

### Common Issues

**1. Database Won't Start**
```bash
docker compose logs db
# Check password complexity
# Check available memory
# Verify volume permissions
```

**2. API Can't Connect to Database**
```bash
docker exec -it ssms-api ping db
# Verify network connectivity
# Check connection string in .env
# Ensure database is healthy
```

**3. Frontend Shows 502 Bad Gateway**
```bash
docker compose ps api
docker exec -it ssms-web curl http://ssms-api:80/health
# Verify API is running
# Check nginx proxy configuration
```

**4. Data Loss After Restart**
```bash
docker volume ls
# Ensure volumes are named (not anonymous)
# Never use: docker compose down -v
```

---

## Migration Notes

### From Development to Production

1. **Export Development Database**
   ```bash
   .\scripts\db-backup.ps1
   ```

2. **Transfer Files**
   - Copy backup file to production server
   - Copy environment configuration
   - Verify migration scripts

3. **Import to Production**
   ```bash
   .\scripts\db-restore.ps1 -BackupFile "dev_backup.bak"
   ```

4. **Verify Data**
   ```bash
   docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password" -C -d SSMS_KhaiThacTau -Q "SELECT COUNT(*) FROM Users"
   ```

### Database Schema Updates

1. Add migration script to `Database/migrations/`
2. Execute manually:
   ```bash
   docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password" -C -d SSMS_KhaiThacTau -i /docker-entrypoint-initdb.d/003_new_migration.sql
   ```
3. No service restart required

---

## Performance Optimization

### Database Tuning
- Memory limit: Adjust `DB_MEMORY_LIMIT_MB` in .env
- Max connections: Configure in SQL Server
- Indexes: Monitor and optimize queries

### API Performance
- Resource limits: Adjust in docker-compose.yml
- Logging level: Set to `Warning` in production
- Connection pooling: Enabled by default

### Nginx Optimization
- Gzip compression: Enabled
- Static file caching: 1 year for assets
- Proxy buffering: Configured for large uploads

---

## Support Resources

- **Deployment Guide**: [DEPLOYMENT.md](./DEPLOYMENT.md)
- **Quick Start**: [QUICKSTART.md](./QUICKSTART.md)
- **SSL Setup**: [nginx/README.md](./nginx/README.md)
- **Validation**: Run `.\scripts\validate-deployment.ps1`

---

## Version Information

- **Docker Compose**: v3.8
- **SQL Server**: 2022 Linux (latest)
- **.NET**: 8.0
- **Node.js**: 20 Alpine
- **Nginx**: Alpine (latest)

---

## Quick Command Reference

```bash
docker compose up -d
docker compose down
docker compose ps
docker compose logs -f [service]
docker compose restart [service]
docker compose build [service]

.\scripts\validate-deployment.ps1
.\scripts\db-backup.ps1
.\scripts\db-restore.ps1 -BackupFile "file.bak"

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Password" -C
docker exec -it ssms-api bash
docker exec -it ssms-web sh

docker volume ls
docker volume inspect ssms-mssql-data
docker network inspect ssms-network
```

---

**Created**: 2026-01-29  
**Last Updated**: 2026-01-29  
**Deployment Version**: 1.0.0
