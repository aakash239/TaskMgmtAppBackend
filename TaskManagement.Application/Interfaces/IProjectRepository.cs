using TaskManagement.Application.DTOs;
using TaskManagement.Core.Enums;
namespace TaskManagement.Application.Interfaces;

// data access for projects - to be implemented in Infrastructure 
public interface IProjectRepository
{
    Task<ProjectRole?> GetUserRoleAsync(int projectId, int userId, bool includeDeleted, CancellationToken cancellationToken = default);

    // all active projects the user belongs to
    Task<List<ProjectDto>> GetMyProjectsAsync(int userId, CancellationToken cancellationToken = default);

    // One active project as seen by user 
    Task<ProjectDto?> GetByIdAsync(int projectId, int userId, CancellationToken cancellationToken = default);
    
    //creates the project and makes the user it's Admin 
    Task<ProjectDto> CreateAsync(int userId, ProjectRequestDto request, CancellationToken cancellationToken = default);

    // To update name, description, status and due date - service first checks permission 
    Task UpdateAsync(int projectId, ProjectRequestDto request, CancellationToken cancellationToken = default);
    
    // moves project to trash : soft deleting 
    Task SoftDeleteAsync(int projectId, int userId, CancellationToken cancellationToken = default);

    // Trashed projects where the user is Admin 
    Task<List<TrashedProjectDto>> GetMyTrashAsync (int userId, CancellationToken cancellationToken = default);
    
    // brings a trashed project back : returns false if there is nothing to restore 
     Task<bool> RestoreAsync(int projectId, CancellationToken cancellationToken = default);

     // to permanently delete projects that were trashed 
     Task<int> PurgeExpiredAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default);


}