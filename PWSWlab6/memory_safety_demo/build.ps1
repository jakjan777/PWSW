$ErrorActionPreference = "Stop"

$sdkLib = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\Lib" -Recurse -Filter "kernel32.lib" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $sdkLib) {
    Write-Host "BLAD: Brakuje Windows SDK." -ForegroundColor Red
    exit 1
}

$vsCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\18\Insiders",
    "C:\Program Files\Microsoft Visual Studio\2022\Community"
)

$msvcRoot = $null
foreach ($vs in $vsCandidates) {
    $candidate = Get-ChildItem "$vs\VC\Tools\MSVC" -Directory -ErrorAction SilentlyContinue |
        Sort-Object Name -Descending |
        Select-Object -First 1 -ExpandProperty FullName
    if ($candidate -and ((Test-Path "$candidate\lib\onecore\x64\msvcrt.lib") -or (Test-Path "$candidate\lib\x64\msvcrt.lib"))) {
        $msvcRoot = $candidate
        break
    }
}

if (-not $msvcRoot) {
    Write-Error "Nie znaleziono MSVC."
}

$sdkVersionPath = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\Lib" -Directory |
    Sort-Object Name -Descending |
    Select-Object -First 1 -ExpandProperty FullName

$libDirs = @()
if (Test-Path "$msvcRoot\lib\x64") { $libDirs += "$msvcRoot\lib\x64" }
if (Test-Path "$msvcRoot\lib\onecore\x64") { $libDirs += "$msvcRoot\lib\onecore\x64" }
if ($sdkVersionPath) {
    if (Test-Path "$sdkVersionPath\um\x64") { $libDirs += "$sdkVersionPath\um\x64" }
    if (Test-Path "$sdkVersionPath\ucrt\x64") { $libDirs += "$sdkVersionPath\ucrt\x64" }
}

$binDir = Get-ChildItem "$msvcRoot\bin\Hostx64\x64", "$msvcRoot\bin\HostX64\x64" -ErrorAction SilentlyContinue |
    Select-Object -First 1 -ExpandProperty FullName

$env:LIB = ($libDirs -join ';')
$env:PATH = "$binDir;$env:PATH"

Set-Location $PSScriptRoot
cargo @args
