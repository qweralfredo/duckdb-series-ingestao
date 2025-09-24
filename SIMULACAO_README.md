# ?? Banking API - Simulador Completo

Este projeto contém um simulador completo que testa todos os fluxos CRUD das entidades bancárias e transações utilizando **Faker.NET** para geração de dados realistas e **MassTransit** para arquitetura orientada a eventos.

## ?? Funcionalidades do Simulador

### ?? Operações Suportadas

#### **Entidades**
- ? **Pessoas**: Criação com dados brasileiros (CPF, telefones, endereços)
- ? **Empresas**: Criação com CNPJ, inscrições estadual/municipal
- ? **Contas Bancárias**: Diferentes tipos (corrente, poupança, salário, investimento)
- ? **Endereços**: Dados completos de localização

#### **Transações**
- ? **Transferência**: Entre contas diferentes
- ? **PIX**: Transferência instantânea
- ? **TED**: Transferência Eletrônica Disponível
- ? **DOC**: Documento de Ordem de Crédito
- ? **Depósito**: Entrada de valores
- ? **Saque**: Retirada de valores
- ? **Pagamento**: Quitação de obrigações

### ?? Tipos de Simulação

#### 1. **Simulação Rápida** ?????
- **Endpoint**: `POST /api/simulation/rapida`
- **Dados**: 5 pessoas, 2 empresas, 20 transações
- **Duração**: 30-60 segundos
- **Uso**: Testes rápidos e desenvolvimento

#### 2. **Simulação Completa** ??
- **Endpoint**: `POST /api/simulation/completa`
- **Dados**: Customizável via parâmetros
- **Duração**: Variável
- **Uso**: Testes específicos

```json
{
  "quantidadePessoas": 25,
  "quantidadeEmpresas": 10,
  "quantidadeTransacoes": 100
}
```

#### 3. **Simulação Intensiva** ??
- **Endpoint**: `POST /api/simulation/intensiva`
- **Dados**: 50 pessoas, 20 empresas, 200 transações
- **Duração**: 5-10 minutos
- **Uso**: Testes de performance e carga

### ?? Monitoramento e Relatórios

#### **Logs Detalhados**
Todos os passos são logados com informações detalhadas:
```
[14:30:15 INF] Criando pessoa 1: João Silva Santos - CPF: 12345678901
[14:30:15 INF] Evento PessoaCriada publicado: PessoaId 1
[14:30:16 INF] Executando transação 1: PIX - Valor: R$ 1.250,00
[14:30:16 INF] Evento TransacaoSolicitada publicado: TXN20241224143016789
```

#### **Relatórios Automáticos**
Gerados em `./SimulationReports/` com métricas:
- Duração total da simulação
- Throughput (transações/segundo)
- Performance por tipo de operação
- Lista de todas as transações executadas

### ?? Geração de Dados Fake

Utilizamos **Bogus (Faker.NET)** com localização brasileira:

```csharp
// Exemplo de pessoa gerada
{
  "nome": "Maria Santos Oliveira",
  "cpf": "12345678901",
  "email": "maria.santos@email.com",
  "telefone": "(11) 3456-7890",
  "celular": "(11) 99876-5432",
  "dataNascimento": "1985-03-15"
}

// Exemplo de empresa gerada
{
  "razaoSocial": "Tech Solutions Brasil LTDA",
  "cnpj": "12345678000195",
  "email": "contato@techsolutions.com.br",
  "inscricaoEstadual": "123.456.789.012"
}
```

### ????? Como Executar

#### **1. Via API (Recomendado)**

```bash
# Simulação rápida
curl -X POST "https://localhost:7125/api/simulation/rapida"

# Simulação customizada
curl -X POST "https://localhost:7125/api/simulation/completa" \
  -H "Content-Type: application/json" \
  -d '{"quantidadePessoas": 15, "quantidadeEmpresas": 8, "quantidadeTransacoes": 75}'

# Simulação intensiva
curl -X POST "https://localhost:7125/api/simulation/intensiva"
```

