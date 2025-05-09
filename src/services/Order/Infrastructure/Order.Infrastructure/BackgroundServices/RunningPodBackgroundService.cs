using System.Reflection;
using BuildingBlocks.Abstractions.Messaging.PersistMessage.Partition.Base;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Messaging.BackgroundServices;
using BuildingBlocks.Core.Messaging.MessagePersistence;
using BuildingBlocks.Core.Web.Extenions;
using BuildingBlocks.Messaging.Persistence.Postgres.MessagePersistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Order.Infrastructure.BackgroundServices;

public class RunningPodAndPartitionCheckBackgroundService<TPodPartition, TRunningPod> : BackgroundService
    where TPodPartition : BasePodPartition, new()
    where TRunningPod : BaseRunningPod, new()
{
    private readonly ILogger<MessagePersistenceBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IMachineInstanceInfo _machineInstanceInfo;

    public RunningPodAndPartitionCheckBackgroundService(
        ILogger<MessagePersistenceBackgroundService> logger,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime lifetime,
        IMachineInstanceInfo machineInstanceInfo
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _lifetime = lifetime;
        _machineInstanceInfo = machineInstanceInfo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await _lifetime.WaitForAppStartup(stoppingToken))
        {
            return;
        }

        _ = Task.Run(() => PodAndPartitionCheckAsync(stoppingToken), stoppingToken);

        _ = Task.Run(() => DeadPodsCheckAsync(stoppingToken), stoppingToken);

        _logger.LogInformation(
            "RunningPodAndPartitionCheckBackgroundService Service is starting on client '{@ClientId}' and group '{@ClientGroup}'",
            _machineInstanceInfo.ClientId,
            _machineInstanceInfo.ClientGroup
        );
    }

    private async Task PodAndPartitionCheckAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                await PodAndPartitionCheckAsync(scope);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "RunningPodAndPartitionCheckBackgroundService PodAndPartitionCheckAsync error");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    private async Task DeadPodsCheckAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                await DeadPodsRemoveAsync(scope);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "RunningPodAndPartitionCheckBackgroundService DeadPodsCheckAsync error");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }

    private async Task DeadPodsRemoveAsync(AsyncServiceScope scope)
    {
        var persistenceContext = scope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

        var deadPods = await persistenceContext.Set<TRunningPod>().Where(p => p.Status == "Dead").AsNoTracking()
            .ToListAsync();
        persistenceContext.Set<TRunningPod>().RemoveRange(deadPods);

        await persistenceContext.SaveChangesAsync();
    }

    private async Task PodAndPartitionCheckAsync(AsyncServiceScope scope)
    {
        var persistenceContext = scope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

        var deadPods = await persistenceContext.Set<TRunningPod>()
            .Where(p => (DateTime.UtcNow - p.LastHearthBeat) > TimeSpan.FromMinutes(1))
            .ToListAsync();

        foreach (var deadPod in deadPods)
        {
            deadPod.Status = "Dead";
        }

        var deadPodIds = deadPods.Select(s => s.Id).ToList();

        var deletedPodPartitions = await persistenceContext.Set<TPodPartition>()
            .Where(p => deadPodIds.Contains(p.PodId))
            .AsNoTracking()
            .ToListAsync();

        persistenceContext.Set<TPodPartition>().RemoveRange(deletedPodPartitions);

        var assignedPodIds = await persistenceContext.Set<TPodPartition>()
            .AsNoTracking()
            .Select(s => s.PodId)
            .Distinct()
            .ToListAsync();

        var existsPodIds = await persistenceContext.Set<TRunningPod>()
            .AsNoTracking()
            .Where(p => assignedPodIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var notExistsPodIds = assignedPodIds?.Except(existsPodIds).ToList();

        if (notExistsPodIds != null &&
            notExistsPodIds.Count > 0) //remove assigned but removed from running-pod table pods
        {
            var partitionsToRemove = await persistenceContext.Set<TPodPartition>()
                .Where(p => notExistsPodIds.Contains(p.PodId))
                .ToListAsync();

            persistenceContext.Set<TPodPartition>().RemoveRange(partitionsToRemove);
        }

        await persistenceContext.SaveChangesAsync();
    }
}

