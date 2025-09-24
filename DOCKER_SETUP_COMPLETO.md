# ?? Banking API - Docker Setup Completo com MinIO

## ? Arquivos Docker Criados

### ?? Estrutura dos Arquivos

```
?? BankingApi/
??? ?? Dockerfile                    # Produção otimizada
??? ?? Dockerfile.dev               # Desenvolvimento com hot reload
??? ?? .dockerignore               # Exclusões para build
??? ?? appsettings.Production.json  # Config produção
??? ?? Controllers/HealthController.cs # Health checks
??? ?? Controllers/ReportsController.cs # Gerenciamento MinIO
??? ?? Services/MinIOService.cs     # Integração MinIO

?? Root/
??? ?? docker-compose.yml          # Produção (Kafka + MinIO + API)
??? ?? docker-compose.dev.yml      # Desenvolvimento
??? ?? scripts/
    ??? ?? deploy-production.sh     # Deploy Linux/Mac
    ??? ?? deploy-production.bat    # Deploy Windows
    ??? ?? dev-start.sh            # Dev Linux/Mac
    ??? ?? dev-start.bat           # Dev Windows
```

## ?? Como Executar

### 1?? **Desenvolvimento (Recomendado para começar)**

#### Linux/Mac:
```bash
chmod +x scripts/dev-start.sh
./scripts/dev-start.sh
```

#### Windows:
```cmd
scripts\dev-start.bat
```

#### Manual:
```bash
docker-compose -f docker-compose.dev.yml up --build
```

### 2?? **Produção (Com Kafka + MinIO)**

#### Linux/Mac:
```bash
chmod +x scripts/deploy-production.sh  
./scripts/deploy-production.sh
```

#### Windows:
```cmd
scripts\deploy-production.bat
```

#### Manual:
```bash
docker-compose up -d
```

## ?? Endpoints Disponíveis

### ?? BankingApi
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/api/health
- **Health Simples**: http://localhost:5000/health
- **Info Sistema**: http://localhost:5000/api/health/info
- **Simulação Rápida**: http://localhost:5000/api/simulation/rapida

### ?? Kafka UI (Apenas Produção)
- **Dashboard**: http://localhost:8080
- **Tópicos**: http://localhost:8080/ui/clusters/banking-cluster/topics

### ??? MinIO (Armazenamento S3)
- **Console MinIO**: http://localhost:9001
- **API MinIO**: http://localhost:9000
- **Relatórios**: http://localhost:5000/api/reports/summaries
- **Upload/Download**: http://localhost:5000/api/reports/files

## ?? Configurações dos Containers

### ?? Dockerfile (Produção)
- **Base**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Build**: Multi-stage otimizado
- **Segurança**: Usuário não-root
- **Portas**: 8080 (HTTP), 8443 (HTTPS)

### ?? Dockerfile.dev (Desenvolvimento)
- **Base**: `mcr.microsoft.com/dotnet/sdk:8.0`
- **Features**: Hot reload, debugging
- **Tools**: dotnet-watch, dotnet-ef
- **Volume**: Bind mount para código

### ?? docker-compose.yml (Produção)
- **Kafka Cluster**: Zookeeper + Kafka + Schema Registry
- **Kafka UI**: Interface web para monitoramento  
- **MinIO**: Armazenamento S3-compatível com console
- **BankingApi**: Aplicação principal
- **Init Containers**: Criação automática de tópicos e buckets
- **Health Checks**: Monitoramento de saúde
- **Volumes**: Persistência de relatórios e dados MinIO

### ?? docker-compose.dev.yml (Desenvolvimento)
- **Kafka Minimal**: Apenas Kafka + UI
- **MinIO Dev**: Configuração simplificada para testes
- **Hot Reload**: Alterações refletidas automaticamente
- **Debug**: Logs verbosos
- **Volume Bind**: Código local montado

## ?? Monitoramento

### Health Checks
```bash
# Status simples
curl http://localhost:5000/health

# Status detalhado  
curl http://localhost:5000/api/health

# Informações do sistema
curl http://localhost:5000/api/health/info

# Status do MinIO
curl http://localhost:5000/api/reports/minio/status
```

### Logs
```bash
# Ver logs em tempo real
docker-compose logs -f banking-api

# Logs do MinIO
docker-compose logs -f minio

# Logs do Kafka
docker-compose logs -f kafka

# Últimas 50 linhas
docker-compose logs --tail=50 banking-api

# Todos os serviços
docker-compose logs -f
```

### Status dos Containers
```bash
# Ver status
docker-compose ps

# Uso de recursos
docker stats

# Reiniciar serviço específico
docker-compose restart banking-api
docker-compose restart minio
```

## ?? Comandos Úteis

### Gerenciamento Geral
```bash
# Parar tudo
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Reconstruir imagens
docker-compose build --no-cache

# Verificar tópicos Kafka
docker exec banking-kafka kafka-topics --list --bootstrap-server localhost:9092
```

