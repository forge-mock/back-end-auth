namespace Auth.Domain.Models;

public sealed class RefreshToken(Guid id, string name, DateTime expirationDate)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public DateTime ExpirationDate { get; set; } = expirationDate;
}