using Auth.Application.DTOs;
using FluentValidation;
using Shared.Validators;

namespace Auth.Application.Services.Validators;

internal sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    internal RegisterDtoValidator()
    {
        LoginValidator.AddUsernameRules(RuleFor(x => x.Username));
        LoginValidator.AddEmailRules(RuleFor(x => x.UserEmail));
        LoginValidator.AddPasswordRules(RuleFor(x => x.Password));
    }
}