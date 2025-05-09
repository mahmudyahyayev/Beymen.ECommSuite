using System.Reflection;
using BuildingBlocks.Core.Persistence.Efcore;
using BuildingBlocks.Core.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Notification.Domain;
using Notification.Domain.AggregateRoot.Notifications;
using Notification.Domain.Projections;

namespace Notification.Infrastructure.Context;

public class ApplicationDbContext : EfDbContextBase, IApplicationDbContext
{
    public const string DefaultSchema = "public";
    public const string NotificationSchema = "notification";
    public const string CustomerSchema = "customer";
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public DatabaseFacade Connection => this.Database;

    DbSet<CustomerReadModel> IApplicationDbContext.Customers => Set<CustomerReadModel>();

    public DbSet<NotificationStatus> NotificationStatuses => Set<NotificationStatus>();

    public DbSet<NotificationType> NotificationTypes => Set<NotificationType>();

    public DbSet<Domain.AggregateRoot.Notifications.Notification> Notifications => Set<Domain.AggregateRoot.Notifications.Notification>();
}
