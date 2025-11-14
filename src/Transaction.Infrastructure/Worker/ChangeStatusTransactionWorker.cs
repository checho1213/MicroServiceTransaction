namespace Transaction.Infrastructure.Workers;
public class ChangeStatusTransactionWorker : BackgroundService
{
    private readonly KafkaSettings _settingsKafka;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChangeStatusTransactionWorker> _logger;

    private readonly IAsyncPolicy _retryPolicy;

    public ChangeStatusTransactionWorker(
        IOptions<KafkaSettings> options,
        IServiceProvider serviceProvider,
        ILogger<ChangeStatusTransactionWorker> logger)
    {
        _settingsKafka = options.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;        
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)),
                onRetry: (ex, time, attempt, context) =>
                {
                    _logger.LogWarning(
                        "Intento {Attempt} fallido. Esperando {Delay}ms. Error: {Message}",
                        attempt, time.TotalMilliseconds, ex.Message);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settingsKafka.BootstrapServers,
            GroupId = "antifraud-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<Null, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Error}", e))
            .Build();

        consumer.Subscribe(_settingsKafka.TopicValidated);

        _logger.LogInformation("Worker escuchando topic: {Topic}", _settingsKafka.TopicValidated);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                if (result?.Message?.Value == null)
                    continue;

                var evt = JsonConvert.DeserializeObject<ChangeStatusTransactionEvent>(result.Message.Value);
                if (evt == null)
                {
                    _logger.LogWarning("Mensaje inválido recibido.");
                    continue;
                }
                await ProcessEventWithPolly(evt, stoppingToken);                
                consumer.Commit(result);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker detenido.");
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessEventWithPolly(ChangeStatusTransactionEvent evt, CancellationToken ct)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

            var tx = await repo.GetByExternalIdAsync(evt.ExternalId, ct);

            if (tx == null)
            {
                _logger.LogWarning("Transacción no encontrada: {ExternalId}", evt.ExternalId);
                return;
            }

            tx.ApplyValidation(evt.status);

            await repo.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Transacción {TransactionId} actualizada a estado {Status}",
                tx.Id, evt.status);
        });
    }
}
