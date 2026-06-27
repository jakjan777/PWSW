# Cwiczenie 7.1 - porownanie kontenerow Linux i Windows
# Uruchom: powershell -ExecutionPolicy Bypass -File run-containers.ps1

$dir = $PSScriptRoot
$labDir = "C:\Users\jakja\source\repos\PWSWlab8\Exercise6_1"

Write-Host "=== Cwiczenie 7.1: Kontenery Windows vs Linux ===" -ForegroundColor Cyan
Write-Host ""

docker info *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker nie dziala. Uruchom Docker Desktop." -ForegroundColor Red
    exit 1
}

$osType = (docker info --format "{{.OSType}}" 2>$null).Trim()
Write-Host "Tryb Docker: $osType" -ForegroundColor Yellow
Write-Host ""

if ($osType -eq "linux") {
    Write-Host "--- Czesc A: Kontener Linux (lab08-app) ---" -ForegroundColor Yellow
    if (-not (docker images lab08-app -q)) {
        Write-Host "Buduje obraz Linux z Exercise6_1..."
        docker build -t lab08-app $labDir
    }

    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    docker run --rm lab08-app | Select-Object -First 6
    $sw.Stop()
    Write-Host "Czas startu Linux: $($sw.ElapsedMilliseconds) ms"
    Write-Host ""
    docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" lab08-app
    Write-Host ""
    Write-Host "--- Czesc B: Kontener Windows ---" -ForegroundColor Yellow
    Write-Host "Docker jest w trybie Linux. Przelacz na Windows containers:" -ForegroundColor Yellow
    Write-Host "  Docker Desktop -> ikona w tray -> Switch to Windows containers" -ForegroundColor Yellow
    Write-Host "  lub: & `"$env:ProgramFiles\Docker\Docker\DockerCli.exe`" -SwitchDaemon" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Po przelaczeniu uruchom ponownie ten skrypt." -ForegroundColor Yellow
    exit 0
}

Write-Host "--- Czesc B: Testy izolacji Windows ---" -ForegroundColor Yellow
docker run --isolation=process `
    mcr.microsoft.com/windows/nanoserver:ltsc2025 `
    cmd /c "echo Hello z Process isolation"
Write-Host ""
docker run --isolation=hyperv `
    mcr.microsoft.com/windows/nanoserver:ltsc2025 `
    cmd /c "echo Hello z Hyper-V isolation"
Write-Host ""

Write-Host "--- Budowanie obrazu Windows ---" -ForegroundColor Yellow
docker build -f "$dir\Dockerfile.windows" -t lab08-app-win $dir
Write-Host ""

Write-Host "--- Uruchomienie kontenera Windows ---" -ForegroundColor Yellow
$sw = [System.Diagnostics.Stopwatch]::StartNew()
docker run --rm lab08-app-win
$sw.Stop()
Write-Host "Czas startu Windows: $($sw.ElapsedMilliseconds) ms"
Write-Host ""

Write-Host "--- Porownanie rozmiarow ---" -ForegroundColor Yellow
docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" | Select-String "lab08-app"
