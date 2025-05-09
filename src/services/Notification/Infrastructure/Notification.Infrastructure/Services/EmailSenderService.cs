using Microsoft.Extensions.Logging;
using Notification.Application.Services;

namespace Notification.Infrastructure.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ILogger<EmailSenderService> _logger;
        public EmailSenderService(ILogger<EmailSenderService> logger)
        {
            _logger = logger;
        }
        public async Task<(bool success, string message)> SendEmailAsync(string email, string message, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(1000, cancellationToken);
                return (true, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, ex.Message);
            }
        }
    }
}
