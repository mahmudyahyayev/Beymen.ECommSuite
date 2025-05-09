using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using MediatR;
using Order.Application.Features.Products.CreatingProduct.v1.Internals;
namespace Order.Application.Features.Products.CreatingProduct.v1;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly ICommandProcessor _commandProcessor;

    public CreateProductCommandHandler(ICommandProcessor commandProcessor)
    {
        _commandProcessor = commandProcessor;
    }

    public async Task<Unit> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var createCustomer = new CreateProduct(
            command.ProductId,
            command.Name,
            command.Price,
            command.Status);

        using (Serilog.Context.LogContext.PushProperty("Command", nameof(CreateProductCommandHandler)))
        using (Serilog.Context.LogContext.PushProperty("ProductId", command.ProductId))
        {
            var result = await _commandProcessor.SendAsync(createCustomer, cancellationToken);

            return Unit.Value;
        }
    }
}
