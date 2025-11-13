namespace Transaction.Infrastructure.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionDbContext _ctx;
    public TransactionRepository(TransactionDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Domain.Entities.Transaction tx, CancellationToken ct) => await _ctx.Transactions.AddAsync(tx, ct);

    public async Task< Domain.Entities.Transaction?> GetByExternalIdAsync(Guid externalId, CancellationToken ct)
        => await _ctx.Transactions.FirstOrDefaultAsync(x => x.ExternalId == externalId, ct);

    public async Task SaveChangesAsync(CancellationToken ct) => await _ctx.SaveChangesAsync(ct);
}
