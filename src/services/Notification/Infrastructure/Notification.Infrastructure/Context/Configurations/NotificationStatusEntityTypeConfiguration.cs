using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.AggregateRoot.Notifications;

namespace Notification.Infrastructure.Context.Configurations;

public class NotificationStatusEntityTypeConfiguration : IEntityTypeConfiguration<NotificationStatus>
{
    public void Configure(EntityTypeBuilder<NotificationStatus> builder)
    {
        builder.ToTable(nameof(NotificationStatus).Pluralize().Underscore(), ApplicationDbContext.NotificationSchema);

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

    private IEnumerable<NotificationStatus> PopulatedDefaults
        => NotificationStatus.List();
}