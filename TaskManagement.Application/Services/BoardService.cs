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
    public Task<BoardDto> GetOrCreateMyBoardAsync(int projectId, int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.GetOrCreateMyBoardAsync(projectId, userId, cancellationToken);
    }
    public Task<bool> DeleteBoardByIdAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.DeleteBoardByIdAsync(boardId, userId, cancellationToken);
    }

    // Column Methods
    public Task<IEnumerable<BoardColumnDto>> GetBoardColumnsAsync(int boardId, int userId, CancellationToken cancellationToken = default)
    {
        return _boardRepository.GetBoardColumnsAsync(boardId, userId, cancellationToken);
    }


    
    // public Task<int> AddBoardColumnAsync(int boardId, int userId, CreateBoardColumnDto dto, CancellationToken cancellationToken = default)
    // {
    //     ValidateColumnName(dto.Name);
    //     return _boardRepository.AddBoardColumnAsync(boardId, userId, dto, cancellationToken);
    // }
    // public Task<bool> UpdateBoardColumnAsync(int boardId, int columnIds, int userId, UpdateBoardColumnDto dto, CancellationToken cancellationToken = default)
    // {
    //     ValidateColumnName(dto.Name);

    //     return _boardRepository.UpdateBoardColumnAsync(boardId, columnIds, userId, dto, cancellationToken);
    // }
    // public Task<bool> DeleteBoardColumnAsync(int boardId, int columnIds, int userId, CancellationToken cancellationToken = default)
    // {
    //     return _boardRepository.DeleteBoardColumnAsync(boardId, columnIds, userId, cancellationToken);
    // }
    // public Task<bool> ReorderBoardColumnsAsync(int boardId, int userId, ReorderColumnsDto dto, CancellationToken cancellationToken = default)
    // {
    //     if(dto.ColumnIds == null) throw new ArgumentException("Column ID's are required");
    //     if(dto.ColumnIds.Any(id => id <= 0)) throw new ArgumentException("Columns Id's must be positive");

    //     return _boardRepository.ReorderBoardColumnsAsync(boardId, userId, dto, cancellationToken);
    // }

    // private static void ValidateColumnName(string name)
    // {
    //     if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Column name is required!");
        
    //     if(name.Trim().Length > 100) throw new ArgumentException("Column names cannot exceed 100 characters");
    // }
    // public static void ValidateBoard(CreateBoardDto dto)
    // {
    //     if(string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Board name is required!");
        
    //     if(dto.TemplateId <= 0) throw new ArgumentException("A valid template is required");
    // }
}

