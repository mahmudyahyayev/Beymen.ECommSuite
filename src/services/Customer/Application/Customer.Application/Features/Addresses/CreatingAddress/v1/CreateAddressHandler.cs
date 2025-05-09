using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Customer.Domain;
using Customer.Domain.AggregateRoot.Addresses;
using Microsoft.Extensions.Logging;

namespace Customer.Application.Features.Addresses.CreatingAddress.v1;

public class CreateAddressHandler : ICommandHandler<CreateAddress, bool>
{
    private readonly ILogger<CreateAddressHandler> _logger;
    private readonly IApplicationDbContext _context;

    public CreateAddressHandler(ILogger<CreateAddressHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Handle(CreateAddress command,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var address = Address.Create(command.CustomerId, command.TypeId, command.Country, command.City, command.District, command.Street, command.ZipCode, command.BuildingNo, command.ApartmentNo, command.Floor, command.Description);

        await _context.Addresses.AddAsync(address, cancellationToken);

        var result = await _context.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
