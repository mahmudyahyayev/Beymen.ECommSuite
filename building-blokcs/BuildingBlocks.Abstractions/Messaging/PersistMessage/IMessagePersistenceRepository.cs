using System.Data.Common;
using System.Linq.Expressions;

namespace BuildingBlocks.Abstractions.Messaging.PersistMessage
{
    public interface IMessagePersistenceRepository
    {
        public DbConnection DbConnection { get; set; }

        Task ChangeStateAsync(
            Guid messageId, MessageStatus status,
            string errorMessage,
            CancellationToken cancellationToken);

        Task ChangeBulkStateAsync(
            List<Guid> messageIds,
            MessageStatus status,
            string errorMessage,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
            Expression<Func<StoreMessage, bool>> predicate,
            CancellationToken cancellationToken,
            int? batchCount = null);

        Task<List<int>> GetPodPartitionByPodIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<StoreMessage?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task AddAsync(
            StoreMessage storeMessage,
            CancellationToken cancellationToken);
    }
}
