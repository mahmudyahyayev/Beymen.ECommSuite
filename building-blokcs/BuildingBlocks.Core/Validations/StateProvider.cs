namespace BuildingBlocks.Core.Validations;

public class StateProvider
{
    public string ExceptionId { get; }
    public IDictionary<object, object> Data { get; }

    public StateProvider(string exceptionId, IDictionary<object, object> data)
    {
        ExceptionId = exceptionId;
        Data = data ?? new Dictionary<object, object>();
    }
    public static StateProvider Create(string exceptionId, params (object key, object value)[] keyValues)
    {
        var data = keyValues.ToDictionary(kv => kv.key, kv => kv.value);
        return new StateProvider(exceptionId, data);
    }
}
