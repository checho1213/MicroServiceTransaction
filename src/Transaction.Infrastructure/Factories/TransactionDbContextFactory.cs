using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Transaction.Infrastructure.TransactionsDbContext;

namespace Transaction.Infrastructure.Factories;

public class TransactionDbContextFactory : IDesignTimeDbContextFactory<TransactionDbContext>
{
    public TransactionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransactionDbContext>();        
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=transaction_db;Username=postgres;Password=sergio123");

        return new TransactionDbContext(optionsBuilder.Options);
    }
}
