namespace Transaction.Infrastructure.Kafka;
public class KafkaProducer : IEventProducer
{
    private readonly IProducer<Null, string> _producer;
    public KafkaProducer(ProducerConfig config)
    {
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync<T>(string topic, T @event)
    {
        var json = JsonSerializer.Serialize(@event);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
    }
}
