using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Entities;

namespace Transaction.Infrastructure.TransactionsDbContext;

public  class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }
    public DbSet<Domain.Entities.Transaction> Transactions { get; set; } = null!;
    public DbSet<ValidationRecord> ValidationRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Transaction>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Value).HasColumnType("numeric(18,2)").IsRequired();
            b.Property(x => x.ExternalId).IsRequired();
            b.Property(x => x.Status).HasConversion<string>().IsRequired();
        });

        modelBuilder.Entity<ValidationRecord>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Value).HasColumnType("numeric(18,2)").IsRequired();
            b.Property(x => x.Status).HasConversion<string>().IsRequired();
        });
    }
}
