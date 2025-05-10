namespace Auth.Domain.Models;

public sealed class UserIdentify(Guid id, string username, string userEmail, string password)
{
    public Guid Id { get; set; } = id;

    public string Username { get; set; } = username;

    public string UserEmail { get; set; } = userEmail;

    public string Password { get; set; } = password;
}