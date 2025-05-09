using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Order.Application.Features.Addresses.CreatingAddress.v1.Internals;
namespace Order.Application.Features.Addresses.CreatingAddress.v1;

public class CreateAddressCommandHandler : ICommandHandler<CreateAddressCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateAddressCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new CreateAddress(
            command.AddressId,
            command.CustomerId,
            command.Type,
            command.FullAddress);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CreateAddressCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("AddressId", command.AddressId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}

