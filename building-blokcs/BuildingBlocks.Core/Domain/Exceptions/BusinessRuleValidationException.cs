using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain.Exceptions;
[Serializable]
public class BusinessRuleValidationException : DomainException
{
    public IBusinessRule BrokenRule { get; }
    public string Details { get; }
    public int StatusCode { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message, brokenRule.ExceptionId, brokenRule.StatusCode)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
        StatusCode = brokenRule.StatusCode;
    }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
    }
}

