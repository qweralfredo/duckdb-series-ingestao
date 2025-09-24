using MassTransit;
using BankingApi.Events;

namespace BankingApi.Services
{
    public class BankingEventContext : IBankingEventContext
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BankingEventContext> _logger;

        public BankingEventContext(IPublishEndpoint publishEndpoint, ILogger<BankingEventContext> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublicarTransacaoSolicitada(TransacaoSolicitada evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento TransacaoSolicitada publicado: {TransactionId}", evento.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar TransacaoSolicitada: {TransactionId}", evento.TransactionId);
                throw;
            }
        }

        public async Task PublicarTransacaoProcessada(TransacaoProcessada evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento TransacaoProcessada publicado: {TransactionId}", evento.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar TransacaoProcessada: {TransactionId}", evento.TransactionId);
                throw;
            }
        }

        public async Task PublicarContaCriada(ContaCriada evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento ContaCriada publicado: {ContaId}", evento.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar ContaCriada: {ContaId}", evento.Id);
                throw;
            }
        }

        public async Task PublicarSaldoAtualizado(SaldoAtualizado evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento SaldoAtualizado publicado: {ContaId} - Saldo: {SaldoNovo}", 
                    evento.ContaId, evento.SaldoNovo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar SaldoAtualizado: {ContaId}", evento.ContaId);
                throw;
            }
        }

        public async Task PublicarPessoaCriada(PessoaCriada evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento PessoaCriada publicado: {PessoaId}", evento.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar PessoaCriada: {PessoaId}", evento.Id);
                throw;
            }
        }

        public async Task PublicarEmpresaCriada(EmpresaCriada evento, CancellationToken cancellationToken = default)
        {
            try
            {
                await _publishEndpoint.Publish(evento, cancellationToken);
                _logger.LogInformation("Evento EmpresaCriada publicado: {EmpresaId}", evento.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar EmpresaCriada: {EmpresaId}", evento.Id);
                throw;
            }
        }

        public async Task PublicarEventosLote(IEnumerable<object> eventos, CancellationToken cancellationToken = default)
        {
            try
            {
                var tasks = eventos.Select(evento => _publishEndpoint.Publish(evento, cancellationToken));
                await Task.WhenAll(tasks);
                _logger.LogInformation("Lote de {Count} eventos publicado com sucesso", eventos.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar lote de eventos");
                throw;
            }
        }
    }
}