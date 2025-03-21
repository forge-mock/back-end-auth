namespace Auth.Application.DTOs;

public sealed class LoginDto
{
    public string UserInput { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}