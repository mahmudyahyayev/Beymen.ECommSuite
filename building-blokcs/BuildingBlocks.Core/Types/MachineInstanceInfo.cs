using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Types;

namespace BuildingBlocks.Core.Types
{
    public record MachineInstanceInfo : IMachineInstanceInfo
    {
        public MachineInstanceInfo(Guid clientId, string clientGroup)
        {
            if (string.IsNullOrEmpty(clientGroup))
            {
                throw new ArgumentException("Required input " + nameof(clientGroup) + " was empty.",
                    nameof(clientGroup));
            }

            ClientId = clientId;
            ClientGroup = clientGroup;
        }

        public Guid ClientId { get; }
        public string ClientGroup { get; }

        public static MachineInstanceInfo New() => new(Guid.NewGuid(), AppDomain.CurrentDomain.FriendlyName);
    }
}
