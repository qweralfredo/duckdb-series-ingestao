# ?? Troubleshooting - Docker Setup BankingApi

## ? **PROBLEMA RESOLVIDO!**

### **? Erro Original:**
```
Open C:\projetos\duckdb-series-ingestao\scripts\docker-compose.dev.yml: The system cannot find the file specified.
```

### **? Solução Aplicada:**
1. **Scripts corrigidos** para navegar ao diretório correto
2. **Encoding UTF-8** corrigido no docker-compose.dev.yml
3. **Caminhos relativos** ajustados

## ?? **Scripts Corrigidos**

### **Windows**
```cmd
# Desenvolvimento
scripts\dev-start.bat

# Produção  
scripts\deploy-production.bat
```

### **Linux/Mac**
```bash
# Desenvolvimento
chmod +x scripts/dev-start.sh
./scripts/dev-start.sh

# Produção
chmod +x scripts/deploy-production.sh
./scripts/deploy-production.sh
```

## ?? **Como Usar Agora**

### **1. Teste de Configuração**
```powershell
# PowerShell (Windows)
powershell -ExecutionPolicy Bypass -File scripts/test-setup.ps1

# Bash (Linux/Mac)
chmod +x scripts/test-setup.sh
./scripts/test-setup.sh
```

### **2. Desenvolvimento**
```cmd
# Windows
scripts\dev-start.bat

# Linux/Mac
./scripts/dev-start.sh
```

### **3. Produção**
```cmd
# Windows
scripts\deploy-production.bat

# Linux/Mac  
./scripts/deploy-production.sh
```

## ?? **Verificação Rápida**

### **Arquivos Necessários** ?
- `docker-compose.yml` (Produção)
- `docker-compose.dev.yml` (Desenvolvimento) 
- `BankingApi/Dockerfile` (Produção)
- `BankingApi/Dockerfile.dev` (Desenvolvimento)
- `scripts/dev-start.bat` / `scripts/dev-start.sh`
- `scripts/deploy-production.bat` / `scripts/deploy-production.sh`

### **Estrutura de Diretórios** ?
```
C:\projetos\duckdb-series-ingestao\
??? BankingApi/
?   ??? Dockerfile
?   ??? Dockerfile.dev
?   ??? BankingApi.csproj
??? scripts/
?   ??? dev-start.bat
?   ??? dev-start.sh
?   ??? deploy-production.bat
?   ??? deploy-production.sh
??? docker-compose.yml
??? docker-compose.dev.yml
```

### **Docker Funcionando** ?
```cmd
docker --version
docker-compose --version
docker run hello-world
```

## ?? **Teste Final - Execução Completa**

### **Passo 1: Verificar Tudo**
```cmd
powershell -ExecutionPolicy Bypass -File scripts/test-setup.ps1
```

### **Passo 2: Executar Desenvolvimento**
```cmd
scripts\dev-start.bat
```

### **Passo 3: Aguardar Subir (2-3 minutos)**
```
- Zookeeper
- Kafka  
- MinIO
- Banking API (com hot reload)
```

### **Passo 4: Testar API**
```cmd
curl http://localhost:5000/health
```

## ?? **Endpoints Disponíveis**

### **Desenvolvimento**
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **MinIO Console**: http://localhost:9001 (devuser/devpassword123)
- **Kafka UI**: http://localhost:8080

### **Produção** 
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **MinIO Console**: http://localhost:9001 (admin/admin123456)
- **Kafka UI**: http://localhost:8080

## ?? **Troubleshooting Comum**

### **? "docker-compose: command not found"**
```bash
# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

### **? "Permission denied" (Linux)**
```bash
chmod +x scripts/dev-start.sh
chmod +x scripts/deploy-production.sh
```

### **? Porta já em uso**
```cmd
# Verificar portas
netstat -ano | findstr :5000
netstat -ano | findstr :9000
netstat -ano | findstr :8080

# Parar containers
docker-compose down
```

### **? Docker não está rodando**
```cmd
# Windows
# Abrir Docker Desktop

# Linux
sudo systemctl start docker
sudo systemctl enable docker
```

### **? Volumes com erro de permissão**
```bash
# Linux
sudo chown -R $USER:$USER ./BankingApi/

# Windows
# Executar PowerShell como Administrador
```

## ?? **Logs e Monitoramento**

### **Ver Logs em Tempo Real**
```cmd
# API
docker-compose logs -f banking-api-dev

# MinIO
docker-compose logs -f dev-banking-minio

# Kafka
docker-compose logs -f dev-banking-kafka

# Todos
docker-compose logs -f
```

### **Status dos Containers**
```cmd
docker-compose ps
docker stats
```

## ?? **Confirmação de Sucesso**

### **? Tudo Funcionando Quando:**
1. `docker-compose ps` mostra todos os containers **Up**
2. `curl http://localhost:5000/health` retorna `{"status":"healthy"}`
3. MinIO Console abre em http://localhost:9001
4. Swagger carrega em http://localhost:5000/swagger
5. Logs não mostram erros críticos

### **?? Teste Completo:**
```cmd
# 1. Health Check
curl http://localhost:5000/health

# 2. Simulação
curl -X POST http://localhost:5000/api/simulation/rapida

# 3. Relatórios
curl http://localhost:5000/api/reports/summaries

# 4. MinIO Status
curl http://localhost:5000/api/reports/minio/status
```

---

## ?? **Dicas Finais**

### **Para Desenvolvimento:**
- Use sempre `docker-compose.dev.yml`
- Hot reload está habilitado
- Logs são mais verbosos
- MinIO usa credenciais simples

### **Para Produção:**
- Use sempre `docker-compose.yml`
- Imagens otimizadas
- Health checks habilitados
- Configurações de segurança

### **Primeiro Uso:**
1. Execute teste de configuração
2. Inicie modo desenvolvimento
3. Execute simulação rápida
4. Explore os endpoints via Swagger

**?? Agora o ambiente Docker está 100% funcional!**