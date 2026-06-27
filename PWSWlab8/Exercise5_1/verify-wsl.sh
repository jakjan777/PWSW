#!/bin/bash
echo "=== Cwiczenie 5.1: Interoperacyjnosc filesystemow ==="
echo ""
echo "--- Informacje o systemie ---"
cat /etc/os-release | head -3
uname -a
echo ""
echo "--- Dostep do dyskow Windows ---"
ls /mnt/c/Users/ | head -5
echo ""
echo "--- Uruchomienie programu Windows z Linuxa ---"
cmd.exe /c "echo Hello z Windows"
echo ""
echo "=== Koniec weryfikacji ==="
