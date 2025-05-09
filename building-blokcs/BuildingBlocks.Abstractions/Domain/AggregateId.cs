namespace BuildingBlocks.Abstractions.Domain
{
    public record AggregateId<T> : Identity<T>
    {
        protected AggregateId(T value)
        {
            Value = value;
        }

        public static implicit operator T(AggregateId<T> id) => id.Value;
        public static AggregateId<T> CreateAggregateId(T id) => new(id);
    }

    public record AggregateId : AggregateId<Guid>
    {
        protected AggregateId(Guid value)
            : base(value) { }
        public static new AggregateId CreateAggregateId(Guid value) => new(value);

        public static implicit operator Guid(AggregateId id) => id.Value;
    }
}
