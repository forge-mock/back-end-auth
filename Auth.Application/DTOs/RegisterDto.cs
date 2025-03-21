namespace Auth.Application.DTOs;

public class RegisterDto
{
    public string UserEmail { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}