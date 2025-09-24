using Microsoft.AspNetCore.Mvc;
using BankingApi.Simulation;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : ControllerBase
    {
        private readonly BankingSimulator _simulator;
        private readonly ILogger<SimulationController> _logger;

        public SimulationController(BankingSimulator simulator, ILogger<SimulationController> logger)
        {
            _simulator = simulator;
            _logger = logger;
        }

        /// <summary>
        /// Executa uma simulação rápida com poucos dados para teste
        /// </summary>
        /// <returns>Resultado da simulação</returns>
        [HttpPost("rapida")]
        public async Task<ActionResult<SimulationResult>> ExecutarSimulacaoRapida()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                _logger.LogInformation("Iniciando simulação rápida às {StartTime}", startTime);

                await _simulator.ExecutarSimulacaoRapida();

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var result = new SimulationResult
                {
                    TipoSimulacao = "Rápida",
                    IniciadaEm = startTime,
                    ConcluidaEm = endTime,
                    DuracaoSegundos = (int)duration.TotalSeconds,
                    Status = "Concluída com Sucesso",
                    Mensagem = "Simulação rápida executada com sucesso. Verifique os logs para detalhes."
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante simulação rápida");
                return BadRequest(new SimulationResult
                {
                    TipoSimulacao = "Rápida",
                    Status = "Erro",
                    Mensagem = $"Erro durante a simulação: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Executa uma simulação completa com quantidade customizada de dados
        /// </summary>
        /// <param name="request">Parâmetros da simulação</param>
        /// <returns>Resultado da simulação</returns>
        [HttpPost("completa")]
        public async Task<ActionResult<SimulationResult>> ExecutarSimulacaoCompleta([FromBody] SimulationRequest request)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                _logger.LogInformation("Iniciando simulação completa às {StartTime} com parâmetros: {Request}", 
                    startTime, System.Text.Json.JsonSerializer.Serialize(request));

                await _simulator.ExecutarSimulacaoCompleta(
                    request.QuantidadePessoas, 
                    request.QuantidadeEmpresas, 
                    request.QuantidadeTransacoes);

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var result = new SimulationResult
                {
                    TipoSimulacao = "Completa",
                    IniciadaEm = startTime,
                    ConcluidaEm = endTime,
                    DuracaoSegundos = (int)duration.TotalSeconds,
                    Status = "Concluída com Sucesso",
                    Mensagem = $"Simulação completa executada: {request.QuantidadePessoas} pessoas, " +
                              $"{request.QuantidadeEmpresas} empresas, {request.QuantidadeTransacoes} transações.",
                    QuantidadePessoas = request.QuantidadePessoas,
                    QuantidadeEmpresas = request.QuantidadeEmpresas,
                    QuantidadeTransacoes = request.QuantidadeTransacoes
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante simulação completa");
                return BadRequest(new SimulationResult
                {
                    TipoSimulacao = "Completa",
                    Status = "Erro",
                    Mensagem = $"Erro durante a simulação: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Executa uma simulação intensiva com grande volume de dados
        /// </summary>
        /// <returns>Resultado da simulação</returns>
        [HttpPost("intensiva")]
        public async Task<ActionResult<SimulationResult>> ExecutarSimulacaoIntensiva()
        {
            try
            {
                var startTime = DateTime.UtcNow;
                _logger.LogInformation("Iniciando simulação intensiva às {StartTime}", startTime);
                _logger.LogWarning("ATENÇÃO: Simulação intensiva pode demorar vários minutos e gerar muitos logs!");

                await _simulator.ExecutarSimulacaoIntensiva();

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                var result = new SimulationResult
                {
                    TipoSimulacao = "Intensiva",
                    IniciadaEm = startTime,
                    ConcluidaEm = endTime,
                    DuracaoSegundos = (int)duration.TotalSeconds,
                    Status = "Concluída com Sucesso",
                    Mensagem = "Simulação intensiva executada com sucesso. Volume alto de dados processado.",
                    QuantidadePessoas = 50,
                    QuantidadeEmpresas = 20,
                    QuantidadeTransacoes = 200
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante simulação intensiva");
                return BadRequest(new SimulationResult
                {
                    TipoSimulacao = "Intensiva",
                    Status = "Erro",
                    Mensagem = $"Erro durante a simulação: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtém informações sobre as simulações disponíveis
        /// </summary>
        /// <returns>Informações das simulações</returns>
        [HttpGet("info")]
        public ActionResult<SimulationInfo> ObterInformacoesSimulacao()
        {
            var info = new SimulationInfo
            {
                SimulacoesDisponiveis = new List<SimulationTypeInfo>
                {
                    new()
                    {
                        Tipo = "Rápida",
                        Endpoint = "/api/simulation/rapida",
                        Descricao = "Simulação rápida com 5 pessoas, 2 empresas e 20 transações",
                        DuracaoEstimada = "30-60 segundos",
                        Recomendacao = "Ideal para testes rápidos e desenvolvimento"
                    },
                    new()
                    {
                        Tipo = "Completa",
                        Endpoint = "/api/simulation/completa",
                        Descricao = "Simulação customizável com parâmetros definidos pelo usuário",
                        DuracaoEstimada = "Variável conforme parâmetros",
                        Recomendacao = "Ideal para testes específicos com controle de volume"
                    },
                    new()
                    {
                        Tipo = "Intensiva",
                        Endpoint = "/api/simulation/intensiva",
                        Descricao = "Simulação intensiva com 50 pessoas, 20 empresas e 200 transações",
                        DuracaoEstimada = "5-10 minutos",
                        Recomendacao = "Ideal para testes de performance e carga"
                    }
                },
                Observacoes = new List<string>
                {
                    "As simulações executam de forma assíncrona usando eventos",
                    "Verifique os logs da aplicação para acompanhar o progresso",
                    "Todos os dados são gerados usando Faker.NET com localização brasileira",
                    "As transações são processadas através do MassTransit",
                    "Cada execução gera IDs únicos para transações ISO20022"
                }
            };

            return Ok(info);
        }
    }

    public class SimulationRequest
    {
        public int QuantidadePessoas { get; set; } = 10;
        public int QuantidadeEmpresas { get; set; } = 5;
        public int QuantidadeTransacoes { get; set; } = 50;
    }

    public class SimulationResult
    {
        public string TipoSimulacao { get; set; } = string.Empty;
        public DateTime? IniciadaEm { get; set; }
        public DateTime? ConcluidaEm { get; set; }
        public int? DuracaoSegundos { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public int? QuantidadePessoas { get; set; }
        public int? QuantidadeEmpresas { get; set; }
        public int? QuantidadeTransacoes { get; set; }
    }

    public class SimulationInfo
    {
        public List<SimulationTypeInfo> SimulacoesDisponiveis { get; set; } = new();
        public List<string> Observacoes { get; set; } = new();
    }

    public class SimulationTypeInfo
    {
        public string Tipo { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string DuracaoEstimada { get; set; } = string.Empty;
        public string Recomendacao { get; set; } = string.Empty;
    }
}