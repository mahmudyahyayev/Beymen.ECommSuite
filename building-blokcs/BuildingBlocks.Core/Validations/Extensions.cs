using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace BuildingBlocks.Core.Validations
{
    public static class Extensions
    {
        private static ValidationResultModel ToValidationResultModel(this ValidationResult? validationResult)
        {
            return new ValidationResultModel(validationResult);
        }
        public static async Task HandleValidationAsync<TRequest>(
            this IValidator<TRequest> validator,
            TRequest request,
            CancellationToken cancellationToken
        )
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.ToValidationResultModel());
        }

        public static IServiceCollection AddCustomValidators(
            this IServiceCollection services,
            Assembly[] assembly,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            services.Scan(
                scan =>
                    scan.FromAssemblies(assembly)
                        .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                        .AsImplementedInterfaces()
                        .WithLifetime(serviceLifetime)
            );

            return services;
        }
    }
}
