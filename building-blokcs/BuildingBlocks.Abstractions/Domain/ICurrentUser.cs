namespace BuildingBlocks.Abstractions.Domain
{
    public interface ICurrentUser<TUserIdType, TAudit>
    {
        TUserIdType UserId { get; }
        string UserName { get; }
        TAudit GetCurrentUser();
    }
}
