namespace BankingApi.Events
{
    public record PessoaCriada
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string Cpf { get; init; } = string.Empty;
        public string? Rg { get; init; }
        public DateTime DataNascimento { get; init; }
        public string Email { get; init; } = string.Empty;
        public string? Telefone { get; init; }
        public string? Celular { get; init; }
        public int? EnderecoId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}