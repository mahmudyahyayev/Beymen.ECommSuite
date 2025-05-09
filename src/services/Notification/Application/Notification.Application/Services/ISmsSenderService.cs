namespace Notification.Application.Services;

public interface ISmsSenderService
{
    Task<(bool success, string message)> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken);
}
