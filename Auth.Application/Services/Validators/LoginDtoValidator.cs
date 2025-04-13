using Auth.Application.DTOs;
using FluentValidation;

namespace Auth.Application.Services.Validators;

internal sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    internal LoginDtoValidator()
    {
        BaseValidator.AddEmailRules(RuleFor(x => x.UserEmail));
        BaseValidator.AddPasswordRules(RuleFor(x => x.Password));
    }
}