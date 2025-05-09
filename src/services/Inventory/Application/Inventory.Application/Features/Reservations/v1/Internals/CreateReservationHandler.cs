using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Inventory.Domain;
using Inventory.Domain.AggregateRoot.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Features.Reservations.v1.Internals;

public class CreateReservationHandler : ICommandHandler<CreateReservation, bool>
{
    private readonly ILogger<CreateReservationHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateReservationHandler(ILogger<CreateReservationHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    //reservation ve stokdan dusme islemleri
    public async Task<bool> Handle(CreateReservation command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var reservation = Reservation.Create(command.CustomerId, command.OrderId);

        foreach (var reservationItem in command.Items)
        {
            var product = await _context.Products.SingleOrDefaultAsync(u => u.Id == reservationItem.ProductId, cancellationToken);

            if (product == null || !product.IsStockAvailable(reservationItem.Quantity))
            {
                reservation.CancelReservation($"Product {reservationItem.ProductId} stock insufficient");

                await _context.Reservations.AddAsync(reservation, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return false;
            }

            reservation.AddItem(reservationItem.ProductId, reservationItem.Quantity);

            product.UpdateStock(-reservationItem.Quantity);
        }

        reservation.ConfirmReservation();

        await _context.Reservations.AddAsync(reservation, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
