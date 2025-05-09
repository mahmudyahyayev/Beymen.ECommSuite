using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Web.Extenions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Core.Messaging.BackgroundServices
{
    public class MessagePersistenceBackgroundService : BackgroundService
    {
        private readonly ILogger<MessagePersistenceBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly MessagePersistenceOptions _options;
        private readonly IMachineInstanceInfo _machineInstanceInfo;

        public MessagePersistenceBackgroundService(
            ILogger<MessagePersistenceBackgroundService> logger,
            IOptions<MessagePersistenceOptions> options,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime lifetime,
            IMachineInstanceInfo machineInstanceInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _lifetime = lifetime;
            _machineInstanceInfo = machineInstanceInfo;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!await _lifetime.WaitForAppStartup(cancellationToken))
            {
                return;
            }

            _logger.LogInformation(
                "MessagePersistence Background Service is starting on client '{@ClientId}' and group '{@ClientGroup}'",
                _machineInstanceInfo.ClientId,
                _machineInstanceInfo.ClientGroup
            );

            _ = Task.Run(() => ProcessAsync(cancellationToken), cancellationToken);
            _ = Task.Run(() => ProcessAllErrorAsync(cancellationToken), cancellationToken);
            _ = Task.Run(() => UpdateStuckProcessingStatusesAsync(cancellationToken), cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "MessagePersistence Background Service is stopping on client '{@ClientId}' and group '{@ClientGroup}'",
                _machineInstanceInfo.ClientId,
                _machineInstanceInfo.ClientGroup
            );

            return base.StopAsync(cancellationToken);
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IMessagePersistenceService>();
                        await service.ProcessAllAsync(cancellationToken);
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "MessagePersistenceBackgroundService ProcessAsync error");
                }
                finally
                {
                    var delay = _options.Interval is { }
                        ? TimeSpan.FromSeconds((int)_options.Interval)
                        : TimeSpan.FromSeconds(30);

                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        private async Task ProcessAllErrorAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IMessagePersistenceService>();
                        await service.ProcessAllErrorAsync(cancellationToken);
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "MessagePersistenceBackgroundService  ProcessAllErrorAsync error");
                }
                finally
                {
                    var delay = _options.ErrorInterval is { }
                        ? TimeSpan.FromSeconds((int)_options.ErrorInterval)
                        : TimeSpan.FromSeconds(30);

                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        private async Task UpdateStuckProcessingStatusesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IMessagePersistenceService>();
                        await service.UpdateStuckProcessingStatusesAsync(cancellationToken);
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex,
                        "MessagePersistenceBackgroundService  UpdateStuckProcessingStatusesAsync error");
                }
                finally
                {
                    var delay = _options.StuckProcessingInterval is { }
                        ? TimeSpan.FromSeconds((int)_options.StuckProcessingInterval)
                        : TimeSpan.FromSeconds(900);

                    await Task.Delay(delay, cancellationToken);
                }
            }
        }
    }
}
