@echo off
REM Script para executar BankingApi em modo desenvolvimento (Windows)

echo ?? BankingApi - Ambiente de Desenvolvimento
echo ==========================================

REM Verificar se Docker está instalado
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ? Docker não encontrado. Por favor, instale o Docker.
    pause
    exit /b 1
)

REM Navegar para o diretório raiz do projeto (um nível acima de scripts)
cd /d "%~dp0.."

echo ?? Diretório atual: %CD%
echo ?? Verificando arquivos Docker Compose...

if not exist "docker-compose.dev.yml" (
    echo ? Arquivo docker-compose.dev.yml não encontrado!
    echo ?? Arquivos disponíveis:
    dir docker-compose*
    pause
    exit /b 1
)

echo ?? Iniciando ambiente de desenvolvimento...
echo ?? Arquivo: docker-compose.dev.yml
echo.

docker-compose -f docker-compose.dev.yml up --build

echo.
echo ?? Parando ambiente de desenvolvimento...
echo Use Ctrl+C para parar os servicos
pause