using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.Extensions.Logging;
using Order.Domain;
using Order.Domain.Projections;

namespace Order.Application.Features.Products.CreatingProduct.v1.Internals;

public class CreateProductHandler : ICommandHandler<CreateProduct, bool>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateProductHandler(ILogger<CreateProductHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var product = ProductReadModel.Create(command.ProductId, command.Name, command.Price, command.Status);

        await _context.Products.AddAsync(product, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
