#!/bin/bash

# Script de teste para verificar configuração do Docker

echo "?? BankingApi - Teste de Configuração Docker"
echo "============================================="

# Verificar diretório atual
echo "?? Diretório atual: $(pwd)"
echo ""

# Verificar arquivos Docker Compose
echo "?? Arquivos Docker Compose encontrados:"
if [ -f "docker-compose.yml" ]; then
    echo "? docker-compose.yml (Produção)"
else
    echo "? docker-compose.yml NÃO encontrado"
fi

if [ -f "docker-compose.dev.yml" ]; then
    echo "? docker-compose.dev.yml (Desenvolvimento)"
else
    echo "? docker-compose.dev.yml NÃO encontrado"
fi
echo ""

# Verificar Dockerfile
echo "?? Dockerfiles encontrados:"
if [ -f "BankingApi/Dockerfile" ]; then
    echo "? BankingApi/Dockerfile (Produção)"
else
    echo "? BankingApi/Dockerfile NÃO encontrado"
fi

if [ -f "BankingApi/Dockerfile.dev" ]; then
    echo "? BankingApi/Dockerfile.dev (Desenvolvimento)"
else
    echo "? BankingApi/Dockerfile.dev NÃO encontrado"
fi
echo ""

# Verificar scripts
echo "?? Scripts encontrados:"
if [ -f "scripts/dev-start.sh" ]; then
    echo "? scripts/dev-start.sh"
    # Verificar se é executável
    if [ -x "scripts/dev-start.sh" ]; then
        echo "   ? Executável"
    else
        echo "   ??  Não executável (execute: chmod +x scripts/dev-start.sh)"
    fi
else
    echo "? scripts/dev-start.sh NÃO encontrado"
fi

if [ -f "scripts/deploy-production.sh" ]; then
    echo "? scripts/deploy-production.sh"
    # Verificar se é executável
    if [ -x "scripts/deploy-production.sh" ]; then
        echo "   ? Executável"
    else
        echo "   ??  Não executável (execute: chmod +x scripts/deploy-production.sh)"
    fi
else
    echo "? scripts/deploy-production.sh NÃO encontrado"
fi
echo ""

# Verificar Docker
echo "?? Verificando Docker:"
if command -v docker &> /dev/null; then
    echo "? Docker instalado"
    docker --version
    
    # Verificar se Docker está rodando
    if docker info &> /dev/null; then
        echo "? Docker está rodando"
    else
        echo "? Docker não está rodando (execute: docker run hello-world)"
    fi
else
    echo "? Docker NÃO instalado"
fi
echo ""

# Verificar Docker Compose
echo "?? Verificando Docker Compose:"
if command -v docker-compose &> /dev/null; then
    echo "? Docker Compose instalado"
    docker-compose --version
else
    echo "? Docker Compose NÃO instalado"
fi
echo ""

# Verificar estrutura de projeto
echo "?? Estrutura do projeto:"
if [ -d "BankingApi" ]; then
    echo "? Diretório BankingApi existe"
    if [ -f "BankingApi/BankingApi.csproj" ]; then
        echo "? BankingApi.csproj encontrado"
    else
        echo "? BankingApi.csproj NÃO encontrado"
    fi
else
    echo "? Diretório BankingApi NÃO encontrado"
fi

if [ -d "scripts" ]; then
    echo "? Diretório scripts existe"
else
    echo "? Diretório scripts NÃO encontrado"
fi
echo ""

# Conclusão
echo "?? Resumo:"
echo "=========="

# Contar arquivos necessários
count=0
total=6

[ -f "docker-compose.yml" ] && ((count++))
[ -f "docker-compose.dev.yml" ] && ((count++))
[ -f "BankingApi/Dockerfile" ] && ((count++))
[ -f "BankingApi/Dockerfile.dev" ] && ((count++))
[ -f "scripts/dev-start.sh" ] && ((count++))
[ -f "scripts/deploy-production.sh" ] && ((count++))

echo "?? Arquivos Docker: $count/$total encontrados"

if command -v docker &> /dev/null && command -v docker-compose &> /dev/null; then
    echo "?? Ferramentas Docker: ? Instaladas"
else
    echo "?? Ferramentas Docker: ? Faltando"
fi

if [ $count -eq $total ] && command -v docker &> /dev/null && command -v docker-compose &> /dev/null; then
    echo ""
    echo "?? CONFIGURAÇÃO COMPLETA!"
    echo "? Pronto para executar:"
    echo "   ./scripts/dev-start.sh        # Desenvolvimento"
    echo "   ./scripts/deploy-production.sh # Produção"
else
    echo ""
    echo "??  CONFIGURAÇÃO INCOMPLETA"
    echo "? Verifique os itens marcados com ? acima"
fi

echo ""
echo "?? Para tornar scripts executáveis:"
echo "   chmod +x scripts/dev-start.sh"
echo "   chmod +x scripts/deploy-production.sh"