using Auth.Application.DTOs;
using FluentValidation;
using Shared.Validators;

namespace Auth.Application.Services.Validators;

internal sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    internal LoginDtoValidator()
    {
        LoginValidator.AddEmailRules(RuleFor(x => x.UserEmail));
        LoginValidator.AddPasswordRules(RuleFor(x => x.Password));
    }
}