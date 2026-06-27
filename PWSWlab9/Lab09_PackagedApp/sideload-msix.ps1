# C:\Users\jakja\source\repos\PWSWlab9\Lab09_PackagedApp\sideload-msix.ps1
$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$msixPath = Join-Path $projectRoot "bin\Release\Lab09_PackagedApp.msix"
$layoutDir = Join-Path $projectRoot "msix-layout"
$manifestPath = Join-Path $layoutDir "AppxManifest.xml"
$signtool = "C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe"

if (-not (Test-Path $manifestPath)) {
    Write-Host "Brak msix-layout. Uruchamiam build-msix.ps1..."
    & (Join-Path $projectRoot "build-msix.ps1")
}

$cert = Get-ChildItem Cert:\CurrentUser\My |
    Where-Object { $_.Subject -eq "CN=Student" } |
    Select-Object -First 1

if ($cert) {
    foreach ($storeName in @("TrustedPeople", "Root", "TrustedPublisher")) {
        $existing = Get-ChildItem "Cert:\CurrentUser\$storeName" -ErrorAction SilentlyContinue |
            Where-Object { $_.Thumbprint -eq $cert.Thumbprint }
        if (-not $existing) {
            Write-Host "Dodawanie certyfikatu do CurrentUser\$storeName..."
            $store = New-Object System.Security.Cryptography.X509Certificates.X509Store(
                $storeName, "CurrentUser")
            $store.Open("ReadWrite")
            $store.Add($cert)
            $store.Close()
        }
    }
}

$existingPkg = Get-AppxPackage -Name "Lab09.PackagedApp" -ErrorAction SilentlyContinue
if ($existingPkg) {
    Write-Host "Odinstalowywanie poprzedniej wersji..."
    Remove-AppxPackage -Package $existingPkg.PackageFullName
}

Write-Host "`n--- Sideloading: rejestracja z msix-layout (tryb deweloperski) ---"
Add-AppxPackage -Register -Path $manifestPath
Write-Host "Instalacja zakonczona pomyslnie."

Write-Host "`n--- Zainstalowane pakiety Lab09 ---"
Get-AppxPackage | Where-Object { $_.Name -like "Lab09*" } |
    Format-List Name, Version, InstallLocation

if (Test-Path $msixPath) {
    Write-Host "`n--- Opcjonalnie: podpis pliku .msix ---"
    if ($cert -and (Test-Path $signtool)) {
        & $signtool sign /fd SHA256 /sha1 $cert.Thumbprint $msixPath
        Write-Host "Podpisano: $msixPath"
        Write-Host "Uwaga: Add-AppxPackage na .msix moze wymagac certyfikatu w LocalMachine\Root (admin)."
    }
}
