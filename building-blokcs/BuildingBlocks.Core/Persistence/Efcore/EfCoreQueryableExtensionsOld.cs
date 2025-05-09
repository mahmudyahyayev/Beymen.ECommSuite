using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.CQRS.Queries;
using BuildingBlocks.Core.Domain;
using BuildingBlocks.Core.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace BuildingBlocks.Core.Persistence.EfCore
{
    public static class EfCoreQueryableExtensionsOld
    {
        #region ApplyPagingAsync
        public static async Task<ListResultModel<T>> ApplyPagingAsync<T>(
            this IQueryable<T> collection,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) where T : notnull
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
            if (isEmpty)
                return ListResultModel<T>.Empty;

            var totalItems = await collection.CountAsync(cancellationToken: cancellationToken);
            var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
            var data = await collection.Limit(page, pageSize).ToListAsync(cancellationToken: cancellationToken);

            return ListResultModel<T>.Create(data, totalItems, page, pageSize);
        }

        public static async Task<ListResultModel<TR>> ApplyPagingAsync<T, TR>(
            this IQueryable<T> collection,
            IConfigurationProvider configuration,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default
        )
            where TR : notnull
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 10;

            var isEmpty = await collection.AnyAsync(cancellationToken: cancellationToken) == false;
            if (isEmpty)
                return ListResultModel<TR>.Empty;

            var totalItems = await collection.CountAsync(cancellationToken: cancellationToken);
            var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
            var data = await collection
                .Limit(page, pageSize)
                .ProjectTo<TR>(configuration)
                .ToListAsync(cancellationToken: cancellationToken);

            return ListResultModel<TR>.Create(data, totalItems, page, pageSize);
        }

        public static IQueryable<TEntity> ApplyPaging<TEntity>(this IQueryable<TEntity> source, int page, int size)
            where TEntity : class
        {
            return source.Skip(page * size).Take(size);
        }

        public static IQueryable<T> Limit<T>(this IQueryable<T> collection, int page = 1, int resultsPerPage = 10)
        {
            if (page <= 0)
                page = 1;

            if (resultsPerPage <= 0)
                resultsPerPage = 10;

            var skip = (page - 1) * resultsPerPage;
            var data = collection.Skip(skip).Take(resultsPerPage);

            return data;
        }
        #endregion ApplyPagingAsync


        #region ApplyFilter
        public static IQueryable<TEntity> ApplyFilter<TEntity>(this IQueryable<TEntity> source, IEnumerable<LoadOptionFilter> filters) where TEntity : class
        {
            if (filters is null)
                return source;

            foreach (var filter in filters)
            {
                ParameterExpression param = Expression.Parameter(typeof(TEntity), "p");
                Expression expr = param;

                foreach (string propertyName in filter.ColumnName.Split("."))
                {
                    PropertyInfo propertyInfo = expr.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    
                    if (propertyInfo == null)
                    {
                        expr = null;
                        break;
                    }

                    expr = Expression.Property(expr, propertyInfo);
                }

                if (expr == null)
                    continue;

                if (expr.Type == typeof(ValueObject))
                    continue;

                object obj = null;

                switch (filter.EqualType)
                {
                    case ConditionTypes.Equals:
                    case ConditionTypes.NotEquals:
                        if (filter.Value != null)
                            obj = TypeDescriptor.GetConverter(expr.Type).ConvertFromInvariantString(filter.Value.ToString());
                        break;
                    case ConditionTypes.StartsWith:
                    case ConditionTypes.Contains:
                    case ConditionTypes.EndsWith:
                        if (!string.IsNullOrEmpty(filter.Value.ToString()) && expr.Type != typeof(string))
                        {
                            expr = Expression.Call(expr, "ToString", null);
                        }
                        obj = filter.Value;
                        break;
                }

                Expression body = filter.EqualType switch
                {
                    ConditionTypes.Equals => Expression.Equal(expr, Expression.Constant(obj)),
                    ConditionTypes.NotEquals => Expression.NotEqual(expr, Expression.Constant(obj)),
                    ConditionTypes.StartsWith => Expression.Call(expr, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(obj)),
                    ConditionTypes.Contains => Expression.Call(expr, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(obj)),
                    ConditionTypes.EndsWith => Expression.Call(expr, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), Expression.Constant(obj)),
                    _ => null,
                };

                if (body != null)
                    source = source.Where(Expression.Lambda<Func<TEntity, bool>>(body, param));
            }
            return source;
        }
        #endregion ApplyFilter


        #region ApplyIncludeList
        public static IQueryable<TEntity> ApplyIncludeList<TEntity>(this IQueryable<TEntity> source, IEnumerable<string>? navigationPropertiesPath) where TEntity : class
        {
            if (navigationPropertiesPath is null)
                return source;

            foreach (var navigationPropertyPath in navigationPropertiesPath)
            {
                source = source.Include(navigationPropertyPath);
            }

            return source;
        }
        #endregion ApplyFilter


        #region ApplyOrdering
        public static IQueryable<TEntity> ApplyOrderBy<TEntity>(this IQueryable<TEntity> source, IEnumerable<Sorting> sorts) where TEntity : class
        {
            if (sorts is null)
                return source;

            foreach (var item in sorts)
            {
                var function = GetOrderBy<TEntity>(item.ColumnName, item.SortingType);
                if (function != null)
                    source = function(source);

            }
            return source;
        }

        private static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderBy<TEntity>(string orderColumn, SortingTypes orderType) where TEntity : class
        {
            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
            Expression expr = param;

            foreach (string propertyName in orderColumn.Split("."))
            {
                PropertyInfo propertyInfo = expr.Type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    expr = null;
                    break;
                }

                expr = Expression.Property(expr, propertyInfo);
            }

            if (expr == null)
                return null;

            if (expr.Type == typeof(ValueObject))
                return null;
            
            LambdaExpression lambda = Expression.Lambda(expr, param);

            string methodName = orderType == SortingTypes.Asc ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TEntity), expr.Type }, outerExpression.Body, Expression.Quote(lambda));

            var finalLambda = Expression.Lambda(resultExp, argQueryable);

            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }

        #endregion ApplyOrdering
    }
}
