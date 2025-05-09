using Microsoft.Extensions.Logging;
using Notification.Application.Services;

namespace Notification.Infrastructure.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        private readonly ILogger<SmsSenderService> _logger;
        public SmsSenderService(ILogger<SmsSenderService> logger)
        {
            _logger = logger;
        }
        public async Task<(bool success, string message)> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken)
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
