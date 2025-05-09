using System.Reflection;

namespace BuildingBlocks.Core.Reflection.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type?> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
