using FluentValidation;

namespace Auth.Application.Services.Validators;

internal abstract class BaseValidator
{
    internal static void AddEmailRules<T>(IRuleBuilder<T, string> rule)
    {
        rule.NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }

    internal static void AddPasswordRules<T>(IRuleBuilder<T, string> rule)
    {
        rule.NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$")
            .WithMessage("Password must be at least 8 characters and contain letters & numbers");
    }
}