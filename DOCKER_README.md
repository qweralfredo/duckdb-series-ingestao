# ?? Docker - BankingApi

Este documento explica como executar a BankingApi usando Docker e Docker Compose.

## ?? Pré-requisitos

- **Docker** 20.10+ 
- **Docker Compose** 2.0+
- **Git** (para clonar o repositório)

### Instalação do Docker

#### Windows/Mac
- Baixe o [Docker Desktop](https://www.docker.com/products/docker-desktop)

#### Linux (Ubuntu/Debian)
```bash
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER
```

## ?? Execução Rápida

### Produção
```bash
# Clone o repositório
git clone https://github.com/qweralfredo/duckdb-series-ingestao
cd duckdb-series-ingestao

# Execute o script de produção (Linux/Mac)
chmod +x scripts/deploy-production.sh
./scripts/deploy-production.sh

# Ou no Windows
scripts\deploy-production.bat
```

### Desenvolvimento
```bash
# Execute o script de desenvolvimento (Linux/Mac)
chmod +x scripts/dev-start.sh  
./scripts/dev-start.sh

# Ou no Windows
scripts\dev-start.bat
```

## ?? Comandos Manuais

### Construir Imagem
```bash
# Imagem de produção
docker build -t banking-api:latest -f BankingApi/Dockerfile .

# Imagem de desenvolvimento  
docker build -t banking-api:dev -f BankingApi/Dockerfile.dev .
```

### Executar com Docker Compose

#### Produção
```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs em tempo real
docker-compose logs -f banking-api

# Parar serviços
docker-compose down

# Reiniciar apenas a API
docker-compose restart banking-api
```

#### Desenvolvimento
```bash
# Iniciar com hot reload
docker-compose -f docker-compose.dev.yml up --build

# Executar em background
docker-compose -f docker-compose.dev.yml up -d --build
```

## ?? Serviços Disponíveis

### Produção (docker-compose.yml)

| Serviço | Porta | URL | Descrição |
|---------|-------|-----|-----------|
| `banking-api` | 5000 | http://localhost:5000 | API Principal |
| `banking-api` | 5001 | https://localhost:5001 | API (HTTPS) |
| `kafka-ui` | 8080 | http://localhost:8080 | Interface do Kafka |
| `kafka` | 9092 | localhost:9092 | Kafka Broker |
| `schema-registry` | 8081 | http://localhost:8081 | Schema Registry |

### Desenvolvimento (docker-compose.dev.yml)

| Serviço | Porta | URL | Descrição |
|---------|-------|-----|-----------|
| `banking-api-dev` | 5000 | http://localhost:5000 | API com Hot Reload |
| `kafka-ui` | 8080 | http://localhost:8080 | Interface do Kafka |
| `kafka` | 9092 | localhost:9092 | Kafka Broker |

## ?? Endpoints Principais

### API BankingApi
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **Simulação Rápida**: http://localhost:5000/api/simulation/rapida
- **Info Simulações**: http://localhost:5000/api/simulation/info

### Kafka UI
- **Dashboard**: http://localhost:8080
- **Tópicos**: http://localhost:8080/ui/clusters/banking-cluster/topics
- **Consumers**: http://localhost:8080/ui/clusters/banking-cluster/consumers

## ?? Monitoramento

### Verificar Status dos Containers
```bash
# Ver status de todos os serviços
docker-compose ps

# Ver uso de recursos
docker stats

# Ver logs específicos
docker-compose logs banking-api
docker-compose logs kafka
docker-compose logs kafka-ui
```

### Logs em Tempo Real
```bash
# Todos os serviços
docker-compose logs -f

# Apenas BankingApi
docker-compose logs -f banking-api

# Últimas 100 linhas
docker-compose logs --tail=100 banking-api
```

### Health Checks
```bash
# Verificar saúde da API
curl http://localhost:5000/health

# Verificar tópicos Kafka
docker exec banking-kafka kafka-topics --list --bootstrap-server localhost:9092

# Verificar consumers ativos
docker exec banking-kafka kafka-consumer-groups --list --bootstrap-server localhost:9092
```

## ?? Troubleshooting

### Problema: Porta já em uso
```bash
# Verificar quais portas estão em uso
netstat -tulpn | grep :5000
netstat -tulpn | grep :8080
netstat -tulpn | grep :9092

# Parar todos os containers
docker-compose down
docker-compose -f docker-compose.dev.yml down
```

### Problema: Container não inicia
```bash
# Ver logs detalhados
docker-compose logs banking-api

# Verificar imagem
docker images | grep banking-api

# Reconstruir imagem
docker-compose build --no-cache banking-api
```

### Problema: Kafka não conecta
```bash
# Verificar se Kafka está rodando
docker-compose ps kafka

# Ver logs do Kafka
docker-compose logs kafka

# Recriar tópicos
docker exec banking-kafka kafka-topics --delete --topic banking-transacao-solicitada --bootstrap-server localhost:9092
docker-compose restart kafka-init
```

### Problema: Volumes/Permissões
```bash
# Limpar volumes
docker-compose down -v
docker volume prune

# Verificar permissões (Linux)
ls -la ./BankingApi/SimulationReports/
sudo chown -R $USER:$USER ./BankingApi/SimulationReports/
```

## ?? Configurações Avançadas

### Variáveis de Ambiente

#### Produção
```bash
# Sobrescrever configurações via .env
echo "KAFKA_BOOTSTRAP_SERVERS=my-kafka:9092" > .env
echo "SIMULATION_AUTO_RUN=true" >> .env
docker-compose up -d
```

#### Desenvolvimento
```bash
# Usar arquivo de configuração personalizado
docker-compose -f docker-compose.dev.yml \
  -e ASPNETCORE_ENVIRONMENT=Staging up
```

### Escalabilidade
```bash
# Executar múltiplas instâncias da API
docker-compose up --scale banking-api=3 -d

# Load balancer com nginx (exemplo)
docker run -d -p 80:80 \
  -v ./nginx.conf:/etc/nginx/nginx.conf \
  nginx:alpine
```

### Persistência de Dados
```bash
# Backup de volumes
docker run --rm \
  -v banking_simulation-reports:/data \
  -v $(pwd):/backup \
  alpine tar czf /backup/simulation-reports-backup.tar.gz -C /data .

# Restaurar backup
docker run --rm \
  -v banking_simulation-reports:/data \
  -v $(pwd):/backup \
  alpine tar xzf /backup/simulation-reports-backup.tar.gz -C /data
```

## ?? Performance

### Recursos Recomendados

#### Desenvolvimento
- **CPU**: 2 cores
- **RAM**: 4 GB
- **Disco**: 10 GB

#### Produção
- **CPU**: 4+ cores  
- **RAM**: 8+ GB
- **Disco**: 50+ GB SSD

### Otimizações
```bash
# Aumentar memória para Kafka
docker-compose up -d \
  -e KAFKA_HEAP_OPTS="-Xmx2G -Xms2G"

# Configurar limites de recursos
echo "
services:
  banking-api:
    deploy:
      resources:
        limits:
          memory: 1G
          cpus: '2'
        reservations:
          memory: 512M
          cpus: '1'
" >> docker-compose.override.yml
```

## ?? Deploy em Produção

### Docker Swarm
```bash
# Inicializar swarm
docker swarm init

# Deploy do stack
docker stack deploy -c docker-compose.yml banking-stack

# Ver serviços
docker service ls
```

### Kubernetes
```bash
# Converter compose para k8s (usando kompose)
kompose convert -f docker-compose.yml

# Aplicar manifests
kubectl apply -f banking-api-deployment.yaml
kubectl apply -f banking-api-service.yaml
```

---

**?? Dicas:**
- Use `docker-compose.dev.yml` para desenvolvimento local
- Use `docker-compose.yml` para produção/staging
- Monitore sempre os logs durante execução
- Faça backup regular dos volumes de dados