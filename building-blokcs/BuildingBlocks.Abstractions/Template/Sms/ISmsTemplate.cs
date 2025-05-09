namespace BuildingBlocks.Abstractions.Template.Sms
{
    public interface ISmsTemplate : ITemplate
    {
        public IEnumerable<string> PhoneNumbers { get; set; }
    }
}
