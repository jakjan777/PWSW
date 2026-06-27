#!/bin/bash
set -e

ROOT="/mnt/c/Users/jakja/source/repos/PWSWlab8"
cd "$ROOT"

if command -v dotnet >/dev/null 2>&1; then
  DOTNET=dotnet
elif [ -x "$HOME/.dotnet/dotnet" ]; then
  DOTNET="$HOME/.dotnet/dotnet"
  export PATH="$HOME/.dotnet:$PATH"
elif [ -x /root/.dotnet/dotnet ]; then
  DOTNET="sudo /root/.dotnet/dotnet"
else
  echo "Brak dotnet w WSL. Zainstaluj jednorazowo:"
  echo "curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0"
  echo "echo 'export PATH=\"\$HOME/.dotnet:\$PATH\"' >> ~/.bashrc"
  echo "export PATH=\"\$HOME/.dotnet:\$PATH\""
  exit 1
fi

echo "=== Cwiczenie 7.2: Cross-Ecosystem Build ==="
echo "dotnet: $($DOTNET --version)"

echo "[1/4] Budowanie .NET w Linux..."
$DOTNET build PWSWlab8.sln -c Release

echo "[2/4] Uruchamianie testow..."
$DOTNET test PWSWlab8.sln -c Release --no-build

echo "[3/4] Budowanie obrazu Docker..."
docker build -t lab08-app:latest -f Exercise6_1/Dockerfile Exercise6_1

echo "[4/4] Gotowe!"
powershell.exe -Command "Write-Host 'Build zakonczony pomyslnie' -ForegroundColor Green" \
  2>/dev/null || echo "Build zakonczony pomyslnie"

echo "=== Build zakonczony ==="
