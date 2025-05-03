namespace Auth.Domain.Models;

public sealed class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string Password { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public ICollection<Token> Tokens { get; set; } = new List<Token>();

    public ICollection<OauthProvider> Providers { get; set; } = new List<OauthProvider>();
}

public sealed class UserIdentify(Guid id, string username, string userEmail, string password)
{
    public Guid Id { get; set; } = id;

    public string Username { get; set; } = username;

    public string UserEmail { get; set; } = userEmail;

    public string Password { get; set; } = password;
}

public sealed class ProviderUser
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string UserEmail { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}