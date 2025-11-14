namespace Transaction.Aplication.Events;
public record ChangeStatusTransactionEvent(Guid ExternalId, ETransactionStatus status);
