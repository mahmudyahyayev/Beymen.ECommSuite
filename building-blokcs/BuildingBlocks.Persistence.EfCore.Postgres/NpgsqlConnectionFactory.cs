using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Persistence.Efcore;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace Core.Persistence.Postgres
{
    public class NpgsqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;
        private DbConnection? _connection;

        public NpgsqlConnectionFactory(string connectionString)
        {
            Guard.Against.NullOrEmpty(connectionString);
            _connectionString = connectionString;
        }

        public async Task<DbConnection> GetOrCreateConnectionAsync(CancellationToken cancellationToken)
        {
            if (_connection is null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync(cancellationToken);
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection is { State: ConnectionState.Open })
                _connection.Dispose();
        }
    }
}
