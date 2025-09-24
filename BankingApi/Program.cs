using Microsoft.EntityFrameworkCore;
using BankingApi.Models;
using MassTransit;
using BankingApi.Services;
using BankingApi.Consumers;
using BankingApi.Simulation;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Configuração do MinIO
builder.Services.Configure<MinIOConfiguration>(
    builder.Configuration.GetSection("MinIO"));

builder.Services.AddSingleton<IMinioClient>(serviceProvider =>
{
    var config = builder.Configuration.GetSection("MinIO").Get<MinIOConfiguration>() ?? new MinIOConfiguration();
    
    var minioClient = new MinioClient()
        .WithEndpoint(config.Endpoint)
        .WithCredentials(config.AccessKey, config.SecretKey);
    
    if (config.UseSSL)
    {
        minioClient = minioClient.WithSSL();
    }
    
    return minioClient.Build();
});

builder.Services.AddScoped<IMinIOService, MinIOService>();

// Configuração do MassTransit com Kafka
builder.Services.AddMassTransit(x =>
{
    // Registrar consumers
    x.AddConsumer<TransacaoSolicitadaConsumer>();
    x.AddConsumer<TransacaoProcessadaConsumer>();
    x.AddConsumer<SaldoAtualizadoConsumer>();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    // Configuração do Kafka (comentado para desenvolvimento local)
    /*
    x.AddRider(rider =>
    {
        rider.AddConsumer<TransacaoSolicitadaConsumer>();
        rider.AddConsumer<TransacaoProcessadaConsumer>();
        rider.AddConsumer<SaldoAtualizadoConsumer>();

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");

            k.TopicEndpoint<TransacaoSolicitada>("banking-transacao-solicitada", "banking-api", e =>
            {
                e.ConfigureConsumer<TransacaoSolicitadaConsumer>(context);
            });

            k.TopicEndpoint<TransacaoProcessada>("banking-transacao-processada", "banking-api", e =>
            {
                e.ConfigureConsumer<TransacaoProcessadaConsumer>(context);
            });

            k.TopicEndpoint<SaldoAtualizado>("banking-saldo-atualizado", "banking-api", e =>
            {
                e.ConfigureConsumer<SaldoAtualizadoConsumer>(context);
            });
        });
    });
    */
});

// Registrar serviços de aplicação
builder.Services.AddScoped<IBankingEventContext, BankingEventContext>();
builder.Services.AddScoped<ITransacaoBancariaService, TransacaoBancariaService>();

// Registrar serviços de simulação
builder.Services.AddScoped<BankingSimulator>();
builder.Services.AddScoped<SimulationReportService>();
builder.Services.AddHostedService<SimulationBackgroundService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Banking API", Version = "v1" });
    c.SwaggerDoc("v2", new() { Title = "Banking API v2 (Event-Driven)", Version = "v2" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Banking API V2 (Event-Driven)");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Mapear health check endpoints
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
