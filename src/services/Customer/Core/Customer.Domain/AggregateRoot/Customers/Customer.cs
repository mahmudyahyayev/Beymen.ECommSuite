using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Domain;
using Customer.Domain.AggregateRoot.Customers.Events.Domain;
using Customer.Domain.Behaviors;

namespace Customer.Domain.AggregateRoot.Customers;

public class Customer : Aggregate<CustomerId, string>,
    IHaveAudit<string>,
    IHaveIdentity<CustomerId>,
    IStateBehavior
{
    /// <summary>
    /// Main Attributes
    /// </summary>
    public string _firstName;
    public string FirstName => _firstName;

    public string _lastName;
    public string LastName => _lastName;

    private PhoneNumber _phoneNumber;
    public PhoneNumber PhoneNumber => _phoneNumber;

    public Email _email;
    public Email Email => _email;

    /// <summary>
    /// Statebehavior
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

    private Customer()
    {
        //for efcore
    }
    public static Customer Create(string firstName, string lastName, string phoneNumber, string email)
    {
        Customer customer = new()
        {
            Id = CustomerId.Of(Guid.NewGuid()),
            _firstName = firstName,
            _lastName = lastName,
            _phoneNumber = PhoneNumber.Of(phoneNumber),
            _email = Email.Of(email),
            _isActive = true
        };
        customer.WhenCreate();

        return customer;
    }

    public void UpdateMainAttributes(string firstName, string lastName, string phoneNumber, string email)
    {
        _firstName = firstName;
        _lastName = lastName;
        _phoneNumber = PhoneNumber.Of(phoneNumber);
        _email = Email.Of(email);

        AddDomainEvents(new CustomerMainAttributesChanged(Id.Value, _firstName, _lastName, _phoneNumber.Value, _email.Value));
    }

    public void WhenCreate()
    {
        AddDomainEvents(new CustomerCreated(Id.Value, _firstName, _lastName, _phoneNumber.Value, _email.Value));
    }

    public void Activate()
    {
        _isActive = true;

        AddDomainEvents(new CustomerActivated(Id.Value));
    }

    public void Deactivate()
    {
        _isActive = false;
        AddDomainEvents(new CustomerDeactivated(Id.Value));
    }
}