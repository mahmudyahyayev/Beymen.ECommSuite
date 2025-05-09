using System.Data.Common;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;

namespace BuildingBlocks.Abstractions.Messaging.PersistMessage
{
    public interface IMessagePersistenceService
    {
        public DbConnection DbConnection { get; set; }
        Task ProcessAllAsync(CancellationToken cancellationToken);
        Task ProcessAllErrorAsync(CancellationToken cancellationToken);
        Task UpdateStuckProcessingStatusesAsync(CancellationToken cancellationToken);

        Task AddPublishMessageAsync<TMessageEnvelope>(
            TMessageEnvelope messageEnvelope,
            CancellationToken cancellationToken
        )
            where TMessageEnvelope : MessageEnvelope;

        Task AddInternalMessageAsync<TCommand>(
            TCommand internalCommand,
            string correlationId,
            CancellationToken cancellationToken)
            where TCommand : class, IInternalCommand;

        Task AddNotificationAsync(
            IDomainNotificationEvent notification,
            CancellationToken cancellationToken);

        Task AddReceivedMessageAsync<TMessageEnvelope>(
            TMessageEnvelope messageEnvelope,
            CancellationToken cancellationToken
        ) where TMessageEnvelope : MessageEnvelope;
    }
}
