using BuildingBlocks.Abstractions.CQRS.Commands;

namespace BuildingBlocks.Core.CQRS.Commands
{
    public abstract record TxInternalCommand : InternalCommand, ITxInternalCommand
    {
        protected TxInternalCommand(
            string messageKey,
            int priority) : base(messageKey, priority)
        {
        }
    }
}
