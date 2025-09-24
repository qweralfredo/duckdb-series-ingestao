using Microsoft.AspNetCore.Mvc;
using BankingApi.Simulation;
using BankingApi.Services;

namespace BankingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly SimulationReportService _reportService;
        private readonly IMinIOService? _minioService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            SimulationReportService reportService,
            IConfiguration configuration,
            ILogger<ReportsController> logger,
            IMinIOService? minioService = null)
        {
            _reportService = reportService;
            _minioService = minioService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Lista os relatórios de simulação disponíveis
        /// </summary>
        /// <param name="limite">Número máximo de relatórios a retornar</param>
        /// <returns>Lista de resumos dos relatórios</returns>
        [HttpGet("summaries")]
        public async Task<ActionResult<IEnumerable<SimulationSummary>>> GetReportSummaries([FromQuery] int limite = 50)
        {
            try
            {
                var summaries = await _reportService.ListarRelatorios(limite);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar relatórios");
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém um relatório específico pelo nome do arquivo
        /// </summary>
        /// <param name="fileName">Nome do arquivo do relatório</param>
        /// <returns>Relatório completo</returns>
        [HttpGet("{fileName}")]
        public async Task<ActionResult<SimulationReport>> GetReport(string fileName)
        {
            try
            {
                var report = await _reportService.ObterRelatorio(fileName);
                
                if (report == null)
                {
                    return NotFound(new { error = "Relatório não encontrado" });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório: {FileName}", fileName);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Lista todos os arquivos armazenados no MinIO
        /// </summary>
        /// <param name="bucket">Nome do bucket (opcional, usa o padrão se não especificado)</param>
        /// <param name="prefix">Prefixo para filtrar arquivos</param>
        /// <returns>Lista de arquivos</returns>
        [HttpGet("files")]
        public async Task<ActionResult<IEnumerable<MinIOFileInfo>>> GetFiles([FromQuery] string? bucket = null, [FromQuery] string prefix = "")
        {
            try
            {
                if (_minioService == null)
                {
                    return BadRequest(new { error = "MinIO não está configurado" });
                }

                var bucketName = bucket ?? _configuration.GetValue<string>("MinIO:BucketReports", "simulation-reports");
                var files = await _minioService.ListFilesAsync(bucketName, prefix);

                var fileInfos = files.Select(fileName => new MinIOFileInfo
                {
                    FileName = fileName,
                    BucketName = bucketName,
                    DownloadUrl = $"/api/reports/download/{bucketName}/{fileName.Replace("/", "%2F")}"
                });

                return Ok(fileInfos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar arquivos do MinIO");
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Faz download de um arquivo específico do MinIO
        /// </summary>
        /// <param name="bucket">Nome do bucket</param>
        /// <param name="fileName">Nome do arquivo (codificado para URL)</param>
        /// <returns>Arquivo para download</returns>
        [HttpGet("download/{bucket}/{**fileName}")]
        public async Task<IActionResult> DownloadFile(string bucket, string fileName)
        {
            try
            {
                if (_minioService == null)
                {
                    return BadRequest(new { error = "MinIO não está configurado" });
                }

                // Decodificar o nome do arquivo
                var decodedFileName = Uri.UnescapeDataString(fileName);

                var exists = await _minioService.FileExistsAsync(bucket, decodedFileName);
                if (!exists)
                {
                    return NotFound(new { error = "Arquivo não encontrado" });
                }

                var fileStream = await _minioService.DownloadFileAsync(bucket, decodedFileName);
                var contentType = GetContentType(decodedFileName);

                return File(fileStream, contentType, Path.GetFileName(decodedFileName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer download do arquivo: {Bucket}/{FileName}", bucket, fileName);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém uma URL pré-assinada para acesso direto ao arquivo
        /// </summary>
        /// <param name="bucket">Nome do bucket</param>
        /// <param name="fileName">Nome do arquivo</param>
        /// <param name="expiryMinutes">Tempo de expiração em minutos (padrão: 60)</param>
        /// <returns>URL pré-assinada</returns>
        [HttpGet("presigned-url/{bucket}/{**fileName}")]
        public async Task<ActionResult<PresignedUrlResponse>> GetPresignedUrl(string bucket, string fileName, [FromQuery] int expiryMinutes = 60)
        {
            try
            {
                if (_minioService == null)
                {
                    return BadRequest(new { error = "MinIO não está configurado" });
                }

                var decodedFileName = Uri.UnescapeDataString(fileName);
                var exists = await _minioService.FileExistsAsync(bucket, decodedFileName);
                
                if (!exists)
                {
                    return NotFound(new { error = "Arquivo não encontrado" });
                }

                var url = await _minioService.GetPresignedUrlAsync(bucket, decodedFileName, expiryMinutes * 60);
                
                var response = new PresignedUrlResponse
                {
                    Url = url,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    FileName = decodedFileName,
                    BucketName = bucket
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar URL pré-assinada: {Bucket}/{FileName}", bucket, fileName);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Remove um arquivo do MinIO
        /// </summary>
        /// <param name="bucket">Nome do bucket</param>
        /// <param name="fileName">Nome do arquivo</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{bucket}/{**fileName}")]
        public async Task<IActionResult> DeleteFile(string bucket, string fileName)
        {
            try
            {
                if (_minioService == null)
                {
                    return BadRequest(new { error = "MinIO não está configurado" });
                }

                var decodedFileName = Uri.UnescapeDataString(fileName);
                var success = await _minioService.DeleteFileAsync(bucket, decodedFileName);

                if (success)
                {
                    return Ok(new { message = "Arquivo removido com sucesso" });
                }
                else
                {
                    return StatusCode(500, new { error = "Falha ao remover o arquivo" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo: {Bucket}/{FileName}", bucket, fileName);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Upload de um arquivo para o MinIO
        /// </summary>
        /// <param name="bucket">Nome do bucket</param>
        /// <param name="file">Arquivo a ser enviado</param>
        /// <param name="customFileName">Nome personalizado para o arquivo (opcional)</param>
        /// <returns>URL do arquivo enviado</returns>
        [HttpPost("upload/{bucket}")]
        public async Task<ActionResult<UploadResponse>> UploadFile(string bucket, IFormFile file, [FromQuery] string? customFileName = null)
        {
            try
            {
                if (_minioService == null)
                {
                    return BadRequest(new { error = "MinIO não está configurado" });
                }

                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { error = "Nenhum arquivo foi enviado" });
                }

                var fileName = customFileName ?? file.FileName;
                var objectName = $"uploads/{DateTime.Now:yyyy/MM/dd}/{fileName}";

                using var stream = file.OpenReadStream();
                var url = await _minioService.UploadFileAsync(bucket, objectName, stream, file.ContentType);

                var response = new UploadResponse
                {
                    FileName = fileName,
                    ObjectName = objectName,
                    BucketName = bucket,
                    Url = url,
                    Size = file.Length,
                    ContentType = file.ContentType,
                    UploadedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo: {Bucket}", bucket);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Informações sobre o status do MinIO
        /// </summary>
        /// <returns>Status do MinIO</returns>
        [HttpGet("minio/status")]
        public ActionResult<MinIOStatus> GetMinIOStatus()
        {
            var status = new MinIOStatus
            {
                IsConfigured = _minioService != null,
                Endpoint = _configuration.GetValue<string>("MinIO:Endpoint", "N/A"),
                BucketReports = _configuration.GetValue<string>("MinIO:BucketReports", "simulation-reports"),
                BucketLogs = _configuration.GetValue<string>("MinIO:BucketLogs", "banking-logs"),
                BucketBackups = _configuration.GetValue<string>("MinIO:BucketBackups", "banking-backups"),
                BucketExports = _configuration.GetValue<string>("MinIO:BucketExports", "banking-exports")
            };

            return Ok(status);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return extension switch
            {
                ".json" => "application/json",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".xml" => "application/xml",
                ".zip" => "application/zip",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }

    public class MinIOFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }

    public class PresignedUrlResponse
    {
        public string Url { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }

    public class UploadResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class MinIOStatus
    {
        public bool IsConfigured { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string BucketReports { get; set; } = string.Empty;
        public string BucketLogs { get; set; } = string.Empty;
        public string BucketBackups { get; set; } = string.Empty;
        public string BucketExports { get; set; } = string.Empty;
    }
}