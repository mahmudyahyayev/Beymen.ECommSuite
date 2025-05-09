using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Inventory.Domain.AggregateRoot.Reservations;
using Humanizer;

namespace Inventory.Infrastructure.Context.Configurations;

public class ReservationEntityTypeConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable(nameof(Reservation).Pluralize().Underscore(), ApplicationDbContext.RezervationSchema);

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
            .Property<int>("_status")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("status")
            .IsRequired();
        builder
            .HasOne(o => o.Status)
            .WithMany()
            .HasForeignKey("_status");

        builder
          .Property(x => x.ErrorMessage)
          .HasColumnName("error_message")
          .HasField("_errorMessage")
          .HasMaxLength(EfConstants.Lenght.SoLong)
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(false);

        builder.OwnsOne(
         x => x.CustomerId,
         a =>
         {
             a.Property(p => p.Value)
                .HasColumnName("customer_id")
                .IsRequired(true)
                .HasField("_value");
         });

        builder.OwnsOne(
         x => x.OrderId,
         a =>
         {
             a.Property(p => p.Value)
                .HasColumnName("order_id")
                .IsRequired(true)
                .HasField("_value");
         });

        builder.HasMany(s => s.Items)
         .WithOne(s => s.Reservation)
         .HasForeignKey(x => x.ReservationId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}

