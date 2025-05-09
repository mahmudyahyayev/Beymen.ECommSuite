using MassTransit.Events;

namespace BuildingBlocks.Abstractions.Messaging;

public interface IFaultConsumer<T> where T : class
{
    Task ConsumeAsync(FaultEvent<T> faultMessage);
}
