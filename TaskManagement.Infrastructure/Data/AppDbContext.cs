using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Data;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<Project> Projects => Set<Project>(); 
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>(); 


    // User
    public DbSet<User> Users => Set<User>(); 

    //RefreshTokens and Sessions
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    
    // Kanban Templates
    public DbSet<KanbanTemplate> KanbanTemplates => Set<KanbanTemplate>();
    public DbSet<KanbanTemplateColumn> KanbanTemplateColumns => Set<KanbanTemplateColumn>();

    // User Boards
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();


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

        modelBuilder.Entity<Project>(entity =>
       {
           entity.ToTable("Projects", "dbo"); 
           entity.HasKey(p => p.ProjectId); // PK_Projects
           entity.Property(p => p.ProjectId).ValueGeneratedOnAdd(); 

           entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
           entity.Property(p => p.Description).HasMaxLength(2000);
           entity.Property(p => p.Status).HasColumnType("tinyint"); // enum stored as a small number
           entity.Property(p => p.DueDate).HasColumnType("date"); // date only, no time
           entity.Property(p => p.CreatedAt).HasColumnType("datetime2");
           entity.Property(p => p.UpdatedAt).HasColumnType("datetime2");
           entity.Property(p => p.DeletedAt).HasColumnType("datetime2");
           entity.HasOne(p => p.Template).WithMany(t => t.Projects).HasForeignKey(p => p.TemplateId).HasConstraintName("FK_Projects_KanbanTemplates").OnDelete(DeleteBehavior.NoAction);

          //  every normal query automatically skips trashed projects.
        
          entity.HasQueryFilter(p => !p.IsDeleted);
     });
    
        modelBuilder.Entity<ProjectMember>(entity =>
    {
       entity.ToTable("ProjectMembers", "dbo");
       entity.HasKey(m => new { m.ProjectId, m.UserId }); // composite PK_ProjectMembers
       entity.Property(m => m.Role).HasColumnType("tinyint");
       entity.Property(m => m.JoinedAt).HasColumnType("datetime2");

       entity.HasOne(m => m.Project) // each member row belongs to one project
       .WithMany(p => p.Members) // a project has many member rows
       .HasForeignKey(m => m.ProjectId)
       .OnDelete(DeleteBehavior.Cascade); 

       // Same filter on the member side, so memberships of trashed projects are hidden too.
      
       entity.HasQueryFilter(m => !m.Project.IsDeleted);
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
    
        modelBuilder.Entity<KanbanTemplate>(entity =>
        {
            entity.ToTable("KanbanTemplates", "dbo");

            // Primary Key
            entity.HasKey(t => t.TemplateId)
                  .HasName("PK_KanbanTemplates");

            //Name
            entity.Property(t => t.Name)
                    .HasMaxLength(100)
                    .IsRequired();

            //Description
            entity.Property(t => t.Description)
                    .HasMaxLength(500);

            entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()", "DF_KanbanTemplates_UpdatedAt");
            
            entity.HasOne(t => t.User)
                    .WithMany(u => u.KanbanTemplates)
                    .HasForeignKey(t => t.UserId)
                    .HasConstraintName("FK_KanbanTemplates_Users")
                    .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(t => t.UserId)
                    .HasDatabaseName("IDX_KanbanTemplates_UserId");

            entity.Property(t => t.IsDeleted)
                    .HasDefaultValue(false);

            entity.Property(t => t.DeletedAt);
        });
    
        modelBuilder.Entity<KanbanTemplateColumn> (entity =>
        {
           entity.ToTable("KanbanTemplateColumns", "dbo", 
                        table => table.HasCheckConstraint("CK_KanbanTemplateColumns_Position", "[Position] >= 1")); 

            entity.HasKey(c => c.TemplateColumnId)
                    .HasName("PK_KanbanTemplateColumns");

            entity.Property(c => c.Name)
                    .HasMaxLength(100)
                    .IsRequired();

            entity.Property(c => c.Description)
                    .HasMaxLength(500);

            entity.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()", "DF_KanbanTemplateColumns_CreatedAt");
            
            entity.Property(c => c.UpdatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()", "DF_KanbanTemplateColumns_UpdatedAt");

            entity.HasOne(c => c.Template)
                    .WithMany(t => t.Columns)
                    .HasForeignKey(c => c.TemplateId)
                    .HasConstraintName("FK_KanbanTemplateColumns_KanbanTemplates")
                    .OnDelete(DeleteBehavior.Cascade);

            entity.HasAlternateKey(c => new {c.TemplateId, c.Position})
                    .HasName("UQ_KanbanTemplateColumns_Position");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.ToTable("Boards", "dbo");

            entity.HasKey(b => b.BoardId)
                .HasName("PK_Boards");

            entity.Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(b => b.Description)
                .HasMaxLength(500);

            entity.Property(b => b.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()",
                    "DF_Boards_CreatedAt");

            entity.Property(b => b.UpdatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()",
                    "DF_Boards_UpdatedAt");

            entity.HasOne(b => b.User)
                .WithMany(u => u.Boards)
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("FK_Boards_Users")
                .OnDelete(DeleteBehavior.NoAction);

            // Board belonging to a project
            entity.HasOne(b => b.Project)
                .WithMany(p => p.Boards)
                .HasForeignKey(b => b.ProjectId)
                .HasConstraintName("FK_Boards_Projects")
                .OnDelete(DeleteBehavior.NoAction);

            //Board references it's original template
            entity.HasOne(b => b.Template)
                .WithMany(t => t.Boards)
                .HasForeignKey(b => b.TemplateId)
                .HasConstraintName("FK_Boards_KanbanTemplates")
                .OnDelete(DeleteBehavior.NoAction);
            
            entity.Property(b => b.IsDeleted)
                    .HasDefaultValue(false);
                    
            entity.Property(b => b.DeletedAt)
                    .IsRequired(false);

            // One board per user per project
            entity.HasIndex(b => new {b.ProjectId, b.UserId})
                .IsUnique()
                .HasDatabaseName("UQ_Boards_ProjectId_UserId");

            entity.HasIndex(b => b.UserId)
                .HasDatabaseName("IX_Boards_UserId");

            entity.HasIndex(b => b.ProjectId)
                .HasDatabaseName("IX_Boards_ProjectId");

            entity.HasIndex(b => b.TemplateId)
                .HasDatabaseName("IX_Boards_TemplateId");


        });
    
        modelBuilder.Entity<BoardColumn>(entity =>
        {
            entity.ToTable("BoardColumns", "dbo",
                table => table.HasCheckConstraint(
                    "CK_BoardColumns_Position",
                    "[Position] >= 1"));


            entity.HasKey(c => c.BoardColumnId)
                .HasName("PK_BoardColumns");


            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();


            entity.Property(c => c.Description)
                .HasMaxLength(500);


            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()",
                    "DF_BoardColumns_CreatedAt");


            entity.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()",
                    "DF_BoardColumns_UpdatedAt");


            entity.HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId)
                .HasConstraintName("FK_BoardColumns_Boards")
                .OnDelete(DeleteBehavior.Cascade);


            entity.HasAlternateKey(c => new { c.BoardId, c.Position })
                .HasName("UQ_BoardColumns_Position");
        });
    
        // Global Query Filters
        // Whenever EF Core queries this entities it automatically applies (Where IsDeleted = 0) to the query
        modelBuilder.Entity<KanbanTemplate>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Board>().HasQueryFilter(b => !b.IsDeleted);
    }    
}

