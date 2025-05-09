using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Core.Web.Extensions.ServiceCollection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationOptions<T>(this IServiceCollection services)
            where T : class
        {
            return services.AddConfigurationOptions<T>(typeof(T).Name);
        }

        public static IServiceCollection AddConfigurationOptions<T>(this IServiceCollection services, string key)
            where T : class
        {
            services.AddOptions<T>().BindConfiguration(key);

            return services.AddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);
        }

        public static IServiceCollection AddValidatedOptions<T>(this IServiceCollection services)
            where T : class
        {
            return AddValidatedOptions<T>(services, typeof(T).Name, RequiredConfigurationValidator.Validate);
        }

        public static IServiceCollection AddValidatedOptions<T>(this IServiceCollection services, string key)
            where T : class
        {
            return AddValidatedOptions<T>(services, key, RequiredConfigurationValidator.Validate);
        }

        public static IServiceCollection AddValidatedOptions<T>(
            this IServiceCollection services,
            string key,
            Func<T, bool> validator
        )
            where T : class
        {
            services.AddOptions<T>().BindConfiguration(key).Validate(validator);
            return services.AddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);
        }
    }

    public static class RequiredConfigurationValidator
    {
        public static bool Validate<T>(T arg)
            where T : class
        {
            var requiredProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => Attribute.IsDefined(x, typeof(RequiredMemberAttribute)));

            foreach (var requiredProperty in requiredProperties)
            {
                var propertyValue = requiredProperty.GetValue(arg);
                if (propertyValue is null)
                {
                    throw new System.Exception($"Required property '{requiredProperty.Name}' was null");
                }
            }

            return true;
        }
    }
}


