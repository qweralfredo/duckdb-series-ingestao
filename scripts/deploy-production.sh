#!/bin/bash

# Script para construir e executar BankingApi em produção

set -e

echo "?? BankingApi - Script de Deploy Produção"
echo "========================================"

# Verificar se Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "? Docker não encontrado. Por favor, instale o Docker."
    exit 1
fi

# Verificar se Docker Compose está instalado
if ! command -v docker-compose &> /dev/null; then
    echo "? Docker Compose não encontrado. Por favor, instale o Docker Compose."
    exit 1
fi

# Navegar para o diretório raiz do projeto (um nível acima de scripts)
cd "$(dirname "$0")/.."

echo "?? Diretório atual: $(pwd)"
echo "?? Verificando arquivos Docker Compose..."

if [ ! -f "docker-compose.yml" ]; then
    echo "? Arquivo docker-compose.yml não encontrado!"
    echo "?? Arquivos disponíveis:"
    ls -la | grep docker-compose
    exit 1
fi

echo "?? Construindo imagem da BankingApi..."
docker build -t banking-api:latest -f BankingApi/Dockerfile .

echo "?? Iniciando serviços em produção..."
docker-compose up -d

echo "? Aguardando serviços ficarem prontos..."
sleep 30

echo "?? Verificando status dos serviços..."
docker-compose ps

echo "?? Verificando logs da aplicação..."
docker-compose logs --tail=20 banking-api

echo ""
echo "? Deploy concluído!"
echo "?? Aplicação disponível em: http://localhost:5000"
echo "?? Kafka UI disponível em: http://localhost:8080"
echo "??? MinIO Console disponível em: http://localhost:9001"
echo "?? Swagger disponível em: http://localhost:5000/swagger"
echo ""
echo "?? Comandos úteis:"
echo "  Ver logs: docker-compose logs -f banking-api"
echo "  Parar: docker-compose down"
echo "  Reiniciar: docker-compose restart banking-api"
echo ""