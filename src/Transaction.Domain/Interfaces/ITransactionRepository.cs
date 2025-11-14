namespace Transaction.Domain.Interfaces;
public interface ITransactionRepository
{
    Task AddAsync(Entities.Transaction tx, CancellationToken ct);
    Task<Entities.Transaction?> GetByExternalIdAsync(Guid externalId, CancellationToken ct);
    Task UpdateAsync(Entities.Transaction tx, CancellationToken ct);   // <-- opcional
    Task SaveChangesAsync(CancellationToken ct);
}
