namespace Auth.Application.DTOs;

public class LoginDto
{
    public string UserEmail { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}