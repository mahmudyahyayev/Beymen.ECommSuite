namespace BuildingBlocks.Abstractions.Messaging.PersistMessage
{
    public class StoreMessage
    {
        public Guid Id { get; private set; }
        public string DataType { get; private set; }
        public string Data { get; private set; }
        public DateTime Created { get; private set; }
        public int RetryCount { get; private set; }
        public MessageStatus MessageStatus { get; private set; }
        public MessageDeliveryType DeliveryType { get; private set; }
        public string LastError { get; private set; }
        public string InstanceName { get; private set; }
        public int? Partition { get; private set; }
        public string Key { get; private set; }
        public int? Priority { get; private set; }
        public DateTime? Modified { get; private set; }

        private StoreMessage() { }

        public static StoreMessage Create(
            Guid id,
            string dataType,
            string data,
            MessageDeliveryType deliveryType,
            string instanceName,
            int partition,
            string key,
            int priority)
        {
            return new StoreMessage
            {
                Id = id,
                DataType = dataType,
                Data = data,
                DeliveryType = deliveryType,
                Created = DateTime.Now,
                MessageStatus = MessageStatus.Stored,
                RetryCount = 0,
                InstanceName = instanceName,
                Partition = partition,
                Key = key,
                Priority = priority
            };
        }

        public void ChangeState(MessageStatus messageStatus, int maxRetryCount)
        {
            MessageStatus = messageStatus;
            ChangeModified();

            if (messageStatus == MessageStatus.Failed || RetryCount > 0)
                IncreaseRetry();

            if (RetryCount > maxRetryCount)
            {
                MessageStatus = MessageStatus.Blocked;
            }
        }

        private void ChangeModified()
        {
            Modified = DateTime.Now;
        }

        private void IncreaseRetry()
        {
            RetryCount++;
        }

        public void ChangeLastError(string message)
        {
            ChangeModified();

            if (!string.IsNullOrEmpty(message))
                LastError = message;
        }
    }
}
