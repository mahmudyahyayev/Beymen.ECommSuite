using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Inventory.Domain;
using Inventory.Domain.AggregateRoot.Products;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.Products.CreatingProducts.v1;

public class CreateProductHandler : ICommandHandler<CreateProduct, bool>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateProductHandler(ILogger<CreateProductHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateProduct command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var product = Product.Create(command.Name, command.Description, command.Price, command.Stock);

        await _context.Products.AddAsync(product, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
