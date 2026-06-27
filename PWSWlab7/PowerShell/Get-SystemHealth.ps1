function Get-SystemHealth {
    [CmdletBinding()]
    [OutputType([PSCustomObject])]
    param (
        [Parameter(Mandatory = $false)]
        [ValidateSet('Quick', 'Full', 'Custom')]
        [string]$ScanType = 'Quick',

        [Parameter(Mandatory = $false)]
        [ValidateRange(1, 100)]
        [int]$CpuThreshold = 80,

        [Parameter(Mandatory = $false)]
        [ValidateRange(1, 100)]
        [int]$MemoryThreshold = 85,

        [Parameter(Mandatory = $false)]
        [ValidateRange(1, 100)]
        [int]$DiskThreshold = 90
    )

    begin {
        Write-Verbose "Rozpoczynam diagnostyke ($ScanType)"
        $script:scanStart = Get-Date
    }

    process {
        $cpu = (Get-CimInstance Win32_Processor |
            Measure-Object -Property LoadPercentage -Average).Average

        $os = Get-CimInstance Win32_OperatingSystem
        $memUsed = [math]::Round(
            ($os.TotalVisibleMemorySize - $os.FreePhysicalMemory) /
            $os.TotalVisibleMemorySize * 100, 1)

        $disks = Get-CimInstance Win32_LogicalDisk -Filter "DriveType=3" |
            ForEach-Object {
                [PSCustomObject]@{
                    Drive   = $_.DeviceID
                    SizeGB  = [math]::Round($_.Size / 1GB, 1)
                    FreeGB  = [math]::Round($_.FreeSpace / 1GB, 1)
                    UsedPct = [math]::Round(
                        ($_.Size - $_.FreeSpace) / $_.Size * 100, 1)
                }
            }

        [PSCustomObject]@{
            Timestamp     = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
            ComputerName  = $env:COMPUTERNAME
            ScanType      = $ScanType
            CpuPercent    = $cpu
            CpuStatus     = if ($cpu -gt $CpuThreshold) { 'WARNING' } else { 'OK' }
            MemoryPercent = $memUsed
            MemoryStatus  = if ($memUsed -gt $MemoryThreshold) { 'WARNING' } else { 'OK' }
            DiskInfo      = $disks
            DiskStatus    = if ($disks | Where-Object { $_.UsedPct -gt $DiskThreshold }) {
                'WARNING'
            }
            else { 'OK' }
        }
    }

    end {
        $elapsed = ((Get-Date) - $script:scanStart).TotalSeconds
        Write-Verbose "Diagnostyka zakonczona (czas: $([math]::Round($elapsed, 2)) s)"
    }
}

function Invoke-SafeOperation {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [scriptblock]$Operation,

        [string]$OperationName = 'Unnamed',
        [string]$LogFile = "$env:TEMP\operations.log"
    )

    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    try {
        Write-Verbose "Start: $OperationName"
        $result = & $Operation
        "$timestamp [OK] $OperationName" | Add-Content -Path $LogFile
        return $result
    }
    catch [System.UnauthorizedAccessException] {
        "$timestamp [BLAD] Brak uprawnien: $_" | Add-Content -Path $LogFile
        Write-Error "Brak uprawnien w '$OperationName'. Uruchom jako administrator."
    }
    catch {
        "$timestamp [BLAD] Nieoczekiwany: $_" | Add-Content -Path $LogFile
        Write-Error "Nieoczekiwany blad w '$OperationName': $_"
    }
}
