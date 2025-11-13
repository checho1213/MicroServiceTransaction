namespace Transaction.Aplication.DTOs;

public record CreateTransactionResultDto(Guid ExternalId, string Status, DateTime CreatedAt);
