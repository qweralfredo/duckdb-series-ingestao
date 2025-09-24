using Bogus;
using BankingApi.Models;
using BankingApi.Services;
using BankingApi.Controllers;

namespace BankingApi.Simulation
{
    public class DataGenerators
    {
        private readonly Faker _faker;
        
        public DataGenerators()
        {
            _faker = new Faker("pt_BR"); // Localização brasileira
        }

        public EnderecoRequest GerarEndereco()
        {
            return new Faker<EnderecoRequest>("pt_BR")
                .RuleFor(e => e.Logradouro, f => f.Address.StreetName())
                .RuleFor(e => e.Numero, f => f.Address.BuildingNumber())
                .RuleFor(e => e.Complemento, f => f.Random.Bool(0.3f) ? f.Address.SecondaryAddress() : null)
                .RuleFor(e => e.Bairro, f => f.Address.County())
                .RuleFor(e => e.Cidade, f => f.Address.City())
                .RuleFor(e => e.Estado, f => f.Address.StateAbbr())
                .RuleFor(e => e.Cep, f => f.Address.ZipCode("########"))
                .RuleFor(e => e.Pais, f => "Brasil")
                .Generate();
        }

        public PessoaRequest GerarPessoa(int? enderecoId = null)
        {
            return new Faker<PessoaRequest>("pt_BR")
                .RuleFor(p => p.Nome, f => f.Name.FullName())
                .RuleFor(p => p.Cpf, f => GerarCpf())
                .RuleFor(p => p.Rg, f => f.Random.Bool(0.8f) ? f.Random.Replace("##.###.###-#") : null)
                .RuleFor(p => p.DataNascimento, f => f.Date.Past(80, DateTime.Now.AddYears(-18)))
                .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.Nome.ToLower()))
                .RuleFor(p => p.Telefone, f => f.Random.Bool(0.7f) ? f.Phone.PhoneNumber("(##) ####-####") : null)
                .RuleFor(p => p.Celular, f => f.Phone.PhoneNumber("(##) #####-####"))
                .RuleFor(p => p.EnderecoId, f => enderecoId)
                .Generate();
        }

        public EmpresaRequest GerarEmpresa(int? enderecoId = null)
        {
            return new Faker<EmpresaRequest>("pt_BR")
                .RuleFor(e => e.RazaoSocial, f => f.Company.CompanyName() + " LTDA")
                .RuleFor(e => e.NomeFantasia, f => f.Random.Bool(0.7f) ? f.Company.CompanyName() : null)
                .RuleFor(e => e.Cnpj, f => GerarCnpj())
                .RuleFor(e => e.InscricaoEstadual, f => f.Random.Bool(0.8f) ? f.Random.Replace("###.###.###.###") : null)
                .RuleFor(e => e.InscricaoMunicipal, f => f.Random.Bool(0.6f) ? f.Random.Replace("########") : null)
                .RuleFor(e => e.Email, (f, e) => f.Internet.Email(e.RazaoSocial.ToLower().Replace(" ", "")))
                .RuleFor(e => e.Telefone, f => f.Phone.PhoneNumber("(##) ####-####"))
                .RuleFor(e => e.Website, f => f.Random.Bool(0.5f) ? f.Internet.Url() : null)
                .RuleFor(e => e.EnderecoId, f => enderecoId)
                .Generate();
        }

        public ContaRequest GerarConta(int? pessoaId = null, int? empresaId = null)
        {
            var bancos = new[] { "001", "033", "104", "237", "341", "389", "422" };
            
            return new Faker<ContaRequest>("pt_BR")
                .RuleFor(c => c.Banco, f => f.PickRandom(bancos))
                .RuleFor(c => c.Agencia, f => f.Random.Number(1000, 9999).ToString())
                .RuleFor(c => c.NumeroConta, f => f.Random.Number(10000, 999999).ToString())
                .RuleFor(c => c.DigitoVerificador, f => f.Random.Number(0, 9).ToString())
                .RuleFor(c => c.TipoConta, f => f.PickRandom<TipoConta>())
                .RuleFor(c => c.SaldoInicial, f => f.Random.Decimal(0, 50000))
                .RuleFor(c => c.PessoaId, f => pessoaId)
                .RuleFor(c => c.EmpresaId, f => empresaId)
                .Generate();
        }

        public TransacaoRequest GerarTransacao(int? contaOrigemId = null, int? contaDestinoId = null)
        {
            return new Faker<TransacaoRequest>("pt_BR")
                .RuleFor(t => t.TipoTransacao, f => f.PickRandom<TipoTransacao>())
                .RuleFor(t => t.Valor, f => f.Random.Decimal(10, 10000))
                .RuleFor(t => t.Moeda, f => "BRL")
                .RuleFor(t => t.ContaOrigemId, f => contaOrigemId ?? f.Random.Number(1, 100))
                .RuleFor(t => t.ContaDestinoId, f => contaDestinoId ?? f.Random.Number(1, 100))
                .RuleFor(t => t.InformacaoRemessa, f => f.Random.Bool(0.6f) ? f.Lorem.Sentence(3, 8) : null)
                .RuleFor(t => t.CodigoFinalidade, f => f.PickRandom("OTHR", "SALA", "GOVT", "TRAD", "RLTI"))
                .RuleFor(t => t.CodigoCategoriaFinalidade, f => f.PickRandom("OTHR", "SALA", "GOVT"))
                .RuleFor(t => t.Descricao, f => f.Random.Bool(0.8f) ? f.Lorem.Sentence(2, 6) : null)
                .RuleFor(t => t.NomeDevedor, f => f.Name.FullName())
                .RuleFor(t => t.ContaDevedor, f => $"{f.Random.Number(100, 999)}-{f.Random.Number(1000, 9999)}-{f.Random.Number(10000, 999999)}")
                .RuleFor(t => t.NomeCredor, f => f.Name.FullName())
                .RuleFor(t => t.ContaCredor, f => $"{f.Random.Number(100, 999)}-{f.Random.Number(1000, 9999)}-{f.Random.Number(10000, 999999)}")
                .RuleFor(t => t.BicAgenteDevedor, f => $"BANK{f.Random.Number(100, 999)}BR")
                .RuleFor(t => t.BicAgenteCredor, f => $"BANK{f.Random.Number(100, 999)}BR")
                .Generate();
        }

        public SimulacaoISO20022Request GerarTransacaoISO20022(int? contaOrigemId = null, int? contaDestinoId = null)
        {
            return new Faker<SimulacaoISO20022Request>("pt_BR")
                .RuleFor(t => t.TipoTransacao, f => f.PickRandom<TipoTransacao>())
                .RuleFor(t => t.Valor, f => f.Random.Decimal(10, 10000))
                .RuleFor(t => t.Moeda, f => "BRL")
                .RuleFor(t => t.ContaOrigemId, f => contaOrigemId ?? f.Random.Number(1, 100))
                .RuleFor(t => t.ContaDestinoId, f => contaDestinoId ?? f.Random.Number(1, 100))
                .RuleFor(t => t.InformacaoRemessa, f => f.Random.Bool(0.6f) ? f.Lorem.Sentence(3, 8) : null)
                .RuleFor(t => t.CodigoFinalidade, f => f.PickRandom("OTHR", "SALA", "GOVT", "TRAD", "RLTI"))
                .RuleFor(t => t.CodigoCategoriaFinalidade, f => f.PickRandom("OTHR", "SALA", "GOVT"))
                .RuleFor(t => t.Descricao, f => f.Random.Bool(0.8f) ? f.Lorem.Sentence(2, 6) : null)
                .RuleFor(t => t.NomeDevedor, f => f.Name.FullName())
                .RuleFor(t => t.ContaDevedor, f => $"{f.Random.Number(100, 999)}-{f.Random.Number(1000, 9999)}-{f.Random.Number(10000, 999999)}")
                .RuleFor(t => t.NomeCredor, f => f.Name.FullName())
                .RuleFor(t => t.ContaCredor, f => $"{f.Random.Number(100, 999)}-{f.Random.Number(1000, 9999)}-{f.Random.Number(10000, 999999)}")
                .RuleFor(t => t.BicAgenteDevedor, f => $"BANK{f.Random.Number(100, 999)}BR")
                .RuleFor(t => t.BicAgenteCredor, f => $"BANK{f.Random.Number(100, 999)}BR")
                .Generate();
        }

        private string GerarCpf()
        {
            var cpf = new int[11];
            var random = new Random();

            for (int i = 0; i < 9; i++)
            {
                cpf[i] = random.Next(0, 10);
            }

            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += cpf[i] * (10 - i);
            }
            int resto = soma % 11;
            cpf[9] = resto < 2 ? 0 : 11 - resto;

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += cpf[i] * (11 - i);
            }
            resto = soma % 11;
            cpf[10] = resto < 2 ? 0 : 11 - resto;

            return string.Join("", cpf);
        }

        private string GerarCnpj()
        {
            var cnpj = new int[14];
            var random = new Random();

            for (int i = 0; i < 12; i++)
            {
                cnpj[i] = random.Next(0, 10);
            }

            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            for (int i = 0; i < 12; i++)
            {
                soma += cnpj[i] * multiplicadores1[i];
            }
            int resto = soma % 11;
            cnpj[12] = resto < 2 ? 0 : 11 - resto;

            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            for (int i = 0; i < 13; i++)
            {
                soma += cnpj[i] * multiplicadores2[i];
            }
            resto = soma % 11;
            cnpj[13] = resto < 2 ? 0 : 11 - resto;

            return string.Join("", cnpj);
        }
    }

    public class EnderecoRequest
    {
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
    }
}