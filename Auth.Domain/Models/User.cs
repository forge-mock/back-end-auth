namespace Auth.Domain.Models;

public sealed class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public ICollection<Token> Tokens { get; set; } = new List<Token>();
}