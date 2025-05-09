namespace BuildingBlocks.Abstractions.Domain
{
    public interface IIdentity<out TId>
    {
        public TId Value { get; }
    }
}
