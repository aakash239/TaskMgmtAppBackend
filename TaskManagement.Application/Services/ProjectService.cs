using Microsoft.Extensions.Options;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Core.Configuration;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

// All project business rules live here
public class ProjectService : IProjectService
{
private readonly IProjectRepository _projectRepository; // data access abstraction
private readonly ICurrentUserService _currentUser; // who is calling
private readonly ProjectSettings _settings; // trash retention days

public ProjectService(
IProjectRepository projectRepository,
ICurrentUserService currentUser,
IOptions<ProjectSettings> options)
{
_projectRepository = projectRepository;
_currentUser = currentUser;
_settings = options.Value; // the bound "Projects" section
}

public Task<List<ProjectDto>> GetMyProjectsAsync(CancellationToken cancellationToken = default) =>
_projectRepository.GetMyProjectsAsync(_currentUser.UserId, cancellationToken);

public async Task<ProjectDto> GetByIdAsync(int projectId, CancellationToken cancellationToken = default)
{
var project = await _projectRepository.GetByIdAsync(projectId, _currentUser.UserId, cancellationToken);

// null 
return project ?? throw new NotFoundException("Project not found.");
}

public Task<ProjectDto> CreateAsync(ProjectRequestDto request, CancellationToken cancellationToken = default)
{
Normalize(request); // trim spaces
// Any logged-in user may create a project. The repository makes them its Admin.
return _projectRepository.CreateAsync(_currentUser.UserId, request, cancellationToken);
}

public async Task<ProjectDto> UpdateAsync(int projectId, ProjectRequestDto request, CancellationToken cancellationToken = default)
{
await RequireAdminAsync(projectId, includeDeleted: false, cancellationToken);      
Normalize(request);
await _projectRepository.UpdateAsync(projectId, request, cancellationToken);
return await GetByIdAsync(projectId, cancellationToken); // return the saved version
}

public async Task DeleteAsync(int projectId, CancellationToken cancellationToken = default)
{
await RequireAdminAsync(projectId, includeDeleted: false, cancellationToken);
await _projectRepository.SoftDeleteAsync(projectId, _currentUser.UserId, cancellationToken);
}

public async Task<List<TrashedProjectDto>> GetMyTrashAsync(CancellationToken cancellationToken = default)
{
var items = await _projectRepository.GetMyTrashAsync(_currentUser.UserId, cancellationToken);
var now = DateTime.UtcNow;

foreach (var item in items)
{
// The day it will be permanently deleted = trashed date + retention days.
var expiresAt = item.DeletedAt.AddDays(_settings.TrashRetentionDays);

// to not go below zero 
item.DaysRemaining = Math.Max(0, (int)Math.Ceiling((expiresAt - now).TotalDays));
}

return items;
}

public async Task<ProjectDto> RestoreAsync(int projectId, CancellationToken cancellationToken = default)
{
await RequireAdminAsync(projectId, includeDeleted: true, cancellationToken); // look inside the trash too

var restored = await _projectRepository.RestoreAsync(projectId, cancellationToken);
if (!restored)
{
// The project exists but is not in the trash (it is already active).
throw new NotFoundException("Project not found in trash.");
}

return await GetByIdAsync(projectId, cancellationToken);
}

// The permission rule used by every admin-only action
private async Task RequireAdminAsync(int projectId, bool includeDeleted, CancellationToken cancellationToken)
{
var role = await _projectRepository.GetUserRoleAsync(projectId, _currentUser.UserId, includeDeleted, cancellationToken);

if (role is null)
throw new NotFoundException("Project not found."); // not a member -> 404

if (role != ProjectRole.Admin)
throw new ForbiddenException("Only project admins can do this."); // member but not admin -> 403
}

// Clean the input: trim the name, and turn an empty description into null.
private static void Normalize(ProjectRequestDto request)
{
request.Name = request.Name.Trim();
request.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
}
}
