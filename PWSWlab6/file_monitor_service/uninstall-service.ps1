$ErrorActionPreference = "Stop"
$serviceName = "RustFileMonitor"

sc.exe stop $serviceName
Start-Sleep -Seconds 2
sc.exe delete $serviceName
sc.exe query $serviceName
