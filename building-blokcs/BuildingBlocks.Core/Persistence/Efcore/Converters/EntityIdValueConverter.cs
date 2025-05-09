using System.Reflection;
using BuildingBlocks.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildingBlocks.Core.Persistence.EfCore.Converters;
public class EntityIdValueConverter<TEntityId, TId> : ValueConverter<TEntityId, TId>
    where TEntityId : EntityId<TId>
{
    public EntityIdValueConverter(ConverterMappingHints mappingHints = null!)
        : base(id => id.Value, value => Create(value), mappingHints) { }

    private static TEntityId Create(TId id) =>
        (
            Activator.CreateInstance(
                typeof(TEntityId),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new object?[] { id },
                null,
                null
            ) as TEntityId
        )!;
}
