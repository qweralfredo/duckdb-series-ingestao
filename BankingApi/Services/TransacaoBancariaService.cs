using BankingApi.Events;
using BankingApi.Models;
using BankingApi.Services;

namespace BankingApi.Services
{
    public class TransacaoBancariaService : ITransacaoBancariaService
    {
        private readonly IBankingEventContext _eventContext;
        private readonly ILogger<TransacaoBancariaService> _logger;

        public TransacaoBancariaService(IBankingEventContext eventContext, ILogger<TransacaoBancariaService> logger)
        {
            _eventContext = eventContext;
            _logger = logger;
        }

        public async Task<string> SolicitarTransacao(TransacaoRequest request)
        {
            var messageId = GerarMessageId();
            var endToEndId = GerarEndToEndId();
            var transactionId = GerarTransactionId();

            _logger.LogInformation("Iniciando solicitação de transação {TransactionId} do tipo {TipoTransacao}",
                transactionId, request.TipoTransacao);

            var evento = new TransacaoSolicitada
            {
                MessageId = messageId,
                EndToEndId = endToEndId,
                TransactionId = transactionId,
                TipoTransacao = request.TipoTransacao,
                Valor = request.Valor,
                Moeda = request.Moeda ?? "BRL",
                ContaOrigemId = request.ContaOrigemId,
                ContaDestinoId = request.ContaDestinoId,
                InformacaoRemessa = request.InformacaoRemessa,
                CodigoFinalidade = request.CodigoFinalidade ?? "OTHR",
                CodigoCategoriaFinalidade = request.CodigoCategoriaFinalidade ?? "OTHR",
                Descricao = request.Descricao,
                NomeDevedor = request.NomeDevedor,
                ContaDevedor = request.ContaDevedor,
                NomeCredor = request.NomeCredor,
                ContaCredor = request.ContaCredor,
                BicAgenteDevedor = request.BicAgenteDevedor,
                BicAgenteCredor = request.BicAgenteCredor,
                DataSolicitacao = DateTime.UtcNow
            };

            await _eventContext.PublicarTransacaoSolicitada(evento);

            _logger.LogInformation("Transação {TransactionId} solicitada com sucesso", transactionId);

            return transactionId;
        }

        public async Task CriarConta(ContaRequest request)
        {
            _logger.LogInformation("Criando conta para {TipoCliente} ID: {ClienteId}",
                request.PessoaId.HasValue ? "Pessoa" : "Empresa",
                request.PessoaId ?? request.EmpresaId);

            var evento = new ContaCriada
            {
                Id = Random.Shared.Next(1, 100000), // Em um cenário real, seria gerado pelo sistema
                Banco = request.Banco,
                Agencia = request.Agencia,
                NumeroConta = request.NumeroConta,
                DigitoVerificador = request.DigitoVerificador,
                TipoConta = request.TipoConta.ToString(),
                SaldoInicial = request.SaldoInicial,
                Ativa = true,
                PessoaId = request.PessoaId,
                EmpresaId = request.EmpresaId,
                DataAbertura = DateTime.UtcNow
            };

            await _eventContext.PublicarContaCriada(evento);

            _logger.LogInformation("Conta {ContaId} criada com sucesso", evento.Id);
        }

        public async Task CriarPessoa(PessoaRequest request)
        {
            _logger.LogInformation("Criando pessoa: {Nome}", request.Nome);

            var evento = new PessoaCriada
            {
                Id = Random.Shared.Next(1, 100000), // Em um cenário real, seria gerado pelo sistema
                Nome = request.Nome,
                Cpf = request.Cpf,
                Rg = request.Rg,
                DataNascimento = request.DataNascimento,
                Email = request.Email,
                Telefone = request.Telefone,
                Celular = request.Celular,
                EnderecoId = request.EnderecoId
            };

            await _eventContext.PublicarPessoaCriada(evento);

            _logger.LogInformation("Pessoa {PessoaId} criada com sucesso", evento.Id);
        }

        public async Task CriarEmpresa(EmpresaRequest request)
        {
            _logger.LogInformation("Criando empresa: {RazaoSocial}", request.RazaoSocial);

            var evento = new EmpresaCriada
            {
                Id = Random.Shared.Next(1, 100000), // Em um cenário real, seria gerado pelo sistema
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                Cnpj = request.Cnpj,
                InscricaoEstadual = request.InscricaoEstadual,
                InscricaoMunicipal = request.InscricaoMunicipal,
                Email = request.Email,
                Telefone = request.Telefone,
                Website = request.Website,
                EnderecoId = request.EnderecoId
            };

            await _eventContext.PublicarEmpresaCriada(evento);

            _logger.LogInformation("Empresa {EmpresaId} criada com sucesso", evento.Id);
        }

        private string GerarMessageId() =>
            $"MSG{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";

        private string GerarEndToEndId() =>
            $"E2E{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000, 99999)}";

        private string GerarTransactionId() =>
            $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000, 99999)}";
    }
}