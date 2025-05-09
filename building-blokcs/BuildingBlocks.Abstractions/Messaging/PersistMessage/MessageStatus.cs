namespace BuildingBlocks.Abstractions.Messaging.PersistMessage
{
    public enum MessageStatus : byte
    {
        Stored = 1,
        Processed = 2,
        Failed = 3,
        Blocked = 4,
        Processing = 5
    }
}
