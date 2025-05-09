namespace BuildingBlocks.Abstractions.CQRS.Commands;

public interface IInternalCommand : ICommand
{
    //string CorrelationId { get; }
    Guid InternalCommandId { get; }
    DateTime OccurredOn { get; }
    string Type { get; }
    string MessageKey { get; }
    int Priority { get; }
}
