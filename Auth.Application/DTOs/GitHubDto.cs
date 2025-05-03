namespace Auth.Application.DTOs;

public class GitHubDto
{
    public string Username { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}