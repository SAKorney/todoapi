using Todo.Application.DTOs;
using Todo.Domain;

namespace Todo.Application.Repositories;

public interface ITodoRepository
{
    Task<PagedResult<TodoItem>> GetPagedAsync(TodoQueryParameters query, CancellationToken cancellationToken);
    Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TodoItem item, CancellationToken cancellationToken);
    Task<TodoItem?> UpdateAsync(Guid id, string title, bool isCompleted, CancellationToken cancellationToken);

    // Позволяет экономить трафик, когда нужно только изменить статус
    Task<TodoItem?> UpdateStatusAsync(Guid id, bool isCompleted, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
