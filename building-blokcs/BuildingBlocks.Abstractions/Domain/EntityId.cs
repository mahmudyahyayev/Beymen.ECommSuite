namespace BuildingBlocks.Abstractions.Domain
{
    public record EntityId<T> : Identity<T>
    {
        protected EntityId(T value)
        {
            Value = value;
        }

        public static implicit operator T(EntityId<T> id) => id.Value;
        public static EntityId<T> CreateEntityId(T id) => new(id);
    }

    public record EntityId : EntityId<Guid>
    {
        protected EntityId(Guid value)
            : base(value) { }
        public static new EntityId CreateEntityId(Guid value) => new(value);

        public static implicit operator Guid(EntityId id) => id.Value;
    }
}
