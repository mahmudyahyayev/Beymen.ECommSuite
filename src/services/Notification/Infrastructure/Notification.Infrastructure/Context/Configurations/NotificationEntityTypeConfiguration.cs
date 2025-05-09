using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.Infrastructure.Context.Configurations;

public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregateRoot.Notifications.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.AggregateRoot.Notifications.Notification> builder)
    {
        builder.ToTable(nameof(Domain.AggregateRoot.Notifications.Notification).Pluralize().Underscore(), ApplicationDbContext.NotificationSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder
            .Property(x => x.LastModified)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasField("_lastModified")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(false);

        builder
            .Property(x => x.LastModifiedBy)
            .HasField("_lastModifiedBy")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(false);

        builder
            .Property<int>("_typeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("type_id")
            .IsRequired();
        builder
            .HasOne(o => o.Type)
            .WithMany()
            .HasForeignKey("_typeId");

        builder
            .Property<int>("_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("status")
            .IsRequired();
        builder
            .HasOne(o => o.Status)
            .WithMany()
            .HasForeignKey("_status");


        builder
            .Property(x => x.Recipient)
            .HasField("_recipient")
            .HasMaxLength(100)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(false);

        builder
            .Property(x => x.Message)
            .HasField("_message")
            .HasMaxLength(EfConstants.Lenght.SoLong)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
           .Property(x => x.ErrorMessage)
           .HasColumnName("error_message")
           .HasField("_errorMessage")
           .HasMaxLength(EfConstants.Lenght.SoLong)
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .IsRequired(false);

        builder
           .Property(x => x.SentAt)
           .HasField("_sentAt")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .IsRequired(false);

    }
}

