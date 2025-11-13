namespace Transaction.Infrastructure.Factories;
public class TransactionDbContextFactory : IDesignTimeDbContextFactory<TransactionDbContext>
{
    public TransactionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransactionDbContext>();        
        optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=transactions_db;Username=postgres;Password=postgres");

        return new TransactionDbContext(optionsBuilder.Options);
    }
}
