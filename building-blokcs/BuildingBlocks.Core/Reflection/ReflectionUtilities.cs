using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Linq.Expressions;
using System.Reflection;

namespace BuildingBlocks.Core.Reflection
{
    public static class ReflectionUtilities
    {
        public static dynamic CreateGenericType(Type genericType, Type[] typeArguments, params object?[] constructorArgs)
        {
            var type = genericType.MakeGenericType(typeArguments);
            return Activator.CreateInstance(type, constructorArgs);
        }

        public static dynamic CreateGenericType<TGenericType>(Type[] typeArguments, params object?[] constructorArgs)
        {
            return CreateGenericType(typeof(TGenericType), typeArguments, constructorArgs);
        }

        public static IEnumerable<Type> GetAllTypesImplementingInterface<TInterface>(params Assembly[] assemblies)
        {
            var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
            return inputAssemblies.SelectMany(GetAllTypesImplementingInterface<TInterface>);
        }

        private static IEnumerable<Type> GetAllTypesImplementingInterface<TInterface>(Assembly? assembly = null)
        {
            var inputAssembly = assembly ?? Assembly.GetExecutingAssembly();
            return inputAssembly
                .GetTypes()
                .Where(
                    type =>
                        typeof(TInterface).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && type.IsClass
                );
        }

        public static IEnumerable<string?> GetPropertyNames<T>(params Expression<Func<T, object>>[] propertyExpressions)
        {
            var retVal = new List<string?>();
            foreach (var propertyExpression in propertyExpressions)
            {
                retVal.Add(GetPropertyName(propertyExpression));
            }

            return retVal;
        }

        public static string? GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
        {
            string? retVal = null;
            if (propertyExpression != null)
            {
                var lambda = (LambdaExpression)propertyExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression unaryExpression)
                {
                    memberExpression = (MemberExpression)unaryExpression.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression)lambda.Body;
                }

                retVal = memberExpression.Member.Name;
            }

            return retVal;
        }

        public static Type? GetTypeFromAnyReferencingAssembly(string typeName)
        {
            var referencedAssemblies = Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Select(a => a.FullName);

            if (referencedAssemblies == null)
                return null;

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => referencedAssemblies.Contains(a.FullName))
                .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
                .FirstOrDefault();
        }

        public static Type? GetFirstMatchingTypeFromCurrentDomainAssemblies(string typeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
                .FirstOrDefault();
        }
        public static TResult CompileMethodInvocation<TResult>(MethodInfo methodInfo)
        {
            var genericArguments = typeof(TResult).GetTypeInfo().GetGenericArguments();
            var methodArgumentList = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
            var funcArgumentList = genericArguments.Skip(1).Take(methodArgumentList.Count).ToList();

            if (funcArgumentList.Count != methodArgumentList.Count)
            {
                throw new ArgumentException("Incorrect number of arguments");
            }

            var instanceArgument = Expression.Parameter(genericArguments[0]);

            var argumentPairs = funcArgumentList
                .Zip(methodArgumentList, (s, d) => new { Source = s, Destination = d })
                .ToList();
            if (argumentPairs.All(a => a.Source == a.Destination))
            {
                var parameters = funcArgumentList.Select(Expression.Parameter).ToList();
                return Expression
                    .Lambda<TResult>(
                        Expression.Call(instanceArgument, methodInfo, parameters),
                        new[] { instanceArgument }.Concat(parameters)
                    )
                    .Compile();
            }

            var lambdaArgument = new List<ParameterExpression> { instanceArgument, };

            var type = methodInfo.DeclaringType;
            var instanceVariable = Expression.Variable(type);
            var blockVariables = new List<ParameterExpression> { instanceVariable, };
            var blockExpressions = new List<Expression>
        {
            Expression.Assign(instanceVariable, Expression.ConvertChecked(instanceArgument, type))
        };
            var callArguments = new List<ParameterExpression>();

            foreach (var a in argumentPairs)
            {
                if (a.Source == a.Destination)
                {
                    var sourceParameter = Expression.Parameter(a.Source);
                    lambdaArgument.Add(sourceParameter);
                    callArguments.Add(sourceParameter);
                }
                else
                {
                    var sourceParameter = Expression.Parameter(a.Source);
                    var destinationVariable = Expression.Variable(a.Destination);
                    var assignToDestination = Expression.Assign(
                        destinationVariable,
                        Expression.Convert(sourceParameter, a.Destination)
                    );

                    lambdaArgument.Add(sourceParameter);
                    callArguments.Add(destinationVariable);
                    blockVariables.Add(destinationVariable);
                    blockExpressions.Add(assignToDestination);
                }
            }

            var callExpression = Expression.Call(instanceVariable, methodInfo, callArguments);
            blockExpressions.Add(callExpression);

            var block = Expression.Block(blockVariables, blockExpressions);

            var lambdaExpression = Expression.Lambda<TResult>(block, lambdaArgument);

            return lambdaExpression.Compile();
        }
        public static IReadOnlyList<Assembly> GetApplicationPartAssemblies(Assembly rootAssembly)
        {
            var rootNamespace = rootAssembly.GetName().Name!.Split('.').First();
            var list = rootAssembly!
                .GetCustomAttributes<ApplicationPartAttribute>()
                .Where(x => x.AssemblyName.StartsWith(rootNamespace, StringComparison.InvariantCulture))
                .Select(name => Assembly.Load(name.AssemblyName))
                .Distinct();

            return list.ToList().AsReadOnly();
        }

        public static IReadOnlyList<Assembly> GetBinDirectoryAssemblies()
        {
            var assemblies = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
                .Distinct();

            return assemblies.ToList().AsReadOnly();
        }

        public static IReadOnlyList<Assembly> GetReferencedAssemblies(Assembly? rootAssembly)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<Assembly?>();
            var listResult = new List<Assembly>();

            var root = rootAssembly ?? Assembly.GetEntryAssembly();
            queue.Enqueue(root);

            while (queue.Any())
            {
                var asm = queue.Dequeue();

                if (asm == null)
                    break;

                listResult.Add(asm);

                foreach (var reference in asm.GetReferencedAssemblies())
                {
                    if (!visited.Contains(reference.FullName))
                    {
                        if (reference.FullName != "System.Data.SqlClient, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                        {
                            queue.Enqueue(Assembly.Load(reference));
                            visited.Add(reference.FullName);
                        }
                    }
                }
            }

            return listResult.Distinct().ToList().AsReadOnly();
        }

        public static IReadOnlyList<Assembly> GetReferencedAssembliesFromRootType<T>()
            where T : Type
        {
            var root = typeof(T).Assembly;
            return GetReferencedAssemblies(root);
        }

        public static IReadOnlyList<Assembly> GetReferencedAssembliesFromRootType(Type rootType)
        {
            var root = rootType.Assembly;
            return GetReferencedAssemblies(root);
        }
    }
}
