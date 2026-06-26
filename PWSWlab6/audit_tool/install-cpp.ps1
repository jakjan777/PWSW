# Uruchom ten skrypt jako Administrator (PPM -> Uruchom jako administrator)
$ErrorActionPreference = "Stop"

$config = Join-Path $PSScriptRoot "vs-cpp-config.json"
$setup = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\setup.exe"

if (-not (Test-Path $setup)) {
    Write-Error "Nie znaleziono Visual Studio Installer."
}

Write-Host "Instalacja: Desktop development with C++ dla VS 2026 Insiders..."
Write-Host "To moze potrwac kilka minut. Nie zamykaj okna."
Write-Host ""

& $setup modify --passive --norestart --installWhileDownloading --config $config
$code = $LASTEXITCODE

if ($code -eq 0) {
    Write-Host ""
    Write-Host "Gotowe. Teraz uruchom: .\build.ps1 run" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Instalator zwrocil kod: $code" -ForegroundColor Yellow
    Write-Host "Jesli instalacja trwa w tle, poczekaj i sprawdz ponownie build.ps1 run"
}
