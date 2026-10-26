using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Data;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User> (entity =>
        {
            entity.ToTable("Users", "dbo");
            entity.HasKey(u => u.UserId);
           
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.CreatedAt).HasPrecision(3).HasDefaultValueSql("SYSUTCDATETIME()");
        });  

        modelBuilder.Entity<UserSession>(entity =>
        {
            // Table mapping
            entity.ToTable("UserSessions", "dbo");

            // Primary key
            entity.HasKey(s => s.SessionId);

            // Properties
            entity.Property(s => s.SessionId)
                .ValueGeneratedNever();

            entity.Property(s => s.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(s => s.LastActivityAt)
                .IsRequired(false);

            entity.Property(s => s.RevokedAt)
                .IsRequired(false);

            // Relationship: User -> UserSessions
            entity.HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Index
            entity.HasIndex(s => s.UserId)
                .HasDatabaseName("IX_UserSessions_UserId");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Table mapping
            entity.ToTable("RefreshTokens", "dbo");

            // Primary key
            entity.HasKey(r => r.RefreshTokenId);

            entity.Property(r => r.RefreshTokenId)
                .ValueGeneratedOnAdd();

            // Properties
            entity.Property(r => r.TokenHash)
                .HasColumnType("varchar(64)")
                .IsRequired();

            entity.Property(r => r.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(r => r.RevokedAt)
                .IsRequired(false);

            // Relationship: UserSession -> RefreshTokens
            entity.HasOne(r => r.Session)
                .WithMany(s => s.RefreshTokens)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Self-referencing relationship
            entity.HasOne(r => r.ReplacedByToken)
                .WithMany()
                .HasForeignKey(r => r.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.NoAction);

            // Indexes
            entity.HasIndex(r => r.TokenHash)
                .IsUnique()
                .HasDatabaseName("UQ_RefreshTokens_TokenHash");

            entity.HasIndex(r => r.SessionId)
                .HasDatabaseName("IX_RefreshTokens_SessionId");
        });
    }    
}