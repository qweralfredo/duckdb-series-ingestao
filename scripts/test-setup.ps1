# Script de teste para verificar configuracao Docker (PowerShell)

Write-Host "?? BankingApi - Teste de Configuracao Docker" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# Verificar diretorio atual
Write-Host "?? Diretorio atual: $(Get-Location)" -ForegroundColor Green
Write-Host ""

# Verificar arquivos Docker Compose
Write-Host "?? Arquivos Docker Compose encontrados:" -ForegroundColor Yellow
if (Test-Path "docker-compose.yml") {
    Write-Host "? docker-compose.yml (Producao)" -ForegroundColor Green
} else {
    Write-Host "? docker-compose.yml NAO encontrado" -ForegroundColor Red
}

if (Test-Path "docker-compose.dev.yml") {
    Write-Host "? docker-compose.dev.yml (Desenvolvimento)" -ForegroundColor Green
} else {
    Write-Host "? docker-compose.dev.yml NAO encontrado" -ForegroundColor Red
}
Write-Host ""

# Verificar Dockerfile
Write-Host "?? Dockerfiles encontrados:" -ForegroundColor Yellow
if (Test-Path "BankingApi/Dockerfile") {
    Write-Host "? BankingApi/Dockerfile (Producao)" -ForegroundColor Green
} else {
    Write-Host "? BankingApi/Dockerfile NAO encontrado" -ForegroundColor Red
}

if (Test-Path "BankingApi/Dockerfile.dev") {
    Write-Host "? BankingApi/Dockerfile.dev (Desenvolvimento)" -ForegroundColor Green
} else {
    Write-Host "? BankingApi/Dockerfile.dev NAO encontrado" -ForegroundColor Red
}
Write-Host ""

# Verificar scripts
Write-Host "?? Scripts encontrados:" -ForegroundColor Yellow
if (Test-Path "scripts/dev-start.bat") {
    Write-Host "? scripts/dev-start.bat" -ForegroundColor Green
} else {
    Write-Host "? scripts/dev-start.bat NAO encontrado" -ForegroundColor Red
}

if (Test-Path "scripts/deploy-production.bat") {
    Write-Host "? scripts/deploy-production.bat" -ForegroundColor Green
} else {
    Write-Host "? scripts/deploy-production.bat NAO encontrado" -ForegroundColor Red
}
Write-Host ""

# Verificar Docker
Write-Host "?? Verificando Docker:" -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "? Docker instalado: $dockerVersion" -ForegroundColor Green
    
    # Verificar se Docker esta rodando
    try {
        docker info > $null 2>&1
        Write-Host "? Docker esta rodando" -ForegroundColor Green
    } catch {
        Write-Host "? Docker nao esta rodando" -ForegroundColor Red
    }
} catch {
    Write-Host "? Docker NAO instalado" -ForegroundColor Red
}
Write-Host ""

# Verificar Docker Compose
Write-Host "?? Verificando Docker Compose:" -ForegroundColor Yellow
try {
    $composeVersion = docker-compose --version
    Write-Host "? Docker Compose instalado: $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "? Docker Compose NAO instalado" -ForegroundColor Red
}
Write-Host ""

# Verificar estrutura de projeto
Write-Host "?? Estrutura do projeto:" -ForegroundColor Yellow
if (Test-Path "BankingApi") {
    Write-Host "? Diretorio BankingApi existe" -ForegroundColor Green
    if (Test-Path "BankingApi/BankingApi.csproj") {
        Write-Host "? BankingApi.csproj encontrado" -ForegroundColor Green
    } else {
        Write-Host "? BankingApi.csproj NAO encontrado" -ForegroundColor Red
    }
} else {
    Write-Host "? Diretorio BankingApi NAO encontrado" -ForegroundColor Red
}

if (Test-Path "scripts") {
    Write-Host "? Diretorio scripts existe" -ForegroundColor Green
} else {
    Write-Host "? Diretorio scripts NAO encontrado" -ForegroundColor Red
}
Write-Host ""

# Conclusao
Write-Host "?? Resumo:" -ForegroundColor Cyan
Write-Host "==========" -ForegroundColor Cyan

# Contar arquivos necessarios
$count = 0
$total = 6

if (Test-Path "docker-compose.yml") { $count++ }
if (Test-Path "docker-compose.dev.yml") { $count++ }
if (Test-Path "BankingApi/Dockerfile") { $count++ }
if (Test-Path "BankingApi/Dockerfile.dev") { $count++ }
if (Test-Path "scripts/dev-start.bat") { $count++ }
if (Test-Path "scripts/deploy-production.bat") { $count++ }

Write-Host "?? Arquivos Docker: $count/$total encontrados" -ForegroundColor Yellow

try {
    docker --version > $null 2>&1
    docker-compose --version > $null 2>&1
    Write-Host "?? Ferramentas Docker: ? Instaladas" -ForegroundColor Green
    $dockerOk = $true
} catch {
    Write-Host "?? Ferramentas Docker: ? Faltando" -ForegroundColor Red
    $dockerOk = $false
}

if ($count -eq $total -and $dockerOk) {
    Write-Host ""
    Write-Host "?? CONFIGURACAO COMPLETA!" -ForegroundColor Green
    Write-Host "? Pronto para executar:" -ForegroundColor Green
    Write-Host "   scripts\dev-start.bat        # Desenvolvimento" -ForegroundColor White
    Write-Host "   scripts\deploy-production.bat # Producao" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "??  CONFIGURACAO INCOMPLETA" -ForegroundColor Yellow
    Write-Host "? Verifique os itens marcados com ? acima" -ForegroundColor Red
}

Write-Host ""
Write-Host "?? Para testar rapidamente:" -ForegroundColor Cyan
Write-Host "   docker --version" -ForegroundColor White
Write-Host "   docker-compose --version" -ForegroundColor White
Write-Host "   docker run hello-world" -ForegroundColor White

Write-Host ""
Read-Host "Pressione Enter para continuar"