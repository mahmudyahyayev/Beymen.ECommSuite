using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Core.Validations
{
    public class ValidateRequest<T>
    {
        private ValidationResult Validation { get; }

        private ValidateRequest(T value, ValidationResult validation)
        {
            Value = value;
            Validation = validation;
        }

        public T Value { get; }
        public bool IsValid => Validation.IsValid;

        public IDictionary<string, string[]> Errors =>
            Validation.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

        public void Deconstruct(out bool isValid, out T value)
        {
            isValid = IsValid;
            value = Value;
        }

        public static async ValueTask<ValidateRequest<T>> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            var value = await context.Request.ReadFromJsonAsync<T>();
            var validator = context.RequestServices.GetRequiredService<IValidator<T>>();

            if (value is null)
            {
                throw new ArgumentException(parameter.Name);
            }

            var results = await validator.ValidateAsync(value);

            return new ValidateRequest<T>(value, results);
        }
    }
}
