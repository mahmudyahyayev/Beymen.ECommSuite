using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.CQRS.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public CommandProcessor(IMediator mediator, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command,
            CancellationToken cancellationToken)
            where TResult : notnull
        {
            return _mediator.Send(command, cancellationToken);
        }

        public async Task ScheduleAsync(
            IInternalCommand internalCommand,
            string correlationId,
            CancellationToken cancellationToken)
        {
            var messagePersistenceService = _serviceProvider.GetRequiredService<IMessagePersistenceService>();

            await messagePersistenceService.AddInternalMessageAsync(
                internalCommand: internalCommand,
                correlationId: correlationId,
                cancellationToken: cancellationToken);
        }

        public async Task ScheduleAsync(
            IInternalCommand[] internalCommands,
            string correlationId,
            CancellationToken cancellationToken)
        {
            foreach (var internalCommand in internalCommands)
            {
                await ScheduleAsync(
                    internalCommand: internalCommand,
                    correlationId,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
