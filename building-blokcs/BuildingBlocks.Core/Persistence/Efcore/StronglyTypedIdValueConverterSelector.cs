﻿using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Persistence.EfCore.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Concurrent;

namespace BuildingBlocks.Core.Persistence.Efcore
{
    public class StronglyTypedIdValueConverterSelector<TId> : ValueConverterSelector
    {
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters =
            new();

        public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies)
            : base(dependencies) { }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type providerClrType = null)
        {
            var baseConverters = base.Select(modelClrType, providerClrType);
            foreach (var converter in baseConverters)
            {
                yield return converter;
            }

            var underlyingModelType = UnwrapNullableType(modelClrType);
            var underlyingProviderType = UnwrapNullableType(providerClrType);

            if (underlyingProviderType is null || underlyingProviderType == typeof(TId))
            {
                var isAggregateTypedIdValue = typeof(AggregateId<TId>).IsAssignableFrom(underlyingModelType);
                var isEntityTypedIdValue = typeof(EntityId<TId>).IsAssignableFrom(underlyingModelType);
                if (isAggregateTypedIdValue)
                {
                    var converterType = typeof(AggregateIdValueConverter<,>).MakeGenericType(
                        underlyingModelType,
                        typeof(TId)
                    );

                    yield return _converters.GetOrAdd(
                        (underlyingModelType, typeof(TId)),
                        _ =>
                        {
                            return new ValueConverterInfo(
                                modelClrType: modelClrType,
                                providerClrType: typeof(TId),
                                factory: valueConverterInfo =>
                                    (ValueConverter)Activator.CreateInstance(converterType, valueConverterInfo.MappingHints)
                            );
                        }
                    );
                }
                else if (isEntityTypedIdValue)
                {
                    var converterType = typeof(EntityIdValueConverter<,>).MakeGenericType(underlyingModelType, typeof(TId));

                    yield return _converters.GetOrAdd(
                        (underlyingModelType, typeof(TId)),
                        _ =>
                        {
                            return new ValueConverterInfo(
                                modelClrType: modelClrType,
                                providerClrType: typeof(TId),
                                factory: valueConverterInfo =>
                                    (ValueConverter)Activator.CreateInstance(converterType, valueConverterInfo.MappingHints)
                            );
                        }
                    );
                }
            }
        }

        private static Type UnwrapNullableType(Type type)
        {
            if (type is null)
            {
                return null;
            }

            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
