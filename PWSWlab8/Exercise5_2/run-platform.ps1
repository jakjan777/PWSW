# Cwiczenie 5.2 - uruchomienie na Windows i WSL
# Uruchom: powershell -ExecutionPolicy Bypass -File run-platform.ps1

$projectDir = $PSScriptRoot
$wslProjectDir = "/mnt/c/Users/jakja/source/repos/PWSWlab8/Exercise5_2"

Write-Host "=== Cwiczenie 5.2: Detekcja platformy ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "--- Windows ---" -ForegroundColor Yellow
dotnet run --project "$projectDir\Exercise5_2.csproj"
Write-Host ""
Write-Host "--- WSL Ubuntu 24.04 ---" -ForegroundColor Yellow
wsl -d Ubuntu-24.04 -u root -- /root/.dotnet/dotnet run --project "$wslProjectDir/Exercise5_2.csproj"
