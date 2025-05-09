using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Core.Extensions;

public static class PersistentHashCodeExtension
{
    public static int GetPersistentHashCode(this string str)
    {
        using var algorithm = SHA256.Create();
        byte[] hash256;
        int hash = 0;

        hash256 = algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
        for (int i = 0; i < hash256.Length; i += 4)
        {
            hash ^= BitConverter.ToInt32(hash256, i);
        }

        return hash;
    }
}