public class RunningPodAndPartitionAssignmentBackgroundService<TPodPartition, TRunningPod> : BackgroundService
    where TPodPartition : BasePodPartition, new()
    where TRunningPod : BaseRunningPod, new()
{
    private readonly ILogger<MessagePersistenceBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly MessagePersistenceOptions _options;
    private readonly IMachineInstanceInfo _machineInstanceInfo;

    public RunningPodAndPartitionAssignmentBackgroundService(
        ILogger<MessagePersistenceBackgroundService> logger,
        IOptions<MessagePersistenceOptions> options,
        IServiceProvider serviceProvider,
        IHostApplicationLifetime lifetime,
        IMachineInstanceInfo machineInstanceInfo
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _lifetime = lifetime;
        _options = options.Value;
        _machineInstanceInfo = machineInstanceInfo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await _lifetime.WaitForAppStartup(stoppingToken))
        {
            return;
        }

        _ = Task.Run(() => PodAssignmentAsync(stoppingToken), stoppingToken);

        _ = Task.Run(() => PartitionAssignmentAsync(stoppingToken), stoppingToken);


        _logger.LogInformation(
            "RunningPodAndPartitionAssignmentBackgroundService Service is starting on client '{@ClientId}' and group '{@ClientGroup}'",
            _machineInstanceInfo.ClientId,
            _machineInstanceInfo.ClientGroup
        );
    }

    private async Task PodAssignmentAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var persistenceContext = scope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

                var runningPodId = _machineInstanceInfo.ClientId;
                if (!await persistenceContext.Set<TRunningPod>().AnyAsync(p => p.Id == runningPodId, cancellationToken))
                {
                    var runningPod = new TRunningPod()
                    {
                        Id = runningPodId,
                        PodName =
                            $"{Assembly.GetExecutingAssembly().GetName().Name}_{Environment.MachineName}_{_machineInstanceInfo.ClientId}",
                        Status = "Alive",
                        LastHearthBeat = DateTime.UtcNow,
                        CreationTime = DateTime.UtcNow
                    };
                    await persistenceContext.Set<TRunningPod>().AddAsync(runningPod, cancellationToken);
                }
                else
                {
                    var runningPod = await persistenceContext.Set<TRunningPod>().Where(p => p.Id == runningPodId)
                        .FirstOrDefaultAsync(cancellationToken);

                    runningPod.Status = "Alive";
                    runningPod.LastHearthBeat = DateTime.UtcNow;
                }

                await persistenceContext.SaveChangesAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "RunningPodAndPartitionAssignmentBackgroundService PodAssignmentAsync error");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }
    }

    private async Task PartitionAssignmentAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var persistenceContext = scope.ServiceProvider.GetRequiredService<MessagePersistenceDbContext>();

                var lastRegisterRunningPod = await persistenceContext.Set<TRunningPod>().Where(p => p.Status == "Alive")
                    .OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync(cancellationToken);

                if (lastRegisterRunningPod == null)
                {
                    continue;
                }

                var executePartition = lastRegisterRunningPod?.CreationTime.GetValueOrDefault() <
                                       DateTime.UtcNow.AddMinutes(-1);
                if (!executePartition)
                {
                    continue;
                }

                var registeredPodList = await persistenceContext.Set<TRunningPod>().Where(p => p.Status == "Alive")
                    .ToListAsync(cancellationToken);

                var assignedPartitions = await persistenceContext.Set<TPodPartition>().AsNoTracking()
                    .ToListAsync(cancellationToken);

                int partitionMaxCount = _options.PartitionMaxCount;

                int activePartitionCount = Convert.ToInt32(Math.Round(Convert.ToDecimal(_options.PartitionMaxCount) /
                                                                      Convert.ToDecimal(registeredPodList.Count)));

                List<int> partitionList = new List<int>();
                foreach (var partition in registeredPodList)
                {
                    partitionList.Add(activePartitionCount);
                    partitionMaxCount -= activePartitionCount;
                    if (partitionMaxCount < activePartitionCount)
                    {
                        activePartitionCount = partitionMaxCount;
                    }

                    if (registeredPodList.IndexOf(partition) == registeredPodList.Count - 2 && partitionMaxCount > 0)
                    {
                        activePartitionCount = partitionMaxCount;
                    }
                }

                if (assignedPartitions.Count() == _options.PartitionMaxCount)
                {
                    var groupAssignedPartition =
                        assignedPartitions.GroupBy(x => x.PodId, (key, g) => new { Key = key, Value = g.Count() });

                    if (!groupAssignedPartition.Where(x => !partitionList.Contains(x.Value)).Any())
                    {
                        continue;
                    }
                }

                ////Partitionları tekrar dağıt

                await persistenceContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    try
                    {
                        using var tran =
                            await persistenceContext.Database.BeginTransactionAsync(System.Data.IsolationLevel
                                .Serializable, cancellationToken);

                        var podPartitionsToDelete =
                            await persistenceContext.Set<TPodPartition>().ToListAsync(cancellationToken);
                        persistenceContext.Set<TPodPartition>().RemoveRange(podPartitionsToDelete);

                        int counter = 0;
                        int podCounter = 0;
                        foreach (var item in partitionList)
                        {
                            Guid podId = registeredPodList[podCounter].Id;
                            for (int i = 0; i < item; i++)
                            {
                                counter++;
                                await persistenceContext.Set<TPodPartition>().AddAsync(
                                    new TPodPartition() { Id = Guid.NewGuid(), PodId = podId, Partition = counter },
                                    cancellationToken);
                            }

                            podCounter++;
                        }

                        await persistenceContext.SaveChangesAsync(cancellationToken);
                        await tran.CommitAsync(cancellationToken);
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException is DbUpdateException &&
                            ex.InnerException.InnerException is PostgresException &&
                            (ex.InnerException.InnerException as PostgresException).SqlState ==
                            PostgresErrorCodes.SerializationFailure)
                        {
                        }
                        else
                        {
                            throw;
                        }
                    }
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex,
                    "RunningPodAndPartitionAssignmentBackgroundService PartitionAssignmentAsync error");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }
    }
}
