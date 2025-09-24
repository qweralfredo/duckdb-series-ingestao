# ?? Exemplo de Uso - Simulador Banking API

Este documento mostra exemplos práticos de como usar o simulador completo da Banking API.

## ?? Pré-requisitos

1. **API executando**: `dotnet run` na pasta BankingApi
2. **Swagger acessível**: https://localhost:7125/swagger
3. **Logs habilitados**: Verifique o console para acompanhar

## ?? Exemplos de Requests

### 1. Informações sobre Simulações
```bash
curl -X GET "https://localhost:7125/api/simulation/info"
```

**Resposta esperada:**
```json
{
  "simulacoesDisponiveis": [
    {
      "tipo": "Rápida",
      "endpoint": "/api/simulation/rapida",
      "descricao": "Simulação rápida com 5 pessoas, 2 empresas e 20 transações",
      "duracaoEstimada": "30-60 segundos"
    }
  ]
}
```

### 2. Simulação Rápida
```bash
curl -X POST "https://localhost:7125/api/simulation/rapida" \
  -H "Content-Type: application/json"
```

**Logs esperados no console:**
```
[14:30:15 INF] === EXECUTANDO SIMULAÇÃO RÁPIDA ===
[14:30:15 INF] === INICIANDO SIMULAÇÃO BANCÁRIA COMPLETA ===
[14:30:15 INF] Pessoas: 5, Empresas: 2, Transações: 20
[14:30:15 INF] Criando 5 pessoas...
[14:30:15 INF] Criando pessoa 1: João Silva Santos - CPF: 12345678901
[14:30:15 INF] Evento PessoaCriada publicado: PessoaId 1
...
[14:30:45 INF] === SIMULAÇÃO CONCLUÍDA COM SUCESSO ===
```

### 3. Simulação Customizada
```bash
curl -X POST "https://localhost:7125/api/simulation/completa" \
  -H "Content-Type: application/json" \
  -d '{
    "quantidadePessoas": 15,
    "quantidadeEmpresas": 8,
    "quantidadeTransacoes": 75
  }'
```

### 4. Simulação Intensiva
```bash
curl -X POST "https://localhost:7125/api/simulation/intensiva" \
  -H "Content-Type: application/json"
```

## ?? Monitoramento em Tempo Real

### Logs Detalhados por Categoria

**Criação de Pessoas:**
```
[14:30:16 INF] Criando pessoa 1: Maria Santos Oliveira - CPF: 12345678901
[14:30:16 INF] Evento PessoaCriada publicado: PessoaId 1
```

**Criação de Empresas:**
```
[14:30:17 INF] Criando empresa 1: Tech Solutions Brasil LTDA - CNPJ: 12345678000195
[14:30:17 INF] Evento EmpresaCriada publicado: EmpresaId 1
```

**Criação de Contas:**
```
[14:30:18 INF] Criando conta para pessoa 1: 237-1234-567890
[14:30:18 INF] Evento ContaCriada publicado: ContaId 1001
```

**Execução de Transações:**
```
[14:30:20 INF] Executando transação 1: PIX - Valor: R$ 1.250,00
[14:30:20 INF] Evento TransacaoSolicitada publicado: TXN20241224143020789
[14:30:20 INF] Processando transação TXN20241224143020789 do tipo PIX
[14:30:20 INF] Evento TransacaoProcessada publicado: TXN20241224143020789
[14:30:20 INF] Saldo atualizado para conta 1001: 5000.00 -> 3750.00 (DEBITO)
[14:30:20 INF] Saldo atualizado para conta 1002: 2500.00 -> 3750.00 (CREDITO)
```

## ?? Exemplos de Dados Gerados

### Pessoa (Fake Data)
```json
{
  "nome": "Maria Santos Oliveira",
  "cpf": "12345678901",
  "rg": "12.345.678-9",
  "dataNascimento": "1985-03-15T00:00:00Z",
  "email": "maria.santos@email.com",
  "telefone": "(11) 3456-7890",
  "celular": "(11) 99876-5432"
}
```

