using BankingApi.Models;

namespace BankingApi.Services
{
    public interface ITransacaoBancariaService
    {
        Task<string> SolicitarTransacao(TransacaoRequest request);
        Task CriarConta(ContaRequest request);
        Task CriarPessoa(PessoaRequest request);
        Task CriarEmpresa(EmpresaRequest request);
    }

    public class TransacaoRequest
    {
        public TipoTransacao TipoTransacao { get; set; }
        public decimal Valor { get; set; }
        public string? Moeda { get; set; }
        public int? ContaOrigemId { get; set; }
        public int? ContaDestinoId { get; set; }
        public string? InformacaoRemessa { get; set; }
        public string? CodigoFinalidade { get; set; }
        public string? CodigoCategoriaFinalidade { get; set; }
        public string? Descricao { get; set; }
        public string? NomeDevedor { get; set; }
        public string? ContaDevedor { get; set; }
        public string? NomeCredor { get; set; }
        public string? ContaCredor { get; set; }
        public string? BicAgenteDevedor { get; set; }
        public string? BicAgenteCredor { get; set; }
    }

    public class ContaRequest
    {
        public string Banco { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string NumeroConta { get; set; } = string.Empty;
        public string? DigitoVerificador { get; set; }
        public TipoConta TipoConta { get; set; }
        public decimal SaldoInicial { get; set; }
        public int? PessoaId { get; set; }
        public int? EmpresaId { get; set; }
    }

    public class PessoaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? Rg { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public int? EnderecoId { get; set; }
    }

    public class EmpresaRequest
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string? InscricaoEstadual { get; set; }
        public string? InscricaoMunicipal { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string? Website { get; set; }
        public int? EnderecoId { get; set; }
    }
}