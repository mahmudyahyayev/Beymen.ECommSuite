using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence.Configurations;

public sealed class RunningPodConfiguration : IEntityTypeConfiguration<RunningPod>
{
    public void Configure(EntityTypeBuilder<RunningPod> builder)
    {
        builder.ToTable("running_pods", MessagePersistenceDbContext.DefaultSchema);

        builder.HasKey(x => x.Id);
    }
}
