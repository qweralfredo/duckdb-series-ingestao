using BankingApi.Models;

namespace BankingApi.Events
{
    public record TransacaoSolicitada
    {
        public string MessageId { get; init; } = string.Empty;
        public string EndToEndId { get; init; } = string.Empty;
        public string TransactionId { get; init; } = string.Empty;
        public TipoTransacao TipoTransacao { get; init; }
        public decimal Valor { get; init; }
        public string Moeda { get; init; } = "BRL";
        public int? ContaOrigemId { get; init; }
        public int? ContaDestinoId { get; init; }
        public string? InformacaoRemessa { get; init; }
        public string? CodigoFinalidade { get; init; }
        public string? CodigoCategoriaFinalidade { get; init; }
        public string? Descricao { get; init; }
        public string? NomeDevedor { get; init; }
        public string? ContaDevedor { get; init; }
        public string? NomeCredor { get; init; }
        public string? ContaCredor { get; init; }
        public string? BicAgenteDevedor { get; init; }
        public string? BicAgenteCredor { get; init; }
        public DateTime DataSolicitacao { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}