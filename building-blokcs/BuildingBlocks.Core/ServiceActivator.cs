﻿using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core
{
    public static class ServiceActivator
    {
        internal static IServiceProvider? _serviceProvider;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope? GetScope(IServiceProvider? serviceProvider = null)
        {
            var provider = serviceProvider ?? _serviceProvider;
            return provider?.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }

        public static T? GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }

        public static T GetRequiredService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public static object GetRequiredServices(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }

        public static IEnumerable<T> GetServices<T>()
        {
            return _serviceProvider.GetServices<T>();
        }

        public static object GetService(Type type)
        {
            return _serviceProvider.GetService(type);
        }

        public static IEnumerable<object> GetServices(Type type)
        {
            return _serviceProvider.GetServices(type);
        }
    }
}