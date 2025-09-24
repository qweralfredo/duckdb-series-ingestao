#!/bin/bash

# Script para executar BankingApi em modo desenvolvimento

set -e

echo "?? BankingApi - Ambiente de Desenvolvimento"
echo "=========================================="

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "? Docker não encontrado. Por favor, instale o Docker."
    exit 1
fi

# Navegar para o diretório raiz do projeto (um nível acima de scripts)
cd "$(dirname "$0")/.."

echo "?? Diretório atual: $(pwd)"
echo "?? Verificando arquivos Docker Compose..."

if [ ! -f "docker-compose.dev.yml" ]; then
    echo "? Arquivo docker-compose.dev.yml não encontrado!"
    echo "?? Arquivos disponíveis:"
    ls -la | grep docker-compose
    exit 1
fi

echo "?? Iniciando ambiente de desenvolvimento..."
echo "?? Arquivo: docker-compose.dev.yml"
echo ""

docker-compose -f docker-compose.dev.yml up --build

echo ""
echo "?? Parando ambiente de desenvolvimento..."
echo "Use Ctrl+C para parar os serviços"