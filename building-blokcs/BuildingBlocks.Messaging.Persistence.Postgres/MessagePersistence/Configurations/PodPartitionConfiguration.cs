using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence.Configurations;

public sealed class PodPartitionConfiguration : IEntityTypeConfiguration<PodPartition>
{
    public void Configure(EntityTypeBuilder<PodPartition> builder)
    {

        builder.ToTable("pod_partition", MessagePersistenceDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Partition);
    }
}
