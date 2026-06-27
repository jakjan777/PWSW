# Cwiczenie 6.1 - budowanie i uruchomienie kontenera
# Wymaga uruchomionego Docker Desktop
# Uruchom: powershell -ExecutionPolicy Bypass -File run-docker.ps1

$dir = $PSScriptRoot

Write-Host "=== Cwiczenie 6.1: Multi-stage Dockerfile ===" -ForegroundColor Cyan
Write-Host ""

docker info *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker nie dziala. Uruchom Docker Desktop i poczekaj az status bedzie Running." -ForegroundColor Red
    Write-Host "Nastepnie uruchom ten skrypt ponownie." -ForegroundColor Yellow
    exit 1
}

Write-Host "--- Budowanie obrazu ---" -ForegroundColor Yellow
docker build -t lab08-app $dir
Write-Host ""

Write-Host "--- Uruchomienie kontenera ---" -ForegroundColor Yellow
docker run --rm lab08-app
Write-Host ""

Write-Host "--- Rozmiar obrazu ---" -ForegroundColor Yellow
docker images lab08-app
Write-Host ""

Write-Host "--- Historia warstw ---" -ForegroundColor Yellow
docker history lab08-app
