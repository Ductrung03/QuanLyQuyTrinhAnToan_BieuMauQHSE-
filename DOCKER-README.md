# Docker Deployment (Windows Server 2019 + Linux containers)

## ✅ Verified Setup (Tested on Arch Linux with Docker Engine)

### Prerequisites
- Docker Engine with Linux containers enabled (Hyper-V/WSL2 on Windows)
- At least 4GB RAM available for containers
- Ports available: 3000 (web), 14330 (database)

## 🚀 Quick Start

If you already cloned the repo on the server, run: `scripts/deploy.ps1`

### 1. Copy environment file
```bash
cp .env.example .env
```

### 2. Configure environment variables
Edit `.env` and change:
- `SA_PASSWORD` - Strong SQL Server password (min 8 chars, mixed case + numbers + symbols)
- `JWT_SECRET` - Strong JWT secret key (min 32 characters)
- `DB_PORT` - SQL Server port (default: 14330 to avoid conflicts)
- `WEB_HTTP_PORT` - Frontend port (default: 3000 to avoid conflicts)

### 3. Build images
```bash
docker compose build
```

**Note**: If pull fails for .NET SDK, use:
```bash
docker pull --platform linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0
```

### 4. Start all services
```bash
docker compose up -d
```

### 5. Verify deployment
```bash
docker compose ps
curl http://localhost:3000/health    # Frontend health
docker exec ssms-api curl http://localhost:8080/health  # API health
```

## 📦 What's Running

- **ssms-db** (SQL Server 2022) - Port 14330 → 1433 (internal)
  - Auto-creates database via EF migrations on first run
  - Data persisted in volume `mssql-data`
  
- **ssms-api** (.NET 8 API) - Port 8080 (internal only)
  - Auto-applies migrations on startup
  - Seeds initial roles/permissions
  
- **ssms-web** (Nginx + React) - Port 3000 → 80 (internal)
  - Serves frontend static files
  - Proxies `/api` requests to backend

## ⚠️ CRITICAL: Data Persistence

- Database data stored in named volume `mssql-data`
- **NEVER** run `docker compose down -v` unless you want to delete all data
- Use `docker compose down` to stop without deleting data
- Use `docker compose up -d` to restart with data intact

## 🔧 Common Commands

### View logs
```bash
docker compose logs -f api       # API logs
docker compose logs -f db        # Database logs
docker compose logs -f web       # Nginx logs
```

### Stop services (keep data)
```bash
docker compose down
```

### Restart services
```bash
docker compose restart
```

### Rebuild after code changes
```bash
docker compose build --no-cache
docker compose up -d
```

## 🐛 Troubleshooting

### Port conflicts
If ports 3000 or 14330 are in use, edit `.env`:
```
WEB_HTTP_PORT=8000
DB_PORT=14400
```

### Database connection errors
Check if migrations ran:
```bash
docker logs ssms-api | grep migration
```
Should see: "Database migrations applied successfully"

### API unhealthy
Check logs:
```bash
docker logs ssms-api --tail 100
```

## 📋 System Requirements

- **Development**: 2GB RAM, 10GB disk
- **Production**: 4GB RAM, 20GB disk (with logs/uploads)

## 🔐 Security Notes

- Default `.env.example` contains weak passwords - CHANGE THEM
- Nginx runs HTTP only - add HTTPS for production
- JWT secret must be 32+ characters in production
- Database not exposed externally (only via Docker network)

## ✅ Deployment Checklist

- [ ] Changed `SA_PASSWORD` in `.env`
- [ ] Changed `JWT_SECRET` in `.env` (32+ chars)
- [ ] Verified ports 3000 and 14330 are available
- [ ] Ran `docker compose build` successfully
- [ ] Ran `docker compose up -d` successfully
- [ ] Verified all containers healthy: `docker compose ps`
- [ ] Tested frontend: `curl http://localhost:3000`
- [ ] Tested API health: `docker exec ssms-api curl http://localhost:8080/health`
