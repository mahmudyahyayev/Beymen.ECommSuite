using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence
{
    public class MessagePersistenceDbContext : DbContext
    {
        public const string DefaultSchema = "messaging";

        public DbSet<StoreMessage> StoreMessages => Set<StoreMessage>();
        public DbSet<PodPartition> PodPartitions => Set<PodPartition>();
        public DbSet<RunningPod> RunningPods => Set<RunningPod>();
        public MessagePersistenceDbContext(DbContextOptions<MessagePersistenceDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
