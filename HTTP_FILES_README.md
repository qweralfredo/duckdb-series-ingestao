# ?? Banking API - Guia dos Arquivos HTTP

Este diretório contém arquivos `.http` com exemplos práticos para testar e demonstrar a Banking API.

## ?? Arquivos Disponíveis

### ?? **BankingApi.http** - Referência Completa
**Uso**: Documentação completa de todos os endpoints
- ? Todos os endpoints disponíveis
- ? Exemplos de cada tipo de transação  
- ? Testes de validação e casos de erro
- ? Upload/Download de arquivos
- ? Operações MinIO completas
- ? Comandos curl equivalentes

### ?? **BankingApi-Dev.http** - Desenvolvimento Local
**Uso**: Testes rápidos durante desenvolvimento
- ? Configuração para http://localhost:5000
- ? Dados de teste brasileiros
- ? Simulações pequenas para debug
- ? Validações específicas
- ? Monitoramento de desenvolvimento

### ?? **BankingApi-Demo.http** - Demonstrações
**Uso**: Apresentações e showcases
- ? Cenários práticos de uso
- ? Roteiro sugerido de 10 minutos
- ? Casos de uso empresariais
- ? Fluxos completos B2B
- ? Comandos otimizados para demo

## ??? Como Usar

### **Opção 1: Visual Studio Code**
1. Instale a extensão **REST Client**
2. Abra qualquer arquivo `.http`
3. Clique em **"Send Request"** acima de cada endpoint
4. Veja a resposta na aba lateral

### **Opção 2: JetBrains (IntelliJ, Rider)**
1. Abra qualquer arquivo `.http` 
2. Clique no ícone ?? ao lado de cada request
3. Veja a resposta no painel inferior

### **Opção 3: Comando curl**
```bash
# Exemplo: Health Check
curl -X GET "https://localhost:5000/health" \
  -H "Accept: application/json" \
  -k

# Exemplo: Simulação Rápida  
curl -X POST "https://localhost:5000/api/simulation/rapida" \
  -H "Content-Type: application/json" \
  -k
```

### **Opção 4: Postman**
1. Importe os exemplos manualmente
2. Configure variáveis de ambiente
3. Execute collections automatizadas

## ?? Pré-requisitos

### **Para Desenvolvimento (BankingApi-Dev.http)**
```bash
# Subir ambiente de desenvolvimento
docker-compose -f docker-compose.dev.yml up -d

# Aguardar serviços ficarem prontos
sleep 30

# Testar conectividade
curl http://localhost:5000/health
```

### **Para Produção (BankingApi.http)**
```bash
# Subir ambiente completo
docker-compose up -d

# Aguardar todos os serviços
sleep 60

# Testar conectividade
curl https://localhost:5000/health -k
```

### **Para Demonstração (BankingApi-Demo.http)**
```bash
# Subir ambiente
./scripts/deploy-production.sh

# Executar simulação prévia para ter dados
curl -X POST "https://localhost:5000/api/simulation/rapida" \
  -H "Content-Type: application/json" -k

# Verificar se há dados para demonstrar
curl "https://localhost:5000/api/reports/summaries?limite=3" -k
```

## ?? Cenários de Uso Recomendados

### **?? Durante Desenvolvimento**
**Arquivo**: `BankingApi-Dev.http`
```
1. Executar simulação rápida para gerar dados
2. Testar endpoints individuais
3. Validar diferentes tipos de transação
4. Debugar com logs detalhados
```

### **?? Testes de Integração**
**Arquivo**: `BankingApi.http`
```
1. Testes completos de todos os endpoints
2. Validação de casos de erro
3. Testes de upload/download
4. Verificação de URLs pré-assinadas
```

### **?? Apresentações & Demos**
**Arquivo**: `BankingApi-Demo.http`
```
1. Seguir roteiro sugerido de 10 minutos
2. Demonstrar casos de uso empresariais
3. Mostrar arquitetura orientada a eventos
4. Destacar compliance internacional
```

## ?? Endpoints Mais Importantes

### **?? Health Checks**
```http
GET /health                 # Status simples
GET /api/health            # Status detalhado
GET /api/health/info       # Informações do sistema
```

