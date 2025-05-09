using BuildingBlocks.Abstractions.CQRS.Commands;

namespace BuildingBlocks.Abstractions.Messaging
{
    public interface IMessageMapper : ICommandMapper { }

    public interface ICommandMapper
    {
        IReadOnlyList<ICommand?>? MapToCommand(IReadOnlyList<IMessage> messages);
        ICommand? MapToCommand(IMessage domainEvent);
    }
}
