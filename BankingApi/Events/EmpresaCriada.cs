namespace BankingApi.Events
{
    public record EmpresaCriada
    {
        public int Id { get; init; }
        public string RazaoSocial { get; init; } = string.Empty;
        public string? NomeFantasia { get; init; }
        public string Cnpj { get; init; } = string.Empty;
        public string? InscricaoEstadual { get; init; }
        public string? InscricaoMunicipal { get; init; }
        public string Email { get; init; } = string.Empty;
        public string? Telefone { get; init; }
        public string? Website { get; init; }
        public int? EnderecoId { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}