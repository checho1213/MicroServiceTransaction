using MediatR;
using Microsoft.AspNetCore.Mvc;
using Transaction.Aplication.Commands;
using Transaction.Aplication.Common;
using Transaction.Aplication.DTOs;


namespace Transaction.Msvc.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TransactionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto transaction)
    {
        var cmd = new CreateTransactionCommand(transaction.SourceAccountId, transaction.TargetAccountId, transaction.TransferTypeId, transaction.Value);
        var res = await _mediator.Send(cmd);
        var response = ApiResponse<CreateTransactionResultDto>.SuccessResponse(res);
        return CreatedAtAction(
            nameof(Get),
            new { externalId = res.ExternalId },
            response
        );
    }

    [HttpGet("{externalId:guid}")]
    public async Task<IActionResult> Get(Guid externalId)
    {
        return Ok(new { externalId });
    }
}

public record CreateTransactionDto(Guid SourceAccountId, Guid TargetAccountId, int TransferTypeId, decimal Value);
