using BuildingBlocks.Abstractions.Messaging;
using MassTransit;
using MassTransit.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Integration.MassTransit;

public class GenericFaultConsumer<T>(
    IServiceProvider serviceProvider) : IFilter<ExceptionReceiveContext>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context,
        IPipe<ExceptionReceiveContext> next)
    {
        context.TryGetPayload(out ConsumeContext consumeContext);
        consumeContext.TryGetMessage(out ConsumeContext<T> concreteContext);

        var fault = new FaultEvent<T>(concreteContext.Message, concreteContext.MessageId, concreteContext.Host,
            context.Exception, context.GetMessageTypes());

        var service = serviceProvider.GetService<IFaultConsumer<T>>();

        if (service != null)
            await service.ConsumeAsync(fault);

        await next.Send(context);
    }
}
