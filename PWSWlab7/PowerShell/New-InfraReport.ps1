param (
    [string[]]$Servers = @('SRV-WEB01', 'SRV-DB01', 'SRV-APP01'),
    [string]$ReportPath = "$env:TEMP\infra-report.json"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\Get-SystemHealth.ps1"
. "$PSScriptRoot\Invoke-FleetDiagnostic.ps1"

$localHealth = Invoke-SafeOperation -OperationName 'Local System Health' -Operation {
    Get-SystemHealth -ScanType Full -Verbose
}

$fleetRaw = Invoke-SafeOperation -OperationName 'Fleet Diagnostic' -Operation {
    Invoke-FleetDiagnostic -Servers $Servers -ThrottleLimit 4
}

$fleetResults = @()
if ($null -ne $fleetRaw -and $fleetRaw.ToString().Trim().Length -gt 0) {
    $parsed = $fleetRaw | ConvertFrom-Json
    $fleetResults = @($parsed)
}

$localOk = 0
$localWarning = 0
if ($null -ne $localHealth) {
    foreach ($status in @($localHealth.CpuStatus, $localHealth.MemoryStatus, $localHealth.DiskStatus)) {
        if ($status -eq 'OK') { $localOk++ }
        elseif ($status -eq 'WARNING') { $localWarning++ }
    }
}

$fleetOk = @($fleetResults | Where-Object { $_.Status -eq 'Online' }).Count
$fleetWarning = @($fleetResults | Where-Object { $_.Status -ne 'Online' }).Count

$report = [PSCustomObject]@{
    Timestamp    = Get-Date -Format 'o'
    LocalHealth  = $localHealth
    FleetResults = $fleetResults
    Summary      = [PSCustomObject]@{
        LocalOk      = $localOk
        LocalWarning = $localWarning
        FleetOk      = $fleetOk
        FleetWarning = $fleetWarning
        TotalOk      = $localOk + $fleetOk
        TotalWarning = $localWarning + $fleetWarning
    }
}

$report | ConvertTo-Json -Depth 4 | Set-Content -Path $ReportPath -Encoding utf8
Write-Host "Raport zapisany: $ReportPath"

Write-Host "`n=== Podsumowanie ==="
$report.Summary | Format-Table -AutoSize

if ($fleetResults.Count -gt 0) {
    Write-Host "=== Flota serwerow ==="
    $fleetResults | Format-Table Server, Status, CpuPct, MemPct, DiskPct -AutoSize
}

if ($null -ne $localHealth) {
    Write-Host "=== Stan lokalny ==="
    $localHealth | Select-Object ComputerName, ScanType, CpuStatus, MemoryStatus, DiskStatus |
        Format-Table -AutoSize
}
