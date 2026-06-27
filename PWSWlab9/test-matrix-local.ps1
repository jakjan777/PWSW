# C:\Users\jakja\source\repos\PWSWlab9\test-matrix-local.ps1
$ErrorActionPreference = "Stop"
$project = "C:\Users\jakja\source\repos\PWSWlab9\Lab09_PackagedApp\Lab09_PackagedApp.csproj"

foreach ($arch in @("x64", "arm64")) {
    $out = "C:\Users\jakja\source\repos\PWSWlab9\publish-$arch"
    if (Test-Path $out) { Remove-Item $out -Recurse -Force }
    Write-Host "=== Publish win-$arch ==="
    dotnet publish $project -c Release -r "win-$arch" -p:EnableMsixTooling=false -o $out
    $exe = Join-Path $out "Lab09_PackagedApp.exe"
    Write-Host "  OK: $(Test-Path $exe) $exe"
}
