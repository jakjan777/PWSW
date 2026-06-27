#!/bin/bash
set -e

if [ -x "$HOME/.dotnet/dotnet" ]; then
  DOTNET="$HOME/.dotnet/dotnet"
elif [ -x /root/.dotnet/dotnet ]; then
  DOTNET="sudo /root/.dotnet/dotnet"
else
  echo "Brak dotnet. Zainstaluj dla swojego uzytkownika:"
  echo "curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0"
  exit 1
fi

cd /mnt/c/Users/jakja/source/repos/PWSWlab8/Exercise5_2
exec $DOTNET run --project Exercise5_2.csproj
