using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Products.CreatingProduct.v1;

public record CreateProductCommand(
    Guid ProductId,
    string Name,
    decimal Price,
    int Status) : ICommand;




