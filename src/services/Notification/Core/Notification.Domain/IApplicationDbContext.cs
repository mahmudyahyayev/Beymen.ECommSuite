using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Notification.Domain.AggregateRoot.Notifications;
using Notification.Domain.Projections;

namespace Notification.Domain;

public interface IApplicationDbContext
{
    DbSet<AggregateRoot.Notifications.Notification> Notifications { get; }
    DbSet<CustomerReadModel> Customers { get; }
    DbSet<NotificationStatus> NotificationStatuses { get; }
    DbSet<NotificationType> NotificationTypes { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

    