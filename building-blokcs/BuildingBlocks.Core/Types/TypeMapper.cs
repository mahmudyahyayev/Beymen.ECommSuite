using System.Collections.Concurrent;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Reflection;
using BuildingBlocks.Core.Utils;

namespace BuildingBlocks.Core.Types
{
    public static class TypeMapper
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNameMap = new();
        private static readonly ConcurrentDictionary<string, Type> TypeMap = new();
        public static string GetFullTypeName<T>() => ToName(typeof(T));

        public static string GetTypeName<T>() => ToName(typeof(T), false);

        public static string GetTypeName(Type type) => ToName(type, false);

        public static string GetFullTypeName(Type type) => ToName(type);

        public static string GetTypeNameByObject(object o) => ToName(o.GetType(), false);

        public static string GetFullTypeNameByObject(object o) => ToName(o.GetType());

        public static Type GetType(string typeName) => ToType(typeName);

        public static void AddType<T>(string name) => AddType(typeof(T), name);

        private static void AddType(Type type, string name)
        {
            ToName(type);
            ToType(name);
        }

        public static bool IsTypeRegistered<T>() => TypeNameMap.ContainsKey(typeof(T));

        private static string ToName(Type type, bool fullName = true)
        {
            Guard.Against.Null(type);

            return TypeNameMap.GetOrAdd(
                type,
                _ =>
                {
                    var eventTypeName = fullName ? type.FullName! : type.Name;

                    TypeMap.GetOrAdd(eventTypeName, type);

                    return eventTypeName;
                }
            );
        }

        private static Type ToType(string typeName)
        {
            Guard.Against.NullOrWhiteSpace(typeName, nameof(typeName));

            return TypeMap.GetOrAdd(
                typeName,
                _ =>
                {
                    var type = ReflectionUtilities.GetFirstMatchingTypeFromCurrentDomainAssemblies(typeName);

                    if (type == null)
                        throw new System.Exception($"Type map for '{typeName}' wasn't found!");

                    return type;
                }
            );
        }
    }
}
