function Get-DiskSpace {
    <#
    .SYNOPSIS
    Zwraca informacje o dyskach w formacie JSON (MCP-ready)
    #>
    Get-CimInstance Win32_LogicalDisk -Filter 'DriveType=3' |
        Select-Object DeviceID,
            @{ N = 'SizeGB'; E = { [math]::Round($_.Size / 1GB, 1) } },
            @{ N = 'FreeGB'; E = { [math]::Round($_.FreeSpace / 1GB, 1) } },
            @{ N = 'UsedPercent'; E = {
                    [math]::Round(($_.Size - $_.FreeSpace) / $_.Size * 100, 1)
                }
            } |
        ConvertTo-Json
}

function Get-RecentErrors {
    <#
    .SYNOPSIS
    Zwraca ostatnie bledy z dziennika System w formacie JSON (MCP-ready)
    #>
    [CmdletBinding()]
    param (
        [int]$Hours = 24
    )

    $start = (Get-Date).AddHours(-$Hours)
    $events = Get-WinEvent -FilterHashtable @{
        LogName   = 'System'
        Level     = 2
        StartTime = $start
    } -MaxEvents 20 -ErrorAction SilentlyContinue |
        Select-Object TimeCreated, ProviderName, Id,
            @{ N = 'Message'; E = { $_.Message.Split([Environment]::NewLine)[0] } }

    @{
        Hours  = $Hours
        Count  = @($events).Count
        Events = $events
    } | ConvertTo-Json -Depth 4
}

function Get-ServiceStatus {
    <#
    .SYNOPSIS
    Sprawdza status serwisu Windows (MCP-callable)
    #>
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string]$ServiceName
    )

    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    if ($service) {
        [PSCustomObject]@{
            Name        = $service.Name
            DisplayName = $service.DisplayName
            Status      = $service.Status.ToString()
            StartType   = $service.StartType.ToString()
        } | ConvertTo-Json
    }
    else {
        @{ Error = "Serwis '$ServiceName' nie istnieje" } | ConvertTo-Json
    }
}

function Invoke-SystemDiagnostic {
    <#
    .SYNOPSIS
    Wykonuje calosciowa diagnostyke systemu i zwraca JSON (MCP-ready)
    #>
    [CmdletBinding()]
    param (
        [int]$ErrorHours = 24
    )

    $cpu = (Get-CimInstance Win32_Processor |
        Measure-Object -Property LoadPercentage -Average).Average

    $os = Get-CimInstance Win32_OperatingSystem
    $memUsed = [math]::Round(
        ($os.TotalVisibleMemorySize - $os.FreePhysicalMemory) /
        $os.TotalVisibleMemorySize * 100, 1)

    $disks = Get-CimInstance Win32_LogicalDisk -Filter 'DriveType=3' |
        Select-Object DeviceID,
            @{ N = 'SizeGB'; E = { [math]::Round($_.Size / 1GB, 1) } },
            @{ N = 'FreeGB'; E = { [math]::Round($_.FreeSpace / 1GB, 1) } },
            @{ N = 'UsedPercent'; E = {
                    [math]::Round(($_.Size - $_.FreeSpace) / $_.Size * 100, 1)
                }
            }

    $errorsJson = Get-RecentErrors -Hours $ErrorHours | ConvertFrom-Json

    [PSCustomObject]@{
        Timestamp     = Get-Date -Format 'o'
        ComputerName  = $env:COMPUTERNAME
        CpuPercent    = $cpu
        MemoryPercent = $memUsed
        Disks         = @($disks)
        ErrorSummary  = @{
            Hours = $ErrorHours
            Count = $errorsJson.Count
        }
    } | ConvertTo-Json -Depth 4
}

$fleetScript = Join-Path $PSScriptRoot 'Invoke-FleetDiagnostic.ps1'
if (Test-Path $fleetScript) {
    . $fleetScript
}

Export-ModuleMember -Function @(
    'Get-DiskSpace'
    'Get-RecentErrors'
    'Get-ServiceStatus'
    'Invoke-SystemDiagnostic'
    'Invoke-FleetDiagnostic'
)
