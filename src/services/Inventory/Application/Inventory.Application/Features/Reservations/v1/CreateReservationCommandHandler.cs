using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Inventory.Application.Features.Reservations.v1.Internals;
using MediatR;
namespace Inventory.Application.Features.Reservations.v1;

public class CreateReservationCommandHandler : ICommandHandler<CreateReservationCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateReservationCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new CreateReservation(
            command.OrderId,
            command.CustomerId,
            command.Items.Select(u=> new ReservationItem {  ProductId = u.ProductId, Quantity = u.Quantity}).ToList());

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CreateReservationCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("OrderId", command.OrderId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}
