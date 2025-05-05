using Auth.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Persistence.Context;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<OauthProvider> OauthProviders { get; set; }

    public virtual DbSet<UserOauthProvider> UserOauthProviders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<OauthProvider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("oauth_providers_pkey");

            entity.ToTable("oauth_providers");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tokens_pkey1");

            entity.ToTable("tokens");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tokens_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
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

        modelBuilder.Entity<UserOauthProvider>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProviderId }).HasName("user_oauth_provider_pkey");

            entity.ToTable("user_oauth_provider");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.ProviderAccountId)
                .HasMaxLength(40)
                .HasColumnName("provider_account_id");

            entity.HasOne(d => d.Provider).WithMany(p => p.UserOauthProviders)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("user_oauth_provider_provider_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserOauthProviders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_oauth_provider_user_id_fkey");
        });
    }
}