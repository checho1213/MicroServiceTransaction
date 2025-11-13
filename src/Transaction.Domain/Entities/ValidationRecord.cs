using Transaction.Domain.Common;

namespace Transaction.Domain.Entities;

public class ValidationRecord
{
    public Guid Id { get; private set; }
    public Guid TransactionExternalId { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public decimal Value { get; private set; }
    public ETransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ValidationRecord() { }

    public ValidationRecord(Guid txExternalId, Guid sourceAccountId, decimal value, ETransactionStatus status)
    {
        Id = Guid.NewGuid();
        TransactionExternalId = txExternalId;
        SourceAccountId = sourceAccountId;
        Value = value;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
}
