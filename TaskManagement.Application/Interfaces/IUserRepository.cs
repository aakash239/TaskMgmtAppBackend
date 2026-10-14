using TaskManagement.Application.Models;

namespace TaskManagement.Application.Interfaces;
public interface IUserRepository
{
    Task<bool> ExistsByNormalizedEmailAsync(
        string NormalizedEmail,
        CancellationToken cancellationToken = default);


}