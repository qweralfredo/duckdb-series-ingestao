using Microsoft.AspNetCore.Mvc;
using BankingApi.Services;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoBancariaService _transacaoService;
        private readonly ILogger<TransacoesController> _logger;

        public TransacoesController(ITransacaoBancariaService transacaoService, ILogger<TransacoesController> logger)
        {
            _transacaoService = transacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Solicita uma nova transação bancária utilizando arquitetura orientada a eventos
        /// </summary>
        /// <param name="request">Dados da transação</param>
        /// <returns>ID da transação criada</returns>
        [HttpPost("solicitar")]
        public async Task<ActionResult<TransacaoResponse>> SolicitarTransacao([FromBody] TransacaoRequest request)
        {
            try
            {
                if (request.Valor <= 0)
                {
                    return BadRequest("O valor da transação deve ser maior que zero.");
                }

                if (string.IsNullOrEmpty(request.Moeda))
                {
                    request.Moeda = "BRL";
                }

                var transactionId = await _transacaoService.SolicitarTransacao(request);

                var response = new TransacaoResponse
                {
                    TransactionId = transactionId,
                    Status = "SOLICITADA",
                    Mensagem = "Transação solicitada com sucesso. O processamento será realizado de forma assíncrona."
                };

                return Accepted(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar transação");
                return BadRequest($"Erro ao processar solicitação: {ex.Message}");
            }
        }

        /// <summary>
        /// Simula uma transação ISO20022 completa
        /// </summary>
        /// <param name="request">Dados da simulação</param>
        /// <returns>Resultado da simulação</returns>
        [HttpPost("simular-iso20022")]
        public async Task<ActionResult<TransacaoResponse>> SimularTransacaoISO20022([FromBody] SimulacaoISO20022Request request)
        {
            try
            {
                var transacaoRequest = new TransacaoRequest
                {
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
                    BicAgenteCredor = request.BicAgenteCredor
                };

                var transactionId = await _transacaoService.SolicitarTransacao(transacaoRequest);

                var response = new TransacaoResponse
                {
                    TransactionId = transactionId,
                    Status = "PROCESSANDO",
                    Mensagem = "Transação ISO20022 iniciada. Acompanhe o processamento pelos eventos publicados."
                };

                return Accepted(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao simular transação ISO20022");
                return BadRequest($"Erro na simulação: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria uma nova conta bancária
        /// </summary>
        /// <param name="request">Dados da conta</param>
        /// <returns>Confirmação da criação</returns>
        [HttpPost("contas")]
        public async Task<ActionResult> CriarConta([FromBody] ContaRequest request)
        {
            try
            {
                await _transacaoService.CriarConta(request);
                return Accepted(new { Mensagem = "Conta criada com sucesso. Evento publicado para processamento." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conta");
                return BadRequest($"Erro ao criar conta: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria uma nova pessoa
        /// </summary>
        /// <param name="request">Dados da pessoa</param>
        /// <returns>Confirmação da criação</returns>
        [HttpPost("pessoas")]
        public async Task<ActionResult> CriarPessoa([FromBody] PessoaRequest request)
        {
            try
            {
                await _transacaoService.CriarPessoa(request);
                return Accepted(new { Mensagem = "Pessoa criada com sucesso. Evento publicado para processamento." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa");
                return BadRequest($"Erro ao criar pessoa: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria uma nova empresa
        /// </summary>
        /// <param name="request">Dados da empresa</param>
        /// <returns>Confirmação da criação</returns>
        [HttpPost("empresas")]
        public async Task<ActionResult> CriarEmpresa([FromBody] EmpresaRequest request)
        {
            try
            {
                await _transacaoService.CriarEmpresa(request);
                return Accepted(new { Mensagem = "Empresa criada com sucesso. Evento publicado para processamento." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar empresa");
                return BadRequest($"Erro ao criar empresa: {ex.Message}");
            }
        }
    }

    public class TransacaoResponse
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }

    public class SimulacaoISO20022Request
    {
        public TipoTransacao TipoTransacao { get; set; }
        public decimal Valor { get; set; }
        public string? Moeda { get; set; }
        public int? ContaOrigemId { get; set; }
        public int? ContaDestinoId { get; set; }
        public string? InformacaoRemessa { get; set; }
        public string? CodigoFinalidade { get; set; }
        public string? CodigoCategoriaFinalidade { get; set; }
        public string? Descricao { get; set; }
        public string? NomeDevedor { get; set; }
        public string? ContaDevedor { get; set; }
        public string? NomeCredor { get; set; }
        public string? ContaCredor { get; set; }
        public string? BicAgenteDevedor { get; set; }
        public string? BicAgenteCredor { get; set; }
    }
}