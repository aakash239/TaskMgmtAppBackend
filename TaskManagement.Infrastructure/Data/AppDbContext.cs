using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>(); 
   public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>(); 


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
}
}






    
