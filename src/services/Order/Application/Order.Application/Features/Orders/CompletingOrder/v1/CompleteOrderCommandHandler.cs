using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Order.Application.Features.Orders.CancelOrder.v1;
namespace Order.Application.Features.Orders.CompletingOrder.v1;

public class CompleteOrderCommandHandler : ICommandHandler<CompleteOrderCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CompleteOrderCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CompleteOrderCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new Internals.CompleteOrder(
            command.ReservationId,
            command.OrderId,
            command.CustomerId);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CancelOrderCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("OrderId", command.OrderId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}