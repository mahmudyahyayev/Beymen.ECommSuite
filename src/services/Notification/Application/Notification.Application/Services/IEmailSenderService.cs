namespace Notification.Application.Services;

public interface IEmailSenderService
{
    Task<(bool success, string message)> SendEmailAsync(string email, string message, CancellationToken cancellationToken);
}
