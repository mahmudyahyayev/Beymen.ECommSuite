namespace BuildingBlocks.Core.Validations
{
    public class ValidationException : System.Exception
    {
        public ValidationException(ValidationResultModel validationResultModel)
        {
            ValidationResultModel = validationResultModel;
        }

        public ValidationResultModel ValidationResultModel { get; }
    }
}