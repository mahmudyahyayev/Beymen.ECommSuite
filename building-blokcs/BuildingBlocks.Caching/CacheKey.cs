using BuildingBlocks.Core.Reflection.Extensions;

namespace BuildingBlocks.Caching
{
    public static class CacheKey
    {
        public static string With(params string[] keys)
        {
            return string.Join("-", keys);
        }

        public static string With(Type type, params string[] keys)
        {
            return With($"{type.GetCacheKey()}:{string.Join("-", keys)}");
        }
    }
}
