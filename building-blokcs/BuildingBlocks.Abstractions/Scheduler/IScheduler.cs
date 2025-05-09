namespace BuildingBlocks.Abstractions.Scheduler
{
    public interface IScheduler : ICommandScheduler, IMessageScheduler
    {
        Task ScheduleAsync(
            ScheduleSerializedObject scheduleSerializedObject,
            DateTimeOffset scheduleAt,
            CancellationToken cancellationToken,
            string? description = null
        );

        Task ScheduleAsync(
            ScheduleSerializedObject scheduleSerializedObject,
            TimeSpan delay,
            CancellationToken cancellationToken,
            string? description = null);

        Task ScheduleRecurringAsync(
            ScheduleSerializedObject scheduleSerializedObject,
            string name,
            string cronExpression,
            CancellationToken cancellationToken,
            string? description = null
        );
    }
}
