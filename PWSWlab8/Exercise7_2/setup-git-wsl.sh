#!/bin/bash
echo "=== Cwiczenie 7.2: Git cross-platform (WSL) ==="
git config --global core.autocrlf input
echo "Ustawiono: core.autocrlf = input (Linux/WSL)"
git config --global --get core.autocrlf
