namespace Transaction.Domain.Entities;
public class Transaction
{
    public Guid Id { get; private set; }
    public Guid ExternalId { get; private set; }
    public Guid SourceAccountId { get; private set; }
    public Guid TargetAccountId { get; private set; }
    public int TransferTypeId { get; private set; }
    public decimal Value { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ETransactionStatus Status { get; private set; }

    private Transaction() { }

    public Transaction(Guid source, Guid target, int transferTypeId, decimal value)
    {
        if (value <= 0) throw new ArgumentException("Value must be > 0");
        Id = Guid.NewGuid();
        ExternalId = Guid.NewGuid();
        SourceAccountId = source;
        TargetAccountId = target;
        TransferTypeId = transferTypeId;
        Value = value;
        CreatedAt = DateTime.UtcNow;
        Status = ETransactionStatus.PENDING;
    }

    public void ApplyValidation(ETransactionStatus status)
    {
        if (Status != ETransactionStatus.PENDING) return;
        Status = status;
    }
}
