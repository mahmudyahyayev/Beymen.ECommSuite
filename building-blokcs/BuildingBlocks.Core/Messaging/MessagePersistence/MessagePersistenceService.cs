using System.Data.Common;
using System.Diagnostics;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internals;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging.Extensions;
using BuildingBlocks.Core.Types;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Messaging.MessagePersistence
{
    public class MessagePersistenceService : IMessagePersistenceService
    {
        private readonly ILogger<MessagePersistenceService> _logger;
        private readonly MessagePersistenceOptions _options;
        private readonly IMessagePersistenceRepository _messagePersistenceRepository;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMediator _mediator;
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMachineInstanceInfo _machineInstanceInfo;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public DbConnection DbConnection { get; set; }

        public MessagePersistenceService(
            ILogger<MessagePersistenceService> logger,
            IMessagePersistenceRepository messagePersistenceRepository,
            IMessageSerializer messageSerializer,
            IMediator mediator,
            IBus bus,
            IServiceProvider serviceProvider,
            IMachineInstanceInfo machineInstanceInfo,
            MessagePersistenceOptions options,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _messagePersistenceRepository = messagePersistenceRepository;
            _messageSerializer = messageSerializer;
            _mediator = mediator;
            _bus = bus;
            _serviceProvider = serviceProvider;
            _machineInstanceInfo = machineInstanceInfo;
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;

        }

        private async Task ProcessAsync(
            Guid messageId,
            Guid batchId,
            CancellationToken cancellationToken)
        {
            var message = await _messagePersistenceRepository.GetByIdAsync(messageId, cancellationToken);

            if (message is null || message.MessageStatus == MessageStatus.Processed)
            {
                return;
            }

            var stopWatch = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation(
                    "{ServiceName} start processing eventType: {DataType} and messageId: {MessageId} and deliveryType: {DeliveryType} and batchId: {BatchId}",
                    nameof(MessagePersistenceService),
                    message.DataType,
                    message.Id,
                    message.DeliveryType,
                    batchId);

                switch (message.DeliveryType)
                {
                    case MessageDeliveryType.Inbox:
                        await ProcessInboxAsync(message, cancellationToken);
                        break;
                    case MessageDeliveryType.Internal:
                        await ProcessInternalAsync(message, cancellationToken);
                        break;
                    case MessageDeliveryType.Outbox:
                        await ProcessOutboxAsync(message, cancellationToken);
                        break;
                }

                await _messagePersistenceRepository.ChangeStateAsync(
                    messageId: message.Id,
                    status: MessageStatus.Processed,
                    errorMessage: null,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "{ServiceName} finish processing timeSpent: {TimeSpent} ms - eventType: {DataType} and messageId: {MessageId} and deliveryType: {DeliveryType} and batchId: {BatchId}",
                    stopWatch.ElapsedMilliseconds,
                    nameof(MessagePersistenceService),
                    message.DataType,
                    message.Id,
                    message.DeliveryType,
                    batchId);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogError($"Concurrent update error encountered messageID:{message.Id}");
                return;
            }
            catch (System.Exception ex)
            {

                var innerException = ex.FindInnerExceptionRecursively();

                await _messagePersistenceRepository.ChangeStateAsync(
                    messageId: message.Id,
                    status: MessageStatus.Failed,
                    errorMessage: innerException.Message,
                    cancellationToken: cancellationToken);

                _logger.LogError(
                    exception: innerException,
                    "{ServiceName} error processing timeSpent: {TimeSpent} eventType: {DataType} and messageId: {MessageId} and deliveryType: {DeliveryType} and batchId: {BatchId}",
                    stopWatch.ElapsedMilliseconds,
                    nameof(MessagePersistenceService),
                    message.DataType,
                    message.Id,
                    message.DeliveryType,
                    batchId);
            }
            finally
            {
                stopWatch.Stop();
            }
        }

        private async Task ProcessOutboxAsync(StoreMessage message, CancellationToken cancellationToken)
        {
            MessageEnvelope? messageEnvelope = _messageSerializer.Deserialize<MessageEnvelope>(message.Data, true);
 
            if (messageEnvelope is null || messageEnvelope.Message is null)
                return;
 
            var data = _messageSerializer.Deserialize(
                messageEnvelope.Message.ToString()!,
                TypeMapper.GetType(message.DataType)
            );

             if (data is IMessage _intMessage)
            {
                var sendMode = _intMessage.SendMode;
                var queueName = _intMessage.QueueName;

                if (sendMode == MessageSendMode.Send && !string.IsNullOrEmpty(queueName))
                {
                    await _bus.PublishAsync(data, messageEnvelope.Headers, null, queueName, cancellationToken);

                    _logger.LogInformation(
                    "Message with id: {MessageId} and delivery type: {DeliveryType} processed from the persistence message store",
                    message.Id,
                    message.DeliveryType);

                    return;
                }

                await _bus.PublishAsync(data, messageEnvelope.Headers, cancellationToken);

                _logger.LogInformation(
                "Message with id: {MessageId} and delivery type: {DeliveryType} processed from the persistence message store",
                message.Id,
                message.DeliveryType);
            }
        }

        private async Task ProcessInternalAsync(StoreMessage message, CancellationToken cancellationToken)
        {
            MessageEnvelope? messageEnvelope = _messageSerializer.Deserialize<MessageEnvelope>(message.Data, true);
 
            if (messageEnvelope is null || messageEnvelope.Message is null)
                return;
 
            var data = _messageSerializer.Deserialize(
                messageEnvelope.Message.ToString()!,
                TypeMapper.GetType(message.DataType)
            );

            if (data is IDomainNotificationEvent domainNotificationEvent)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Publish(domainNotificationEvent, cancellationToken);

                _logger.LogInformation(
                    "Domain-Notification with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                    message.Id,
                    message.DeliveryType
                );
            }

            if (data is IInternalCommand internalCommand)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(internalCommand, cancellationToken);

                _logger.LogInformation(
                    "InternalCommand with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                    message.Id,
                    message.DeliveryType
                );
            }
        }

        private async Task ProcessInboxAsync(StoreMessage message, CancellationToken cancellationToken)
        {
            MessageEnvelope? messageEnvelope = _messageSerializer.Deserialize<MessageEnvelope>(message.Data, true);
 
            if (messageEnvelope is null || messageEnvelope.Message is null)
                return;
 
            var data = _messageSerializer.Deserialize(
                messageEnvelope.Message.ToString()!,
                TypeMapper.GetType(message.DataType)
            );

            if (data is IMessage _data)
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var commandMapper = scope.ServiceProvider.GetRequiredService<ICommandMapper>();

                if (commandMapper is not null)
                {
                    var command = commandMapper.MapToCommand(_data);

                    await _mediator.Send(command, cancellationToken);

                    _logger.LogInformation(
                        "Message with id: {MessageId} and delivery type: {DeliveryType} processed from the persistence message store",
                        message.Id,
                        message.DeliveryType);
                }
            }
        }

        public async Task ProcessAllAsync(
            CancellationToken cancellationToken)
        {
            var podPartitions =
                await _messagePersistenceRepository.GetPodPartitionByPodIdAsync(_machineInstanceInfo.ClientId,
                    cancellationToken);

            var messages = await _messagePersistenceRepository.GetByFilterAsync(
                x => x.MessageStatus == MessageStatus.Stored
                     && podPartitions.Contains((int)x.Partition),
                cancellationToken
            );

            if (!messages.Any())
            {
                return;
            }

            var batchId = Guid.NewGuid();

            await _messagePersistenceRepository.ChangeBulkStateAsync(
                messageIds: messages.Select(x => x.Id).ToList(),
                status: MessageStatus.Processing,
                errorMessage: null,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "{ServiceName} bulk status updated with batchId: {BatchId} and messageIds: {MessageIdAggregate}",
                nameof(MessagePersistenceService),
                batchId,
                JsonConvert.SerializeObject(messages.Select(x => x.Id).ToList()));

            foreach (var message in messages)
            {
                await ProcessAsync(message.Id, batchId, cancellationToken);
            }
        }

        public async Task ProcessAllErrorAsync(
            CancellationToken cancellationToken)
        {
            var podPartitions =
                await _messagePersistenceRepository.GetPodPartitionByPodIdAsync(_machineInstanceInfo.ClientId,
                    cancellationToken);
            if (podPartitions.Count == 0)
            {
                return;
            }

            var messages = await _messagePersistenceRepository.GetByFilterAsync(
                x => x.MessageStatus == MessageStatus.Failed
                     && x.RetryCount < _options.ErrorRetryCount
                     && podPartitions.Contains((int)x.Partition),
                cancellationToken
            );

            if (!messages.Any())
            {
                return;
            }

            var batchId = Guid.NewGuid();

            await _messagePersistenceRepository.ChangeBulkStateAsync(
                messageIds: messages.Select(x => x.Id).ToList(),
                status: MessageStatus.Processing,
                errorMessage: null,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "{ServiceName} bulk status updated with batchId: {BatchId} and messageIds: {MessageIdAggregate}",
                nameof(MessagePersistenceService),
                batchId,
                JsonConvert.SerializeObject(messages.Select(x => x.Id).ToList()));

            foreach (var message in messages)
            {
                await ProcessAsync(message.Id, batchId, cancellationToken);
            }
        }

        public async Task UpdateStuckProcessingStatusesAsync(
            CancellationToken cancellationToken)
        {
            var podPartitions =
                await _messagePersistenceRepository.GetPodPartitionByPodIdAsync(_machineInstanceInfo.ClientId,
                    cancellationToken);

            if (podPartitions.Count == 0)
            {
                return;
            }

            var messages = await _messagePersistenceRepository.GetByFilterAsync(
                x => x.MessageStatus == MessageStatus.Processing
                     && podPartitions.Contains((int)x.Partition)
                     && x.Modified < DateTime.Now.AddSeconds(-_options.StuckProcessingInterval),
                batchCount: _options.StuckProcessingBatchCount,
                cancellationToken: cancellationToken
            );

            if (!messages.Any())
            {
                return;
            }

            var batchId = Guid.NewGuid();

            try
            {
                _logger.LogInformation(
                    $"{nameof(MessagePersistenceService)} start to update stuck processing events with batchId: {batchId}");

                await _messagePersistenceRepository.ChangeBulkStateAsync(
                    messageIds: messages.Select(x => x.Id).ToList(),
                    status: MessageStatus.Stored,
                    errorMessage: null,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    $"{nameof(MessagePersistenceService)} finish to update stuck processing events with batchId: {batchId}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(
                    $"{nameof(MessagePersistenceService)} error to update stuck processing events with batchId: {batchId}",
                    ex);
            }
        }




        public async Task AddPublishMessageAsync<TMessageEnvelope>(
            TMessageEnvelope messageEnvelope,
            CancellationToken cancellationToken
        )
            where TMessageEnvelope : MessageEnvelope
        {
            await AddMessageCoreAsync(
                messageEnvelope,
                MessageDeliveryType.Outbox,
                cancellationToken);
        }

        public async Task AddInternalMessageAsync<TCommand>(
            TCommand internalCommand,
            string correlationId,
            CancellationToken cancellationToken
        )
            where TCommand : class, IInternalCommand
        {
            var headers = new Dictionary<string, object>();
            headers.AddCorrelationId( correlationId );

            await AddMessageCoreAsync(
                messageEnvelope: new MessageEnvelope(
                    message: internalCommand,
                    headers: headers),
                    deliveryType: MessageDeliveryType.Internal,
                cancellationToken: cancellationToken);
        }

        public async Task AddNotificationAsync(
            IDomainNotificationEvent notification,
            CancellationToken cancellationToken)
        {
            var internalMessage = new MessageEnvelope(
                message: notification);

            var storeMessage = StoreMessage.Create(
                id: notification.EventId,
                dataType: TypeMapper.GetFullTypeName(internalMessage.Message.GetType()),
                data: _messageSerializer.Serialize(internalMessage),
                deliveryType: MessageDeliveryType.Internal,
                instanceName: _machineInstanceInfo.ClientId.ToString(),
                partition: Math.Abs(notification.MessageKey.GetPersistentHashCode() % _options.PartitionMaxCount) + 1,
                key: notification.MessageKey,
                priority: notification.Priority);

            _messagePersistenceRepository.DbConnection = this.DbConnection;

            await _messagePersistenceRepository.AddAsync(storeMessage, cancellationToken);
        }

        public async Task AddReceivedMessageAsync<TMessageEnvelope>(
            TMessageEnvelope messageEnvelope,
            CancellationToken cancellationToken)
            where TMessageEnvelope : MessageEnvelope
        {
            await AddMessageCoreAsync(
                messageEnvelope: messageEnvelope,
                deliveryType: MessageDeliveryType.Inbox,
                cancellationToken);
        }


        private async Task AddMessageCoreAsync(
           MessageEnvelope messageEnvelope,
           MessageDeliveryType deliveryType,
           CancellationToken cancellationToken)
        {
            Guard.Against.Null(messageEnvelope.Message, nameof(messageEnvelope.Message));

            Guid id;
            string key = "";
            int priority = 0;

            if (messageEnvelope.Message is IMessage im)
            {
                id = im.MessageId;
                key = im.MessageKey;
                priority = im.Priority;
            }
            else if (messageEnvelope.Message is IInternalCommand command)
            {
                id = command.InternalCommandId;
                key = command.MessageKey;
                priority = command.Priority;
            }
            else
            {
                id = Guid.NewGuid();
            }

            messageEnvelope.Headers.AddMessageId(id.ToString());

            var storeMessage = StoreMessage.Create(
                id: id,
                dataType: TypeMapper.GetFullTypeName(messageEnvelope.Message.GetType()),
                data: _messageSerializer.Serialize(messageEnvelope),
                deliveryType: deliveryType,
                instanceName: _machineInstanceInfo.ClientId.ToString(),
                partition: Math.Abs(key.GetPersistentHashCode() % _options.PartitionMaxCount) + 1,
                key: key,
                priority: priority);

            await _messagePersistenceRepository.AddAsync(storeMessage, cancellationToken);

            _logger.LogDebug(
                "Message with id: {MessageID} and delivery type: {DeliveryType} saved in persistence message store",
                id,
                deliveryType.ToString()
            );
        }
    }
}
