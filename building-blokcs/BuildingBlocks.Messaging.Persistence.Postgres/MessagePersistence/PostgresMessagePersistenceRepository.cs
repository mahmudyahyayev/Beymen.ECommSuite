using System.Data.Common;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence
{
    public class PostgresMessagePersistenceRepository : IMessagePersistenceRepository
    {
        private readonly MessagePersistenceDbContext _persistenceDbContext;
        private readonly ILogger<PostgresMessagePersistenceRepository> _logger;
        private readonly MessagePersistenceOptions _options;
        public DbConnection DbConnection { get; set; }

        public PostgresMessagePersistenceRepository(
            MessagePersistenceDbContext persistenceDbContext,
            ILogger<PostgresMessagePersistenceRepository> logger,
            IOptions<MessagePersistenceOptions> options
        )
        {
            _persistenceDbContext = persistenceDbContext;
            _logger = logger;
            _options = options.Value;
        }

        public async Task ChangeStateAsync(
            Guid messageId,
            MessageStatus status,
            string errorMessage,
            CancellationToken cancellationToken)
        {
            var message =
                await _persistenceDbContext.StoreMessages
                    .FirstOrDefaultAsync(x => x.Id == messageId,
                        cancellationToken);

            if (message is not null)
            {
                message.ChangeState(status, _options.ErrorRetryCount);
                message.ChangeLastError(errorMessage);
                await _persistenceDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task ChangeBulkStateAsync(
            List<Guid> messageIds,
            MessageStatus status,
            string errorMessage,
            CancellationToken cancellationToken)
        {
            var messages =
                await _persistenceDbContext.StoreMessages
                    .Where(x => messageIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    message.ChangeState(status, _options.ErrorRetryCount);
                    message.ChangeLastError(errorMessage);
                }

                await _persistenceDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
            Expression<Func<StoreMessage, bool>> predicate,
            CancellationToken cancellationToken,
            int? batchCount = null)
        {
            return (
                await _persistenceDbContext.StoreMessages
                    .AsNoTracking()
                    .Where(predicate)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.Created)
                    .Take(batchCount ?? _options.BatchCount)
                    .ToListAsync(cancellationToken)
            ).AsReadOnly();
        }

        public async Task<List<int>> GetPodPartitionByPodIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return (
                await _persistenceDbContext.PodPartitions
                    .AsNoTracking()
                    .Where(t => t.PodId == id)
                    .Select(t => (int)t.Partition)
                    .ToListAsync(cancellationToken));
        }

        public async Task<StoreMessage?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _persistenceDbContext.StoreMessages
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(
            StoreMessage storeMessage,
            CancellationToken cancellationToken)
        {
            if (DbConnection != null)
                _persistenceDbContext.Database.SetDbConnection(DbConnection);

            await _persistenceDbContext.StoreMessages.AddAsync(storeMessage, cancellationToken);

            await _persistenceDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
