using MediatR;
using Microsoft.Extensions.Options;
using Transaction.Aplication.Commands;
using Transaction.Aplication.Common.Kakfa;
using Transaction.Aplication.DTOs;
using Transaction.Aplication.Events;
using Transaction.Aplication.Interfaces;
using Transaction.Domain.Interfaces;

namespace Transaction.Aplication.Handlers;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResultDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IEventProducer _eventProducer;
    private readonly KafkaSettings _settingsKafka;

    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IEventProducer eventProducer, IOptions<KafkaSettings> options)
    {
        _transactionRepository = transactionRepository;
        _eventProducer = eventProducer;
        _settingsKafka = options.Value;
    }

    public async Task<CreateTransactionResultDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var tx = new Domain.Entities.Transaction(request.SourceAccountId, request.TargetAccountId, request.TransferTypeId, request.Value);
        await _transactionRepository.AddAsync(tx, cancellationToken);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        var evt = new TransactionCreatedEvent(tx.ExternalId, tx.SourceAccountId, tx.Value, tx.CreatedAt);
        await _eventProducer.ProduceAsync(_settingsKafka.TopicCreated, evt);

        return new CreateTransactionResultDto(tx.ExternalId, tx.Status.ToString(), tx.CreatedAt);
    }
}
