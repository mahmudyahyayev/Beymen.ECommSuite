using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Customer.Domain.AggregateRoot.Addresses.Events.Domain;
using Customer.Domain.Behaviors;

namespace Customer.Domain.AggregateRoot.Addresses;

public class Address : Aggregate<AddressId, string>,
    IHaveAudit<string>,
    IHaveIdentity<AddressId>,
    IStateBehavior
{
    /// <summary>
    /// Main attributes
    /// </summary>
    private Ownership _customerId;
    public Ownership CustomerId => _customerId;

    private int _typeId;
    public AddressType Type { get; private set; }

    private Location _country;
    public Location Country => _country;

    private Location _city;
    public Location City => _city;

    private Location _district;
    public Location District => _district;

    private Location _street;
    public Location Street => _street;

    private ZipCode _zipCode;
    public ZipCode ZipCode => _zipCode;

    private string _buildingNo;
    public string BuildingNo => _buildingNo;

    private string _apartmentNo;
    public string ApartmentNo => _apartmentNo;

    private string _floor;
    public string Floor => _floor;

    private string _description;
    public string Description => _description;


    /// <summary>
    /// State Behavior
    /// </summary>
    private bool _isActive = true;
    public bool IsActive => _isActive;


    /// <summary>
    /// Auditable
    /// </summary>
    private string _lastModifiedBy;
    public string LastModifiedBy => _lastModifiedBy;
    private DateTime? _lastModified;
    public DateTime? LastModified => _lastModified;

    private Address()
    {
        //for efcore
    }

    public static Address Create(Guid customerId, int typeId, string country, string city, string district, string street, string zipCode, string buildingNo, string apartmentNo, string floor, string description)
    {
        Address address = new()
        {
            Id = AddressId.Of(Guid.NewGuid()),
            _customerId = Ownership.Of(customerId),
            _typeId = typeId,
            _country = Location.Of(country),
            _city = Location.Of(city),
            _district = Location.Of(district),
            _street = Location.Of(street),
            _zipCode = ZipCode.Of(zipCode),
            _buildingNo = buildingNo,
            _apartmentNo = apartmentNo,
            _floor = floor,
            _description = description,
            _isActive = true
        };

        address.WhenCreate();

        return address;
    }

    public string GetFullAddress() =>
    $"{_street.Value} {_buildingNo}, Kat: {_floor}, Daire: {_apartmentNo}, {_district.Value}, {_city.Value}, {_country.Value}, {_zipCode.Value} - {_description}";

    public void UpdateMainAttributes(string country, string city, string district, string street, string zipCode, string buildingNo, string apartmentNo, string floor, string description)
    {
        _country = Location.Of(country);
        _city = Location.Of(city);
        _district = Location.Of(district);
        _street = Location.Of(street);
        _zipCode = ZipCode.Of(zipCode);
        _buildingNo = buildingNo;
        _apartmentNo = apartmentNo;
        _floor = floor;
        _description = description;

        string fullAddress = GetFullAddress();

        AddDomainEvents(new AddressMainAttributesChanged(Id.Value, fullAddress));
    }

    public void WhenCreate()
    {
        string fullAddress = GetFullAddress();

        AddDomainEvents(new AddressCreated(Id.Value, _customerId.Value, _typeId, fullAddress));
    }

    public void ChangeAddressType(int typeId)
    {
        _typeId = typeId;

        AddDomainEvents(new AddressTypeChanged(Id.Value, _typeId));
    }

    public void Activate()
    {
        _isActive = true;

        AddDomainEvents(new AddressActivated(Id.Value));
    }

    public void Deactivate()
    {
        _isActive = false;
        AddDomainEvents(new AddressDeactivated(Id.Value));
    }
}
