using BankingApi.Models;

namespace BankingApi.Events
{
    public record TransacaoProcessada
    {
        public string MessageId { get; init; } = string.Empty;
        public string EndToEndId { get; init; } = string.Empty;
        public string TransactionId { get; init; } = string.Empty;
        public StatusTransacao Status { get; init; }
        public decimal Valor { get; init; }
        public string Moeda { get; init; } = "BRL";
        public int? ContaOrigemId { get; init; }
        public int? ContaDestinoId { get; init; }
        public decimal? SaldoOrigemAnterior { get; init; }
        public decimal? SaldoOrigemNovo { get; init; }
        public decimal? SaldoDestinoAnterior { get; init; }
        public decimal? SaldoDestinoNovo { get; init; }
        public DateTime? DataProcessamento { get; init; }
        public DateTime? DataLiquidacao { get; init; }
        public string? MotivoRejeicao { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}