### **?? Simulações**
```http
POST /api/simulation/rapida     # 5 pessoas, 2 empresas, 20 transações
POST /api/simulation/completa   # Customizável
POST /api/simulation/intensiva  # 50 pessoas, 20 empresas, 200 transações
```

### **?? Transações**
```http
POST /api/v2/transacoes/solicitar        # Transação regular
POST /api/v2/transacoes/simular-iso20022 # Padrão internacional
```

### **?? Relatórios**
```http
GET /api/reports/summaries               # Lista relatórios
GET /api/reports/files                   # Lista arquivos MinIO
POST /api/reports/upload/{bucket}        # Upload arquivo
```

## ?? Variáveis de Ambiente

### **Para HTTPS (Produção)**
Ajuste os endpoints para:
```
https://localhost:5000
```

### **Para HTTP (Desenvolvimento)**
Ajuste os endpoints para:
```
http://localhost:5000
```

### **Para Deploy Remoto**
Substitua `localhost` pelo IP/domínio:
```
https://seu-dominio.com:5000
```

## ?? Monitoramento Durante Testes

### **Logs em Tempo Real**
```bash
# API
docker-compose logs -f banking-api

# MinIO
docker-compose logs -f minio

# Kafka
docker-compose logs -f kafka
```

### **Interfaces Web**
- **Swagger**: https://localhost:5000/swagger
- **MinIO Console**: http://localhost:9001 (admin/admin123456)
- **Kafka UI**: http://localhost:8080 (apenas produção)

## ?? Roteiro de Demonstração (10 minutos)

### **1. Preparação (30s)**
```bash
# Verificar se tudo está funcionando
curl https://localhost:5000/health -k
```

### **2. Simulação Automática (2 min)**
```http
POST /api/simulation/rapida
GET /api/reports/summaries
```

### **3. Transação Manual (2 min)**
```http
POST /api/v2/transacoes/solicitar
# Exemplo PIX de R$ 250,00
```

### **4. ISO20022 Internacional (2 min)**
```http
POST /api/v2/transacoes/simular-iso20022
# Com dados completos de compliance
```

### **5. Arquivos & MinIO (2 min)**
```http
GET /api/reports/files
# Mostrar console: http://localhost:9001
```

### **6. Monitoramento (1.5 min)**
```http
GET /api/health/info
GET /api/reports/minio/status
```

## ?? Troubleshooting

### **? Erro de Conexão**
```bash
# Verificar se API está rodando
docker-compose ps banking-api

# Ver logs de erro
docker-compose logs banking-api
```

### **? Certificado SSL**
Para HTTPS auto-assinado, adicione `-k` no curl:
```bash
curl https://localhost:5000/health -k
```

### **? Endpoints Não Encontrados**  
Verifique se está usando a URL correta:
- Desenvolvimento: `http://localhost:5000`
- Produção: `https://localhost:5000`

### **? MinIO Não Responde**
```bash
# Verificar se MinIO está rodando
curl http://localhost:9000/minio/health/live

# Reiniciar se necessário
docker-compose restart minio
```

## ?? Dicas de Uso

### **?? Para Desenvolvimento**
1. Use `BankingApi-Dev.http` com http://localhost:5000
2. Execute simulação rápida primeiro
3. Monitore logs: `docker-compose logs -f banking-api-dev`

### **?? Para Testes**
1. Use `BankingApi.http` com endpoints completos
2. Teste cenários de erro propositalmente
3. Valide todos os códigos de resposta

### **?? Para Apresentação**
1. Use `BankingApi-Demo.http` com roteiro sugerido
2. Prepare dados prévios com simulação
3. Tenha MinIO Console aberto
4. Use `jq` para formatar JSON: `curl ... | jq`

---

## ?? Resumo dos Arquivos

| Arquivo | Uso Principal | Ambiente | Complexidade |
|---------|---------------|----------|--------------|
| `BankingApi.http` | Referência completa | Produção | Alta |
| `BankingApi-Dev.http` | Desenvolvimento | Dev Local | Média |
| `BankingApi-Demo.http` | Apresentações | Produção | Baixa |

**?? Escolha o arquivo adequado para seu caso de uso e comece a testar a Banking API!**