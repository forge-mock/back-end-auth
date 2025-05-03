namespace Auth.Application.DTOs;

public class ProviderDto(string username, string userEmail, string accessToken)
{
    public string Username { get; set; } = username;
    public string UserEmail { get; set; } = userEmail;
    public string AccessToken { get; set; } = accessToken;
}