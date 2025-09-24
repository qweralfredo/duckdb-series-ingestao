using MassTransit;
using BankingApi.Events;
using BankingApi.Services;

namespace BankingApi.Consumers
{
    public class TransacaoSolicitadaConsumer : IConsumer<TransacaoSolicitada>
    {
        private readonly IBankingEventContext _eventContext;
        private readonly ILogger<TransacaoSolicitadaConsumer> _logger;

        public TransacaoSolicitadaConsumer(IBankingEventContext eventContext, ILogger<TransacaoSolicitadaConsumer> logger)
        {
            _eventContext = eventContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransacaoSolicitada> context)
        {
            var transacao = context.Message;
            
            _logger.LogInformation("Processando transação {TransactionId} do tipo {TipoTransacao}", 
                transacao.TransactionId, transacao.TipoTransacao);

            try
            {
                // Simular validações de negócio
                await ValidarTransacao(transacao);

                // Simular processamento
                await Task.Delay(100); // Simular tempo de processamento

                var eventoProcessado = new TransacaoProcessada
                {
                    MessageId = transacao.MessageId,
                    EndToEndId = transacao.EndToEndId,
                    TransactionId = transacao.TransactionId,
                    Status = Models.StatusTransacao.Concluida,
                    Valor = transacao.Valor,
                    Moeda = transacao.Moeda,
                    ContaOrigemId = transacao.ContaOrigemId,
                    ContaDestinoId = transacao.ContaDestinoId,
                    DataProcessamento = DateTime.UtcNow,
                    DataLiquidacao = DateTime.UtcNow
                };

                // Publicar eventos de saldo atualizado
                var eventosLote = new List<object>();
                eventosLote.Add(eventoProcessado);

                if (transacao.ContaOrigemId.HasValue && DeveLancarDebito(transacao.TipoTransacao))
                {
                    eventosLote.Add(new SaldoAtualizado
                    {
                        ContaId = transacao.ContaOrigemId.Value,
                        Banco = "001", // Simulado
                        Agencia = "0001",
                        NumeroConta = "123456",
                        SaldoAnterior = 1000, // Simulado
                        SaldoNovo = 1000 - transacao.Valor,
                        ValorMovimentacao = -transacao.Valor,
                        TipoOperacao = "DEBITO",
                        TransacaoId = transacao.TransactionId
                    });
                }

                if (transacao.ContaDestinoId.HasValue && DeveLancarCredito(transacao.TipoTransacao))
                {
                    eventosLote.Add(new SaldoAtualizado
                    {
                        ContaId = transacao.ContaDestinoId.Value,
                        Banco = "001", // Simulado
                        Agencia = "0001",
                        NumeroConta = "654321",
                        SaldoAnterior = 500, // Simulado
                        SaldoNovo = 500 + transacao.Valor,
                        ValorMovimentacao = transacao.Valor,
                        TipoOperacao = "CREDITO",
                        TransacaoId = transacao.TransactionId
                    });
                }

                await _eventContext.PublicarEventosLote(eventosLote);

                _logger.LogInformation("Transação {TransactionId} processada com sucesso", transacao.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar transação {TransactionId}", transacao.TransactionId);

                var eventoRejeitado = new TransacaoProcessada
                {
                    MessageId = transacao.MessageId,
                    EndToEndId = transacao.EndToEndId,
                    TransactionId = transacao.TransactionId,
                    Status = Models.StatusTransacao.Rejeitada,
                    Valor = transacao.Valor,
                    Moeda = transacao.Moeda,
                    ContaOrigemId = transacao.ContaOrigemId,
                    ContaDestinoId = transacao.ContaDestinoId,
                    MotivoRejeicao = ex.Message,
                    DataProcessamento = DateTime.UtcNow
                };

                await _eventContext.PublicarTransacaoProcessada(eventoRejeitado);
            }
        }

        private async Task ValidarTransacao(TransacaoSolicitada transacao)
        {
            // Simular validações
            if (transacao.Valor <= 0)
            {
                throw new InvalidOperationException("Valor da transação deve ser maior que zero");
            }

            if (string.IsNullOrEmpty(transacao.Moeda))
            {
                throw new InvalidOperationException("Moeda é obrigatória");
            }

            await Task.CompletedTask;
        }

        private static bool DeveLancarDebito(Models.TipoTransacao tipo) =>
            tipo is Models.TipoTransacao.Transferencia or Models.TipoTransacao.TED or 
                   Models.TipoTransacao.PIX or Models.TipoTransacao.DOC or 
                   Models.TipoTransacao.Saque or Models.TipoTransacao.Pagamento;

        private static bool DeveLancarCredito(Models.TipoTransacao tipo) =>
            tipo is Models.TipoTransacao.Transferencia or Models.TipoTransacao.TED or 
                   Models.TipoTransacao.PIX or Models.TipoTransacao.DOC or 
                   Models.TipoTransacao.Deposito;
    }
}