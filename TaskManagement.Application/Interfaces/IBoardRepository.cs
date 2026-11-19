using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IBoardRepository
{
    Task<IEnumerable<BoardDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<BoardDto?> GetByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default);
    Task<int> CreateBoardAsync(int userId, CreateBoardDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteBoardByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default); 

    // Column Methods
    Task<IEnumerable<BoardColumnDto>> GetBoardColumnsAsync(int boardId, int userId, CancellationToken cancellationToken = default);
    Task<int> AddBoardColumnAsync(int boardId, int userId, CreateBoardColumnDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateBoardColumnAsync(int boardId, int columnIds, int userId, UpdateBoardColumnDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteBoardColumnAsync(int boardId, int columnIds, int userId, CancellationToken cancellationToken = default);
    Task<bool> ReorderBoardColumnsAsync(int boardId, int userId, ReorderColumnsDto dto, CancellationToken cancellationToken = default);
}