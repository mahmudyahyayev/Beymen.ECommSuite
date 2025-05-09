using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Notification.Application.Features.Customers.CreatingCustomer.v1.Internals;
namespace Notification.Application.Features.Customers.CreatingCustomer.v1;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateCustomerCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new CreateCustomer(
            command.CustomerId,
            command.FirstName,
            command.LastName,
            command.PhoneNumber,
            command.Email);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CreateCustomerCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("CustomerId", command.CustomerId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}