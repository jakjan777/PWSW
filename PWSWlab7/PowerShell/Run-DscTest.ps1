$ErrorActionPreference = 'Stop'
$config = Join-Path $PSScriptRoot 'dev-machine.dsc.yaml'

Write-Host "=== dsc --version ==="
dsc --version

Write-Host "`n=== dsc config test (bez zmian) ==="
dsc config test --file $config -o yaml

Write-Host "`n=== dsc config get (aktualny stan) ==="
dsc config get --file $config -o yaml

Write-Host "`n=== Zastosowanie (administrator, opcjonalnie) ==="
Write-Host "dsc config set --file `"$config`""
