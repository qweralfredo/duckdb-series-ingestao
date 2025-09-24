@echo off
REM Script para construir e executar BankingApi em produção (Windows)

echo ?? BankingApi - Script de Deploy Producao
echo ========================================

REM Verificar se Docker está instalado
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ? Docker não encontrado. Por favor, instale o Docker.
    pause
    exit /b 1
)

REM Verificar se Docker Compose está instalado
where docker-compose >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ? Docker Compose não encontrado. Por favor, instale o Docker Compose.
    pause
    exit /b 1
)

REM Navegar para o diretório raiz do projeto (um nível acima de scripts)
cd /d "%~dp0.."

echo ?? Diretório atual: %CD%
echo ?? Verificando arquivos Docker Compose...

if not exist "docker-compose.yml" (
    echo ? Arquivo docker-compose.yml não encontrado!
    echo ?? Arquivos disponíveis:
    dir docker-compose*
    pause
    exit /b 1
)

echo ?? Construindo imagem da BankingApi...
docker build -t banking-api:latest -f BankingApi/Dockerfile .

echo ?? Iniciando servicos em producao...
docker-compose up -d

echo ? Aguardando servicos ficarem prontos...
timeout /t 30 /nobreak >nul

echo ?? Verificando status dos servicos...
docker-compose ps

echo ?? Verificando logs da aplicacao...
docker-compose logs --tail=20 banking-api

echo.
echo ? Deploy concluido!
echo ?? Aplicacao disponivel em: http://localhost:5000
echo ?? Kafka UI disponivel em: http://localhost:8080
echo ??? MinIO Console disponivel em: http://localhost:9001
echo ?? Swagger disponivel em: http://localhost:5000/swagger
echo.
echo ?? Comandos uteis:
echo   Ver logs: docker-compose logs -f banking-api
echo   Parar: docker-compose down
echo   Reiniciar: docker-compose restart banking-api
echo.
pause