namespace BankingApi.Events
{
    public record SaldoAtualizado
    {
        public int ContaId { get; init; }
        public string Banco { get; init; } = string.Empty;
        public string Agencia { get; init; } = string.Empty;
        public string NumeroConta { get; init; } = string.Empty;
        public decimal SaldoAnterior { get; init; }
        public decimal SaldoNovo { get; init; }
        public decimal ValorMovimentacao { get; init; }
        public string TipoOperacao { get; init; } = string.Empty; // DEBITO, CREDITO
        public string? TransacaoId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}