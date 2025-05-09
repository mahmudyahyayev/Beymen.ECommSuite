using System.Reflection;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.Core.Persistence.EfCore.Converters;
public class AggregateIdValueConverter<TAggregateId, TId> : ValueConverter<TAggregateId, TId>
    where TAggregateId : AggregateId<TId>
{
    public AggregateIdValueConverter(ConverterMappingHints mappingHints = null!)
        : base(id => id.Value, value => Create(value), mappingHints) { }
    private static TAggregateId Create(TId id) =>
        (
            Activator.CreateInstance(
                typeof(TAggregateId),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object?[] { id },
                null,
                null
            ) as TAggregateId
        )!;
}
