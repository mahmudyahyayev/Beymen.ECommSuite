using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
namespace Order.Application.Features.Orders.CancelOrder.v1;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CancelOrderCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new Internals.CancelOrder(
            command.ReservationId,
            command.OrderId,
            command.CustomerId,
            command.Message);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CancelOrderCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("OrderId", command.OrderId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}
