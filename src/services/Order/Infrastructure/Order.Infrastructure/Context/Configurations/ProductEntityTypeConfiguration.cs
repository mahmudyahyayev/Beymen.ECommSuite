using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Projections;

namespace Order.Infrastructure.Context.Configurations;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.ToTable("products", ApplicationDbContext.ProductSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();


        builder
            .Property(x => x.Name)
            .HasField("_name")
            .HasMaxLength(100)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);


        builder
            .Property(x => x.UnitPrice)
            .HasField("_unitPrice")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
          .Property(x => x.Status)
          .HasField("_status")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);
    }
}
