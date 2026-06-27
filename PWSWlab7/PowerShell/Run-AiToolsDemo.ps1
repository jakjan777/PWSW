param (
    [switch]$SetupSecrets
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host '=== Import modulu ai-tools ==='
Import-Module "$PSScriptRoot\ai-tools.psm1" -Force

if ($SetupSecrets) {
    Write-Host "`n=== Konfiguracja SecretManagement (interaktywnie) ==="
    & "$PSScriptRoot\Setup-SecretVault.ps1"
}

Write-Host "`n--- Get-DiskSpace ---"
Get-DiskSpace

Write-Host "`n--- Get-ServiceStatus (wuauserv) ---"
Get-ServiceStatus -ServiceName 'wuauserv'

Write-Host "`n--- Get-RecentErrors (24h) ---"
Get-RecentErrors -Hours 24

Write-Host "`n--- Invoke-SystemDiagnostic ---"
Invoke-SystemDiagnostic -ErrorHours 24

if ($PSVersionTable.PSVersion.Major -ge 7) {
    Write-Host "`n--- Invoke-FleetDiagnostic ---"
    Invoke-FleetDiagnostic -Servers @('SRV-WEB01', 'SRV-DB01') -ThrottleLimit 2
}

Write-Host "`n=== Eksportowane funkcje ==="
Get-Command -Module ai-tools | Select-Object Name | Format-Table -HideTableHeaders

Write-Host "SecretManagement (recznie): .\Setup-SecretVault.ps1"
