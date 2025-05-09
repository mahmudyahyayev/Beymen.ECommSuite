using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Domain;
using Order.Domain.AggregateRoot.Orders;
using Order.Domain.Projections;

namespace Order.Application.Features.Orders.CreatingOrder.v1;

public class CreateOrderHandler : ICommandHandler<CreateOrder, CreateOrderResponse>
{
    private readonly ILogger<CreateOrderHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateOrderHandler(ILogger<CreateOrderHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrder command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var shippingAddress = await _context.Addresses.SingleOrDefaultAsync(u => u.Id == command.ShippingAddressId && u.Type == 1, cancellationToken);
        ArgumentNullException.ThrowIfNull(shippingAddress, nameof(shippingAddress));
        var billingAddress = await  _context.Addresses.SingleOrDefaultAsync(u => u.Id == command.BillingAddressId && u.Type == 2, cancellationToken);
        if (billingAddress is null)
        {
            billingAddress = shippingAddress;
        }

        List<OrderItem> orderItems = new List<OrderItem>();

        foreach (var item in command.Items)
        {
            var product = await _context.Products.SingleOrDefaultAsync(u => u.Id == item.ProductId,cancellationToken);

            ArgumentNullException.ThrowIfNull(product, nameof(product));

            var orderItem =  OrderItem.Create(ProductSnapshot.Of(product.Id, product.Name, product.UnitPrice), item.Quantity);

            orderItems.Add(orderItem);
        }


        var order = Domain.AggregateRoot.Orders.Order.Create(
            command.CustomerId, 
            AddressSnapshot.Of(shippingAddress.Id, shippingAddress.FullAddress),
             AddressSnapshot.Of(billingAddress.Id, billingAddress.FullAddress),
             orderItems
            );

        await _context.Orders.AddAsync(order, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponse("Your order is being prepared.");
    }
}

