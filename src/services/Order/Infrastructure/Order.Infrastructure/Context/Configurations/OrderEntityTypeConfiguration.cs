using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.AggregateRoot.Orders;

namespace Order.Infrastructure.Context.Configurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregateRoot.Orders.Order>
{
    public void Configure(EntityTypeBuilder<Domain.AggregateRoot.Orders.Order> builder)
    {
        builder.ToTable(nameof(Domain.AggregateRoot.Orders.Order).Pluralize().Underscore(), ApplicationDbContext.OrderSchema);

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

        builder.OwnsOne(
         x => x.CustomerId,
         a =>
         {
             a.Property(p => p.Value)
                .HasColumnName("customer_id")
                .IsRequired(true)
                .HasField("_value");
         });

        builder.HasMany(s => s.Items)
         .WithOne(s => s.Order)
         .HasForeignKey(x => x.OrderId)
         .OnDelete(DeleteBehavior.Cascade);

        builder
        .Property(x => x.TotalPrice)
        .HasColumnName("total_price")
        .HasField("_totalPrice")
        .HasMaxLength(EfConstants.Lenght.SoLong)
        .UsePropertyAccessMode(PropertyAccessMode.Field)
        .IsRequired(true);

        builder.OwnsOne(
        x => x.ShippingAddress,
        a =>
        {
            a.Property(p => p.AddressId)
               .HasColumnName("shipping_address_id")
               .IsRequired(true)
               .HasField("_addressId");

            a.Property(p => p.Address)
              .HasColumnName("shipping_address")
              .IsRequired(true)
              .HasField("_address");
        });

        builder.OwnsOne(
       x => x.BillingAddress,
       a =>
       {
           a.Property(p => p.AddressId)
              .HasColumnName("billing_address_id")
              .IsRequired(true)
              .HasField("_addressId");

           a.Property(p => p.Address)
             .HasColumnName("billing_address")
             .IsRequired(true)
             .HasField("_address");
       });

    }
}
