namespace Auth.Domain.Models;

public sealed class OauthProvider
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<UserOauthProvider> UserOauthProviders { get; set; } = new List<UserOauthProvider>();
}