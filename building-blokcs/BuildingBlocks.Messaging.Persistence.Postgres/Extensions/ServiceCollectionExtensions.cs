using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Web.Extensions.ServiceCollection;
using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.Persistence.Postgres.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPostgresMessagePersistence(
            this IServiceCollection services, 
            IConfiguration configuration,
            string renameDefaultSchemaForPrefix = null)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            string schema = MessagePersistenceDbContext.DefaultSchema;

            if (!string.IsNullOrEmpty(renameDefaultSchemaForPrefix))
            {
                schema = $"{renameDefaultSchemaForPrefix}_{schema}";
            }

            services.AddValidatedOptions<MessagePersistenceOptions>(nameof(MessagePersistenceOptions));

            services.AddScoped<IMessagePersistenceConnectionFactory>(sp =>
            {
                var postgresOptions = sp.GetService<MessagePersistenceOptions>();
                Guard.Against.NullOrEmpty(postgresOptions?.ConnectionString);

                //Console.WriteLine(postgresOptions.ConnectionString);

                return new NpgsqlMessagePersistenceConnectionFactory(postgresOptions.ConnectionString);
            });

            services.AddDbContext<MessagePersistenceDbContext>(
                (sp, options) =>
                {

                    var postgresOptions = sp.GetRequiredService<MessagePersistenceOptions>();

                    //Console.WriteLine(postgresOptions.ConnectionString);

                    options
                        .UseNpgsql(
                            postgresOptions.ConnectionString,
                           
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(
                                    postgresOptions.MigrationAssembly
                                        ?? typeof(MessagePersistenceDbContext).Assembly.GetName().Name)
                                .MigrationsHistoryTable(
                                    "efcore_message_migration_history", schema);

                                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                            }
                        )
                        .UseSnakeCaseNamingConvention();
                }
            );

            services.ReplaceScoped<IMessagePersistenceRepository, PostgresMessagePersistenceRepository>();
        }
    }
}