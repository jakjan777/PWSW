$ErrorActionPreference = "Stop"

$serviceName = "RustFileMonitor"
$exePath = Join-Path $PSScriptRoot "target\release\file_monitor_service.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "Brak pliku exe. Uruchom najpierw: .\build.ps1 build --release" -ForegroundColor Yellow
    exit 1
}

$existing = sc.exe query $serviceName 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Serwis juz istnieje. Zatrzymuje i usuwam..."
    sc.exe stop $serviceName | Out-Null
    Start-Sleep -Seconds 2
    sc.exe delete $serviceName | Out-Null
    Start-Sleep -Seconds 1
}

Write-Host "Instalacja serwisu: $serviceName"
Write-Host "Sciezka: $exePath"

sc.exe create $serviceName binPath= "`"$exePath`""
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

sc.exe description $serviceName "Monitoruje nowe pliki w C:\MonitoredFolder"
sc.exe start $serviceName
sc.exe query $serviceName

Write-Host ""
Write-Host "Logi: C:\MonitoredFolder\service.log"
Write-Host "Test: echo test > C:\MonitoredFolder\plik_testowy.txt"
