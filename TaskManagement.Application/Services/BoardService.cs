using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Application.Services;

public class BoardService(IBoardRepository boardRepository) : IBoardService
{
    private readonly IBoardRepository _boardRepository = boardRepository;
    public Task<IEnumerable<BoardDto>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.GetAllByUserIdAsync(userId, cancellationToken);
    }
    public Task<BoardDto?> GetByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.GetByIdAsync(boardId, userId, cancellationToken);
    }
    public Task<int> CreateBoardAsync(int userId, CreateBoardDto dto, CancellationToken cancellationToken = default)
    {
        ValidateBoard(dto);
        return _boardRepository.CreateBoardAsync(userId, dto, cancellationToken);
    }
    public Task<bool> DeleteBoardByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.DeleteBoardByIdAsync(boardId, userId, cancellationToken);
    }

    public static void ValidateBoard(CreateBoardDto dto)
    {
        if(string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Board name is required!");
        
        if(dto.TemplateId <= 0) throw new ArgumentException("A valid template is required");
    }
}