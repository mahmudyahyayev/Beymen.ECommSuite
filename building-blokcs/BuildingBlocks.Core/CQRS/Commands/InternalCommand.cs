using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Core.Types;

namespace BuildingBlocks.Core.CQRS.Commands
{
    public abstract record InternalCommand : IInternalCommand
    {
        public Guid InternalCommandId { get; protected set; } = Guid.NewGuid();

        //public string CorrelationId { get; protected set; }

        public DateTime OccurredOn { get; protected set; } = DateTime.Now;
        
        public string MessageKey { get; protected set; }
        public int Priority { get; protected set; }

        protected InternalCommand(
            //string correlationId,
            string messageKey,
            int priority)
        {
            //CorrelationId = correlationId;
            MessageKey = messageKey;
            Priority = priority;
        }

        public string Type
        {
            get { return TypeMapper.GetFullTypeName(GetType()); }
        }
    }
}
