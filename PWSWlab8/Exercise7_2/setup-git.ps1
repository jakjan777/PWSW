# Cwiczenie 7.2 - konfiguracja Git na Windows
# Uruchom w katalogu repo: powershell -ExecutionPolicy Bypass -File Exercise7_2/setup-git.ps1

$root = Split-Path $PSScriptRoot -Parent
Set-Location $root

Write-Host "=== Cwiczenie 7.2: Git cross-platform (Windows) ===" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path ".git")) {
    git init
    Write-Host "Zainicjowano repozytorium git." -ForegroundColor Yellow
}

git config core.autocrlf true
Write-Host "Ustawiono: core.autocrlf = true (Windows)" -ForegroundColor Green
Write-Host ""

Write-Host "--- git config ---" -ForegroundColor Yellow
git config --get core.autocrlf
Write-Host ""

Write-Host "--- .gitattributes ---" -ForegroundColor Yellow
Get-Content .gitattributes
Write-Host ""

Write-Host "W WSL uruchom: bash Exercise7_2/setup-git-wsl.sh" -ForegroundColor Yellow
