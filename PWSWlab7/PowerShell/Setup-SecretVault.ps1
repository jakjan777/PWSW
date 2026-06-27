[CmdletBinding()]
param (
    [switch]$NonInteractive
)

$ErrorActionPreference = 'Stop'

$modules = @(
    'Microsoft.PowerShell.SecretManagement'
    'Microsoft.PowerShell.SecretStore'
)

foreach ($name in $modules) {
    if (-not (Get-Module -ListAvailable -Name $name)) {
        Write-Host "Instalowanie modulu $name..."
        Install-Module $name -Force -Scope CurrentUser -AllowClobber
    }
}

Import-Module Microsoft.PowerShell.SecretManagement
Import-Module Microsoft.PowerShell.SecretStore

Set-SecretStoreConfiguration -Authentication None -Interaction None -ErrorAction SilentlyContinue

$existingVault = Get-SecretVault -Name 'LabVault' -ErrorAction SilentlyContinue
if ($existingVault) {
    Unregister-SecretVault -Name 'LabVault' -ErrorAction SilentlyContinue
}

Register-SecretVault -Name 'LabVault' -ModuleName 'Microsoft.PowerShell.SecretStore'
Write-Host 'Zarejestrowano vault LabVault'

if ($NonInteractive) {
    $cred = [PSCredential]::new(
        'LabService',
        (ConvertTo-SecureString 'DemoPassword123!' -AsPlainText -Force))
}
else {
    $cred = Get-Credential -Message 'Konto serwisowe'
}

Set-Secret -Name 'ServiceAccount' -Secret $cred -Vault 'LabVault'
$stored = Get-Secret -Name 'ServiceAccount' -Vault 'LabVault'
Write-Host "Uzytkownik: $($stored.UserName)"
