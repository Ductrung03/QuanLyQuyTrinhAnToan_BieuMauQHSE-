param(
    [string]$TargetPath = "C:\SSMS"
)

$ErrorActionPreference = "Stop"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message"
}

function Assert-Command {
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Write-Log "$Name is not installed or not in PATH" "ERROR"
        exit 1
    }
}

function Set-EnvValue {
    param(
        [string]$FilePath,
        [string]$Key,
        [string]$Value
    )
    if (-not (Test-Path $FilePath)) {
        New-Item -ItemType File -Path $FilePath -Force | Out-Null
    }

    $content = Get-Content $FilePath
    $pattern = "^$Key="
    $updated = $false

    $newContent = $content | ForEach-Object {
        if ($_ -match $pattern) {
            $updated = $true
            return "$Key=$Value"
        }
        $_
    }

    if (-not $updated) {
        $newContent += "$Key=$Value"
    }

    Set-Content -Path $FilePath -Value $newContent
}

Write-Log "Checking prerequisites..."
Assert-Command "git"
Assert-Command "docker"

Write-Log "Using existing repository at: $TargetPath"
if (-not (Test-Path "$TargetPath\.git")) {
    Write-Log "Repository not found at $TargetPath" "ERROR"
    Write-Log "Please clone the repository first, then re-run this script" "ERROR"
    exit 1
}

Push-Location $TargetPath

if (-not (Test-Path ".env")) {
    Write-Log "Creating .env from .env.example"
    Copy-Item ".env.example" ".env"
}

Write-Log "Configuring environment variables..."
$saPassword = Read-Host "Enter SQL SA password (strong)"
$jwtSecret = Read-Host "Enter JWT secret (min 32 chars)"
$dbPort = Read-Host "Enter DB port (default 14330)"
$webPort = Read-Host "Enter Web port (default 3000)"

if ([string]::IsNullOrWhiteSpace($dbPort)) { $dbPort = "14330" }
if ([string]::IsNullOrWhiteSpace($webPort)) { $webPort = "3000" }

Set-EnvValue -FilePath ".env" -Key "SA_PASSWORD" -Value $saPassword
Set-EnvValue -FilePath ".env" -Key "DB_PORT" -Value $dbPort
Set-EnvValue -FilePath ".env" -Key "WEB_HTTP_PORT" -Value $webPort
Set-EnvValue -FilePath ".env" -Key "JWT_SECRET" -Value $jwtSecret

Write-Log "Building docker images..."
docker compose build

Write-Log "Starting services..."
docker compose up -d

Write-Log "Deployment completed"
Write-Log "Check status: docker compose ps"
Write-Log "Web: http://localhost:$webPort"
Write-Log "API health: docker exec ssms-api curl http://localhost:8080/health"

Pop-Location
