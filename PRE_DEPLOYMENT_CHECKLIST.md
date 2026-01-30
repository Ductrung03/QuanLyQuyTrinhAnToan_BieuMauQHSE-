# Pre-Deployment Checklist

Complete this checklist before deploying to production.

---

## ☑️ Environment Setup

- [ ] Windows Server 2019 with latest updates installed
- [ ] Docker Desktop installed and configured for Linux containers
- [ ] Docker daemon is running: `docker info`
- [ ] Docker Compose available: `docker compose version`
- [ ] At least 8GB RAM available on server
- [ ] At least 50GB free disk space

---

## ☑️ Configuration Files

- [ ] `.env` file created from `.env.example`
- [ ] `DB_SA_PASSWORD` changed from default (min 8 chars, complexity)
- [ ] `JWT_SECRET_KEY` changed from default (min 32 chars)
- [ ] `DB_NAME` verified: `SSMS_KhaiThacTau`
- [ ] `ASPNETCORE_ENVIRONMENT` set to `Production`
- [ ] All required environment variables defined

---

## ☑️ Security Configuration

- [ ] Strong SA password set (uppercase, lowercase, numbers, special chars)
- [ ] JWT secret key is random and secure (32+ characters)
- [ ] `.env` file is in `.gitignore` (never commit secrets)
- [ ] Database port 1433 NOT exposed externally in docker-compose.yml
- [ ] API port 5000 NOT exposed externally in docker-compose.yml
- [ ] Only ports 80 and 443 are public-facing

---

## ☑️ Database Preparation

- [ ] Migration scripts present in `Database/migrations/`
- [ ] OR backup file ready for restoration
- [ ] Database name matches: `SSMS_KhaiThacTau`
- [ ] Understand migration strategy (see DEPLOYMENT.md)
- [ ] Backup plan documented

---

## ☑️ File Structure Validation

Run validation script:
```powershell
.\scripts\validate-deployment.ps1
```

Manual checks:
- [ ] `docker-compose.yml` present
- [ ] `backend/Dockerfile` present
- [ ] `frontend/Dockerfile` present
- [ ] `frontend/nginx.conf` present
- [ ] `scripts/` directory with PowerShell scripts
- [ ] `nginx/ssl/` directory created (for certificates)

---

## ☑️ Docker Configuration

- [ ] `docker-compose.yml` syntax valid: `docker compose config`
- [ ] Build configuration verified: `BUILD_CONFIGURATION=Release`
- [ ] Resource limits appropriate for server:
  - Database: 4GB RAM max
  - API: 1GB RAM max
  - Web: 512MB RAM max
- [ ] Volume names configured: `ssms-mssql-data`, etc.
- [ ] Network name configured: `ssms-network`

---

## ☑️ SSL/HTTPS Setup (Optional but Recommended)

