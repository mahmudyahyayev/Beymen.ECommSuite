using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Inventory.Application.Features.Products.CreatingProducts.v1;

public record CreateProduct(
    string Name,
    string Description,
    decimal Price,
    int Stock) : ITxCreateCommand<bool>;