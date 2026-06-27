# Cwiczenie 5.1 - weryfikacja WSL z PowerShell
# Uruchom: powershell -ExecutionPolicy Bypass -File verify-wsl.ps1

Write-Host "=== Cwiczenie 5.1: WSL 2 i konfiguracja zasobow ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "--- Status WSL ---" -ForegroundColor Yellow
wsl --status
Write-Host ""

Write-Host "--- Zainstalowane dystrybucje ---" -ForegroundColor Yellow
wsl --list --verbose
Write-Host ""

Write-Host "--- Plik .wslconfig ---" -ForegroundColor Yellow
Get-Content "$env:USERPROFILE\.wslconfig"
Write-Host ""

Write-Host "--- Interop: komenda Linux z PowerShell ---" -ForegroundColor Yellow
wsl cat /etc/os-release 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Brak dystrybucji Linux. Zainstaluj: wsl --install -d Ubuntu-24.04" -ForegroundColor Red
}
Write-Host ""

Write-Host "--- Interop: dostep do plikow WSL przez UNC ---" -ForegroundColor Yellow
Get-ChildItem "\\wsl$\" -ErrorAction SilentlyContinue | Select-Object Name

Write-Host ""
Write-Host "=== Koniec weryfikacji ===" -ForegroundColor Cyan