### Empresa (Fake Data)
```json
{
  "razaoSocial": "Tech Solutions Brasil LTDA",
  "nomeFantasia": "Tech Solutions",
  "cnpj": "12345678000195",
  "inscricaoEstadual": "123.456.789.012",
  "email": "contato@techsolutions.com.br",
  "telefone": "(11) 3456-7890"
}
```

### Conta Bancária (Fake Data)
```json
{
  "banco": "237",
  "agencia": "1234",
  "numeroConta": "567890",
  "digitoVerificador": "1",
  "tipoConta": "ContaCorrente",
  "saldoInicial": 5000.00,
  "pessoaId": 1
}
```

### Transação ISO20022 (Fake Data)
```json
{
  "messageId": "MSG20241224143020123",
  "endToEndId": "E2E2024122414302056789",
  "transactionId": "TXN20241224143020789",
  "tipoTransacao": "PIX",
  "valor": 1250.00,
  "moeda": "BRL",
  "contaOrigemId": 1001,
  "contaDestinoId": 1002,
  "codigoFinalidade": "OTHR",
  "nomeDevedor": "Maria Santos Oliveira",
  "nomeCredor": "João Silva Santos"
}
```

## ?? Testando via Swagger UI

1. **Acesse**: https://localhost:7125/swagger
2. **Navegue**: SimulationController
3. **Teste**: Clique em "Try it out" em qualquer endpoint
4. **Execute**: Clique em "Execute"
5. **Monitore**: Observe os logs no console da aplicação

### Endpoint de Informações
- **GET** `/api/simulation/info`
- Não requer parâmetros
- Retorna informações sobre todas as simulações

### Endpoint Simulação Rápida
- **POST** `/api/simulation/rapida`
- Não requer corpo
- Ideal para testes rápidos

### Endpoint Simulação Completa
- **POST** `/api/simulation/completa`
- Requer corpo JSON com parâmetros
- Permite customização completa

### Endpoint Simulação Intensiva
- **POST** `/api/simulation/intensiva`
- Não requer corpo
- Para testes de carga

## ?? Performance Esperada

### Simulação Rápida (5+2+20)
- **Tempo**: 30-60 segundos
- **Throughput**: ~15 transações/segundo
- **Eventos**: ~54 eventos publicados

### Simulação Completa (15+8+75)
- **Tempo**: 2-4 minutos
- **Throughput**: ~12 transações/segundo
- **Eventos**: ~198 eventos publicados

### Simulação Intensiva (50+20+200)
- **Tempo**: 5-10 minutos
- **Throughput**: ~10 transações/segundo
- **Eventos**: ~540 eventos publicados

## ??? Troubleshooting

### Problema: API não responde
**Solução**: Verifique se a aplicação está rodando
```bash
dotnet run --project BankingApi
```

### Problema: Logs não aparecem
**Solução**: Verifique o nível de log no appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "BankingApi.Simulation": "Information"
    }
  }
}
```

### Problema: Simulação muito lenta
**Solução**: Reduza os delays no código ou use simulação rápida

### Problema: Muitos logs
**Solução**: Configure o nível de log para Warning
```json
{
  "Logging": {
    "LogLevel": {
      "BankingApi": "Warning"
    }
  }
}
```

## ?? Casos de Uso Práticos

### 1. Desenvolvimento
```bash
# Teste rápido durante desenvolvimento
curl -X POST "localhost:7125/api/simulation/rapida"
```

### 2. QA/Testes
```bash
# Teste com volume médio
curl -X POST "localhost:7125/api/simulation/completa" \
  -d '{"quantidadePessoas": 25, "quantidadeEmpresas": 10, "quantidadeTransacoes": 100}'
```

### 3. Performance/Carga
```bash
# Teste de stress
curl -X POST "localhost:7125/api/simulation/intensiva"
```

### 4. Demonstração
```bash
# Para apresentações - volume controlado
curl -X POST "localhost:7125/api/simulation/completa" \
  -d '{"quantidadePessoas": 10, "quantidadeEmpresas": 5, "quantidadeTransacoes": 30}'
```

---

**?? Dica Final**: Sempre monitore os logs no console para acompanhar o progresso e verificar se todos os eventos estão sendo publicados corretamente!