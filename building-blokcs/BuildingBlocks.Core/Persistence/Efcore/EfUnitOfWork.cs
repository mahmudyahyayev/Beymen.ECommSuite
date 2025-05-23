﻿using BuildingBlocks.Abstractions.CQRS.Events;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BuildingBlocks.Core.Persistence.Efcore
{
    public class EfUnitOfWork<TDbContext> : IEfUnitOfWork<TDbContext>
     where TDbContext : EfDbContextBase
    {
        private readonly TDbContext _context;
        private readonly IDomainEventsAccessor _domainEventsAccessor;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly ILogger<EfUnitOfWork<TDbContext>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EfUnitOfWork(
            TDbContext context,
            IDomainEventsAccessor domainEventsAccessor,
            IDomainEventPublisher domainEventPublisher,
            ILogger<EfUnitOfWork<TDbContext>> logger,
            IServiceProvider serviceProvider
        )
        {
            _context = context;
            _domainEventsAccessor = domainEventsAccessor;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public TDbContext DbContext => _context;

        public DbSet<TEntity> Set<TEntity>()
            where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            return _context.BeginTransactionAsync(isolationLevel, cancellationToken);
        }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return _context.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = _domainEventsAccessor.UnCommittedDomainEvents;
            await _domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

            await _context.CommitTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = _domainEventsAccessor.UnCommittedDomainEvents;
            await _domainEventPublisher.PublishAsync(domainEvents.ToArray(), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            return _context.RollbackTransactionAsync(cancellationToken);
        }

        public Task RetryOnExceptionAsync(Func<Task> operation)
        {
            return _context.RetryOnExceptionAsync(operation);
        }

        public Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation)
        {
            return _context.RetryOnExceptionAsync(operation);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            return _context.ExecuteTransactionalAsync(action, cancellationToken);
        }

        public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            return _context.ExecuteTransactionalAsync(action, cancellationToken);
        }

        public T GetRepository<T>() where T : IRepository
        {
            return _serviceProvider.GetService<T>();
        }
    }
}