using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.AggregateRoot.Orders;

namespace Order.Infrastructure.Context.Configurations;
public class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable(nameof(OrderStatus).Pluralize().Underscore(), ApplicationDbContext.OrderSchema);

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

    private IEnumerable<OrderStatus> PopulatedDefaults
        => OrderStatus.List();
}
