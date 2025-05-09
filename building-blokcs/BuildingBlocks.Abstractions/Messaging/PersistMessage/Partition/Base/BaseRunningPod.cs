namespace BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition.Base;

public class BaseRunningPod
{
    public Guid Id { get; set; }
    public string PodName { get; set; }
    public string Status { get; set; }
    public DateTime LastHearthBeat { get; set; }
    public DateTime? CreationTime { get; set; }
}
