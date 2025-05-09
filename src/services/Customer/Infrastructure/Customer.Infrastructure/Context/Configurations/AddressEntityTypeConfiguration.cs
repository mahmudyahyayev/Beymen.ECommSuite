using BuildingBlocks.Core.Persistence.Efcore;
using Customer.Domain.AggregateRoot.Addresses;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Context.Configurations;

public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable(nameof(Address).Pluralize().Underscore(), ApplicationDbContext.AddressSchema);

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
            .Property<int>("_typeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("type_id")
            .IsRequired();
        builder
            .HasOne(o => o.Type)
            .WithMany()
            .HasForeignKey("_typeId");

        builder.OwnsOne(
          x => x.CustomerId,
          a =>
          {
              a.Property(p => p.Value)
                 .HasColumnName("customer_id")
                 .IsRequired(true)
                 .HasField("_value");
          });

        builder.OwnsOne(
          x => x.Country,
          a =>
          {
              a.Property(p => p.Value)
                 .HasColumnName("country")
                 .IsRequired(true)
                 .HasMaxLength(EfConstants.Lenght.Long)
                 .HasField("_value");
          });

        builder.OwnsOne(
          x => x.City,
          a =>
          {
              a.Property(p => p.Value)
                 .HasColumnName("city")
                 .IsRequired(true)
                 .HasMaxLength(EfConstants.Lenght.Long)
                 .HasField("_value");
          });

        builder.OwnsOne(
            x => x.District,
            a =>
            {
                a.Property(p => p.Value)
                   .HasColumnName("district")
                   .IsRequired(true)
                   .HasMaxLength(EfConstants.Lenght.Long)
                   .HasField("_value");
            });

        builder.OwnsOne(
           x => x.Street,
           a =>
           {
               a.Property(p => p.Value)
                  .HasColumnName("street")
                  .IsRequired(true)
                  .HasMaxLength(EfConstants.Lenght.Long)
                  .HasField("_value");
           });

        builder.OwnsOne(
          x => x.ZipCode,
          a =>
          {
              a.Property(p => p.Value)
                 .HasColumnName("zip_code")
                 .IsRequired(true)
                 .HasMaxLength(EfConstants.Lenght.CitizenId)
                 .HasField("_value");
          });

        builder
           .Property(x => x.BuildingNo)
           .HasMaxLength(EfConstants.Lenght.CitizenId)
           .HasField("_buildingNo")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .IsRequired(true);

        builder
          .Property(x => x.ApartmentNo)
          .HasMaxLength(EfConstants.Lenght.CitizenId)
          .HasField("_apartmentNo")
          .UsePropertyAccessMode(PropertyAccessMode.Field)
          .IsRequired(true);

        builder
         .Property(x => x.Floor)
         .HasMaxLength(EfConstants.Lenght.CitizenId)
         .HasField("_floor")
         .UsePropertyAccessMode(PropertyAccessMode.Field)
         .IsRequired(true);

        builder
        .Property(x => x.Description)
        .HasMaxLength(EfConstants.Lenght.SoLong)
        .HasField("_description")
        .UsePropertyAccessMode(PropertyAccessMode.Field)
        .IsRequired(false);
    }
}