#!/bin/bash
set -e
echo "=== Instalacja Docker Engine w WSL ==="
sudo apt-get update
sudo apt-get install -y docker.io docker-compose-v2
sudo usermod -aG docker "$USER"
sudo service docker start
docker --version
docker run hello-world
