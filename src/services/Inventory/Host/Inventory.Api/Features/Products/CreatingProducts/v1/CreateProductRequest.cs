namespace Inventory.Api.Features.Products.CreatingProducts.v1;

public record CreateProductRequest(
     string Name,
    string Description,
    decimal Price,
    int Stock);
