namespace Transaction.Aplication.Commands;

public record CreateTransactionCommand(Guid SourceAccountId, Guid TargetAccountId, int TransferTypeId, decimal Value) : IRequest<CreateTransactionResultDto>;
