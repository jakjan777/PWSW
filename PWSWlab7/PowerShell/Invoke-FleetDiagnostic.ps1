function Invoke-FleetDiagnostic {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory)]
        [string[]]$Servers,

        [int]$ThrottleLimit = 4
    )

    if ($PSVersionTable.PSVersion.Major -ge 7) {
        $results = $Servers | ForEach-Object -Parallel {
            $server = $_
            try {
                [PSCustomObject]@{
                    Server    = $server
                    Status    = 'Online'
                    CpuPct    = Get-Random -Minimum 5 -Maximum 95
                    MemPct    = Get-Random -Minimum 20 -Maximum 90
                    DiskPct   = Get-Random -Minimum 30 -Maximum 95
                    Uptime    = Get-Random -Minimum 1 -Maximum 365
                    Timestamp = Get-Date -Format 'o'
                }
            }
            catch {
                [PSCustomObject]@{
                    Server    = $server
                    Status    = 'Error'
                    Error     = $_.Exception.Message
                    Timestamp = Get-Date -Format 'o'
                }
            }
        } -ThrottleLimit $ThrottleLimit
    }
    else {
        $results = foreach ($server in $Servers) {
            try {
                [PSCustomObject]@{
                    Server    = $server
                    Status    = 'Online'
                    CpuPct    = Get-Random -Minimum 5 -Maximum 95
                    MemPct    = Get-Random -Minimum 20 -Maximum 90
                    DiskPct   = Get-Random -Minimum 30 -Maximum 95
                    Uptime    = Get-Random -Minimum 1 -Maximum 365
                    Timestamp = Get-Date -Format 'o'
                }
            }
            catch {
                [PSCustomObject]@{
                    Server    = $server
                    Status    = 'Error'
                    Error     = $_.Exception.Message
                    Timestamp = Get-Date -Format 'o'
                }
            }
        }
    }

    $results | ConvertTo-Json -Depth 3
}
