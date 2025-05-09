using BuildingBlocks.Core.Persistence.Efcore;
using Customer.Domain.AggregateRoot.Addresses;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Context.Configurations;

public class AddressTypeEntityTypeConfiguration : IEntityTypeConfiguration<AddressType>
{
    public void Configure(EntityTypeBuilder<AddressType> builder)
    {
        builder.ToTable(nameof(AddressType).Pluralize().Underscore(), ApplicationDbContext.AddressSchema);

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

    private IEnumerable<AddressType> PopulatedDefaults
        => AddressType.List();
}

