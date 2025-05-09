namespace BuildingBlocks.Abstractions.CQRS.Commands;

public interface ICommandProcessor
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
        where TResult : notnull;

    Task ScheduleAsync(IInternalCommand internalCommandCommand, string correlationId, CancellationToken cancellationToken);
    Task ScheduleAsync(IInternalCommand[] internalCommandCommands, string correlationId, CancellationToken cancellationToken);
}
