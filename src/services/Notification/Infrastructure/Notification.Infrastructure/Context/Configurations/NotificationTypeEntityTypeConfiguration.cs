using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.AggregateRoot.Notifications;

namespace Notification.Infrastructure.Context.Configurations;

public class NotificationTypeEntityTypeConfiguration : IEntityTypeConfiguration<NotificationType>
{
    public void Configure(EntityTypeBuilder<NotificationType> builder)
    {
        builder.ToTable(nameof(NotificationType).Pluralize().Underscore(), ApplicationDbContext.NotificationSchema);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasDefaultValue(1);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Id).IsUnique();

        builder
            .Property(x => x.Name)
            .HasColumnType(EfConstants.ColumnTypes.MediumText)
            .IsRequired();

        builder.HasData(PopulatedDefaults);
    }

    private IEnumerable<NotificationType> PopulatedDefaults
        => NotificationType.List();
}

