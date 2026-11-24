using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Application.Services;

// All project business rules live here
public class ProjectService(IProjectRepository projectRepository, ICurrentUserService currentUser) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository; // data access abstraction
    private readonly ICurrentUserService _currentUser = currentUser; // who is calling

    public Task<List<ProjectDto>> GetMyProjectsAsync(CancellationToken cancellationToken = default) =>
    _projectRepository.GetMyProjectsAsync(_currentUser.UserId, cancellationToken);

    public async Task<ProjectDto> GetByIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, _currentUser.UserId, cancellationToken);

        // null 
        return project ?? throw new NotFoundException("Project not found.");
    }

    public Task<ProjectDto> CreateProjectAsync(ProjectRequestDto request, CancellationToken cancellationToken = default)
    {
        Normalize(request); // trim spaces
                            // Any logged-in user may create a project. The repository makes them its Admin.
        return _projectRepository.CreateProjectAsync(_currentUser.UserId, request, cancellationToken);
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

    public async Task<ProjectDto> UpdateTemplateAsync(int projectId, UpdateProjectTemplateDto request, CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(projectId, includeDeleted: false, cancellationToken);
        await _projectRepository.UpdateTemplateAsync(projectId, request.TemplateId, _currentUser.UserId, cancellationToken);
        return await GetByIdAsync(projectId, cancellationToken);
    }
}