### Gerenciamento MinIO
```bash
# Listar buckets
docker exec banking-minio mc ls banking-minio

# Criar bucket manualmente
docker exec banking-minio mc mb banking-minio/novo-bucket

# Definir política pública
docker exec banking-minio mc anonymous set public banking-minio/novo-bucket

# Upload via MinIO CLI
docker exec banking-minio mc cp /tmp/arquivo.txt banking-minio/simulation-reports/
```

### Limpeza
```bash
# Remover containers parados
docker container prune

# Remover imagens não utilizadas
docker image prune

# Limpeza completa (cuidado!)
docker system prune -a
```

## ?? Testes Rápidos

### 1. Verificar se API está rodando
```bash
curl http://localhost:5000/health
```

### 2. Executar simulação (salva automaticamente no MinIO)
```bash
curl -X POST http://localhost:5000/api/simulation/rapida
```

### 3. Listar relatórios gerados
```bash
curl http://localhost:5000/api/reports/summaries
```

### 4. Ver MinIO Console
Acesse: http://localhost:9001
- **Produção**: admin / admin123456
- **Desenvolvimento**: devuser / devpassword123

### 5. Ver Kafka UI
Acesse: http://localhost:8080

### 6. Ver Swagger
Acesse: http://localhost:5000/swagger

## ? Performance

### Recursos Mínimos
- **Development**: 6GB RAM, 4 CPU cores
- **Production**: 12GB RAM, 6 CPU cores

### Recursos por Serviço
- **BankingApi**: 1GB RAM, 1 CPU
- **Kafka**: 2GB RAM, 2 CPU  
- **MinIO**: 1GB RAM, 1 CPU
- **Postgres** (futuro): 2GB RAM, 1 CPU

### Otimizações
```bash
# Aumentar heap do Kafka
docker-compose up -d -e KAFKA_HEAP_OPTS="-Xmx2G -Xms2G"

# Limitar recursos da API
echo "
services:
  banking-api:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '2'
  minio:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '1'
" >> docker-compose.override.yml
```

## ?? Troubleshooting Comum

### ? Porta já em uso
```bash
# Verificar processos
netstat -tulpn | grep :5000
netstat -tulpn | grep :9000
netstat -tulpn | grep :9001
# Parar containers
docker-compose down
```

### ? MinIO não conecta
```bash
# Ver logs do MinIO
docker-compose logs minio
# Recriar buckets
docker-compose restart minio-init
# Verificar se buckets existem
docker exec banking-minio mc ls banking-minio
```

### ? Kafka não conecta
```bash
# Ver logs do Kafka
docker-compose logs kafka
# Recriar tópicos
docker-compose restart kafka-init
```

### ? Relatórios não salvam no MinIO
```bash
# Verificar configuração
curl http://localhost:5000/api/reports/minio/status
# Ver logs da API
docker-compose logs banking-api | grep -i minio
# Testar upload manual
curl -X POST "http://localhost:5000/api/reports/upload/simulation-reports" \
  -F "file=@test.txt"
```

### ? Permissões (Linux)
```bash
# Ajustar permissões
sudo chown -R $USER:$USER ./BankingApi/
# Verificar volumes
docker volume inspect banking_minio-data
```

### ? Baixa performance
```bash
# Ver uso de recursos
docker stats
# Aumentar recursos no Docker Desktop
# Verificar espaço em disco
df -h
```

## ?? Deploy Avançado

### Docker Swarm
```bash
docker swarm init
docker stack deploy -c docker-compose.yml banking-stack
docker service ls
```

### Kubernetes (com Kompose)
```bash
kompose convert -f docker-compose.yml
kubectl apply -f .
kubectl get pods
```

### Ambiente de Produção
```bash
# Usar certificados SSL
# Configurar backup automático do MinIO
# Monitoramento com Prometheus
# Log aggregation com ELK Stack
```

## ?? Fluxo de Dados Completo

```
[API Request] 
    ?
[Banking API] ? [Kafka Topics] ? [Event Consumers]
    ?                              ?
[MinIO Storage] ? [Simulation Reports] ? [Processing]
    ?
[Web Console / API Download]
```

## ?? Estrutura de Volumes

```
/var/lib/docker/volumes/
??? banking_minio-data/          # Dados do MinIO
?   ??? simulation-reports/      # Relatórios JSON
?   ??? banking-logs/           # Logs da aplicação  
?   ??? banking-backups/        # Backups do sistema
?   ??? banking-exports/        # Dados exportados
??? banking_simulation-reports/ # Volume local alternativo
```

---

## ?? Resumo dos Benefits

? **Isolamento completo** - Não interfere no sistema host  
? **Kafka integrado** - Message broker pronto para uso  
? **MinIO S3-compatível** - Armazenamento escalável  
? **Hot reload** - Desenvolvimento ágil  
? **Health checks** - Monitoramento automático  
? **Logs estruturados** - Fácil debugging  
? **Scripts automatizados** - Deploy com um comando  
? **Multi-plataforma** - Windows, Linux, Mac  
? **Produção ready** - Configurações otimizadas  
? **Armazenamento persistente** - Dados seguros no MinIO  
? **APIs completas** - Upload, download, listagem  
? **Console web** - Interface visual para MinIO  

**?? Stack completa: API + Kafka + MinIO em containers!**