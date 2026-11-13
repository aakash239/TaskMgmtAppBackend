using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IBoardRepository
{
    Task<IEnumerable<BoardDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<BoardDto?> GetByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default);
    Task<int> CreateBoardAsync(CreateBoardDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteBoardByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default); 
}