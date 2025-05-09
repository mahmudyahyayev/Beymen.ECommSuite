using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Projections;

namespace Notification.Infrastructure.Context.Configurations;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<CustomerReadModel>
{
    public void Configure(EntityTypeBuilder<CustomerReadModel> builder)
    {
        builder.ToTable("customers", ApplicationDbContext.CustomerSchema);

        builder.Property(x => x.Created)
            .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
            .HasDefaultValueSql(EfConstants.DateAlgorithm);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();


        builder
            .Property(x => x.IsActive)
            .HasField("_isActive")
            .HasColumnName("is_active")
            .HasDefaultValue(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();

        builder
            .Property(x => x.Email)
            .HasField("_email")
            .HasMaxLength(100)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
            .Property(x => x.PhoneNumber)
            .HasField("_phoneNumber")
            .HasMaxLength(15)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);


        builder
            .Property(x => x.LastName)
            .HasField("_lastName")
            .HasMaxLength(100)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired(true);

        builder
          .Property(x => x.FirstName)
          .HasField("_firstName")
          .HasMaxLength(100)
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);
    }
}