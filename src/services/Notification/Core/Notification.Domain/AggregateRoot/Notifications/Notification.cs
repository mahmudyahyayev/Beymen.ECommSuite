using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Notification.Domain.AggregateRoot.Notifications.Events.Domain;

namespace Notification.Domain.AggregateRoot.Notifications;

public class Notification : Aggregate<NotificationId, string>,
    IHaveAudit<string>,
    IHaveIdentity<NotificationId>
{
    /// <summary>
    /// Main Attributes
    /// </summary>

    private Ownership _customerId;
    public Ownership CustomerId => _customerId;

    private int _typeId;
    public NotificationType Type { get; private set; }

    private int _status;
    public NotificationStatus Status { get; set; }

    private string _recipient;
    public string Recipient => _recipient;

    private string _message;
    public string Message => _message;

    private DateTime? _sentAt;
    public DateTime? SentAt => _sentAt;

    private string _errorMessage;
    public string ErrorMessage => _errorMessage;

    /// <summary>
    /// Auditable
    /// </summary>
    private string _lastModifiedBy;
    public string LastModifiedBy => _lastModifiedBy;
    private DateTime? _lastModified;
    public DateTime? LastModified => _lastModified;

    private Notification()
    {
        //for efcore
    }
    public static Notification Create(Guid customerId, int typeId, string message)
    {
        Notification notification = new()
        {
            Id = NotificationId.Of(Guid.NewGuid()),
            _typeId = typeId,
            _status = NotificationStatus.Pending.Id,
            _message = message,
            _customerId = Ownership.Of(customerId)
        };


        notification.WhenNotificationCreated();

        return notification;
    }

    private void WhenNotificationCreated()
    {
        if (_typeId == NotificationType.Email.Id)
        {
            AddDomainEvents(new EmailNotificationCreated(Id.Value, _customerId.Value, _message));
        }
        else if (_typeId == NotificationType.Sms.Id)
        {
            AddDomainEvents(new SmsNotificationCreated(Id.Value, _customerId.Value, _message));
        }
    }

    public void SetRecipient(string recipient)
    { 
        _recipient = recipient;
    }

    public void MarkAsFailed(string errorMessage)
    {
        _errorMessage = errorMessage;
        _status = NotificationStatus.Failed.Id;
    }

    public void MarkAsSent()
    {
        _status = NotificationStatus.Sent.Id;
        _sentAt = DateTime.UtcNow;
    }
}
