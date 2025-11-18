using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Common;
using Transaction.Infrastructure.Repositories;
using Transaction.Infrastructure.TransactionsDbContext;
using Xunit;

namespace Transaction.UnitTests.Infrastructure;

public class TransactionRepositoryTests
{
    private static TransactionDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TransactionDbContext(options);
    }

    [Fact]
    public async Task AddAsync_And_GetByExternalIdAsync_ShouldPersistAndRetrieve()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var ctx = CreateInMemoryContext(dbName);

        var repo = new TransactionRepository(ctx);

        var tx = new Transaction.Domain.Entities.Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            1000m);

        // Act
        await repo.AddAsync(tx, CancellationToken.None);
        await repo.SaveChangesAsync(CancellationToken.None);

        var found = await repo.GetByExternalIdAsync(tx.ExternalId, CancellationToken.None);

        // Assert
        found.Should().NotBeNull();
        found!.ExternalId.Should().Be(tx.ExternalId);
        found.Value.Should().Be(1000m);
        found.Status.Should().Be(ETransactionStatus.PENDING);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingTransaction()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var ctx = CreateInMemoryContext(dbName);

        var repo = new TransactionRepository(ctx);

        var tx = new Transaction.Domain.Entities.Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            500m);

        await repo.AddAsync(tx, CancellationToken.None);
        await repo.SaveChangesAsync(CancellationToken.None);

        // Act
        tx.ApplyValidation(ETransactionStatus.APPROVED);
        await repo.UpdateAsync(tx, CancellationToken.None);
        await repo.SaveChangesAsync(CancellationToken.None);

        var found = await repo.GetByExternalIdAsync(tx.ExternalId, CancellationToken.None);

        // Assert
        found!.Status.Should().Be(ETransactionStatus.APPROVED);
    }
}
