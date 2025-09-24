using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Diagnostics;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint de health check para monitoramento
        /// </summary>
        /// <returns>Status da aplicação</returns>
        [HttpGet]
        public ActionResult<HealthStatus> Get()
        {
            try
            {
                var healthStatus = new HealthStatus
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    MachineName = Environment.MachineName,
                    ProcessId = Environment.ProcessId,
                    Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                    Services = new ServiceHealthStatus
                    {
                        MassTransit = "Healthy",
                        Kafka = CheckKafkaConnection(),
                        Simulation = "Ready"
                    }
                };

                _logger.LogDebug("Health check executado com sucesso");
                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante health check");
                
                var unhealthyStatus = new HealthStatus
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                };

                return StatusCode(503, unhealthyStatus);
            }
        }

        /// <summary>
        /// Health check resumido para load balancers
        /// </summary>
        /// <returns>Status simples</returns>
        [HttpGet("simple")]
        public ActionResult GetSimple()
        {
            return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Informações detalhadas da aplicação
        /// </summary>
        /// <returns>Informações do sistema</returns>
        [HttpGet("info")]
        public ActionResult<SystemInfo> GetInfo()
        {
            var systemInfo = new SystemInfo
            {
                ApplicationName = "BankingApi",
                Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                MachineName = Environment.MachineName,
                ProcessId = Environment.ProcessId,
                StartTime = Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                Uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime(),
                WorkingSet = Environment.WorkingSet,
                GcMemory = GC.GetTotalMemory(false),
                ProcessorCount = Environment.ProcessorCount,
                OsVersion = Environment.OSVersion.ToString(),
                RuntimeVersion = Environment.Version.ToString(),
                Timestamp = DateTime.UtcNow
            };

            return Ok(systemInfo);
        }

        private string CheckKafkaConnection()
        {
            try
            {
                // Em um cenário real, você faria uma verificação real do Kafka
                // Por enquanto, apenas retornamos "Healthy" pois estamos usando InMemory
                var kafkaBootstrapServers = Environment.GetEnvironmentVariable("Kafka__BootstrapServers");
                return string.IsNullOrEmpty(kafkaBootstrapServers) ? "InMemory" : "Connected";
            }
            catch
            {
                return "Disconnected";
            }
        }
    }

    public class HealthStatus
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Version { get; set; }
        public string? Environment { get; set; }
        public string? MachineName { get; set; }
        public int? ProcessId { get; set; }
        public TimeSpan? Uptime { get; set; }
        public ServiceHealthStatus? Services { get; set; }
        public string? Error { get; set; }
    }

    public class ServiceHealthStatus
    {
        public string MassTransit { get; set; } = string.Empty;
        public string Kafka { get; set; } = string.Empty;
        public string Simulation { get; set; } = string.Empty;
    }

    public class SystemInfo
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string? Version { get; set; }
        public string? Environment { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public long WorkingSet { get; set; }
        public long GcMemory { get; set; }
        public int ProcessorCount { get; set; }
        public string OsVersion { get; set; } = string.Empty;
        public string RuntimeVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}