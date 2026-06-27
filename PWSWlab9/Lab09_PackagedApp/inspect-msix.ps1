# C:\Users\jakja\source\repos\PWSWlab9\Lab09_PackagedApp\inspect-msix.ps1
$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$msixPath = Join-Path $projectRoot "bin\Release\Lab09_PackagedApp.msix"
$zipPath = Join-Path $projectRoot "bin\Release\Lab09_PackagedApp.zip"
$extractDir = Join-Path $projectRoot "msix-contents"

if (-not (Test-Path $msixPath)) {
    throw "Brak pakietu MSIX. Uruchom najpierw build-msix.ps1"
}

Copy-Item $msixPath $zipPath -Force
if (Test-Path $extractDir) { Remove-Item $extractDir -Recurse -Force }
Expand-Archive $zipPath $extractDir -Force

Write-Host "=== Zawartosc pakietu MSIX ==="
Get-ChildItem $extractDir -Recurse |
    Select-Object @{N="Name";E={$_.Name}}, Length, Directory |
    Format-Table -AutoSize

$blockMap = Join-Path $extractDir "AppxBlockMap.xml"
if (Test-Path $blockMap) {
    $content = Get-Content $blockMap -Raw
    $blockCount = ([regex]::Matches($content, "<Block ")).Count
    Write-Host "Liczba blokow w AppxBlockMap.xml: $blockCount"
}
