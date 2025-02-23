namespace Auth.Domain.Models.Tokens;

public class RefreshToken(Guid id, string name)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;
}