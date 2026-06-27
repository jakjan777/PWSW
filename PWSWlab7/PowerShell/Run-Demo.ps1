. "$PSScriptRoot\Get-SystemHealth.ps1"

Write-Host "=== Diagnostyka przez Invoke-SafeOperation ==="
$health = Invoke-SafeOperation -OperationName 'System Health Check' -Operation {
    Get-SystemHealth -ScanType Full -Verbose
}
$health | Format-List

Write-Host "`n=== Log operacji ==="
Get-Content "$env:TEMP\operations.log" -Tail 3

Write-Host "`n=== Test walidacji ScanType (oczekiwany blad) ==="
try {
    Get-SystemHealth -ScanType Invalid -ErrorAction Stop
}
catch {
    Write-Host $_.Exception.Message
}

Write-Host "`n=== Test walidacji CpuThreshold (oczekiwany blad) ==="
try {
    Get-SystemHealth -CpuThreshold 150 -ErrorAction Stop
}
catch {
    Write-Host $_.Exception.Message
}
