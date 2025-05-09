using System.Linq.Expressions;
using System.Text.Json;
using BuildingBlocks.Abstractions.CQRS;
using MongoDB.Driver.Core.Misc;

namespace BuildingBlocks.Core.Persistence.Efcore;

public static class ExpressionExtensions
{
    public static MemberExpression GetPropertyExpression(Expression parameter, string propertyName)
    {
        var properties = propertyName.Split('.');
        Expression property = parameter;

        foreach (var prop in properties)
        {
            property = Expression.Property(property, prop);
        }

        return (MemberExpression)property;
    }

    public static Expression GenerateEqualExpression(Expression member, object? value)
    {
        var constant = Expression.Constant(ConvertValue(value, member.Type), member.Type);

        return Expression.Equal(
            Expression.Convert(member, typeof(object)),
            Expression.Convert(constant, typeof(object)));
    }
    public static Expression GenerateIgnoreCaseStringExpression(string methodName, Expression member, object? value)
    {
        if (member.Type != typeof(string)) throw new InvalidOperationException("Ignore case ops only valid on string");

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var memberToLower = Expression.Call(member, toLowerMethod);

        var constantValue = Expression.Constant(value?.ToString()?.ToLower() ?? "", typeof(string));
        return Expression.Call(memberToLower, typeof(string).GetMethod(methodName, new[] { typeof(string) })!, constantValue);
    }

    public static Expression? ProcessFilters(IEnumerable<LoadOptionFilter> filters, ParameterExpression parameter)
    {
        Expression? combined = null;
        foreach (var filter in filters)
        {
            Expression? expression = null;
            if (filter.FilterType == FilterTypes.Constraint)
            {
                var member = GetPropertyExpression(parameter, filter?.ColumnName);

                expression = filter.EqualType switch
                {
                    ConditionTypes.Equals => GenerateEqualExpression(member, filter?.Value),
                    ConditionTypes.NotEquals => GenerateNotEqualExpression(member, filter?.Value),
                    ConditionTypes.Contains => GenerateIgnoreCaseStringExpression(nameof(string.Contains), member, filter?.Value),
                    ConditionTypes.StartsWith => GenerateIgnoreCaseStringExpression(nameof(string.StartsWith), member, filter?.Value),
                    ConditionTypes.EndsWith => GenerateIgnoreCaseStringExpression(nameof(string.EndsWith), member, filter?.Value),
                    ConditionTypes.GreaterThan => GenerateGreaterThanExpression(member, filter?.Value),
                    ConditionTypes.LessThan => GenerateLessThanExpression(member, filter?.Value),
                    ConditionTypes.In => GenerateInExpression(member, filter?.Value),
                    _ => expression
                };
            }
            else if (filter.Filters != null && filter.FilterType == FilterTypes.Group)
            {
                expression = ProcessFilters(filter.Filters, parameter);
            }

            if (expression == null) continue;

            if (combined == null)
            {
                combined = expression;
            }
            else
            {
                combined = filter.Type switch
                {
                    OperatorTypes.And => Expression.AndAlso(combined, expression),
                    OperatorTypes.Or => Expression.OrElse(combined, expression),
                    _ => combined
                };
            }
        }
        return combined;
    }

    public static object? ConvertValue(object value, Type targetType)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value is not JsonElement jsonElement)
        {
            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value.ToString()!);
            }

            if (value == null || !typeof(IConvertible).IsAssignableFrom(value.GetType()))
            {
                return null;
            }

            return Convert.ChangeType(value, underlyingType);
        }

        return underlyingType switch
        {
            _ when underlyingType == typeof(string) => jsonElement.GetString(),
            _ when underlyingType == typeof(int) => jsonElement.GetInt32(),
            _ when underlyingType == typeof(bool) => jsonElement.GetBoolean(),
            _ when underlyingType == typeof(double) => jsonElement.GetDouble(),
            _ when underlyingType == typeof(DateTime) => jsonElement.GetDateTime(),
            _ when underlyingType == typeof(long) => jsonElement.GetInt64(),
            _ when underlyingType == typeof(float) => jsonElement.GetSingle(),
            _ when underlyingType == typeof(Guid) => jsonElement.GetGuid(),
            _ when underlyingType == typeof(byte) => jsonElement.GetByte(),
            _ when underlyingType == typeof(short) => jsonElement.GetInt16(),
            _ when underlyingType == typeof(decimal) => jsonElement.GetDecimal(),
            _ when underlyingType == typeof(uint) => jsonElement.GetUInt32(),
            _ when underlyingType == typeof(ulong) => jsonElement.GetUInt64(),
            _ when underlyingType == typeof(ushort) => jsonElement.GetUInt16(),
            _ when underlyingType == typeof(sbyte) => jsonElement.GetSByte(),
            _ => underlyingType.IsEnum ? Enum.Parse(underlyingType, jsonElement.GetString()!) : Convert.ChangeType(value, underlyingType)
        };
    }

    public static Expression GenerateNotEqualExpression(Expression member, object? value)
    {
        var constant = Expression.Constant(ConvertValue(value, member.Type), member.Type);

        return Expression.NotEqual(
            Expression.Convert(member, typeof(object)),
            Expression.Convert(constant, typeof(object)));
    }
    public static Expression GenerateLessThanExpression(Expression member, object? value)
    {
        var constant = Expression.Constant(ConvertValue(value, member.Type), member.Type);

        return Expression.LessThan(
            Expression.Convert(member, typeof(object)),
            Expression.Convert(constant, typeof(object)));
    }

    public static Expression GenerateInExpression(MemberExpression member, object? values)
    {
        if (values is not JsonElement || ((JsonElement)values).ValueKind != JsonValueKind.Array)
        {
            throw new System.Exception("Bir hata ile karşılaştık. Lütfen işlemi yeniden deneyin. Sorunun devamı halinde Müşteri Hizmetleri ile iletişime geçebilirsiniz.");
        }

        Expression result = default!;
        List<object> valueList = JsonSerializer.Deserialize<List<object>>((JsonElement)values!)!;

        foreach (var value in valueList)
        {
            var expression = GenerateEqualExpression(member, value);
            result = (result == null) ? expression : Expression.OrElse(result, expression);
        }

        return result;
    }

    public static Expression GenerateExpressionFor<T>(string methodName, Expression member, object? value)
    {
        var constant = Expression.Constant(ConvertValue(value, member.Type), member.Type);
        return Expression.Call(member, typeof(T).GetMethod(methodName, new[] { typeof(T) })!, constant);
    }
    public static Expression GenerateGreaterThanExpression(Expression member, object? value)
    {
        var targetType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
        var constant = Expression.Constant(ConvertValue(value, targetType), targetType);

        if (targetType.IsPrimitive || targetType == typeof(DateTime) || targetType == typeof(decimal))
        {
            if (Nullable.GetUnderlyingType(member.Type) != null)
            {
                var nullableConstant = Expression.Convert(constant, member.Type);
                return Expression.GreaterThan(member, nullableConstant);
            }
            return Expression.GreaterThan(member, constant);
        }

        if (typeof(IComparable).IsAssignableFrom(targetType))
        {
            var compareToMethod = typeof(IComparable).GetMethod(nameof(IComparable.CompareTo), new[] { typeof(object) });

            var compareToExpression = Expression.Call(
                Expression.Convert(member, typeof(IComparable)),
                compareToMethod!,
                Expression.Convert(constant, typeof(object))
            );

            return Expression.GreaterThan(compareToExpression, Expression.Constant(0));
        }
        return Expression.Constant(false);
    }
}
