using BuildingBlocks.Abstractions.CQRS.Commands;

namespace Order.Application.Features.Products.CreatingProduct.v1.Internals;


public record CreateProduct(
     Guid ProductId,
    string Name,
    decimal Price,
    int Status) : ITxCreateCommand<bool>;