namespace Auth.Domain.Models.Tokens;

public class RefreshToken(Guid id, string name, DateTime expirationDate)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public DateTime ExpirationDate { get; set; } = expirationDate;
}