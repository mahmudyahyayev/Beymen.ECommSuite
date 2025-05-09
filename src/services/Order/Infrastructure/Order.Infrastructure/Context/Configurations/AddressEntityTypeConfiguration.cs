using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Projections;

namespace Order.Infrastructure.Context.Configurations;

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<AddressReadModel>
{
    public void Configure(EntityTypeBuilder<AddressReadModel> builder)
    {
        builder.ToTable("addresses", ApplicationDbContext.AddressSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();


        builder
            .Property(x => x.FullAddress)
            .HasField("_fullAddress")
            .HasMaxLength(2000)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
          .Property(x => x.CustomerId)
          .HasField("_customerId")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);


        builder
          .Property(x => x.Type)
          .HasField("_type")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);
    }
}
