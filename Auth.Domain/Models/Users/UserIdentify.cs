namespace Auth.Domain.Models.Users;

public sealed class UserIdentify(string username, string userEmail, string password)
{
    public string Username { get; set; } = username;

    public string UserEmail { get; set; } = userEmail;

    public string Password { get; set; } = password;
}