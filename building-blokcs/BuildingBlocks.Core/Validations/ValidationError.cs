namespace BuildingBlocks.Core.Validations
{
    public class ValidationError
    {
        public ValidationError(string field, string message, string exceptionId, IDictionary<object, object> data)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
            ExceptionId = exceptionId;
            Data = data;
        }

        public string? Field { get; }
        public string Message { get; }
        public string ExceptionId { get; }
        public IDictionary<object, object> Data { get; set; }
    }
}
