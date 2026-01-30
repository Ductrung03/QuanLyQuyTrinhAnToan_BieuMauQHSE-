# ============================================================
# SSMS Deployment Validation Script
# ============================================================
# Usage: .\scripts\validate-deployment.ps1
# Validates Docker deployment configuration
# ============================================================

$ErrorActionPreference = "Stop"

function Write-Status {
    param([string]$Message, [bool]$Success)
    if ($Success) {
        Write-Host "[✓] $Message" -ForegroundColor Green
    } else {
        Write-Host "[✗] $Message" -ForegroundColor Red
    }
}

function Write-Section {
    param([string]$Title)
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host $Title -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

$validationErrors = 0

Write-Section "1. Checking Required Files"

$requiredFiles = @(
    "docker-compose.yml",
    ".env",
    "backend/Dockerfile",
    "frontend/Dockerfile",
    "frontend/nginx.conf"
)

foreach ($file in $requiredFiles) {
    $exists = Test-Path $file
    Write-Status "$file" -Success $exists
    if (-not $exists) { $validationErrors++ }
}

Write-Section "2. Checking Environment Variables"

if (Test-Path .env) {
    $envContent = Get-Content .env
    
    $requiredVars = @(
        "DB_SA_PASSWORD",
        "JWT_SECRET_KEY",
        "DB_NAME"
    )
    
    foreach ($var in $requiredVars) {
        $found = $envContent | Where-Object { $_ -match "^$var=" }
        $hasValue = $found -and ($found -notmatch "=\s*$")
        Write-Status "$var defined" -Success $hasValue
        if (-not $hasValue) { $validationErrors++ }
    }
    
    $dbPassword = ($envContent | Where-Object { $_ -match "^DB_SA_PASSWORD=" }) -replace "DB_SA_PASSWORD=", ""
    if ($dbPassword.Length -lt 8) {
        Write-Status "DB_SA_PASSWORD length (min 8 chars)" -Success $false
        $validationErrors++
    } else {
        Write-Status "DB_SA_PASSWORD length (min 8 chars)" -Success $true
    }
    
    $jwtSecret = ($envContent | Where-Object { $_ -match "^JWT_SECRET_KEY=" }) -replace "JWT_SECRET_KEY=", ""
    if ($jwtSecret.Length -lt 32) {
        Write-Status "JWT_SECRET_KEY length (min 32 chars)" -Success $false
        $validationErrors++
    } else {
        Write-Status "JWT_SECRET_KEY length (min 32 chars)" -Success $true
    }
    
    $defaultPassword = "YourStrong@Password123!ChangeMe"
    if ($dbPassword -eq $defaultPassword) {
        Write-Host "[!] WARNING: Using default DB password" -ForegroundColor Yellow
    }
    
    $defaultJWT = "SSMS-Super-Secret-Key-For-JWT-Authentication-MUST-CHANGE-IN-PRODUCTION-Min-32-Chars"
    if ($jwtSecret -eq $defaultJWT) {
        Write-Host "[!] WARNING: Using default JWT secret" -ForegroundColor Yellow
    }
} else {
    Write-Status ".env file exists" -Success $false
    $validationErrors++
}

Write-Section "3. Checking Docker"

try {
    $dockerVersion = docker --version
    Write-Status "Docker installed: $dockerVersion" -Success $true
} catch {
    Write-Status "Docker installed" -Success $false
    $validationErrors++
}

try {
    docker info | Out-Null
    Write-Status "Docker daemon running" -Success $true
} catch {
    Write-Status "Docker daemon running" -Success $false
    $validationErrors++
}

Write-Section "4. Checking Docker Compose Configuration"

try {
    docker compose config | Out-Null
    Write-Status "docker-compose.yml valid" -Success $true
} catch {
    Write-Status "docker-compose.yml valid" -Success $false
    $validationErrors++
}

Write-Section "5. Checking Directory Structure"

$requiredDirs = @(
    "backend",
    "frontend",
    "Database/migrations",
    "nginx",
    "scripts"
)

foreach ($dir in $requiredDirs) {
    $exists = Test-Path $dir
    Write-Status "$dir directory" -Success $exists
    if (-not $exists) { $validationErrors++ }
}

Write-Section "6. Checking Running Services"

try {
    $services = docker compose ps --format json | ConvertFrom-Json
    
    if ($services) {
        foreach ($service in $services) {
            $isHealthy = $service.Health -eq "healthy" -or $service.State -eq "running"
            Write-Status "$($service.Service) service: $($service.State)" -Success $isHealthy
        }
    } else {
        Write-Host "[!] No services running (docker compose up -d to start)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "[!] Could not check running services" -ForegroundColor Yellow
}

Write-Section "7. Checking Network Connectivity"

try {
    $dbRunning = docker ps --filter "name=ssms-db" --format "{{.Names}}"
    if ($dbRunning) {
        $apiRunning = docker ps --filter "name=ssms-api" --format "{{.Names}}"
        if ($apiRunning) {
            try {
                docker exec ssms-api ping -c 1 db 2>&1 | Out-Null
                Write-Status "API can reach database" -Success $true
            } catch {
                Write-Status "API can reach database" -Success $false
                $validationErrors++
            }
        }
    }
} catch {
    Write-Host "[!] Could not test network connectivity" -ForegroundColor Yellow
}

Write-Section "8. Security Checks"

$securityIssues = 0

if (Test-Path .env) {
    $gitIgnore = Get-Content .gitignore -ErrorAction SilentlyContinue
    if ($gitIgnore -contains ".env" -or $gitIgnore -match "^\.env$") {
        Write-Status ".env in .gitignore" -Success $true
    } else {
        Write-Status ".env in .gitignore" -Success $false
        $securityIssues++
    }
}

$dockerComposeContent = Get-Content docker-compose.yml
if ($dockerComposeContent -match "1433:1433" -and $dockerComposeContent -notmatch "#.*1433:1433") {
    Write-Host "[!] WARNING: Database port 1433 is exposed externally" -ForegroundColor Yellow
    $securityIssues++
}

Write-Section "Summary"

if ($validationErrors -eq 0) {
    Write-Host "`n✓ All validation checks passed!" -ForegroundColor Green
    if ($securityIssues -gt 0) {
        Write-Host "⚠ $securityIssues security warning(s) detected" -ForegroundColor Yellow
    }
} else {
    Write-Host "`n✗ $validationErrors validation error(s) found" -ForegroundColor Red
    Write-Host "Please fix the errors above before deploying" -ForegroundColor Red
    exit 1
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Review warnings above (if any)"
Write-Host "2. Run: docker compose up -d"
Write-Host "3. Check status: docker compose ps"
Write-Host "4. View logs: docker compose logs -f"
