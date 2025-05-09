using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Efcore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Immutable;
using System.Data;
using System.Linq.Expressions;

namespace BuildingBlocks.Core.Persistence.EfCore
{
    public abstract class EfIdentityDbContextBase<T, TRole> : IdentityDbContext<T, TRole, string>, IDbFacadeResolver,
        IDbContext, IDomainEventContext
        where T : IdentityUser where TRole : IdentityRole
    {
        private IDbContextTransaction? _currentTransaction;

        protected EfIdentityDbContextBase(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            AddingSofDeletes(modelBuilder);
            AddingVersioning(modelBuilder);
        }

        private static void AddingVersioning(ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes()
                .Where(x => x.ClrType.IsAssignableTo(typeof(IHaveAggregateVersion)));
            foreach (var entityType in types)
            {
                builder
                    .Entity(entityType.ClrType)
                    .Property(nameof(IHaveAggregateVersion.OriginalVersion))
                    .IsConcurrencyToken();
            }
        }

        private static void AddingSofDeletes(ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().Where(x => x.ClrType.IsAssignableTo(typeof(ISoftDeletable)));
            foreach (var entityType in types)
            {
                entityType.AddProperty("IsDeleted", typeof(bool));
                var parameter = Expression.Parameter(entityType.ClrType);
                var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                var isDeletedProperty =
                    Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));
                BinaryExpression compareExpression = Expression.MakeBinary(
                    ExpressionType.Equal,
                    isDeletedProperty,
                    Expression.Constant(false)
                );
                var lambda = Expression.Lambda(compareExpression, parameter);
                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        public async Task BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken
        )
        {
            _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);

                if (_currentTransaction is not null)
                    await _currentTransaction.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_currentTransaction is not null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken)!;
                }
            }
            finally
            {
                if (_currentTransaction is not null)
                {
                    _currentTransaction.Dispose();
                }

                _currentTransaction = null;
            }
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task RetryOnExceptionAsync(Func<Task> operation)
        {
            return Database.CreateExecutionStrategy().ExecuteAsync(operation);
        }

        public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
        {
            return Database.CreateExecutionStrategy().ExecuteAsync(operation);
        }

        public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
        {
            var domainEvents = ChangeTracker
                .Entries<IHaveAggregate>()
                .Where(x => x.Entity.GetUncommittedDomainEvents().Any())
                .SelectMany(x => x.Entity.GetUncommittedDomainEvents())
                .ToList();

            return domainEvents.ToImmutableList();
        }

        public void MarkUncommittedDomainEventAsCommitted()
        {
            ChangeTracker
                .Entries<IHaveAggregate>()
                .Where(x => x.Entity.GetUncommittedDomainEvents().Any())
                .ToList()
                .ForEach(x => x.Entity.MarkUncommittedDomainEventAsCommitted());
        }

        public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            var strategy = Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await action();

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
        {
            var strategy = Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await action();

                    await transaction.CommitAsync(cancellationToken);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}
