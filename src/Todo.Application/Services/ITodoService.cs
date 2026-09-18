using Todo.Application.DTOs;

namespace Todo.Application.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TodoResponseDto> CreateAsync(CreateTodoDto item, CancellationToken cancellationToken);
    Task<TodoResponseDto?> UpdateAsync(Guid id, UpdateTodoDto item, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
