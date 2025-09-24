using System.Text.Json;
using BankingApi.Services;

namespace BankingApi.Simulation
{
    public class SimulationReportService
    {
        private readonly ILogger<SimulationReportService> _logger;
        private readonly IMinIOService? _minioService;
        private readonly IConfiguration _configuration;
        
        public SimulationReportService(
            ILogger<SimulationReportService> logger,
            IConfiguration configuration,
            IMinIOService? minioService = null)
        {
            _logger = logger;
            _minioService = minioService;
            _configuration = configuration;
        }

        public async Task GerarRelatorioSimulacao(
            DateTime inicioSimulacao,
            DateTime fimSimulacao,
            int totalPessoas,
            int totalEmpresas,
            int totalContas,
            int totalTransacoes,
            List<string> transacaoIds)
        {
            var relatorio = new SimulationReport
            {
                DataHoraInicio = inicioSimulacao,
                DataHoraFim = fimSimulacao,
                DuracaoTotal = fimSimulacao - inicioSimulacao,
                Estatisticas = new SimulationStatistics
                {
                    TotalPessoas = totalPessoas,
                    TotalEmpresas = totalEmpresas,
                    TotalContas = totalContas,
                    TotalTransacoes = totalTransacoes,
                    TransacoesPorMinuto = CalcularTransacoesPorMinuto(totalTransacoes, fimSimulacao - inicioSimulacao),
                    TransacoesPorSegundo = CalcularTransacoesPorSegundo(totalTransacoes, fimSimulacao - inicioSimulacao)
                },
                TransacaoIds = transacaoIds,
                MetricasPerformance = new PerformanceMetrics
                {
                    TempoMedioTransacao = CalcularTempoMedioTransacao(totalTransacoes, fimSimulacao - inicioSimulacao),
                    ThroughputTransacoes = CalcularThroughput(totalTransacoes, fimSimulacao - inicioSimulacao)
                }
            };

            // Salvar relatório localmente
            await SalvarRelatorioJson(relatorio);

            // Salvar no MinIO se configurado
            if (_minioService != null && _configuration.GetValue<bool>("Simulation:SaveToMinIO", false))
            {
                await SalvarRelatorioMinIO(relatorio);
            }

            // Gerar sumário no log
            GerarSumarioLog(relatorio);
        }

        private double CalcularTransacoesPorMinuto(int totalTransacoes, TimeSpan duracao)
        {
            if (duracao.TotalMinutes == 0) return 0;
            return totalTransacoes / duracao.TotalMinutes;
        }

        private double CalcularTransacoesPorSegundo(int totalTransacoes, TimeSpan duracao)
        {
            if (duracao.TotalSeconds == 0) return 0;
            return totalTransacoes / duracao.TotalSeconds;
        }

        private double CalcularTempoMedioTransacao(int totalTransacoes, TimeSpan duracao)
        {
            if (totalTransacoes == 0) return 0;
            return duracao.TotalMilliseconds / totalTransacoes;
        }

        private double CalcularThroughput(int totalTransacoes, TimeSpan duracao)
        {
            if (duracao.TotalHours == 0) return 0;
            return totalTransacoes / duracao.TotalHours;
        }

        private async Task SalvarRelatorioJson(SimulationReport relatorio)
        {
            try
            {
                var reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "SimulationReports");
                Directory.CreateDirectory(reportDirectory);

                var fileName = $"simulation_report_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(reportDirectory, fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = JsonSerializer.Serialize(relatorio, options);
                await File.WriteAllTextAsync(filePath, jsonContent);

                _logger.LogInformation("Relatório de simulação salvo localmente em: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar relatório de simulação localmente");
            }
        }

        private async Task SalvarRelatorioMinIO(SimulationReport relatorio)
        {
            try
            {
                if (_minioService == null) return;

                var bucketName = _configuration.GetValue<string>("MinIO:BucketReports", "simulation-reports");
                var objectName = $"reports/{DateTime.Now:yyyy/MM/dd}/simulation_report_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                var url = await _minioService.UploadJsonAsync(bucketName, objectName, relatorio);

                _logger.LogInformation("Relatório de simulação salvo no MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                _logger.LogInformation("URL do relatório: {Url}", url);

                // Também salvar um relatório resumido
                var resumo = new SimulationSummary
                {
                    Id = Guid.NewGuid(),
                    DataHora = relatorio.DataHoraInicio,
                    DuracaoSegundos = (int)relatorio.DuracaoTotal.TotalSeconds,
                    TotalEntidades = relatorio.Estatisticas.TotalPessoas + relatorio.Estatisticas.TotalEmpresas,
                    TotalTransacoes = relatorio.Estatisticas.TotalTransacoes,
                    Throughput = relatorio.Estatisticas.TransacoesPorSegundo,
                    Status = "Completed"
                };

                var resumoObjectName = $"summaries/{DateTime.Now:yyyy/MM}/simulation_summary_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                await _minioService.UploadJsonAsync(bucketName, resumoObjectName, resumo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar relatório de simulação no MinIO");
            }
        }

