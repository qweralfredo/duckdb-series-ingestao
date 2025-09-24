namespace BankingApi.Events
{
    public record ContaCriada
    {
        public int Id { get; init; }
        public string Banco { get; init; } = string.Empty;
        public string Agencia { get; init; } = string.Empty;
        public string NumeroConta { get; init; } = string.Empty;
        public string? DigitoVerificador { get; init; }
        public string TipoConta { get; init; } = string.Empty;
        public decimal SaldoInicial { get; init; }
        public bool Ativa { get; init; }
        public int? PessoaId { get; init; }
        public int? EmpresaId { get; init; }
        public DateTime DataAbertura { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}