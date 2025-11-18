using FluentAssertions;
using Transaction.Domain.Common;
using Transaction.Domain.Entities;
using Xunit;

namespace Transaction.UnitTests.Domain;

public class ValidationRecordTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var txExternalId = Guid.NewGuid();
        var sourceAccountId = Guid.NewGuid();
        var value = 250m;

        // Act
        var record = new ValidationRecord(txExternalId, sourceAccountId, value, ETransactionStatus.APPROVED);

        // Assert
        record.Id.Should().NotBeEmpty();
        record.TransactionExternalId.Should().Be(txExternalId);
        record.SourceAccountId.Should().Be(sourceAccountId);
        record.Value.Should().Be(value);
        record.Status.Should().Be(ETransactionStatus.APPROVED);
        record.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
