# ============================================================
# SSMS Database Backup Script
# ============================================================
# Usage: .\scripts\db-backup.ps1
# Requires: Docker, .env file with DB_SA_PASSWORD
# ============================================================

param(
    [string]$BackupDir = ".\backups",
    [string]$Container = "ssms-db",
    [int]$RetentionDays = 30
)

$ErrorActionPreference = "Stop"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message"
}

if (-not (Test-Path $BackupDir)) {
    New-Item -ItemType Directory -Path $BackupDir -Force | Out-Null
    Write-Log "Created backup directory: $BackupDir"
}

if (-not (Test-Path .env)) {
    Write-Log "ERROR: .env file not found" -Level "ERROR"
    exit 1
}

$envContent = Get-Content .env
$dbPassword = ($envContent | Where-Object { $_ -match "^DB_SA_PASSWORD=" }) -replace "DB_SA_PASSWORD=", ""

if ([string]::IsNullOrEmpty($dbPassword)) {
    Write-Log "ERROR: DB_SA_PASSWORD not found in .env" -Level "ERROR"
    exit 1
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupName = "ssms_backup_$timestamp.bak"
$backupPath = "/var/opt/mssql/backups/$backupName"

Write-Log "Starting database backup..."

try {
    docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P "$dbPassword" -C `
        -Q "BACKUP DATABASE SSMS_KhaiThacTau TO DISK = '$backupPath' WITH COMPRESSION, STATS = 10"
    
    Write-Log "Backup created in container: $backupPath"
    
    docker cp "${Container}:$backupPath" "$BackupDir\$backupName"
    
    Write-Log "Backup copied to host: $BackupDir\$backupName"
    
    $backupFile = Get-Item "$BackupDir\$backupName"
    $sizeMB = [math]::Round($backupFile.Length / 1MB, 2)
    Write-Log "Backup size: $sizeMB MB"
    
    docker exec $Container rm "$backupPath"
    Write-Log "Removed backup from container"
    
    Write-Log "Cleaning up old backups (older than $RetentionDays days)..."
    $oldBackups = Get-ChildItem $BackupDir -Filter "ssms_backup_*.bak" | 
        Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-$RetentionDays) }
    
    foreach ($oldBackup in $oldBackups) {
        Remove-Item $oldBackup.FullName -Force
        Write-Log "Deleted old backup: $($oldBackup.Name)"
    }
    
    $remainingBackups = (Get-ChildItem $BackupDir -Filter "ssms_backup_*.bak").Count
    Write-Log "Backup completed successfully. Total backups: $remainingBackups"
    
} catch {
    Write-Log "ERROR: Backup failed - $_" -Level "ERROR"
    exit 1
}