If enabling HTTPS:
- [ ] SSL certificate obtained (Let's Encrypt, commercial, or self-signed)
- [ ] Certificate placed in `nginx/ssl/certificate.crt`
- [ ] Private key placed in `nginx/ssl/private.key`
- [ ] `nginx/conf.d/ssl.conf` configured from example
- [ ] Domain name configured in SSL config
- [ ] HTTPS redirect enabled (HTTP → HTTPS)

If not using HTTPS:
- [ ] Understand security implications
- [ ] Plan to add HTTPS before production use

---

## ☑️ Network Configuration

- [ ] Firewall rules configured:
  - Allow inbound: 80 (HTTP)
  - Allow inbound: 443 (HTTPS)
  - Block external: 1433 (SQL Server)
  - Block external: 5000 (API direct access)
- [ ] Server IP address or domain name known
- [ ] DNS configured (if using domain name)

---

## ☑️ Backup Strategy

- [ ] Backup directory created: `./backups/`
- [ ] Backup script tested: `.\scripts\db-backup.ps1`
- [ ] Backup retention policy defined (default: 30 days)
- [ ] Backup schedule planned (recommended: daily at 2 AM)
- [ ] Backup verification process documented
- [ ] Off-server backup storage configured (cloud, network drive)

---

## ☑️ Monitoring & Logging

- [ ] Understand log locations:
  - Database: `docker compose logs db`
  - API: `docker compose logs api`
  - Web: `docker compose logs web`
- [ ] Log retention policy defined
- [ ] Health check endpoints documented:
  - Web: `http://server/health`
  - API: `http://server/api/health`
- [ ] Monitoring plan in place

---

## ☑️ Disaster Recovery

- [ ] Database backup procedure tested
- [ ] Database restore procedure tested: `.\scripts\db-restore.ps1`
- [ ] Volume backup strategy defined
- [ ] Recovery Time Objective (RTO) documented
- [ ] Recovery Point Objective (RPO) documented
- [ ] Emergency contact information available

---

## ☑️ Testing

Before production deployment:
- [ ] Test deployment in staging environment
- [ ] Verify all services start: `docker compose ps`
- [ ] Check service health: All show "healthy"
- [ ] Test database connection from API
- [ ] Test API endpoints: `http://server/api/health`
- [ ] Test frontend loads: `http://server`
- [ ] Test file upload functionality
- [ ] Test authentication/authorization
- [ ] Performance test under load
- [ ] Test backup and restore procedures

---

## ☑️ Documentation

- [ ] Read [DEPLOYMENT.md](./DEPLOYMENT.md) completely
- [ ] Read [QUICKSTART.md](./QUICKSTART.md)
- [ ] Review [DOCKER_DEPLOYMENT_SUMMARY.md](./DOCKER_DEPLOYMENT_SUMMARY.md)
- [ ] Document server-specific configuration
- [ ] Document custom environment variables
- [ ] Create runbook for common operations
- [ ] Document emergency procedures

---

## ☑️ Team Preparation

- [ ] Team trained on Docker operations
- [ ] Access credentials distributed securely
- [ ] Deployment schedule communicated
- [ ] Downtime window (if needed) scheduled
- [ ] Rollback plan documented
- [ ] Support contacts available during deployment

---

## ☑️ Final Verification

Run these commands:

```powershell
# Validate deployment configuration
.\scripts\validate-deployment.ps1

# Check Docker Compose config
docker compose config

# Verify environment variables
Get-Content .env | Select-String -Pattern "PASSWORD|SECRET|KEY"

# Check available disk space
Get-PSDrive C | Select-Object Used,Free

# Check available memory
Get-CimInstance -ClassName Win32_ComputerSystem | Select-Object TotalPhysicalMemory
```

Expected results:
- [ ] Validation script passes (0 errors)
- [ ] docker-compose.yml is valid
- [ ] No default passwords in .env
- [ ] At least 50GB free disk space
- [ ] At least 8GB RAM available

---

## ☑️ Deployment Execution

### Pre-Deployment

- [ ] All checklist items above completed
- [ ] Validation script passed
- [ ] Team notified of deployment
- [ ] Backup of current system (if updating)

### Deployment Steps

```powershell
# 1. Build and start services
docker compose up -d

# 2. Wait for services to be healthy (1-2 minutes)
Start-Sleep -Seconds 120

# 3. Check service status
docker compose ps

# 4. Check logs for errors
docker compose logs

# 5. Initialize database
# Option A: Run migrations
docker exec -it ssms-db /opt/mssql-tools18/bin/sqlcmd `
  -S localhost -U sa -P "$env:DB_SA_PASSWORD" -C `
  -Q "CREATE DATABASE SSMS_KhaiThacTau"

# Option B: Restore backup
.\scripts\db-restore.ps1 -BackupFile "backup.bak"

# 6. Verify application
curl http://localhost/health
curl http://localhost/api/health

# 7. First backup
.\scripts\db-backup.ps1
```

Checklist:
- [ ] All services started: `docker compose up -d`
- [ ] All services healthy: `docker compose ps`
- [ ] Database initialized successfully
- [ ] Health checks passing
- [ ] Frontend accessible
- [ ] API accessible via frontend
- [ ] Initial backup created

### Post-Deployment

- [ ] Full application test
- [ ] User acceptance testing
- [ ] Monitor logs for 24 hours
- [ ] Verify backup schedule
- [ ] Document any issues encountered
- [ ] Update team documentation
- [ ] Close deployment ticket

---

## ☑️ Production Hardening

After successful deployment:

- [ ] Remove external database port exposure:
  ```yaml
  # In docker-compose.yml, comment out:
  # ports:
  #   - "1433:1433"
  ```
- [ ] Remove external API port exposure
- [ ] Enable HTTPS if not already done
- [ ] Configure automated backups (Task Scheduler)
- [ ] Set up log rotation
- [ ] Configure monitoring alerts
- [ ] Review and tighten resource limits
- [ ] Enable Docker auto-start on boot
- [ ] Document production configuration

---

## 🚨 Stop Deployment If:

- [ ] Validation script fails
- [ ] Required files missing
- [ ] Default passwords still in use
- [ ] Less than 8GB RAM available
- [ ] Less than 50GB disk space available
- [ ] Docker daemon not running
- [ ] Team not prepared
- [ ] No backup/rollback plan

---

## 📞 Emergency Contacts

Update with your team's information:

- **System Administrator**: _________________
- **Database Administrator**: _________________
- **Application Owner**: _________________
- **On-Call Support**: _________________

---

## 📋 Sign-Off

Deployment approved by:

- [ ] System Administrator: _________________ Date: _______
- [ ] Database Administrator: _________________ Date: _______
- [ ] Application Owner: _________________ Date: _______
- [ ] Security Review: _________________ Date: _______

---

**Deployment Date**: _________________  
**Deployed By**: _________________  
**Deployment Version**: 1.0.0  
**Notes**: _________________________________________________

---

## Quick Reference

**Validation**: `.\scripts\validate-deployment.ps1`  
**Deploy**: `docker compose up -d`  
**Status**: `docker compose ps`  
**Logs**: `docker compose logs -f`  
**Backup**: `.\scripts\db-backup.ps1`  
**Restore**: `.\scripts\db-restore.ps1 -BackupFile "file.bak"`  

**Documentation**: [DEPLOYMENT.md](./DEPLOYMENT.md) | [QUICKSTART.md](./QUICKSTART.md)