        private void GerarSumarioLog(SimulationReport relatorio)
        {
            _logger.LogInformation("=== RELATÓRIO DE SIMULAÇÃO ===");
            _logger.LogInformation("Data/Hora Início: {Inicio}", relatorio.DataHoraInicio);
            _logger.LogInformation("Data/Hora Fim: {Fim}", relatorio.DataHoraFim);
            _logger.LogInformation("Duração Total: {Duracao}", relatorio.DuracaoTotal);
            _logger.LogInformation("Total de Pessoas: {Pessoas}", relatorio.Estatisticas.TotalPessoas);
            _logger.LogInformation("Total de Empresas: {Empresas}", relatorio.Estatisticas.TotalEmpresas);
            _logger.LogInformation("Total de Contas: {Contas}", relatorio.Estatisticas.TotalContas);
            _logger.LogInformation("Total de Transações: {Transacoes}", relatorio.Estatisticas.TotalTransacoes);
            _logger.LogInformation("Transações por Minuto: {TPM:F2}", relatorio.Estatisticas.TransacoesPorMinuto);
            _logger.LogInformation("Transações por Segundo: {TPS:F2}", relatorio.Estatisticas.TransacoesPorSegundo);
            _logger.LogInformation("Tempo Médio por Transação: {TMT:F2} ms", relatorio.MetricasPerformance.TempoMedioTransacao);
            _logger.LogInformation("Throughput: {Throughput:F2} transações/hora", relatorio.MetricasPerformance.ThroughputTransacoes);
            _logger.LogInformation("================================");
        }

        public async Task<IEnumerable<SimulationSummary>> ListarRelatorios(int limite = 50)
        {
            try
            {
                if (_minioService == null) return Enumerable.Empty<SimulationSummary>();

                var bucketName = _configuration.GetValue<string>("MinIO:BucketReports", "simulation-reports");
                var files = await _minioService.ListFilesAsync(bucketName, "summaries/");

                var summaries = new List<SimulationSummary>();

                foreach (var file in files.Take(limite))
                {
                    try
                    {
                        var summary = await _minioService.DownloadJsonAsync<SimulationSummary>(bucketName, file);
                        if (summary != null)
                        {
                            summaries.Add(summary);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erro ao ler relatório: {File}", file);
                    }
                }

                return summaries.OrderByDescending(s => s.DataHora);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar relatórios do MinIO");
                return Enumerable.Empty<SimulationSummary>();
            }
        }

        public async Task<SimulationReport?> ObterRelatorio(string fileName)
        {
            try
            {
                if (_minioService == null) return null;

                var bucketName = _configuration.GetValue<string>("MinIO:BucketReports", "simulation-reports");
                var objectName = $"reports/{fileName}";

                return await _minioService.DownloadJsonAsync<SimulationReport>(bucketName, objectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório do MinIO: {FileName}", fileName);
                return null;
            }
        }
    }

    public class SimulationReport
    {
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public TimeSpan DuracaoTotal { get; set; }
        public SimulationStatistics Estatisticas { get; set; } = new();
        public PerformanceMetrics MetricasPerformance { get; set; } = new();
        public List<string> TransacaoIds { get; set; } = new();
    }

    public class SimulationStatistics
    {
        public int TotalPessoas { get; set; }
        public int TotalEmpresas { get; set; }
        public int TotalContas { get; set; }
        public int TotalTransacoes { get; set; }
        public double TransacoesPorMinuto { get; set; }
        public double TransacoesPorSegundo { get; set; }
    }

    public class PerformanceMetrics
    {
        public double TempoMedioTransacao { get; set; }
        public double ThroughputTransacoes { get; set; }
    }

    public class SimulationSummary
    {
        public Guid Id { get; set; }
        public DateTime DataHora { get; set; }
        public int DuracaoSegundos { get; set; }
        public int TotalEntidades { get; set; }
        public int TotalTransacoes { get; set; }
        public double Throughput { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}