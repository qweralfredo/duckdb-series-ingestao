using MassTransit;
using BankingApi.Events;

namespace BankingApi.Consumers
{
    public class TransacaoProcessadaConsumer : IConsumer<TransacaoProcessada>
    {
        private readonly ILogger<TransacaoProcessadaConsumer> _logger;

        public TransacaoProcessadaConsumer(ILogger<TransacaoProcessadaConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TransacaoProcessada> context)
        {
            var transacao = context.Message;

            _logger.LogInformation("Transação processada: {TransactionId} - Status: {Status}",
                transacao.TransactionId, transacao.Status);

            // Aqui você poderia:
            // 1. Atualizar sistemas de relatório
            // 2. Enviar notificações ao cliente
            // 3. Integrar com sistemas externos
            // 4. Aplicar regras de auditoria e compliance
            // 5. Atualizar dashboards em tempo real

            if (transacao.Status == Models.StatusTransacao.Rejeitada)
            {
                _logger.LogWarning("Transação rejeitada {TransactionId}: {Motivo}",
                    transacao.TransactionId, transacao.MotivoRejeicao);
            }

            await Task.CompletedTask;
        }
    }
}