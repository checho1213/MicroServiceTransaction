using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Transaction.Aplication.Commands;
using Transaction.Aplication.Common.Kakfa;
using Transaction.Aplication.DTOs;
using Transaction.Aplication.Events;
using Transaction.Aplication.Handlers;
using Transaction.Aplication.Interfaces;
using Transaction.Domain.Interfaces;
using Xunit;

namespace Transaction.UnitTests.Application;

public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<ITransactionRepository> _repoMock = new();
    private readonly Mock<IEventProducer> _producerMock = new();
    private readonly IOptions<KafkaSettings> _options;

    public CreateTransactionCommandHandlerTests()
    {
        _options = Options.Create(new KafkaSettings
        {
            TopicCreated = "transaction.created"
        });
    }

    [Fact]
    public async Task Handle_ShouldCreateTransaction_SaveAndPublishEvent()
    {
        // Arrange
        var command = new CreateTransactionCommand(
            SourceAccountId: Guid.NewGuid(),
            TargetAccountId: Guid.NewGuid(),
            TransferTypeId: 1,
            Value: 500m
        );

        var handler = new CreateTransactionCommandHandler(
            _repoMock.Object,
            _producerMock.Object,
            _options
        );

        // Act
        CreateTransactionResultDto result = await handler.Handle(command, CancellationToken.None);

        // Assert - resultado
        result.Should().NotBeNull();
        result.ExternalId.Should().NotBeEmpty();
        result.Status.Should().Be("PENDING"); // estado inicial de la transacción

        // Assert - comportamiento repositorio
        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Transaction.Domain.Entities.Transaction>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _repoMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        // Assert - evento publicado
        _producerMock.Verify(
            p => p.ProduceAsync(
                "transaction.created",
                It.IsAny<TransactionCreatedEvent>()),
            Times.Once);
    }
}
