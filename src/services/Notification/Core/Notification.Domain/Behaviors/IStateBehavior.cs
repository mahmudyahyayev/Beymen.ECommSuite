namespace Notification.Domain.Behaviors
{
    public interface IStateBehavior : IDomainBehavior
    {
        public void Activate();
        public void Deactivate();
        bool IsActive { get; }
    }
}
