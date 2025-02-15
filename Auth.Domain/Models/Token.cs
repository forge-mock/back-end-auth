namespace Auth.Domain.Models;

public sealed class Token
{
    public Guid Id { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}