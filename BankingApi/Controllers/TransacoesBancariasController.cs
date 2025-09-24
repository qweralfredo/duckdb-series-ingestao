using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingApi.Models;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacoesBancariasController : ControllerBase
    {
        private readonly BankingContext _context;

        public TransacoesBancariasController(BankingContext context)
        {
            _context = context;
        }

        // GET: api/TransacoesBancarias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransacaoBancaria>>> GetTransacoesBancarias()
        {
            return await _context.TransacoesBancarias
                .Include(t => t.ContaOrigem)
                .Include(t => t.ContaDestino)
                .ToListAsync();
        }

        // GET: api/TransacoesBancarias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoBancaria>> GetTransacaoBancaria(int id)
        {
            var transacao = await _context.TransacoesBancarias
                .Include(t => t.ContaOrigem)
                .Include(t => t.ContaDestino)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transacao == null)
            {
                return NotFound();
            }

            return transacao;
        }

        // POST: api/TransacoesBancarias/simular-iso20022
        [HttpPost("simular-iso20022")]
        public async Task<ActionResult<TransacaoBancaria>> SimularTransacaoISO20022([FromBody] SimulacaoTransacaoRequest request)
        {
            // Validações básicas
            if (request.Valor <= 0)
            {
                return BadRequest("O valor da transação deve ser maior que zero.");
            }

            // Verificar se as contas existem
            var contaOrigem = await _context.ContasBancarias.FindAsync(request.ContaOrigemId);
            var contaDestino = await _context.ContasBancarias.FindAsync(request.ContaDestinoId);

            if (contaOrigem == null)
            {
                return BadRequest("Conta de origem não encontrada.");
            }

            if (contaDestino == null)
            {
                return BadRequest("Conta de destino não encontrada.");
            }

            if (!contaOrigem.Ativa || !contaDestino.Ativa)
            {
                return BadRequest("Uma das contas não está ativa.");
            }

            // Verificar saldo para débito
            if (request.TipoTransacao == TipoTransacao.Transferencia || 
                request.TipoTransacao == TipoTransacao.TED || 
                request.TipoTransacao == TipoTransacao.PIX ||
                request.TipoTransacao == TipoTransacao.DOC)
            {
                if (contaOrigem.Saldo < request.Valor)
                {
                    return BadRequest("Saldo insuficiente na conta de origem.");
                }
            }

            // Criar transação com campos ISO20022
            var transacao = new TransacaoBancaria
            {
                // Gerar IDs únicos conforme ISO20022
                MessageId = GerarMessageId(),
                EndToEndId = GerarEndToEndId(),
                TransactionId = GerarTransactionId(),
                
                TipoTransacao = request.TipoTransacao,
                Status = StatusTransacao.Processando,
                Valor = request.Valor,
                Moeda = "BRL",
                
                ContaOrigemId = request.ContaOrigemId,
                ContaDestinoId = request.ContaDestinoId,
                
                InformacaoRemessa = request.InformacaoRemessa,
                CodigoFinalidade = request.CodigoFinalidade ?? "OTHR", // Other
                CodigoCategoriaFinalidade = request.CodigoCategoriaFinalidade ?? "OTHR",
                Descricao = request.Descricao,
                
                // Preencher informações do devedor e credor
                NomeDevedor = await ObterNomeProprietarioConta(contaOrigem),
                ContaDevedor = $"{contaOrigem.Banco}-{contaOrigem.Agencia}-{contaOrigem.NumeroConta}",
                NomeCredor = await ObterNomeProprietarioConta(contaDestino),
                ContaCredor = $"{contaDestino.Banco}-{contaDestino.Agencia}-{contaDestino.NumeroConta}",
                
                BicAgenteDevedor = $"BANK{contaOrigem.Banco}BR",
                BicAgenteCredor = $"BANK{contaDestino.Banco}BR",
                
                DataSolicitacao = DateTime.UtcNow,
                DataProcessamento = DateTime.UtcNow
            };

            _context.TransacoesBancarias.Add(transacao);

            // Processar a transação
            try
            {
                // Simular processamento
                await Task.Delay(100); // Simular tempo de processamento

                // Atualizar saldos
                if (request.TipoTransacao == TipoTransacao.Transferencia || 
                    request.TipoTransacao == TipoTransacao.TED || 
                    request.TipoTransacao == TipoTransacao.PIX ||
                    request.TipoTransacao == TipoTransacao.DOC)
                {
                    contaOrigem.Saldo -= request.Valor;
                    contaDestino.Saldo += request.Valor;
                }
                else if (request.TipoTransacao == TipoTransacao.Deposito)
                {
                    contaDestino.Saldo += request.Valor;
                }
                else if (request.TipoTransacao == TipoTransacao.Saque)
                {
                    contaOrigem.Saldo -= request.Valor;
                }

                transacao.Status = StatusTransacao.Concluida;
                transacao.DataLiquidacao = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTransacaoBancaria", new { id = transacao.Id }, transacao);
            }
            catch (Exception ex)
            {
                transacao.Status = StatusTransacao.Rejeitada;
                transacao.MotivoRejeicao = ex.Message;
                await _context.SaveChangesAsync();
                
                return BadRequest($"Erro ao processar transação: {ex.Message}");
            }
        }

        private string GerarMessageId()
        {
            return $"MSG{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }

        private string GerarEndToEndId()
        {
            return $"E2E{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000, 99999)}";
        }

        private string GerarTransactionId()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(10000, 99999)}";
        }

        private async Task<string> ObterNomeProprietarioConta(ContaBancaria conta)
        {
            if (conta.PessoaId.HasValue)
            {
                var pessoa = await _context.Pessoas.FindAsync(conta.PessoaId.Value);
                return pessoa?.Nome ?? "Desconhecido";
            }
            else if (conta.EmpresaId.HasValue)
            {
                var empresa = await _context.Empresas.FindAsync(conta.EmpresaId.Value);
                return empresa?.RazaoSocial ?? "Desconhecido";
            }
            
            return "Desconhecido";
        }
    }

    public class SimulacaoTransacaoRequest
    {
        public TipoTransacao TipoTransacao { get; set; }
        public decimal Valor { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        public string? InformacaoRemessa { get; set; }
        public string? CodigoFinalidade { get; set; }
        public string? CodigoCategoriaFinalidade { get; set; }
        public string? Descricao { get; set; }
    }
}