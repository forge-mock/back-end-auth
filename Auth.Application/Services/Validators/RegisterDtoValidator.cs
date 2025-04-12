using Auth.Application.DTOs;
using FluentValidation;

namespace Auth.Application.Services.Validators;

internal sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    internal RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Matches("^[a-zA-Z0-9 ]{3,20}$")
            .WithMessage("Username must be 3-20 characters long and contain only letters and numbers");

        BaseValidator.AddEmailRules(RuleFor(x => x.UserEmail));
        BaseValidator.AddPasswordRules(RuleFor(x => x.Password));
    }
}