using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Inventory.Domain.AggregateRoot.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Context.Configurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(Product).Pluralize().Underscore(), ApplicationDbContext.ProductSchema);

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
                .Property(x => x.Name)
                .HasField("_name")
                .HasMaxLength(EfConstants.Lenght.Medium)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .IsRequired(true);

        builder
            .Property(x => x.Description)
            .HasField("_description")
            .HasMaxLength(EfConstants.Lenght.ExtraLong)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);


        builder
            .Property(x => x.Price)
            .HasField("_price")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
          .Property(x => x.Stock)
          .HasField("_stock")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);


        builder
           .Property<int>("_status")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("status")
           .IsRequired();
        builder
            .HasOne(o => o.Status)
            .WithMany()
            .HasForeignKey("_status");
    }
}
