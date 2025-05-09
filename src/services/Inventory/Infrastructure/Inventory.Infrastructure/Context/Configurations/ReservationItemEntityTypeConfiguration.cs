using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Inventory.Domain.AggregateRoot.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Context.Configurations;
public class ReservationItemEntityTypeConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.ToTable(nameof(ReservationItem).Pluralize().Underscore(), ApplicationDbContext.RezervationSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();


        builder
          .Property(x => x.ProductId)
          .HasColumnName("product_id")
          .HasField("_productId")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);

        builder
         .Property(x => x.Quantity)
         .HasColumnName("quantity")
         .HasField("_quantity")
         .UsePropertyAccessMode(PropertyAccessMode.Field)
         .IsRequired(true);
    }
}