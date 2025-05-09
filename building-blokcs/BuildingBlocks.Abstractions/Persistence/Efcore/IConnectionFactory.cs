using System.Data.Common;

namespace BuildingBlocks.Abstractions.Persistence.Efcore
{
    public interface IConnectionFactory : IDisposable
    {
        Task<DbConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken);
    }
}