#### **2. Via Swagger UI**
1. Acesse `https://localhost:7125/swagger`
2. Navegue até **SimulationController**
3. Execute a simulação desejada
4. Monitore os logs no console

#### **3. Via Background Service (Automático)**
Configure no `appsettings.json`:
```json
{
  "Simulation": {
    "AutoRun": true,
    "IntervalMinutes": 10
  }
}
```

### ?? Arquitetura Orientada a Eventos

#### **Fluxo de Eventos**
1. **Solicitação** ? Publica `TransacaoSolicitada`
2. **Processamento** ? Consumer processa e publica `TransacaoProcessada`
3. **Atualização** ? Publica `SaldoAtualizado`
4. **Auditoria** ? Todos os eventos são logados

#### **Vantagens**
- ? **Desacoplamento**: Componentes independentes
- ? **Escalabilidade**: Processamento assíncrono
- ? **Auditoria**: Histórico completo de eventos
- ? **Resiliência**: Tolerância a falhas
- ? **Observabilidade**: Rastreamento completo

### ?? Execução com Kafka (Produção)

Para usar Kafka real ao invés de InMemory:

1. **Inicie o Kafka**:
```bash
docker-compose up -d kafka zookeeper
```

2. **Descomente a configuração Kafka** no `Program.cs`

3. **Configure os tópicos**:
```bash
# Criar tópicos
docker exec -it kafka kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic banking-transacao-solicitada \
  --partitions 3 \
  --replication-factor 1
```

### ?? Endpoints Disponíveis

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/simulation/info` | Informações sobre simulações |
| `POST` | `/api/simulation/rapida` | Executa simulação rápida |
| `POST` | `/api/simulation/completa` | Executa simulação customizada |
| `POST` | `/api/simulation/intensiva` | Executa simulação intensiva |
| `POST` | `/api/v2/transacoes/solicitar` | Solicita transação (novo) |
| `POST` | `/api/v2/transacoes/simular-iso20022` | Transação ISO20022 |

### ?? Monitoramento em Tempo Real

#### **Logs Estruturados**
```
[INFO] Simulação rápida executada com sucesso
[INFO] Total de pessoas criadas: 5
[INFO] Total de empresas criadas: 2  
[INFO] Total de contas criadas: 7
[INFO] Total de transações executadas: 20
[INFO] Throughput: 15,2 transações/segundo
```

#### **Relatórios JSON**
```json
{
  "dataHoraInicio": "2024-12-24T14:30:00Z",
  "dataHoraFim": "2024-12-24T14:32:15Z",
  "duracaoTotal": "00:02:15",
  "estatisticas": {
    "totalPessoas": 5,
    "totalEmpresas": 2,
    "totalContas": 7,
    "totalTransacoes": 20,
    "transacoesPorMinuto": 8.89,
    "transacoesPorSegundo": 0.15
  }
}
```

### ? Performance

#### **Métricas Típicas**
- **Simulação Rápida**: ~15 transações/segundo
- **Simulação Completa**: ~12 transações/segundo  
- **Simulação Intensiva**: ~10 transações/segundo

#### **Otimizações**
- Processamento assíncrono com MassTransit
- Lotes de eventos para reduzir latência
- Delays configuráveis entre operações
- Pool de conexões otimizado

### ??? Tecnologias Utilizadas

- **ASP.NET Core 8**: Framework web
- **MassTransit**: Messaging e eventos
- **Bogus**: Geração de dados fake
- **Kafka**: Message broker (produção)
- **Swagger**: Documentação da API
- **Serilog**: Logging estruturado

### ?? Casos de Uso

1. **Desenvolvimento**: Testes rápidos com dados realistas
2. **QA**: Validação de fluxos completos
3. **Performance**: Teste de carga e stress
4. **Demonstração**: Showcase da arquitetura
5. **Treinamento**: Aprendizado de padrões de eventos

---

**?? Dica**: Inicie sempre com a **simulação rápida** para validar o ambiente, depois execute simulações maiores conforme necessário!