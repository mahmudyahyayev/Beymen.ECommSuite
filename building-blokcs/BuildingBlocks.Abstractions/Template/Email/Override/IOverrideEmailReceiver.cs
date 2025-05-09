namespace BuildingBlocks.Abstractions.Template.Email.Override
{
    public interface IOverrideEmailReceiver
    {
        IEnumerable<string> Receivers { get; set; }
        IEnumerable<string> CarbonCopy { get; set; }
        IEnumerable<string> BlindCarbonCopy { get; set; }
    }
}
