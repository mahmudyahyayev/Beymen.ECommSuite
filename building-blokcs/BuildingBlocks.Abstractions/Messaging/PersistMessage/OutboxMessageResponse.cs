namespace BuildingBlocks.Abstractions.Messaging.PersistMessage
{
    public class OutboxMessageResponse
    {
        public Guid Id { get; set; }
        public string Data_Type { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
        public int Retry_Count { get; set; }
        public string Message_Status { get; set; }
        public string Delivery_Type { get; set; }
        public string Last_Error { get; set; }
        public string Instance_Name { get; set; }
    }
}
