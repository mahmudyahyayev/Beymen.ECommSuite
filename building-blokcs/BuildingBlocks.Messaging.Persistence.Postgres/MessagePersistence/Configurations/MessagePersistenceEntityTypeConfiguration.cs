using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Persistence.Efcore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence.Configurations
{
    public sealed class MessagePersistenceEntityTypeConfiguration : IEntityTypeConfiguration<StoreMessage>
    {
        public void Configure(EntityTypeBuilder<StoreMessage> builder)
        {
            builder.ToTable("store_messages", MessagePersistenceDbContext.DefaultSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder
                .Property(x => x.DeliveryType)
                .HasMaxLength(50)
                .HasConversion(v => v.ToString(), v => (MessageDeliveryType)Enum.Parse(typeof(MessageDeliveryType), v))
                .IsRequired()
                .IsUnicode(false);

            builder
                .Property(x => x.MessageStatus)
                .HasMaxLength(50)
                .HasConversion(v => v.ToString(), v => (MessageStatus)Enum.Parse(typeof(MessageStatus), v))
                .IsRequired()
                .IsUnicode(false);

            builder
                .Property(x => x.DataType)
                .IsRequired();

            builder
                .Property(x => x.Data)
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
                .IsRequired()
                .HasDefaultValueSql(EfConstants.DateAlgorithm);

            builder
                .Property(x => x.RetryCount)
                .IsRequired()
                .HasDefaultValue(EfConstants.Zero);

            builder
              .Property(x => x.InstanceName)
              .IsRequired()
              .HasMaxLength(500);

            builder
                .Property(x => x.LastError)
                .IsRequired(false)
                .HasMaxLength(EfConstants.Lenght.SoLong);
            
            builder
                .Property(x => x.Partition)
                .IsRequired(false);

            builder
                .Property(x => x.Key)
                .IsRequired(false);

            builder
                .Property(x => x.Priority)
                .HasDefaultValue(EfConstants.Zero)
                .IsRequired(false);

            builder
                .Property(x => x.Modified)
                .HasColumnType(EfConstants.ColumnTypes.DateTimeUtc)
                .IsRequired(false);
        }
    }
}
