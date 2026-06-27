# Cwiczenie 6.2 - Docker Compose
# Wymaga uruchomionego Docker Desktop
# Uruchom: powershell -ExecutionPolicy Bypass -File run-compose.ps1

$dir = $PSScriptRoot

Write-Host "=== Cwiczenie 6.2: Docker Compose ===" -ForegroundColor Cyan
Write-Host ""

docker info *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker nie dziala. Uruchom Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "--- Uruchamianie stosu ---" -ForegroundColor Yellow
docker compose -f "$dir\docker-compose.yml" up -d --build
Write-Host ""

Write-Host "--- Status serwisow ---" -ForegroundColor Yellow
docker compose -f "$dir\docker-compose.yml" ps
Write-Host ""

Write-Host "--- Logi web ---" -ForegroundColor Yellow
docker compose -f "$dir\docker-compose.yml" logs web --tail 15
Write-Host ""

Write-Host "Aplikacja:    http://localhost:5000" -ForegroundColor Green
Write-Host "Adminer:      http://localhost:8080  (Server: db, User: postgres, Pass: devpass)" -ForegroundColor Green
Write-Host ""
Write-Host "Test API:     curl http://localhost:5000/api/status" -ForegroundColor Green
Write-Host "Zatrzymanie:  docker compose -f `"$dir\docker-compose.yml`" down -v" -ForegroundColor Yellow
