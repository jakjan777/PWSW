# C:\Users\jakja\source\repos\PWSWlab9\Lab09_PackagedApp\build-msix.ps1
$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$makeAppx = "C:\Program Files (x86)\Windows Kits\10\App Certification Kit\makeappx.exe"

Write-Host "=== Budowanie pakietu MSIX ==="

if (-not (Test-Path $makeAppx)) {
    throw "Nie znaleziono makeappx.exe: $makeAppx"
}

$cert = Get-ChildItem Cert:\CurrentUser\My |
    Where-Object { $_.Subject -eq "CN=Student" } |
    Select-Object -First 1

if (-not $cert) {
    Write-Host "Tworzenie certyfikatu self-signed CN=Student..."
    $cert = New-SelfSignedCertificate `
        -Type Custom `
        -Subject "CN=Student" `
        -KeyUsage DigitalSignature `
        -FriendlyName "Lab09 Dev Cert" `
        -CertStoreLocation "Cert:\CurrentUser\My" `
        -TextExtension @(
            "2.5.29.37={text}1.3.6.1.5.5.7.3.3",
            "2.5.29.19={text}"
        )
}

Write-Host "Certyfikat thumbprint: $($cert.Thumbprint)"

Push-Location $projectRoot
try {
    dotnet publish -c Release -r win-x64 -p:EnableMsixTooling=false

    $publishDir = Join-Path $projectRoot "bin\Release\net10.0-windows10.0.26100.0\win-x64\publish"
    $layoutDir = Join-Path $projectRoot "msix-layout"
    $outputMsix = Join-Path $projectRoot "bin\Release\Lab09_PackagedApp.msix"

    if (Test-Path $layoutDir) { Remove-Item $layoutDir -Recurse -Force }
    New-Item -ItemType Directory -Path $layoutDir | Out-Null

    Copy-Item "$publishDir\*" $layoutDir -Recurse -Force
    Copy-Item "$projectRoot\Assets" $layoutDir -Recurse -Force

    $manifest = @"
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">
  <Identity Name="Lab09.PackagedApp" Publisher="CN=Student" Version="1.0.0.0" />
  <Properties>
    <DisplayName>Lab09 Packaged App</DisplayName>
    <PublisherDisplayName>Student Developer</PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
  </Properties>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.26100.0" />
  </Dependencies>
  <Applications>
    <Application Id="App" Executable="Lab09_PackagedApp.exe" EntryPoint="Windows.FullTrustApplication">
      <uap:VisualElements
        DisplayName="Lab09 Packaged App"
        Description="Aplikacja spakowana jako MSIX"
        BackgroundColor="transparent"
        Square150x150Logo="Assets\Square150x150Logo.scale-200.png"
        Square44x44Logo="Assets\Square44x44Logo.scale-200.png">
        <uap:DefaultTile Wide310x150Logo="Assets\Wide310x150Logo.scale-200.png" />
      </uap:VisualElements>
    </Application>
  </Applications>
  <Capabilities>
    <rescap:Capability Name="runFullTrust" />
  </Capabilities>
</Package>
"@

    Set-Content -Path (Join-Path $layoutDir "AppxManifest.xml") -Value $manifest -Encoding UTF8

    if (Test-Path $outputMsix) { Remove-Item $outputMsix -Force }
    & $makeAppx pack /d $layoutDir /p $outputMsix /o | Write-Host

    Write-Host "Utworzono pakiet: $outputMsix"
    Write-Host "Rozmiar: $((Get-Item $outputMsix).Length) bajtow"
}
finally {
    Pop-Location
}
