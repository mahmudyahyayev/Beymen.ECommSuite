namespace BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition.Base;

public class BasePodPartition
{
    public Guid Id { get; set; }
    public Guid PodId { get; set; }
    public int Partition { get; set; }
}
