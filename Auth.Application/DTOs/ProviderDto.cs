namespace Auth.Application.DTOs;

public class ProviderDto(string username, string userEmail, string accessToken, string providerAccountId)
{
    public string Username { get; set; } = username;
    public string UserEmail { get; set; } = userEmail;
    public string AccessToken { get; set; } = accessToken;
    public string ProviderAccountId { get; set; } = providerAccountId;
}