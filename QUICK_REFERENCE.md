# Docker Deployment Quick Reference

## 🚀 Quick Start (3 Commands)

```powershell
cp .env.example .env
docker compose up -d
.\scripts\db-backup.ps1
```

---

## 📋 Essential Commands

### Deployment
```powershell
.\scripts\validate-deployment.ps1
docker compose up -d
docker compose ps
docker compose logs -f
```

### Database
```powershell
.\scripts\db-backup.ps1
.\scripts\db-restore.ps1 -BackupFile "backup.bak"

docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P "$env:DB_SA_PASSWORD" -C `
  -Q "SELECT @@VERSION"
```

### Maintenance
```powershell
docker compose restart [service]
docker compose build [service]
docker compose down
docker volume ls
docker stats
```

---

## 🔧 Configuration

### Critical Environment Variables
```bash
DB_SA_PASSWORD=YourStrong@Password123!
JWT_SECRET_KEY=Your-32-Char-Random-Secret
DB_NAME=SSMS_KhaiThacTau
```

### Service URLs
- **Frontend**: http://localhost
- **API**: http://localhost/api
- **Health**: http://localhost/health

---

## 📦 Volumes (Data Persistence)

| Volume | Purpose | Priority |
|--------|---------|----------|
| mssql-data | Database files | CRITICAL |
| mssql-backups | SQL backups | HIGH |
| api-uploads | User files | HIGH |
| api-logs | App logs | MEDIUM |
| nginx-logs | Web logs | LOW |

**⚠️ NEVER RUN**: `docker compose down -v` (deletes all data!)

---

## 🌐 Network Architecture

```
Internet → Nginx:80/443 → API:5000 → Database:1433
         (public)       (internal)  (internal)
```

All services on: `ssms-internal` network

---

## 🔒 Security Checklist

- [ ] Change default passwords in `.env`
- [ ] Database port NOT exposed (comment out in docker-compose.yml)
- [ ] API port NOT exposed (comment out in docker-compose.yml)
- [ ] SSL certificates configured (optional)
- [ ] `.env` in `.gitignore`

---

## 📚 Documentation

| File | Purpose |
|------|---------|
| [QUICKSTART.md](./QUICKSTART.md) | 5-minute deployment |
| [DEPLOYMENT.md](./DEPLOYMENT.md) | Complete guide |
| [PRE_DEPLOYMENT_CHECKLIST.md](./PRE_DEPLOYMENT_CHECKLIST.md) | Production checklist |
| [DOCKER_DEPLOYMENT_SUMMARY.md](./DOCKER_DEPLOYMENT_SUMMARY.md) | Files overview |

---

## 🆘 Troubleshooting

### Services Won't Start
```powershell
docker compose logs [service]
docker compose ps -a
.\scripts\validate-deployment.ps1
```

### Data Loss Recovery
```powershell
.\scripts\db-restore.ps1 -BackupFile "latest_backup.bak"
```

### Reset Everything
```powershell
docker compose down
docker compose up -d --build
```

---

## 📞 Health Checks

```powershell
curl http://localhost/health
curl http://localhost/api/health

docker compose ps
docker stats
```

---

## 🎯 Production Deployment Steps

1. **Validate**: `.\scripts\validate-deployment.ps1`
2. **Configure**: Edit `.env` with production values
3. **Deploy**: `docker compose up -d`
4. **Initialize**: Restore database or run migrations
5. **Verify**: Check health endpoints
6. **Backup**: `.\scripts\db-backup.ps1`
7. **Harden**: Remove external ports, enable HTTPS

---

## 🔄 Update Application

```powershell
git pull
docker compose build
docker compose up -d
```

---

**Version**: 1.0.0  
**Target**: Windows Server 2019 + Linux containers  
**Stack**: SQL Server 2022 + .NET 8.0 + React + Nginx
