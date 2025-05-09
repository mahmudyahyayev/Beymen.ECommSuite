using System.Linq.Expressions;
using System.Reflection;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.CQRS.Queries;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Core.Persistence.Efcore;

public static class EfCoreQueryableExtensions
{
    public static async Task<PagedList<T>> ApplyLoadOptions<T>(
        this IQueryable<T> query,
        string? searchValue,
        List<string>? selectedRowKeys,
        List<Sorting>? sortings,
        List<LoadOptionFilter>? filters,
        Pagination pageOption,
        List<string>? searchableColumns = null,
        string? selectedColumnName = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(pageOption, nameof(pageOption));

        var _query = query;

        if (searchValue is not null && searchableColumns is not null && searchableColumns.Count > 0)
            query = query.ToApplySearchValues(searchableColumns!, searchValue);

        if (sortings is not null)
            query = query.ApplySortings(sortings);

        if (filters is not null)
            query = query.ApplyFilters(filters);

        var response = await query.ToPaginatedListAsync(pageOption.Page, pageOption.PageSize, cancellationToken);


        if (selectedRowKeys is not null && selectedRowKeys.Count > 0)
        {
            var selectedRowsQuery = _query.FilterSelectedRows(selectedRowKeys, selectedColumnName!);

            var queryString = selectedRowsQuery.ToQueryString();

            response.SelectedRows = await selectedRowsQuery.ToListAsync(cancellationToken);
        }

        return response;
    }

    public static IQueryable<T> ApplySortings<T>(this IQueryable<T> source, IEnumerable<Sorting> sortings)
    {
        if (sortings == null || !sortings.Any())
            return source;

        IOrderedQueryable<T>? orderQuery = null;

        foreach (var sorting in sortings)
        {
            if (!Enum.IsDefined(sorting.SortingType))
                continue;

            var expression = GetExpression<T>(sorting.ColumnName);

            if (sorting.SortingType == SortingTypes.Asc)
                orderQuery = orderQuery is null
                    ? source.OrderBy(expression)
                    : orderQuery!.ThenBy(expression);
            else
                orderQuery = orderQuery is null
                    ? source.OrderByDescending(expression)
                    : orderQuery!.ThenByDescending(expression);
        }

        return orderQuery != null ? orderQuery!.AsQueryable() : source;
    }

    public static IQueryable<T> ToApplySearchValues<T>(this IQueryable<T> records, IEnumerable<string> searchableColumns, string searchValue) where T : class
    {
        ArgumentNullException.ThrowIfNull(searchValue, nameof(searchValue));
        ArgumentNullException.ThrowIfNull(searchableColumns, nameof(searchableColumns));

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        var loweredSearch = Expression.Constant(searchValue.ToLower(), typeof(string));

        foreach (var propertyName in searchableColumns)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            var propertyAccess = Expression.Property(parameter, property);

            Expression stringExpr = property.PropertyType == typeof(string)
                ? propertyAccess
                : Expression.Call(propertyAccess, nameof(object.ToString), Type.EmptyTypes);

            var toLowerExpr = Expression.Call(stringExpr, toLowerMethod);

            var containsExpr = Expression.Call(toLowerExpr, containsMethod, loweredSearch);

            combinedExpression = combinedExpression == null
                ? containsExpr
                : Expression.OrElse(combinedExpression, containsExpr);
        }

        if (combinedExpression == null)
            return records;

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return records.Where(lambda);
    }

    //public static IQueryable<T> ToApplySearchValues<T>(this IQueryable<T> records, IEnumerable<string> searchableColumns, string searchValue) where T : class
    //{
    //    ArgumentNullException.ThrowIfNull(searchValue, nameof(searchValue));
    //    ArgumentNullException.ThrowIfNull(searchableColumns, nameof(searchableColumns));


    //    var parameter = Expression.Parameter(typeof(T), "x");
    //    Expression? combinedExpression = null;
    //    foreach (var propertyName in searchableColumns)
    //    {
    //        var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

    //        ArgumentNullException.ThrowIfNull(property, nameof(property));

    //        var propertyAccess = Expression.Property(parameter, property);

    //        Expression propertyString = property.PropertyType == typeof(string) ? propertyAccess : Expression.Call(propertyAccess, "ToString", null);

    //        var searchExpression = Expression.Constant(searchValue, typeof(string));

    //        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    //        var containsCall = Expression.Call(propertyString, containsMethod, searchExpression);

    //        combinedExpression = combinedExpression == null
    //            ? containsCall
    //            : Expression.OrElse(combinedExpression, containsCall);
    //    }

    //    if (combinedExpression == null)
    //        return records;

    //    var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    //    return records.Where(lambda);
    //}

    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> source, List<LoadOptionFilter> filters)
    {
        if (filters == null || !filters.Any())
            return source;

        var parameter = Expression.Parameter(typeof(T), nameof(T));
        var predicateBody = ExpressionExtensions.ProcessFilters(filters, parameter);
        if (predicateBody == null)
            return source;

        var predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
        return source.Where(predicate);
    }

    public static async Task<PagedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) where T : class
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;

        var skippedRecordCount = (pageNumber - 1) * pageSize;
        var records = await query
            .Skip(skippedRecordCount)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalRecordsCount = await query.CountAsync(cancellationToken);

        return new PagedList<T>(records, pageNumber, pageSize, totalRecordsCount);
    }

    public static IQueryable<T> FilterSelectedRows<T>(this IQueryable<T> records,
    List<string> selectedRowKeys, string selectedColumnName) where T : class
    {
        if (!selectedRowKeys.Any()) return records; 

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, selectedColumnName);
        var propertyType = property.Type;
        if (propertyType == typeof(Guid))
        {
            List<Guid> convertedList = selectedRowKeys.Select(Guid.Parse).ToList();
            var valueProperty = Expression.Property(property, "Value"); 
            var containsMethod = typeof(List<Guid>).GetMethod("Contains", new[] { typeof(Guid) });
            var keysAsConstant = Expression.Constant(convertedList);
            var containsCall = Expression.Call(keysAsConstant, containsMethod, valueProperty);

            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return records.Where(lambda);
        }

        else if (propertyType == typeof(int))
        {
            List<int> convertedList = selectedRowKeys.Select(int.Parse).ToList();

            var containsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) });
            var keysAsConstant = Expression.Constant(convertedList);
            var containsCall = Expression.Call(keysAsConstant, containsMethod, property);

            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return records.Where(lambda);
        }
        else if (propertyType == typeof(long))
        {
            List<long> convertedList = selectedRowKeys.Select(long.Parse).ToList();

            var containsMethod = typeof(List<long>).GetMethod("Contains", new[] { typeof(long) });
            var keysAsConstant = Expression.Constant(convertedList);
            var containsCall = Expression.Call(keysAsConstant, containsMethod, property);

            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return records.Where(lambda);
        }
        else
        {
            List<string> convertedList = selectedRowKeys;

            var containsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) });
            var keysAsConstant = Expression.Constant(convertedList);
            var containsCall = Expression.Call(keysAsConstant, containsMethod, property);

            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return records.Where(lambda);
        }
    }

    private static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
    {
        var param = Expression.Parameter(typeof(TSource), "x");
        Expression body = param;

        foreach (var member in propertyName.Split('.'))
        {
            body = member.EndsWith("?")
                ? Expression.PropertyOrField(body, member.TrimEnd('?'))
                : Expression.PropertyOrField(body, member);
        }

        Expression conversion = Expression.Convert(body, typeof(object));
        return Expression.Lambda<Func<TSource, object>>(conversion, param);
    }
}
