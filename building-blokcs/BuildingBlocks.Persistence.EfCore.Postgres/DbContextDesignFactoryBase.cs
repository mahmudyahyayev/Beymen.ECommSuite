using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
namespace BuildingBlocks.Persistence.EfCore.Postgres
{
    public abstract class DbContextDesignFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        private readonly string _connectionStringSection;
        private readonly string? _env;
        private readonly string? _migrationDefaultSchema;

        protected DbContextDesignFactoryBase(string connectionStringSection, string? env = null, string migrationDefaultSchema = null)
        {
            _connectionStringSection = connectionStringSection;
            _env = env;
            _migrationDefaultSchema = migrationDefaultSchema;
        }

        public TDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine($"BaseDirectory: {AppContext.BaseDirectory}");

            var environmentName = _env ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory ?? "")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            Console.WriteLine(environmentName);
            Console.WriteLine(_connectionStringSection);

            var connectionStringSectionValue = configuration.GetValue<string>(_connectionStringSection);

            Console.WriteLine(connectionStringSectionValue);

            if (string.IsNullOrWhiteSpace(connectionStringSectionValue))
            {
                throw new InvalidOperationException($"Could not find a value for {_connectionStringSection} section.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>()
                .UseNpgsql(
                    connectionStringSectionValue,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(GetType().Assembly.FullName);
                        sqlOptions.MigrationsHistoryTable(
                                    $"efcore_{_migrationDefaultSchema}_migration_history", _migrationDefaultSchema);
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                    }
                )
                .UseSnakeCaseNamingConvention()
                .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector<Guid>>();

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options);
        }
    }
}
