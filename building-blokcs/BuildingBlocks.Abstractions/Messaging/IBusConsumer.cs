namespace BuildingBlocks.Abstractions.Messaging
{
    public interface IBusConsumer
    {
        void Consume<TMessage>(
            IMessageHandler<TMessage> handler,
            Action<IConsumeConfigurationBuilder>? consumeBuilder = null) where TMessage : class, IMessage;

        Task Consume<TMessage>(
            MessageHandler<TMessage> subscribeMethod,
            Action<IConsumeConfigurationBuilder>? consumeBuilder = null,
            CancellationToken cancellationToken = default) where TMessage : class, IMessage;

        Task Consume<TMessage>(CancellationToken cancellationToken) where TMessage : class, IMessage;


        Task Consume(Type messageType, CancellationToken cancellationToken);

        Task Consume<THandler, TMessage>(CancellationToken cancellationToken)
            where THandler : IMessageHandler<TMessage>
            where TMessage : class, IMessage;

        Task ConsumeAll(CancellationToken cancellationToken);
        Task ConsumeAllFromAssemblyOf<TType>(CancellationToken cancellationToken);
        Task ConsumeAllFromAssemblyOf(CancellationToken cancellationToken, params Type[] assemblyMarkerTypes);
    }
}
