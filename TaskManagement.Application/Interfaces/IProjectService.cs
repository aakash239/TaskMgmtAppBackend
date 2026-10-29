using TaskManagement.Application.DTOs;
namespace TaskManagement.Application.Interfaces;

//Business Operations for projects . Controller depends on this 
// methods use the current logged-in user internally : controllers never pass a user id 

public interface IProjectService
{
    Task<List<ProjectDto>> GetMyProjectsAsync(CancellationToken cancellationToken = default);
    Task<ProjectDto> GetByIdAsync(int projectId, CancellationToken cancellationToken = default);

    Task<ProjectDto> CreateAsync(ProjectRequestDto request, CancellationToken cancellationToken = default);

    Task<ProjectDto> UpdateAsync(int projectId,  ProjectRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int projectId, CancellationToken cancellationToken = default);

    Task<List<TrashedProjectDto>> GetMyTrashAsync(CancellationToken cancellationToken = default);

    Task<ProjectDto> RestoreAsync(int projectId, CancellationToken cancellationToken = default);


}