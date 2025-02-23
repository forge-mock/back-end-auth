using Auth.Domain.Models.Users;

namespace Auth.Domain.Models.Tokens;

public sealed class Token
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}