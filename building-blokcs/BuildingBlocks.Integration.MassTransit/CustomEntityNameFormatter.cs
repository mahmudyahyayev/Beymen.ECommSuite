using Humanizer;
using MassTransit;
using MassTransit.Topology;

namespace BuildingBlocks.Integration.MassTransit
{
    public class CustomEntityNameFormatter : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return typeof(T).Name.Underscore();
        }
    }
}
