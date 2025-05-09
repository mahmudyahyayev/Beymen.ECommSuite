using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Inventory.Domain.AggregateRoot.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Context.Configurations;


public class ProductStatusEntityTypeConfiguration : IEntityTypeConfiguration<ProductStatus>
{
    public void Configure(EntityTypeBuilder<ProductStatus> builder)
    {
        builder.ToTable(nameof(ProductStatus).Pluralize().Underscore(), ApplicationDbContext.ProductSchema);

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

    private IEnumerable<ProductStatus> PopulatedDefaults
        => ProductStatus.List();
}
