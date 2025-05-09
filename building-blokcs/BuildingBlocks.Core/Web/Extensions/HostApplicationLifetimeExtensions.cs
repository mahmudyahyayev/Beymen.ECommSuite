using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Core.Web.Extenions
{
    public static class HostApplicationLifetimeExtensions
    {
        public static async Task<bool> WaitForAppStartup(
            this IHostApplicationLifetime lifetime,
            CancellationToken stoppingToken)
        {
            var startedSource = new TaskCompletionSource();
            var cancelledSource = new TaskCompletionSource();

            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
            using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

            Task completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);
            return completedTask == startedSource.Task;
        }
    }
}
