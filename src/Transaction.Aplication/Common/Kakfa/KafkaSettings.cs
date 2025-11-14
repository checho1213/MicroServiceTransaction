namespace Transaction.Aplication.Common.Kakfa;
public sealed class KafkaSettings
{
    public string? BootstrapServers { get; set; }
    public string? TopicCreated { get; set; }
    public string? TopicValidated { get; set; }
}
