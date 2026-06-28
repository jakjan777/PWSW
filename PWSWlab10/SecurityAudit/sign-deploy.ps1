param(
    [string]$ScriptPath = "$PSScriptRoot\deploy.ps1"
)

$cert = New-SelfSignedCertificate `
    -Subject "CN=Lab10 Student Developer" `
    -Type CodeSigning `
    -KeyUsage DigitalSignature `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -HashAlgorithm SHA256 `
    -NotAfter (Get-Date).AddYears(2)

foreach ($storeName in @("Root", "TrustedPublisher")) {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, "CurrentUser")
    $store.Open("ReadWrite")
    $store.Add($cert)
    $store.Close()
}

Set-AuthenticodeSignature `
    -FilePath $ScriptPath `
    -Certificate $cert `
    -HashAlgorithm SHA256 | Out-Null

Get-AuthenticodeSignature $ScriptPath
