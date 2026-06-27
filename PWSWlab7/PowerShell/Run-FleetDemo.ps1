if ($PSVersionTable.PSVersion.Major -lt 7) {
    Write-Error 'Invoke-FleetDiagnostic wymaga PowerShell 7+ (ForEach-Object -Parallel). Zainstaluj: winget install Microsoft.PowerShell'
    exit 1
}

. "$PSScriptRoot\Invoke-FleetDiagnostic.ps1"

$servers = @('SRV-WEB01', 'SRV-DB01', 'SRV-APP01', 'SRV-CACHE01')

Write-Host "=== Invoke-FleetDiagnostic (ThrottleLimit 4) ==="
$output = Invoke-FleetDiagnostic -Servers $servers -ThrottleLimit 4
Write-Host $output

Write-Host "`n=== Obiekty po konwersji z JSON ==="
$output | ConvertFrom-Json | Format-Table Server, Status, CpuPct, MemPct, DiskPct -AutoSize
