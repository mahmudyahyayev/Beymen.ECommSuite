using BuildingBlocks.Abstractions.Messaging;

namespace BuildingBlocks.Abstractions.Scheduler
{
    public interface IMessageScheduler
    {
        Task ScheduleAsync(IMessage message, CancellationToken cancellationToken);
        Task ScheduleAsync(IMessage[] messages, CancellationToken cancellationToken);
    }
}
