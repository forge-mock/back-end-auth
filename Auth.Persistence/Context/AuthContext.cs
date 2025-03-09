using Auth.Domain.Models.Tokens;
using Auth.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Context;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Token>(
            entity =>
            {
                entity.HasKey(e => e.Id).HasName("tokens_pkey");

                entity.ToTable("tokens");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .HasColumnName("name");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date");
                entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tokens");
            });

        modelBuilder.Entity<User>(
            entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.UserEmail, "users_user_email_key").IsUnique();

                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date");
                entity.Property(e => e.Password)
                    .HasMaxLength(128)
                    .HasColumnName("password");
                entity.Property(e => e.UserEmail)
                    .HasMaxLength(255)
                    .HasColumnName("user_email");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");
            });
    }
}