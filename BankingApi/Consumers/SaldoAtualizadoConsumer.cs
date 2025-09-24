using MassTransit;
using BankingApi.Events;

namespace BankingApi.Consumers
{
    public class SaldoAtualizadoConsumer : IConsumer<SaldoAtualizado>
    {
        private readonly ILogger<SaldoAtualizadoConsumer> _logger;

        public SaldoAtualizadoConsumer(ILogger<SaldoAtualizadoConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SaldoAtualizado> context)
        {
            var saldoAtualizado = context.Message;

            _logger.LogInformation("Saldo atualizado para conta {ContaId}: {SaldoAnterior} -> {SaldoNovo} ({TipoOperacao})",
                saldoAtualizado.ContaId,
                saldoAtualizado.SaldoAnterior,
                saldoAtualizado.SaldoNovo,
                saldoAtualizado.TipoOperacao);

            // Aqui você poderia:
            // 1. Atualizar views materializadas
            // 2. Enviar notificações para clientes
            // 3. Registrar em sistemas de auditoria
            // 4. Aplicar regras de compliance
            // 5. Atualizar cache

            await Task.CompletedTask;
        }
    }
}