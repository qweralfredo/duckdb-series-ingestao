using BankingApi.Events;

namespace BankingApi.Services
{
    public interface IBankingEventContext
    {
        Task PublicarTransacaoSolicitada(TransacaoSolicitada evento, CancellationToken cancellationToken = default);
        Task PublicarTransacaoProcessada(TransacaoProcessada evento, CancellationToken cancellationToken = default);
        Task PublicarContaCriada(ContaCriada evento, CancellationToken cancellationToken = default);
        Task PublicarSaldoAtualizado(SaldoAtualizado evento, CancellationToken cancellationToken = default);
        Task PublicarPessoaCriada(PessoaCriada evento, CancellationToken cancellationToken = default);
        Task PublicarEmpresaCriada(EmpresaCriada evento, CancellationToken cancellationToken = default);
        Task PublicarEventosLote(IEnumerable<object> eventos, CancellationToken cancellationToken = default);
    }
}