using Minio;
using Minio.DataModel.Args;
using System.Text.Json;

namespace BankingApi.Services
{
    public interface IMinIOService
    {
        Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType = "application/octet-stream");
        Task<string> UploadJsonAsync<T>(string bucketName, string objectName, T data);
        Task<Stream> DownloadFileAsync(string bucketName, string objectName);
        Task<T?> DownloadJsonAsync<T>(string bucketName, string objectName);
        Task<bool> FileExistsAsync(string bucketName, string objectName);
        Task<bool> DeleteFileAsync(string bucketName, string objectName);
        Task<IEnumerable<string>> ListFilesAsync(string bucketName, string prefix = "");
        Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expiryInSeconds = 3600);
    }

    public class MinIOService : IMinIOService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinIOService> _logger;
        private readonly MinIOConfiguration _config;

        public MinIOService(IMinioClient minioClient, ILogger<MinIOService> logger, IConfiguration configuration)
        {
            _minioClient = minioClient;
            _logger = logger;
            _config = configuration.GetSection("MinIO").Get<MinIOConfiguration>() ?? new MinIOConfiguration();
        }

        public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream data, string contentType = "application/octet-stream")
        {
            try
            {
                await EnsureBucketExistsAsync(bucketName);

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(data)
                    .WithObjectSize(data.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                var url = await GetPresignedUrlAsync(bucketName, objectName);
                
                _logger.LogInformation("Arquivo enviado para MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar arquivo para MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                throw;
            }
        }

        public async Task<string> UploadJsonAsync<T>(string bucketName, string objectName, T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
                using var stream = new MemoryStream(jsonBytes);

                return await UploadFileAsync(bucketName, objectName, stream, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar JSON para MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string objectName)
        {
            try
            {
                var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream));

                await _minioClient.GetObjectAsync(getObjectArgs);
                
                memoryStream.Position = 0;
                
                _logger.LogInformation("Arquivo baixado do MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao baixar arquivo do MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                throw;
            }
        }

        public async Task<T?> DownloadJsonAsync<T>(string bucketName, string objectName)
        {
            try
            {
                using var stream = await DownloadFileAsync(bucketName, objectName);
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao baixar JSON do MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(string bucketName, string objectName)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _minioClient.StatObjectAsync(statObjectArgs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string bucketName, string objectName)
        {
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _minioClient.RemoveObjectAsync(removeObjectArgs);
                
                _logger.LogInformation("Arquivo removido do MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo do MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
                return false;
            }
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string bucketName, string prefix = "")
        {
            try
            {
                var files = new List<string>();

                var listObjectsArgs = new ListObjectsArgs()
                    .WithBucket(bucketName)
                    .WithPrefix(prefix);

                await foreach (var item in _minioClient.ListObjectsEnumAsync(listObjectsArgs))
                {
                    files.Add(item.Key);
                }

                _logger.LogInformation("Listados {Count} arquivos no MinIO: {BucketName}/{Prefix}", files.Count, bucketName, prefix);
                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar arquivos do MinIO: {BucketName}/{Prefix}", bucketName, prefix);
                throw;
            }
        }

        public async Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expiryInSeconds = 3600)
        {
            try
            {
                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(expiryInSeconds);

                return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar URL pré-assinada: {BucketName}/{ObjectName}", bucketName, objectName);
                throw;
            }
        }

        private async Task EnsureBucketExistsAsync(string bucketName)
        {
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
                bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);

                if (!bucketExists)
                {
                    var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs);
                    
                    _logger.LogInformation("Bucket criado no MinIO: {BucketName}", bucketName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar/criar bucket no MinIO: {BucketName}", bucketName);
                throw;
            }
        }
    }

    public class MinIOConfiguration
    {
        public string Endpoint { get; set; } = "localhost:9000";
        public string AccessKey { get; set; } = "admin";
        public string SecretKey { get; set; } = "admin123456";
        public bool UseSSL { get; set; } = false;
        public string Region { get; set; } = "us-east-1";
        public string BucketReports { get; set; } = "simulation-reports";
        public string BucketLogs { get; set; } = "banking-logs";
        public string BucketBackups { get; set; } = "banking-backups";
        public string BucketExports { get; set; } = "banking-exports";
    }
}