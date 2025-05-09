using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Context.Configurations;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregateRoot.Customers.Customer>
{
    public void Configure(EntityTypeBuilder<Domain.AggregateRoot.Customers.Customer> builder)
    {
        builder.ToTable(nameof(Domain.AggregateRoot.Customers.Customer).Pluralize().Underscore(), ApplicationDbContext.CustomerSchema);

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
            .Property(x => x.IsActive)
            .HasField("_isActive")
            .HasColumnName("is_active")
            .HasDefaultValue(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();


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


        builder.OwnsOne(
           x => x.PhoneNumber,
           a =>
           {
               a.Property(p => p.Value)
                  .HasColumnName("phone_number")
                  .IsRequired(true)
                  .HasMaxLength(EfConstants.Lenght.Tiny)
                  .HasField("_value");
           });

        builder.OwnsOne(
          x => x.Email,
          a =>
          {
              a.Property(p => p.Value)
                 .HasColumnName("email")
                 .IsRequired(true)
                 .HasMaxLength(EfConstants.Lenght.ExtraMedium)
                 .HasField("_value");
          });


    }
}
