using BuildingBlocks.Core.Validations;
using FluentValidation;

namespace BuildingBlocks.FluentValidation.Extensions
{
    public static class Extension
    {
        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> rule)
        {
            return rule.Matches(@"^(1-)?\d{3}\d{3}\d{4}$").WithMessage("Phone number address is invalid.");
        }
        public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> rule)
        {
            string[] blacklistedWords = { "123456", "123456789", "qwerty", "12345678", "111111", "1234567890", "1234567", "password", "123123", "987654321", "qwertyuiop", "mynoob", "123321", "666666", "18atcskd2w", "7777777", "1q2w3e4r", "654321", "555555", "3rjs1la7qe", "google", "1q2w3e4r5t", "123qwe", "zxcvbnm", "1q2w3e" };
            return
               rule.NotEmpty()
               .MinimumLength(6)
               .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain one or more capital letters.")
               .Matches("[a-z]").WithMessage("'{PropertyName}' must contain one or more lowercase letters.")
               .Matches(@"\d").WithMessage("'{PropertyName}' must contain one or more digits.")
               .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]").WithMessage("'{PropertyName}' must contain one or more special characters.")
               .Matches("^[^£# “”]*$").WithMessage("'{PropertyName}' must not contain the following characters £ # “” or spaces.")
               .Must(pass => !blacklistedWords.Any(word => pass.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0))
                   .WithMessage("'{PropertyName}' contains a word that is not allowed.")
               .Must(pass => !blacklistedWords.Any(word => pass.Equals(word, StringComparison.OrdinalIgnoreCase)))
                    .WithMessage("'{PropertyName}' contains a word that is not allowed.");
        }
        public static IRuleBuilderOptions<T, string> SimplePassword<T>(this IRuleBuilder<T, string> rule)
        {
            return
                rule.NotEmpty()
                    .MinimumLength(6)
                    .MaximumLength(6);
        }

        public static IRuleBuilderOptions<T, TProperty> WithExceptionId<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder,
        string exceptionId,
        params (object key, object value)[] keyValues)
        {
            var data = keyValues.ToDictionary(kv => kv.key, kv => kv.value);
            var stateProvider = new StateProvider(exceptionId, data); 

            return ruleBuilder.WithState(x => stateProvider);
        }
    }
}
