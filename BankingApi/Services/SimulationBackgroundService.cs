using BankingApi.Simulation;

namespace BankingApi.Services
{
    public class SimulationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SimulationBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public SimulationBackgroundService(
            IServiceProvider serviceProvider, 
            ILogger<SimulationBackgroundService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Verificar se a simulação automática está habilitada
            var autoRunEnabled = _configuration.GetValue<bool>("Simulation:AutoRun", false);
            
            if (!autoRunEnabled)
            {
                _logger.LogInformation("Simulação automática desabilitada. Use os endpoints da API para executar manualmente.");
                return;
            }

            _logger.LogInformation("Serviço de simulação iniciado. Aguardando 30 segundos antes da primeira execução...");

            // Aguardar a aplicação inicializar completamente
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var simulator = scope.ServiceProvider.GetRequiredService<BankingSimulator>();

                    _logger.LogInformation("Executando simulação automática...");
                    
                    await simulator.ExecutarSimulacaoRapida();

                    _logger.LogInformation("Simulação automática concluída. Próxima execução em 10 minutos.");
                    
                    // Aguardar 10 minutos antes da próxima execução
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Serviço de simulação foi cancelado.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante execução automática da simulação");
                    
                    // Em caso de erro, aguardar 2 minutos antes de tentar novamente
                    await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                }
            }
        }
    }
}