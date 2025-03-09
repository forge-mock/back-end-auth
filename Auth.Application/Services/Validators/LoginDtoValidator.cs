using Auth.Application.DTOs;
using FluentValidation;

namespace Auth.Application.Services.Validators;

internal sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    internal LoginDtoValidator()
    {
        RuleFor(x => x.UserInput)
            .NotEmpty().WithMessage("Username or email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}