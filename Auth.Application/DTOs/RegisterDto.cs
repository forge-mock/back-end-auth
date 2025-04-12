namespace Auth.Application.DTOs;

public sealed class RegisterDto : LoginDto
{
    public string Username { get; set; } = string.Empty;
}