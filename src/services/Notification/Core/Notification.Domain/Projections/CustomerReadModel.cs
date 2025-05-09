using BuildingBlocks.Core.Domain;

namespace Notification.Domain.Projections;
public class CustomerReadModel : Entity<Guid, string>
{
    /// <summary>
    /// Main Attributes
    /// </summary>
    public string _firstName;
    public string FirstName => _firstName;

    public string _lastName;
    public string LastName => _lastName;

    private string _phoneNumber;
    public string PhoneNumber => _phoneNumber;

    public string _email;
    public string Email => _email;

    private bool _isActive = true;
    public bool IsActive => _isActive;

    private CustomerReadModel()
    {
        //for efcore
    }
    public static CustomerReadModel Create(Guid id, string firstName, string lastName, string phoneNumber, string email)
    {
        CustomerReadModel customer = new()
        {
            Id = id,
            _firstName = firstName,
            _lastName = lastName,
            _phoneNumber = phoneNumber,
            _email = email,
            _isActive = true
        };
        return customer;
    }

    public void UpdateMainAttributes(string firstName, string lastName, string phoneNumber, string email)
    {
        _firstName = firstName;
        _lastName = lastName;
        _phoneNumber = phoneNumber;
        _email = email;
    }

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
}