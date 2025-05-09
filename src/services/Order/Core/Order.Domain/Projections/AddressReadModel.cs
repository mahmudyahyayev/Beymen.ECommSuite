using BuildingBlocks.Core.Domain;

namespace Order.Domain.Projections;

public class AddressReadModel : Entity<Guid, string>
{
    /// <summary>
    /// Main Attributes
    /// </summary>
    public string _fullAddress;
    public string FullAddress => _fullAddress;

    public int _type;
    public int Type => _type;

    public Guid _customerId;
    public Guid CustomerId => _customerId;

    private AddressReadModel()
    {
        //for efcore
    }
    public static AddressReadModel Create(Guid id, Guid customerId, string fullAddress, int type)
    {
        AddressReadModel address = new()
        {
            Id = id,
            _fullAddress = fullAddress,
            _type = type,
            _customerId = customerId
        };
        return address;
    }

    public void ChangeAddress(string fullAddress)
    {
        _fullAddress = fullAddress;
    }
    public void ChangeType(int type)
    { 
        _type = type;
    }
}
