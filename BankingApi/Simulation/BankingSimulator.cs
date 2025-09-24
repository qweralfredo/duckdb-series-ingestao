using BankingApi.Services;
using BankingApi.Controllers;
using BankingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Simulation
{
    public class BankingSimulator
    {
        private readonly DataGenerators _dataGenerators;
        private readonly ITransacaoBancariaService _transacaoService;
        private readonly SimulationReportService _reportService;
        private readonly ILogger<BankingSimulator> _logger;
        private readonly List<int> _pessoaIds = new();
        private readonly List<int> _empresaIds = new();
        private readonly List<int> _contaIds = new();
        private readonly List<string> _transacaoIds = new();

        public BankingSimulator(
            ITransacaoBancariaService transacaoService, 
            SimulationReportService reportService,
            ILogger<BankingSimulator> logger)
        {
            _dataGenerators = new DataGenerators();
            _transacaoService = transacaoService;
            _reportService = reportService;
            _logger = logger;
        }

        public async Task ExecutarSimulacaoCompleta(int quantidadePessoas = 10, int quantidadeEmpresas = 5, int quantidadeTransacoes = 50)
        {
            var inicioSimulacao = DateTime.UtcNow;
            
            _logger.LogInformation("=== INICIANDO SIMULAÇÃO BANCÁRIA COMPLETA ===");
            _logger.LogInformation("Pessoas: {QtdPessoas}, Empresas: {QtdEmpresas}, Transações: {QtdTransacoes}", 
                quantidadePessoas, quantidadeEmpresas, quantidadeTransacoes);

            try
            {
                // Limpar listas de IDs anteriores
                _pessoaIds.Clear();
                _empresaIds.Clear();
                _contaIds.Clear();
                _transacaoIds.Clear();

                // 1. Criar Pessoas
                await CriarPessoas(quantidadePessoas);
                await Task.Delay(1000); // Aguardar processamento dos eventos

                // 2. Criar Empresas
                await CriarEmpresas(quantidadeEmpresas);
                await Task.Delay(1000);

                // 3. Criar Contas para Pessoas e Empresas
                await CriarContas();
                await Task.Delay(1000);

                // 4. Executar Transações Diversas
                await ExecutarTransacoes(quantidadeTransacoes);
                await Task.Delay(2000);

                // 5. Executar Transações ISO20022
                await ExecutarTransacoesISO20022(quantidadeTransacoes / 2);
                await Task.Delay(2000);

                // 6. Simular operações de alteração e consulta
                await SimularOperacoesAdicionais();

                var fimSimulacao = DateTime.UtcNow;

                _logger.LogInformation("=== SIMULAÇÃO CONCLUÍDA COM SUCESSO ===");
                _logger.LogInformation("Total de pessoas criadas: {PessoasCount}", _pessoaIds.Count);
                _logger.LogInformation("Total de empresas criadas: {EmpresasCount}", _empresaIds.Count);
                _logger.LogInformation("Total de contas criadas: {ContasCount}", _contaIds.Count);
                _logger.LogInformation("Total de transações executadas: {TransacoesCount}", _transacaoIds.Count);

                // Gerar relatório da simulação
                await _reportService.GerarRelatorioSimulacao(
                    inicioSimulacao,
                    fimSimulacao,
                    _pessoaIds.Count,
                    _empresaIds.Count,
                    _contaIds.Count,
                    _transacaoIds.Count,
                    _transacaoIds);

                _logger.LogInformation("Relatório de simulação gerado e salvo com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a simulação bancária");
                
                // Tentar gerar relatório mesmo em caso de erro
                try
                {
                    await _reportService.GerarRelatorioSimulacao(
                        inicioSimulacao,
                        DateTime.UtcNow,
                        _pessoaIds.Count,
                        _empresaIds.Count,
                        _contaIds.Count,
                        _transacaoIds.Count,
                        _transacaoIds);
                }
                catch (Exception reportEx)
                {
                    _logger.LogError(reportEx, "Erro ao gerar relatório de simulação com falha");
                }

                throw;
            }
        }

        private async Task CriarPessoas(int quantidade)
        {
            _logger.LogInformation("Criando {Quantidade} pessoas...", quantidade);

            for (int i = 0; i < quantidade; i++)
            {
                try
                {
                    var pessoa = _dataGenerators.GerarPessoa();
                    
                    _logger.LogInformation("Criando pessoa {Index}: {Nome} - CPF: {Cpf}", 
                        i + 1, pessoa.Nome, pessoa.Cpf);

                    await _transacaoService.CriarPessoa(pessoa);
                    
                    // Simular ID (em produção, viria do evento de resposta)
                    _pessoaIds.Add(i + 1);

                    // Pequena pausa entre criações
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar pessoa {Index}", i + 1);
                }
            }

            _logger.LogInformation("Criação de pessoas concluída. Total: {Total}", _pessoaIds.Count);
        }

        private async Task CriarEmpresas(int quantidade)
        {
            _logger.LogInformation("Criando {Quantidade} empresas...", quantidade);

            for (int i = 0; i < quantidade; i++)
            {
                try
                {
                    var empresa = _dataGenerators.GerarEmpresa();
                    
                    _logger.LogInformation("Criando empresa {Index}: {RazaoSocial} - CNPJ: {Cnpj}", 
                        i + 1, empresa.RazaoSocial, empresa.Cnpj);

                    await _transacaoService.CriarEmpresa(empresa);
                    
                    // Simular ID
                    _empresaIds.Add(i + 1);

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar empresa {Index}", i + 1);
                }
            }

            _logger.LogInformation("Criação de empresas concluída. Total: {Total}", _empresaIds.Count);
        }

        private async Task CriarContas()
        {
            _logger.LogInformation("Criando contas bancárias...");

            // Criar contas para pessoas
            foreach (var pessoaId in _pessoaIds)
            {
                try
                {
                    var conta = _dataGenerators.GerarConta(pessoaId: pessoaId);
                    
                    _logger.LogInformation("Criando conta para pessoa {PessoaId}: {Banco}-{Agencia}-{Conta}", 
                        pessoaId, conta.Banco, conta.Agencia, conta.NumeroConta);

                    await _transacaoService.CriarConta(conta);
                    
                    _contaIds.Add(Random.Shared.Next(1, 10000)); // Simular ID
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar conta para pessoa {PessoaId}", pessoaId);
                }
            }

            // Criar contas para empresas
            foreach (var empresaId in _empresaIds)
            {
                try
                {
                    var conta = _dataGenerators.GerarConta(empresaId: empresaId);
                    
                    _logger.LogInformation("Criando conta para empresa {EmpresaId}: {Banco}-{Agencia}-{Conta}", 
                        empresaId, conta.Banco, conta.Agencia, conta.NumeroConta);

                    await _transacaoService.CriarConta(conta);
                    
                    _contaIds.Add(Random.Shared.Next(1, 10000)); // Simular ID
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar conta para empresa {EmpresaId}", empresaId);
                }
            }

            _logger.LogInformation("Criação de contas concluída. Total: {Total}", _contaIds.Count);
        }

        private async Task ExecutarTransacoes(int quantidade)
        {
            _logger.LogInformation("Executando {Quantidade} transações regulares...", quantidade);

            for (int i = 0; i < quantidade; i++)
            {
                try
                {
                    int? contaOrigemId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;
                    int? contaDestinoId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;

                    var transacao = _dataGenerators.GerarTransacao(contaOrigemId, contaDestinoId);
                    
                    _logger.LogInformation("Executando transação {Index}: {Tipo} - Valor: {Valor:C}", 
                        i + 1, transacao.TipoTransacao, transacao.Valor);

                    var transactionId = await _transacaoService.SolicitarTransacao(transacao);
                    _transacaoIds.Add(transactionId);

                    await Task.Delay(200); // Pausa entre transações
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar transação {Index}", i + 1);
                }
            }

            _logger.LogInformation("Execução de transações regulares concluída. Total: {Total}", quantidade);
        }

        private async Task ExecutarTransacoesISO20022(int quantidade)
        {
            _logger.LogInformation("Executando {Quantidade} transações ISO20022...", quantidade);

            for (int i = 0; i < quantidade; i++)
            {
                try
                {
                    int? contaOrigemId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;
                    int? contaDestinoId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;

                    var transacao = _dataGenerators.GerarTransacaoISO20022(contaOrigemId, contaDestinoId);
                    
                    _logger.LogInformation("Executando transação ISO20022 {Index}: {Tipo} - Valor: {Valor:C}", 
                        i + 1, transacao.TipoTransacao, transacao.Valor);

                    // Simular uso do controller ISO20022
                    var transacaoRequest = new TransacaoRequest
                    {
                        TipoTransacao = transacao.TipoTransacao,
                        Valor = transacao.Valor,
                        Moeda = transacao.Moeda,
                        ContaOrigemId = transacao.ContaOrigemId,
                        ContaDestinoId = transacao.ContaDestinoId,
                        InformacaoRemessa = transacao.InformacaoRemessa,
                        CodigoFinalidade = transacao.CodigoFinalidade,
                        CodigoCategoriaFinalidade = transacao.CodigoCategoriaFinalidade,
                        Descricao = transacao.Descricao,
                        NomeDevedor = transacao.NomeDevedor,
                        ContaDevedor = transacao.ContaDevedor,
                        NomeCredor = transacao.NomeCredor,
                        ContaCredor = transacao.ContaCredor,
                        BicAgenteDevedor = transacao.BicAgenteDevedor,
                        BicAgenteCredor = transacao.BicAgenteCredor
                    };

                    var transactionId = await _transacaoService.SolicitarTransacao(transacaoRequest);
                    _transacaoIds.Add(transactionId);

                    await Task.Delay(300); // Pausa maior para transações ISO20022
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar transação ISO20022 {Index}", i + 1);
                }
            }

            _logger.LogInformation("Execução de transações ISO20022 concluída. Total: {Total}", quantidade);
        }

        private async Task SimularOperacoesAdicionais()
        {
            _logger.LogInformation("Simulando operações adicionais...");

            // Simular diferentes tipos de transação
            var tiposTransacao = Enum.GetValues<TipoTransacao>();
            
            foreach (var tipo in tiposTransacao)
            {
                try
                {
                    _logger.LogInformation("Testando transação do tipo: {TipoTransacao}", tipo);

                    int? contaOrigemId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;
                    int? contaDestinoId = _contaIds.Count > 0 ? _contaIds[Random.Shared.Next(_contaIds.Count)] : (int?)null;

                    var transacao = new TransacaoRequest
                    {
                        TipoTransacao = tipo,
                        Valor = Random.Shared.Next(100, 5000),
                        Moeda = "BRL",
                        ContaOrigemId = contaOrigemId,
                        ContaDestinoId = contaDestinoId,
                        Descricao = $"Teste automático - {tipo}"
                    };

                    var transactionId = await _transacaoService.SolicitarTransacao(transacao);
                    _transacaoIds.Add(transactionId);

                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao testar transação do tipo {TipoTransacao}", tipo);
                }
            }

            _logger.LogInformation("Operações adicionais concluídas.");
        }

        public async Task ExecutarSimulacaoRapida()
        {
            _logger.LogInformation("=== EXECUTANDO SIMULAÇÃO RÁPIDA ===");
            await ExecutarSimulacaoCompleta(5, 2, 20);
        }

        public async Task ExecutarSimulacaoIntensiva()
        {
            _logger.LogInformation("=== EXECUTANDO SIMULAÇÃO INTENSIVA ===");
            await ExecutarSimulacaoCompleta(50, 20, 200);
        }
    }
}