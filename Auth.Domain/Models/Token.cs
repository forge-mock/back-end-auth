namespace Auth.Domain.Models;

public sealed class Token
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}

public class RefreshToken(Guid id, string name, DateTime expirationDate)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public DateTime ExpirationDate { get; set; } = expirationDate;
}