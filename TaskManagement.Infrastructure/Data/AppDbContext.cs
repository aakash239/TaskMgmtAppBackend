using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users
    {
        get { return Set<User>(); }
    }

    public DbSet<UserSession> UserSessions
    {
        get { return Set<UserSession>(); }
    }

    public DbSet<RefreshToken> RefreshTokens
    {
        get { return Set<RefreshToken>(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User> (entity =>
        {
            entity.ToTable("Users", "dbo");
            entity.HasKey(u => u.UserId).HasName("PK_Users");
           
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();
            entity.HasAlternateKey(u => u.NormalizedEmail).HasName("UniqueNormalizedEmail");
            entity.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(u => u.IsActive).HasDefaultValue(true, "DF_Users_IsActive");
            entity.Property(u => u.CreatedAt).HasPrecision(7)
                .HasDefaultValueSql("SYSUTCDATETIME()", "DF_Users_CreatedAt");
        });  

        modelBuilder.Entity<UserSession>(entity =>
        {
            // Table mapping
            entity.ToTable("UserSessions", "dbo", table =>
                table.HasCheckConstraint("CK_UserSessions_Expiration", "[ExpiresAt] > [CreatedAt]"));

            // Primary key
            entity.HasKey(s => s.SessionId).HasName("PK_UserSessions");

            // Properties
            entity.Property(s => s.SessionId)
                .ValueGeneratedNever();

            entity.Property(s => s.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()", "DF_UserSessions_CreatedAt");

            entity.Property(s => s.LastActivityAt)
                .IsRequired(false);

            entity.Property(s => s.RevokedAt)
                .IsRequired(false);

            // Relationship: User -> UserSessions
            entity.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_UserSessions_Users")
                .OnDelete(DeleteBehavior.NoAction);

            // Index
            entity.HasIndex(s => s.UserId)
                .HasDatabaseName("IX_UserSessions_UserId");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Table mapping
            entity.ToTable("RefreshTokens", "dbo", table =>
                table.HasCheckConstraint("CK_RefreshTokens_Expiration", "[ExpiresAt] > [CreatedAt]"));

            // Primary key
            entity.HasKey(r => r.RefreshTokenId).HasName("PK_RefreshTokens");

            entity.Property(r => r.RefreshTokenId)
                .ValueGeneratedOnAdd();

            // Properties
            entity.Property(r => r.TokenHash)
                .HasColumnType("varchar(64)")
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsRequired();

            entity.Property(r => r.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()", "DF_RefreshTokens_CreatedAt");

            entity.Property(r => r.RevokedAt)
                .IsRequired(false);

            // Relationship: UserSession -> RefreshTokens
            entity.HasOne(r => r.Session)
                .WithMany(s => s.RefreshTokens)
                .HasForeignKey(r => r.SessionId)
                .HasConstraintName("FK_RefreshTokens_UserSessions")
                .OnDelete(DeleteBehavior.NoAction);

            // Self-referencing relationship
            entity.HasOne(r => r.ReplacedByToken)
                .WithMany()
                .HasForeignKey(r => r.ReplacedByTokenId)
                .HasConstraintName("FK_RefreshTokens_Replacement")
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            entity.HasAlternateKey(r => r.TokenHash)
                .HasName("UQ_RefreshTokens_TokenHash");

            entity.HasIndex(r => r.SessionId)
                .HasDatabaseName("IX_RefreshTokens_SessionId");
            entity.HasIndex(r => r.ReplacedByTokenId)
                .HasDatabaseName("IX_RefreshTokens_ReplacedByTokenId");
        });
    }    
}
