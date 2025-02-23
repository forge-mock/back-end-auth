using Auth.Application.DTOs;
using FluentValidation;

namespace Auth.Application.Services.Validators;

internal sealed class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    internal RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .Matches("^[a-zA-Z0-9]{3,20}$")
            .WithMessage(
                "Username must be 3-20 characters long and contain only letters and numbers (no special characters)");

        RuleFor(x => x.UserEmail)
            .NotEmpty().WithMessage("Email is required")
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$").WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
            .WithMessage("Password must be at least 8 characters and contain letters & numbers");
    }
}