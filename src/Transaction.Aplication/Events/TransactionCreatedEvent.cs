namespace Transaction.Aplication.Events;

public record TransactionCreatedEvent(Guid ExternalId, Guid SourceAccountId, decimal Value, DateTime CreatedAt);
