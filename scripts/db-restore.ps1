# ============================================================
# SSMS Database Restore Script
# ============================================================
# Usage: .\scripts\db-restore.ps1 -BackupFile "backup.bak"
# Requires: Docker, .env file with DB_SA_PASSWORD
# ============================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile,
    [string]$Container = "ssms-db",
    [string]$DatabaseName = "SSMS_KhaiThacTau"
)

$ErrorActionPreference = "Stop"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message"
}

if (-not (Test-Path $BackupFile)) {
    Write-Log "ERROR: Backup file not found: $BackupFile" -Level "ERROR"
    exit 1
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

$backupFileName = Split-Path $BackupFile -Leaf
$containerPath = "/var/opt/mssql/backups/$backupFileName"

Write-Log "WARNING: This will replace the existing database '$DatabaseName'" -Level "WARN"
Write-Host -NoNewLine "Continue? (y/N): "
$confirmation = Read-Host

if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Log "Restore cancelled by user"
    exit 0
}

Write-Log "Starting database restore..."

try {
    Write-Log "Copying backup file to container..."
    docker cp $BackupFile "${Container}:$containerPath"
    Write-Log "Backup file copied to container: $containerPath"
    
    Write-Log "Setting database to single-user mode..."
    docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P "$dbPassword" -C `
        -Q "ALTER DATABASE $DatabaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
    
    Write-Log "Restoring database from backup..."
    docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P "$dbPassword" -C `
        -Q "RESTORE DATABASE $DatabaseName FROM DISK = '$containerPath' WITH REPLACE, STATS = 10"
    
    Write-Log "Setting database to multi-user mode..."
    docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P "$dbPassword" -C `
        -Q "ALTER DATABASE $DatabaseName SET MULTI_USER"
    
    Write-Log "Removing backup file from container..."
    docker exec $Container rm "$containerPath"
    
    Write-Log "Verifying database..."
    $result = docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
        -S localhost -U sa -P "$dbPassword" -C `
        -d $DatabaseName `
        -Q "SELECT DB_NAME() as DatabaseName, GETDATE() as RestoredAt" -h -1
    
    Write-Log "Database restored successfully!"
    Write-Log "Verification result: $result"
    
} catch {
    Write-Log "ERROR: Restore failed - $_" -Level "ERROR"
    
    Write-Log "Attempting to set database back to multi-user mode..." -Level "WARN"
    try {
        docker exec $Container /opt/mssql-tools18/bin/sqlcmd `
            -S localhost -U sa -P "$dbPassword" -C `
            -Q "ALTER DATABASE $DatabaseName SET MULTI_USER"
    } catch {
        Write-Log "Could not restore multi-user mode" -Level "ERROR"
    }
    
    exit 1
}
