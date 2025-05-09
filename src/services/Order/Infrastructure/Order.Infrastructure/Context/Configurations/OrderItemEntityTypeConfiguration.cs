using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Order.Domain.AggregateRoot.Orders;
using Humanizer;

namespace Order.Infrastructure.Context.Configurations;

public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(nameof(OrderItem).Pluralize().Underscore(), ApplicationDbContext.OrderSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();


        builder
         .Property(x => x.Quantity)
         .HasColumnName("quantity")
         .HasField("_quantity")
         .UsePropertyAccessMode(PropertyAccessMode.Field)
         .IsRequired(true);

        builder
        .Property(x => x.TotalPrice)
        .HasColumnName("total_price")
        .HasField("_totalPrice")
        .UsePropertyAccessMode(PropertyAccessMode.Field)
        .IsRequired(true);

        builder.OwnsOne(
       x => x.Product,
       a =>
       {
           a.Property(p => p.ProductId)
              .HasColumnName("product_id")
              .IsRequired(true)
              .HasField("_productId");

           a.Property(p => p.Name)
             .HasColumnName("product_name")
             .IsRequired(true)
             .HasField("_name");

           a.Property(p => p.UnitPrice)
            .HasColumnName("unit_price")
            .IsRequired(true)
            .HasField("_unitPrice");
       });

    }
}