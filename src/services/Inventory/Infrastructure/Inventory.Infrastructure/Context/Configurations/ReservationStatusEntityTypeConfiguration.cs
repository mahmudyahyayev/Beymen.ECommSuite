using BuildingBlocks.Core.Persistence.Efcore;
using Humanizer;
using Inventory.Domain.AggregateRoot.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Context.Configurations;

public class ReservationStatusEntityTypeConfiguration : IEntityTypeConfiguration<ReservationStatus>
{
    public void Configure(EntityTypeBuilder<ReservationStatus> builder)
    {
        builder.ToTable(nameof(ReservationStatus).Pluralize().Underscore(), ApplicationDbContext.RezervationSchema);

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

    private IEnumerable<ReservationStatus> PopulatedDefaults
        => ReservationStatus.List();
}
