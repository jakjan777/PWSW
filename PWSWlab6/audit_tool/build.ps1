$ErrorActionPreference = "Stop"

function Find-SdkLibVersion {
    $libRoot = "C:\Program Files (x86)\Windows Kits\10\Lib"
    if (-not (Test-Path $libRoot)) { return $null }
    return Get-ChildItem $libRoot -Directory |
        Sort-Object Name -Descending |
        Select-Object -First 1 -ExpandProperty FullName
}

function Find-MsvcRoot {
    param([string[]]$VsPaths)
    foreach ($vs in $VsPaths) {
        $msvcRoot = Get-ChildItem "$vs\VC\Tools\MSVC" -Directory -ErrorAction SilentlyContinue |
            Sort-Object Name -Descending |
            Select-Object -First 1 -ExpandProperty FullName
        if ($msvcRoot) { return $msvcRoot }
    }
    return $null
}

function Test-MsvcLibs {
    param([string]$MsvcRoot)
    @(
        "$MsvcRoot\lib\x64\msvcrt.lib",
        "$MsvcRoot\lib\onecore\x64\msvcrt.lib"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1
}

$sdkLib = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\Lib" -Recurse -Filter "kernel32.lib" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $sdkLib) {
    Write-Host "BLAD: Brakuje Windows SDK. Zainstaluj: winget install Microsoft.WindowsSDK.10.0.22621" -ForegroundColor Red
    exit 1
}

$vsCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\18\Insiders",
    "C:\Program Files\Microsoft Visual Studio\2022\Community"
)

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path $vswhere) {
    $fromWhere = & $vswhere -prerelease -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath 2>$null
    if ($fromWhere) { $vsCandidates = @($fromWhere) + $vsCandidates | Select-Object -Unique }
}

$msvcRoot = Find-MsvcRoot -VsPaths $vsCandidates
if (-not $msvcRoot) {
    Write-Error "Nie znaleziono MSVC. Uruchom install-cpp.ps1 jako Administrator."
}

$msvcLib = Test-MsvcLibs -MsvcRoot $msvcRoot
if (-not $msvcLib) {
    Write-Error "Brakuje msvcrt.lib. Uruchom install-cpp.ps1 jako Administrator."
}

$sdkVersionPath = Find-SdkLibVersion
$libDirs = @()
if (Test-Path "$msvcRoot\lib\x64") { $libDirs += "$msvcRoot\lib\x64" }
if (Test-Path "$msvcRoot\lib\onecore\x64") { $libDirs += "$msvcRoot\lib\onecore\x64" }
if ($sdkVersionPath) {
    if (Test-Path "$sdkVersionPath\um\x64") { $libDirs += "$sdkVersionPath\um\x64" }
    if (Test-Path "$sdkVersionPath\ucrt\x64") { $libDirs += "$sdkVersionPath\ucrt\x64" }
}

$binDir = Get-ChildItem "$msvcRoot\bin\Hostx64\x64", "$msvcRoot\bin\HostX64\x64" -ErrorAction SilentlyContinue | Select-Object -First 1 -ExpandProperty FullName
if (-not $binDir) {
    Write-Error "Nie znaleziono folderu bin MSVC."
}

$env:LIB = ($libDirs -join ';')
$env:PATH = "$binDir;$env:PATH"

Write-Host "Uzywam MSVC: $msvcRoot"
Write-Host "LIB: $env:LIB"

Set-Location $PSScriptRoot
cargo @args
