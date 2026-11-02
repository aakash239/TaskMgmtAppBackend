using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Exceptions;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Entities;

namespace TaskManagement.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
private readonly AppDbContext _context;

public ProjectRepository(AppDbContext context) => _context = context;


private static readonly Expression<Func<ProjectMember, ProjectDto>> ToProjectDto = m => new ProjectDto
{
ProjectId = m.Project.ProjectId,
Name = m.Project.Name,
Description = m.Project.Description,
Status = m.Project.Status,
DueDate = m.Project.DueDate,
CreatedAt = m.Project.CreatedAt,
UpdatedAt = m.Project.UpdatedAt,
MyRole = m.Role
};

public async Task<ProjectRole?> GetUserRoleAsync(int projectId, int userId, bool includeDeleted, CancellationToken cancellationToken = default)
{
IQueryable<ProjectMember> query = _context.ProjectMembers.AsNoTracking(); // read-only, so no change tracking needed

if (includeDeleted)
query = query.IgnoreQueryFilters(); // also search projects that are in the trash

return await query
.Where(m => m.ProjectId == projectId && m.UserId == userId)
.Select(m => (ProjectRole?)m.Role) // nullable so "no row" can be represented as null
.FirstOrDefaultAsync(cancellationToken);
}

public async Task<List<ProjectDto>> GetMyProjectsAsync(int userId, CancellationToken cancellationToken = default)
{
return await _context.ProjectMembers
.AsNoTracking()
.Where(m => m.UserId == userId) // only my memberships (trashed projects filtered out automatically)
.OrderByDescending(m => m.Project.CreatedAt) // newest first
.Select(ToProjectDto)   
.ToListAsync(cancellationToken);
}

public Task<ProjectDto?> GetByIdAsync(int projectId, int userId, CancellationToken cancellationToken = default)
{
return _context.ProjectMembers
.AsNoTracking()
.Where(m => m.ProjectId == projectId && m.UserId == userId) // null result = not found or not a member
.Select(ToProjectDto)
.FirstOrDefaultAsync(cancellationToken);
}

public async Task<ProjectDto> CreateAsync(int userId, ProjectRequestDto request, CancellationToken cancellationToken = default)
{
var project = new Project
{
Name = request.Name,
Description = request.Description,
Status = request.Status,
DueDate = request.DueDate,
CreatedByUserId = userId,
CreatedAt = DateTime.UtcNow,


Members = { new ProjectMember { UserId = userId, Role = ProjectRole.Admin } }
};

_context.Projects.Add(project); // mark the project for insert
await _context.SaveChangesAsync(cancellationToken); // runs the inserts; EF fills in project.ProjectId

return new ProjectDto
{
ProjectId = project.ProjectId,
Name = project.Name,
Description = project.Description,
Status = project.Status,
DueDate = project.DueDate,
CreatedAt = project.CreatedAt,
UpdatedAt = project.UpdatedAt,
MyRole = ProjectRole.Admin
};
}

public async Task UpdateAsync(int projectId, ProjectRequestDto request, CancellationToken cancellationToken = default)
{
// Load the active project (the global filter already excludes trashed ones).
var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId, cancellationToken)
?? throw new NotFoundException("Project not found.");

project.Name = request.Name; // EF tracks these changes
project.Description = request.Description;
project.Status = request.Status;
project.DueDate = request.DueDate;
project.UpdatedAt = DateTime.UtcNow;

await _context.SaveChangesAsync(cancellationToken); 
}

public async Task SoftDeleteAsync(int projectId, int userId, CancellationToken cancellationToken = default)
{
// ExecuteUpdate sends ONE UPDATE statement without loading the row first.
await _context.Projects
.Where(p => p.ProjectId == projectId) 
.ExecuteUpdateAsync(setters => setters
.SetProperty(p => p.IsDeleted, true) // move to trash
.SetProperty(p => p.DeletedAt, (DateTime?)DateTime.UtcNow) // start the countdown
.SetProperty(p => p.DeletedByUserId, (int?)userId), // remember who did it
cancellationToken);
}

public async Task<List<TrashedProjectDto>> GetMyTrashAsync(int userId, CancellationToken cancellationToken = default)
{
return await _context.ProjectMembers
.IgnoreQueryFilters() //  trashed projects wanted here
.AsNoTracking()
.Where(m => m.UserId == userId
&& m.Role == ProjectRole.Admin // only admins see (and can restore) the trash
&& m.Project.IsDeleted) // only trashed projects
.OrderByDescending(m => m.Project.DeletedAt) // most recently trashed first
.Select(m => new TrashedProjectDto
{
ProjectId = m.Project.ProjectId,
Name = m.Project.Name,
Status = m.Project.Status,
DeletedAt = m.Project.DeletedAt!.Value // always set when IsDeleted = 1
})
.ToListAsync(cancellationToken);
}

public async Task<bool> RestoreAsync(int projectId, CancellationToken cancellationToken = default)
{
var rows = await _context.Projects
.IgnoreQueryFilters() // look inside the trash
.Where(p => p.ProjectId == projectId && p.IsDeleted) // only if it really is trashed
.ExecuteUpdateAsync(setters => setters
.SetProperty(p => p.IsDeleted, false)
.SetProperty(p => p.DeletedAt, (DateTime?)null) // clear the countdown
.SetProperty(p => p.DeletedByUserId, (int?)null),
cancellationToken);

return rows > 0; // 0 rows = nothing was in the trash with that id
}

public Task<int> PurgeExpiredAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default)
{

return _context.Projects
.IgnoreQueryFilters()
.Where(p => p.IsDeleted && p.DeletedAt != null && p.DeletedAt < cutoffUtc)
.ExecuteDeleteAsync(cancellationToken); // returns how many projects were removed
}
}
