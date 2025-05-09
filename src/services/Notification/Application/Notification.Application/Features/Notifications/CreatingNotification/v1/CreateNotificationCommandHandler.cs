using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Notification.Application.Features.Customers.CreatingCustomer.v1;
using Notification.Application.Features.Notifications.CreatingNotification.v1.Internals;
namespace Notification.Application.Features.Notifications.CreatingNotification.v1;

public class CreateNotificationCommandHandler : ICommandHandler<CreateNotificationCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateNotificationCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CreateNotificationCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createNotification = new CreateNotification(
            command.CustomerId,
            command.Message,
            command.Type);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CreateCustomerCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("CustomerId", command.CustomerId))
        {
            var result = await _commandProcessor.SendAsync(createNotification, cancellationToken);

            return Unit.Value;
        }
    }
}
