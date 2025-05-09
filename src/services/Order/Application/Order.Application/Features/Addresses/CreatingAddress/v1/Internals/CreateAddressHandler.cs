using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Microsoft.Extensions.Logging;
using Order.Domain;
using Order.Domain.Projections;

namespace Order.Application.Features.Addresses.CreatingAddress.v1.Internals;
public class CreateAddressHandler : ICommandHandler<CreateAddress, bool>
{
    private readonly ILogger<CreateAddressHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateAddressHandler(ILogger<CreateAddressHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateAddress command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var address = AddressReadModel.Create(command.AddressId, command.CustomerId, command.FullAddress, command.Type);

        await _context.Addresses.AddAsync(address, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}