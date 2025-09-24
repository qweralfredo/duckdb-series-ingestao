# Banking API with ISO20022 Transaction Simulation

A .NET C# Web API that simulates banking operations with ISO20022 standard transaction processing.

## Features

- **Complete CRUD operations** for banking entities
- **ISO20022 transaction simulation** with proper message formatting
- **In-memory database** for development and testing
- **Swagger/OpenAPI** documentation
- **Comprehensive validation** and error handling

## Data Models

### 1. Endereco (Address)
- Complete address information with creation/update timestamps
- Used by both Pessoa and Empresa entities

### 2. Pessoa (Person)
- Personal information including CPF (unique)
- Linked to address and bank accounts
- Supports multiple bank accounts per person

### 3. Empresa (Company)  
- Company information including CNPJ (unique)
- Linked to address and bank accounts
- Supports multiple bank accounts per company

### 4. ContaBancaria (Bank Account)
- Bank account details with balance tracking
- Belongs to either a Person OR Company (not both)
- Supports different account types (checking, savings, etc.)
- Unique constraint on Bank + Agency + Account Number

### 5. TransacaoBancaria (Bank Transaction)
- **ISO20022 compliant** transaction structure
- Includes required ISO20022 fields:
  - MessageId: Unique message identifier
  - EndToEndId: End-to-end transaction identifier  
  - TransactionId: Transaction identifier
  - Remittance information
  - Purpose codes
  - Agent (BIC) information
  - Debtor/Creditor details

## API Endpoints

### CRUD Operations

- **GET/POST/PUT/DELETE** `/api/Enderecos` - Address management
- **GET/POST/PUT/DELETE** `/api/Pessoas` - Person management  
- **GET/POST/PUT/DELETE** `/api/Empresas` - Company management
- **GET/POST/PUT/DELETE** `/api/ContasBancarias` - Bank account management

### ISO20022 Transaction Simulation

- **POST** `/api/TransacoesBancarias/simular-iso20022` - Simulate ISO20022 transaction
- **GET** `/api/TransacoesBancarias` - View all transactions
- **GET** `/api/TransacoesBancarias/{id}` - View specific transaction

## Running the Application

```bash
cd BankingApi
dotnet run
```

The API will be available at `http://localhost:5274` with Swagger UI at `http://localhost:5274/swagger`

## Example Usage

### Create a Person
```bash
curl -X POST "http://localhost:5274/api/Pessoas" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpf": "12345678901",
    "email": "joao@email.com",
    "enderecoId": 1
  }'
```

### Create a Bank Account
```bash
curl -X POST "http://localhost:5274/api/ContasBancarias" \
  -H "Content-Type: application/json" \
  -d '{
    "banco": "001",
    "agencia": "1234", 
    "numeroConta": "12345-6",
    "saldo": 1000.00,
    "pessoaId": 1
  }'
```

### Simulate ISO20022 Transaction
```bash
curl -X POST "http://localhost:5274/api/TransacoesBancarias/simular-iso20022" \
  -H "Content-Type: application/json" \
  -d '{
    "tipoTransacao": 0,
    "valor": 150.00,
    "contaOrigemId": 1,
    "contaDestinoId": 2,
    "informacaoRemessa": "Payment for services",
    "codigoFinalidade": "CBFF"
  }'
```

## ISO20022 Compliance

The transaction simulation includes proper ISO20022 fields:

- **Message Identification**: Auto-generated unique IDs
- **Transaction Parties**: Debtor/Creditor information
- **Agent Information**: BIC codes for financial institutions  
- **Remittance Information**: Payment details and purpose codes
- **Transaction Status**: Processing lifecycle management

## Validation Features

- CPF/CNPJ uniqueness validation
- Bank account uniqueness (Bank + Agency + Account)
- Business rules: Account belongs to Person OR Company (not both)
- Balance validation for debit transactions
- Transaction amount validation

## Technical Stack

- **.NET 8.0** - Web API framework
- **Entity Framework Core** - ORM with In-Memory database
- **System.Text.Json** - JSON serialization with cycle handling
- **Swagger/OpenAPI** - API documentation