using System.Net;
using System.Text.Json;
using FluentValidation.Results;

namespace BuildingBlocks.Core.Validations
{
    public class ValidationResultModel
    {
        public ValidationResultModel(ValidationResult? validationResult = null)
        {
            Errors = new List<ValidationError>();

            if (validationResult?.Errors != null && validationResult.Errors.Any())
            {
                foreach (var error in validationResult.Errors)
                {
                    StateProvider? stateProvider = null;
                    if (error.CustomState is StateProvider sp)
                    {
                        stateProvider = sp;
                    }

                    Errors.Add(new ValidationError(error.PropertyName, error.ErrorMessage, stateProvider?.ExceptionId, stateProvider?.Data));
                }
            }
        }

        public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;

        public IList<ValidationError>? Errors { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
