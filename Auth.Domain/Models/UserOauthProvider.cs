namespace Auth.Domain.Models;

public sealed class UserOauthProvider
{
    public Guid UserId { get; set; }

    public Guid ProviderId { get; set; }

    public string ProviderAccountId { get; set; } = null!;

    public OauthProvider Provider { get; set; } = null!;

    public User User { get; set; } = null!;
}