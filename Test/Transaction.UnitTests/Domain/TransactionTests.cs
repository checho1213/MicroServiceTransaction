using FluentAssertions;
using Transaction.Domain.Common;
using Xunit;

namespace Transaction.UnitTests.Domain;

public class TransactionTests
{
    [Fact]
    public void CreatingTransaction_WithNegativeValue_ShouldThrow()
    {
        // Act
        Action act = () => new Transaction.Domain.Entities.Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            transferTypeId: 1,
            value: -10m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreatingTransaction_WithValidData_ShouldInitializeCorrectly()
    {
        // Arrange
        var source = Guid.NewGuid();
        var target = Guid.NewGuid();

        // Act
        var tx = new Transaction.Domain.Entities.Transaction(source, target, 1, 100m);

        // Assert
        tx.Id.Should().NotBeEmpty();
        tx.ExternalId.Should().NotBeEmpty();
        tx.SourceAccountId.Should().Be(source);
        tx.TargetAccountId.Should().Be(target);
        tx.TransferTypeId.Should().Be(1);
        tx.Value.Should().Be(100m);
        tx.Status.Should().Be(ETransactionStatus.PENDING);
        tx.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ApplyValidation_WhenStatusIsPending_ShouldChangeStatus()
    {
        // Arrange
        var tx = new Transaction.Domain.Entities.Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);

        // Act
        tx.ApplyValidation(ETransactionStatus.APPROVED);

        // Assert
        tx.Status.Should().Be(ETransactionStatus.APPROVED);
    }

    [Fact]
    public void ApplyValidation_WhenStatusIsNotPending_ShouldNotChangeStatus()
    {
        // Arrange
        var tx = new Transaction.Domain.Entities.Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        tx.ApplyValidation(ETransactionStatus.APPROVED);

        // Act
        tx.ApplyValidation(ETransactionStatus.REJECTED);

        // Assert
        tx.Status.Should().Be(ETransactionStatus.APPROVED);
    }
}
